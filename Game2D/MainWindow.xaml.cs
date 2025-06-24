using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Game2D.Engine;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;

namespace Game2D
{
    public partial class MainWindow : Window
    {
        private readonly GameWorld _world;
        private readonly DispatcherTimer _timer;
        private GameWorld _gameWorld;
        private Hero _hero;
        private HashSet<Key> _pressedKeys = new HashSet<Key>();
        private int _score = 0;
        private DateTime _startTime = DateTime.Now;
        private bool _gameOver = false;
        public static GameWorld CurrentGameWorld { get; private set; }

        public enum DifficultyLevel { Easy, Medium, Hard, Hardcore }
        private DifficultyLevel _selectedDifficulty = DifficultyLevel.Medium;

        // --- Управление уровнями ---
        private int currentLevelIndex = 0;
        public static int CurrentLevelIndex { get; private set; } = 0;
        private readonly Type[] levelTypes = new Type[] { typeof(Game2D.Engine.MAP), typeof(Game2D.Engine.MAP3), typeof(Game2D.Engine.MAP4), typeof(Game2D.Engine.MAP5) };
        private GameObject currentPortal = null;
        private int[] levelScoreThresholds = new int[] { 650, 820, 400 };
        private bool portalSpawned = false;
        private double[] levelCoefs = new double[] { 1.0, 1.5, 2.0 };

        // --- Система постепенного спавна мобов ---
        private List<EnemySpawnData> _enemySpawnQueue = new List<EnemySpawnData>();
        private int _spawnTimer = 0;
        private int _spawnInterval = 120; // Интервал между спавном мобов (в кадрах)
        private bool _spawningComplete = false;
        private int _maxEnemiesOnScreen = 3; // Максимальное количество врагов на экране одновременно

        // --- Система индикаторов здоровья ---
        private List<HealthIndicator> _healthIndicators = new List<HealthIndicator>();

        private class EnemySpawnData
        {
            public Type EnemyType { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public double HpCoef { get; set; }
            public double DmgCoef { get; set; }
            public double SpeedCoef { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            ShowMenu();
            StartButton.Click += (s, e) => StartGame();
            ControlsButton.Click += (s, e) => ShowControls();
            DifficultyButton.Click += (s, e) => ShowDifficulty();
            ExitButton.Click += (s, e) => Close();
            EasyButton.Click += (s, e) => { _selectedDifficulty = DifficultyLevel.Easy; ShowMenu(); };
            MediumButton.Click += (s, e) => { _selectedDifficulty = DifficultyLevel.Medium; ShowMenu(); };
            HardButton.Click += (s, e) => { _selectedDifficulty = DifficultyLevel.Hard; ShowMenu(); };
            HardcoreButton.Click += (s, e) => { _selectedDifficulty = DifficultyLevel.Hardcore; ShowMenu(); };
            this.KeyDown += MainMenu_KeyDown;
            this.KeyUp += MainWindow_KeyUp;
            GameCanvas.MouseLeftButtonDown += GameCanvas_MouseLeftButtonDown;
            CompositionTarget.Rendering += UpdateUI;
        }

        private void ShowMenu()
        {
            MenuGrid.Visibility = Visibility.Visible;
            ControlsGrid.Visibility = Visibility.Collapsed;
            DifficultyGrid.Visibility = Visibility.Collapsed;
            GameCanvas.Visibility = Visibility.Collapsed;
            HealthBar.Visibility = Visibility.Collapsed;
            ScoreLabel.Visibility = Visibility.Collapsed;
            AmmoLabel.Visibility = Visibility.Collapsed;
            WeaponLabel.Visibility = Visibility.Collapsed;
            TimerLabel.Visibility = Visibility.Collapsed;
            MessageLabel.Visibility = Visibility.Collapsed;
            
            for (int i = GameCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (GameCanvas.Children[i] is Game2D.Engine.Controls || 
                    GameCanvas.Children[i] is Game2D.Engine.DifficultSelect ||
                    GameCanvas.Children[i] is Game2D.Engine.Win || 
                    GameCanvas.Children[i] is Game2D.Engine.Exit ||
                    GameCanvas.Children[i] is Grid)
                    GameCanvas.Children.RemoveAt(i);
            }
            
            if (_gameWorld != null)
            {
                _gameWorld.Stop();
                _gameWorld = null;
            }
            _hero = null;
            ClearHealthIndicators();
        }
        private void ShowControls()
        {
            MenuGrid.Visibility = Visibility.Collapsed;
            ControlsGrid.Visibility = Visibility.Visible;
            DifficultyGrid.Visibility = Visibility.Collapsed;
        }
        private void ShowDifficulty()
        {
            MenuGrid.Visibility = Visibility.Collapsed;
            ControlsGrid.Visibility = Visibility.Collapsed;
            DifficultyGrid.Visibility = Visibility.Visible;
        }
        private void StartGame()
        {
            MenuGrid.Visibility = Visibility.Collapsed;
            ControlsGrid.Visibility = Visibility.Collapsed;
            DifficultyGrid.Visibility = Visibility.Collapsed;
            GameCanvas.Visibility = Visibility.Visible;
            HealthBar.Visibility = Visibility.Visible;
            ScoreLabel.Visibility = Visibility.Visible;
            AmmoLabel.Visibility = Visibility.Visible;
            WeaponLabel.Visibility = Visibility.Visible;
            TimerLabel.Visibility = Visibility.Visible;
            MessageLabel.Visibility = Visibility.Visible;
            currentLevelIndex = 0;
            portalSpawned = false;
            _score = 0;
            _startTime = DateTime.Now;
            _gameOver = false;
            // --- Масштабируем параметры героя по сложности ---
            int heroHP = 3000;
            switch (_selectedDifficulty)
            {
                case DifficultyLevel.Easy: heroHP = 3000; break;
                case DifficultyLevel.Medium: heroHP = 2000; break;
                case DifficultyLevel.Hard: heroHP = 1000; break;
                case DifficultyLevel.Hardcore: heroHP = 1; break;
            }
            _hero = new Hero(200, 200);
            _hero.MaxHealth = heroHP;
            _hero.Health = heroHP;
            _hero.ResetAmmo();
            HealthBar.Maximum = heroHP;
            HealthBar.Value = heroHP;
            LoadLevel(currentLevelIndex);
            GameCanvas.Focus();
        }

        private void LoadLevel(int levelIdx)
        {
            // Удаляем все спрайты старых объектов с Canvas
            if (_gameWorld != null)
            {
                foreach (var obj in _gameWorld.Objects)
                {
                    if (obj.Sprite != null && obj.Sprite.Parent is Canvas canvas)
                        canvas.Children.Remove(obj.Sprite);
                }
                _gameWorld.Objects.Clear();
            }
            GameCanvas.Children.Clear();
            double coef = levelCoefs[Math.Min(levelIdx, levelCoefs.Length - 1)];
            if (levelIdx == 0 || levelIdx == 1 || levelIdx == 2) // Первые три уровня — случайная карта с разным фоном
            {
                var difficulty = _selectedDifficulty switch
                {
                    DifficultyLevel.Easy => RandomMap.MapDifficulty.Easy,
                    DifficultyLevel.Medium => RandomMap.MapDifficulty.Medium,
                    DifficultyLevel.Hard => RandomMap.MapDifficulty.Hard,
                    DifficultyLevel.Hardcore => RandomMap.MapDifficulty.Hardcore,
                    _ => RandomMap.MapDifficulty.Medium
                };
                string[] backgrounds = { "MAP1.jpg", "MAP3.jpg", "MAP4.jpg" };
                // Устанавливаем фон
                var brush = new System.Windows.Media.ImageBrush();
                brush.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new System.Uri($"pack://application:,,,/Project/images/{backgrounds[levelIdx]}"));
                brush.Stretch = System.Windows.Media.Stretch.UniformToFill;
                GameCanvas.Background = brush;
                _gameWorld = new RandomMap(GameCanvas, difficulty);
                CurrentGameWorld = _gameWorld;
                CurrentLevelIndex = levelIdx;
                var (hx, hy) = ((_gameWorld as RandomMap)?.GetStartPosition()).Value;
                _hero.X = hx;
                _hero.Y = hy;
                _hero.Health = _hero.MaxHealth;
                _hero.ResetAmmo();
                if (_hero.Sprite != null && _hero.Sprite.Parent is Canvas oldCanvas)
                    oldCanvas.Children.Remove(_hero.Sprite);
                _gameWorld.AddObject(_hero);
                // Портал не добавляем сразу, он появится при достижении нужного количества очков
                SpawnLevelObjects(_selectedDifficulty, levelIdx, coef);
                _gameWorld.Start();
                currentPortal = null;
                portalSpawned = false;
            }
            else if (levelIdx == 3) // Финальный уровень — арена с боссом
            {
                // Устанавливаем фон
                var brush = new System.Windows.Media.ImageBrush();
                brush.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new System.Uri("pack://application:,,,/Project/images/boss_map.png"));
                brush.Stretch = System.Windows.Media.Stretch.UniformToFill;
                GameCanvas.Background = brush;
                // Создаем пустую арену без стен для боя с боссом
                _gameWorld = new BossArena(GameCanvas);
                CurrentGameWorld = _gameWorld;
                CurrentLevelIndex = levelIdx;
                var (hx, hy) = ((_gameWorld as BossArena)?.GetStartPosition()).Value;
                _hero.X = hx;
                _hero.Y = hy;
                _hero.Health = _hero.MaxHealth;
                _hero.ResetAmmo();
                if (_hero.Sprite != null && _hero.Sprite.Parent is Canvas oldCanvas)
                    oldCanvas.Children.Remove(_hero.Sprite);
                _gameWorld.AddObject(_hero);
                // Добавляем босса
                double bossX = _gameWorld._canvas.ActualWidth/2-64;
                double bossY = 100;
                _gameWorld.AddObject(new Boss(_hero, bossX, bossY, coef));
                // Добавляем две летающие руки
                _gameWorld.AddObject(new Boss_Hand1(bossX - 100, bossY + 200, _hero));
                _gameWorld.AddObject(new Boss_Hand2(bossX + 200, bossY + 200, _hero));
                _gameWorld.Start();
                currentPortal = null;
                portalSpawned = false;
            }
            else
            {
                var levelType = levelTypes[levelIdx];
                _gameWorld = (GameWorld)Activator.CreateInstance(levelType, GameCanvas);
                CurrentGameWorld = _gameWorld;
                CurrentLevelIndex = levelIdx;
                if (_hero.Sprite != null && _hero.Sprite.Parent is Canvas oldCanvas)
                    oldCanvas.Children.Remove(_hero.Sprite);
                _gameWorld.AddObject(_hero);
                _hero.X = 200; _hero.Y = 200;
                _hero.Health = _hero.MaxHealth;
                _hero.ResetAmmo();
                SpawnLevelObjects(_selectedDifficulty, levelIdx, coef);
                _gameWorld.Start();
                currentPortal = null;
                portalSpawned = false;
            }
        }

        private void SpawnLevelObjects(DifficultyLevel level, int levelIdx, double coef)
        {
            // Очищаем очередь спавна
            _enemySpawnQueue.Clear();
            _spawnTimer = 0;
            _spawningComplete = false;
            
            // --- Масштабируем параметры врагов по сложности ---
            double hpCoef = 1, dmgCoef = 1, speedCoef = 1;
            int beastCount = 5, zombiCount = 3, cosmoCount = 4, shotgunCount = 2, rocketCount = 4;
            switch (level)
            {
                case DifficultyLevel.Easy:
                    hpCoef = 1; dmgCoef = 1; speedCoef = 1;
                    beastCount = 5; zombiCount = 3; cosmoCount = 4; shotgunCount = 2; rocketCount = 4;
                    _maxEnemiesOnScreen = 2;
                    break;
                case DifficultyLevel.Medium:
                    hpCoef = 1.5; dmgCoef = 1.5; speedCoef = 1.2;
                    beastCount = 7; zombiCount = 5; cosmoCount = 6; shotgunCount = 3; rocketCount = 6;
                    _maxEnemiesOnScreen = 3;
                    break;
                case DifficultyLevel.Hard:
                    hpCoef = 2.2; dmgCoef = 2.2; speedCoef = 1.5;
                    beastCount = 10; zombiCount = 8; cosmoCount = 8; shotgunCount = 4; rocketCount = 8;
                    _maxEnemiesOnScreen = 4;
                    break;
                case DifficultyLevel.Hardcore:
                    hpCoef = 3.5; dmgCoef = 100; speedCoef = 2.2;
                    beastCount = 15; zombiCount = 12; cosmoCount = 12; shotgunCount = 6; rocketCount = 12;
                    _maxEnemiesOnScreen = 5;
                    break;
            }
            
            if (levelIdx == 0) // MAP
            {
                // Добавляем зверей в очередь спавна
                for (int i = 0; i < beastCount; i++)
                {
                    _enemySpawnQueue.Add(new EnemySpawnData
                    {
                        EnemyType = typeof(EnemyBeast),
                        X = 300 + i * 100,
                        Y = 400,
                        HpCoef = coef * hpCoef,
                        DmgCoef = dmgCoef,
                        SpeedCoef = speedCoef
                    });
                }
                // Добавляем зомби в очередь спавна
                for (int i = 0; i < zombiCount; i++)
                {
                    _enemySpawnQueue.Add(new EnemySpawnData
                    {
                        EnemyType = typeof(Zombi),
                        X = 500 + i * 120,
                        Y = 600,
                        HpCoef = coef * hpCoef,
                        DmgCoef = dmgCoef,
                        SpeedCoef = speedCoef
                    });
                }
            }
            else if (levelIdx == 1) // MAP3
            {
                // Добавляем космо-врагов в очередь спавна
                for (int i = 0; i < cosmoCount; i++)
                {
                    _enemySpawnQueue.Add(new EnemySpawnData
                    {
                        EnemyType = typeof(CosmoEnemy),
                        X = 300 + i * 120,
                        Y = 400,
                        HpCoef = coef * hpCoef,
                        DmgCoef = dmgCoef,
                        SpeedCoef = speedCoef
                    });
                }
                // Добавляем дробовиков в очередь спавна
                for (int i = 0; i < shotgunCount; i++)
                {
                    _enemySpawnQueue.Add(new EnemySpawnData
                    {
                        EnemyType = typeof(ShotgunEnemy),
                        X = 500 + i * 180,
                        Y = 600,
                        HpCoef = coef * hpCoef,
                        DmgCoef = dmgCoef,
                        SpeedCoef = speedCoef
                    });
                }
            }
            else if (levelIdx == 2) // MAP4
            {
                // Добавляем ракетчиков в очередь спавна
                for (int i = 0; i < rocketCount; i++)
                {
                    _enemySpawnQueue.Add(new EnemySpawnData
                    {
                        EnemyType = typeof(RocketEnemy),
                        X = 300 + i * 120,
                        Y = 400,
                        HpCoef = coef * hpCoef,
                        DmgCoef = dmgCoef,
                        SpeedCoef = speedCoef
                    });
                }
            }
            else if (levelIdx == 3) // MAP5 (босс)
            {
                // Босса создаем сразу
                _gameWorld.AddObject(new Boss(_hero, _gameWorld._canvas.ActualWidth/2-64, 100, coef));
                _spawningComplete = true;
            }
        }

        private void MainMenu_KeyDown(object sender, KeyEventArgs e)
        {
            if (MenuGrid.Visibility == Visibility.Visible) return;
            
            if (ControlsGrid.Visibility == Visibility.Visible || DifficultyGrid.Visibility == Visibility.Visible)
            {
                if (e.Key == Key.Escape)
                {
                    ShowMenu();
                }
            }
            if (_gameOver)
            {
                if (e.Key == Key.Escape)
                {
                    ShowMenu();
                    MessageLabel.Visibility = Visibility.Hidden;
                }
                return;
            }
            if (e.Key == Key.R && _hero != null)
            {
                _hero.Reload();
            }
            if (_hero != null)
            {
                if (e.Key == Key.D1) _hero.SwitchWeapon(1);
                if (e.Key == Key.D2) _hero.SwitchWeapon(2);
                if (e.Key == Key.D3) _hero.SwitchWeapon(3);
            }
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (_gameOver) return;
            // Только перезарядка
            if (e.Key == Key.R && _hero != null)
            {
                _hero.Reload();
            }
        }
        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (_gameOver) return;
            // Больше не нужно хранить _pressedKeys
        }
        private void GameCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(GameCanvas);
            _hero.ShootTo(pos.X, pos.Y, _gameWorld);
            GameCanvas.Focus(); // Всегда возвращаем фокус на Canvas
        }
        public HashSet<Key> GetPressedKeys() => _pressedKeys; // Можно удалить, если больше не используется

        private void UpdateUI(object sender, EventArgs e)
        {
            if (_gameOver) return;
            if (_gameWorld == null || _hero == null || MenuGrid.Visibility == Visibility.Visible)
                return;
            
            // --- Постепенный спавн мобов ---
            if (!_spawningComplete && _enemySpawnQueue.Count > 0)
            {
                // Подсчитываем количество живых врагов
                int aliveEnemies = 0;
                if (_gameWorld != null)
                {
                    aliveEnemies = _gameWorld.Objects.Count(obj => 
                        obj is EnemyBeast || obj is Zombi || obj is CosmoEnemy || 
                        obj is ShotgunEnemy || obj is RocketEnemy);
                }
                
                // Спавним нового врага только если на экране меньше максимального количества
                if (aliveEnemies < _maxEnemiesOnScreen)
                {
                    _spawnTimer++;
                    if (_spawnTimer >= _spawnInterval)
                    {
                        SpawnNextEnemy();
                        _spawnTimer = 0;
                    }
                }
            }
            
            // --- Инфо-блок ---
            HealthBar.Value = Math.Max(0, Math.Min(_hero.Health, _hero.MaxHealth));
            string ammoText = _hero.CurrentWeapon switch
            {
                Hero.WeaponType.Rifle => $"Ammo: {_hero.RifleAmmo}/{_hero.MaxRifleAmmo}",
                Hero.WeaponType.Shotgun => $"Ammo: {_hero.ShotgunAmmo}/{_hero.MaxShotgunAmmo}",
                Hero.WeaponType.Lasergun => $"Ammo: {_hero.LasergunAmmo}/{_hero.MaxLasergunAmmo}",
                _ => "Ammo: -"
            };
            AmmoLabel.Content = ammoText;
            WeaponLabel.Content = $"Weapon: {_hero.CurrentWeapon}";
            var elapsed = DateTime.Now - _startTime;
            TimerLabel.Content = $"Time: {elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
            ScoreLabel.Content = $"Score: {_gameWorld.Score}";
            
            // --- Обновление индикаторов здоровья ---
            UpdateHealthIndicators();
            
            // --- Переход на следующий уровень ---
            if (currentLevelIndex < levelScoreThresholds.Length)
            {
                if (_gameWorld.Score >= levelScoreThresholds[currentLevelIndex] && !portalSpawned)
                {
                    SpawnPortalForLevel(currentLevelIndex);
                    portalSpawned = true;
                }
            }
            // Проверка конца игры
            if (_hero != null && _hero.Health <= 0 && !_gameOver)
            {
                GameOver();
                return;
            }
            // Победа над боссом
            if (CurrentLevelIndex == 3 && !_gameOver && _hero != null && _hero.Health > 0)
            {
                var boss = _gameWorld.Objects.Find(o => o is Game2D.Engine.Boss) as Game2D.Engine.Boss;
                if (boss != null && boss.IsDead && _hero.Health > 0)
                {
                    _gameOver = true;
                    _gameWorld.Stop();
                    var winScreen = new Game2D.Engine.Win();
                    winScreen.ExitButton.Click += (s, e) => ShowMenu();
                    winScreen.Width = GameCanvas.ActualWidth;
                    winScreen.Height = GameCanvas.ActualHeight;
                    Canvas.SetLeft(winScreen, 0);
                    Canvas.SetTop(winScreen, 0);
                    GameCanvas.Children.Add(winScreen);
                }
            }
        }

        private void SpawnPortalForLevel(int levelIdx)
        {
            double x = 0, y = 0;
            GameObject portal = null;
            switch (levelIdx)
            {
                case 0:
                    x = GameCanvas.ActualWidth - 150; y = GameCanvas.ActualHeight / 2;
                    portal = new Game2D.Engine.Portal(x, y);
                    ((Game2D.Engine.Portal)portal).OnEnter = () => NextLevel();
                    break;
                case 1:
                    x = GameCanvas.ActualWidth / 2; y = GameCanvas.ActualHeight - 200;
                    portal = new Game2D.Engine.Portal2(x, y);
                    ((Game2D.Engine.Portal2)portal).OnEnter = () => NextLevel();
                    break;
                case 2:
                    x = GameCanvas.ActualWidth / 2 + 10; y = GameCanvas.ActualHeight - 300;
                    portal = new Game2D.Engine.Portal3(x, y);
                    ((Game2D.Engine.Portal3)portal).OnEnter = () => NextLevel();
                    break;
            }
            if (portal != null)
            {
                _gameWorld.AddObject(portal);
                currentPortal = portal;
            }
        }

        private void NextLevel()
        {
            if (currentLevelIndex == 2) // после 3-й карты всегда переход на уровень с боссом
            {
                currentLevelIndex = 3;
                LoadLevel(currentLevelIndex);
                _hero?.ResetAmmo();
                return;
            }
            if (currentLevelIndex + 1 < levelTypes.Length)
            {
                currentLevelIndex++;
                LoadLevel(currentLevelIndex);
                _hero?.ResetAmmo();
            }
            else
            {
                // Победа
                _gameOver = true;
                MessageLabel.Content = "YOU WIN!";
                _gameWorld.Stop();
            }
        }

        private void GameOver()
        {
            _gameOver = true;
            _gameWorld.Stop();
            // Очищаем индикаторы
            ClearHealthIndicators();
            // Удаляем все старые lose/win экраны
            for (int i = GameCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (GameCanvas.Children[i] is Game2D.Engine.Win || GameCanvas.Children[i] is Game2D.Engine.Exit || GameCanvas.Children[i] is Grid)
                    GameCanvas.Children.RemoveAt(i);
            }
            // Создаём экран смерти
            var grid = new Grid();
            grid.Width = GameCanvas.ActualWidth;
            grid.Height = GameCanvas.ActualHeight;
            var label = new System.Windows.Controls.Label
            {
                Content = "YOU DIED",
                FontSize = 64,
                Foreground = System.Windows.Media.Brushes.Red,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalContentAlignment = System.Windows.VerticalAlignment.Center
            };
            var exitButton = new Game2D.Engine.Exit();
            exitButton.Content = "В меню";
            exitButton.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            exitButton.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            exitButton.Margin = new System.Windows.Thickness(0,0,0,80);
            exitButton.Click += (s, e) => ShowMenu();
            grid.Children.Add(label);
            grid.Children.Add(exitButton);
            Canvas.SetLeft(grid, 0);
            Canvas.SetTop(grid, 0);
            GameCanvas.Children.Add(grid);
        }

        private void SpawnNextEnemy()
        {
            if (_enemySpawnQueue.Count > 0)
            {
                var spawnData = _enemySpawnQueue[0];
                _enemySpawnQueue.RemoveAt(0);
                
                GameObject enemy = null;
                
                // Создаем врага в зависимости от типа
                if (spawnData.EnemyType == typeof(EnemyBeast))
                {
                    enemy = new EnemyBeast(_hero, spawnData.X, spawnData.Y, spawnData.HpCoef, spawnData.DmgCoef, spawnData.SpeedCoef);
                }
                else if (spawnData.EnemyType == typeof(Zombi))
                {
                    enemy = new Zombi(_hero, spawnData.X, spawnData.Y, spawnData.HpCoef, spawnData.DmgCoef, spawnData.SpeedCoef);
                }
                else if (spawnData.EnemyType == typeof(CosmoEnemy))
                {
                    enemy = new CosmoEnemy(_hero, spawnData.X, spawnData.Y, spawnData.HpCoef, spawnData.DmgCoef, spawnData.SpeedCoef);
                }
                else if (spawnData.EnemyType == typeof(ShotgunEnemy))
                {
                    enemy = new ShotgunEnemy(_hero, spawnData.X, spawnData.Y, spawnData.HpCoef, spawnData.DmgCoef, spawnData.SpeedCoef);
                }
                else if (spawnData.EnemyType == typeof(RocketEnemy))
                {
                    enemy = new RocketEnemy(_hero, spawnData.X, spawnData.Y, spawnData.HpCoef, spawnData.DmgCoef, spawnData.SpeedCoef);
                }
                
                if (enemy != null)
                {
                    _gameWorld.AddObject(enemy);
                }
                
                // Если очередь пуста, отмечаем спавн завершенным
                if (_enemySpawnQueue.Count == 0)
                {
                    _spawningComplete = true;
                }
            }
        }

        private void UpdateHealthIndicators()
        {
            // Очищаем старые индикаторы
            foreach (var indicator in _healthIndicators)
            {
                indicator.Remove();
            }
            _healthIndicators.Clear();

            // Добавляем индикатор для героя
            if (_hero != null && _hero.IsActive)
            {
                var heroIndicator = new HealthIndicator(GameCanvas, _hero, true);
                _healthIndicators.Add(heroIndicator);
            }

            // Добавляем индикаторы для всех врагов
            if (_gameWorld != null)
            {
                foreach (var obj in _gameWorld.Objects)
                {
                    if (obj.IsActive && (obj is EnemyBeast || obj is Zombi || obj is CosmoEnemy || 
                                        obj is ShotgunEnemy || obj is RocketEnemy))
                    {
                        var enemyIndicator = new HealthIndicator(GameCanvas, obj, false);
                        _healthIndicators.Add(enemyIndicator);
                    }
                }
            }

            // Обновляем все индикаторы
            foreach (var indicator in _healthIndicators)
            {
                indicator.Update();
            }
        }

        private void ClearHealthIndicators()
        {
            foreach (var indicator in _healthIndicators)
            {
                indicator.Remove();
            }
            _healthIndicators.Clear();
        }
    }
}