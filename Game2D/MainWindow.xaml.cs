using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Game2D.Engine;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

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
        private int[] levelScoreThresholds = new int[] { 400, 600, 800 };
        private bool portalSpawned = false;
        private double[] levelCoefs = new double[] { 1.0, 1.5, 2.0 };

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
            GameCanvas.Visibility = Visibility.Hidden;
            HealthBar.Visibility = Visibility.Hidden;
            ScoreLabel.Visibility = Visibility.Hidden;
            AmmoLabel.Visibility = Visibility.Hidden;
            WeaponLabel.Visibility = Visibility.Hidden;
            TimerLabel.Visibility = Visibility.Hidden;
            MessageLabel.Visibility = Visibility.Hidden;
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
            _hero = new Hero(200, 200);
            LoadLevel(currentLevelIndex);
            GameCanvas.Focus();
        }

        private void LoadLevel(int levelIdx)
        {
            GameCanvas.Children.Clear();
            var levelType = levelTypes[levelIdx];
            var coef = levelCoefs[levelIdx];
            _gameWorld = (GameWorld)Activator.CreateInstance(levelType, GameCanvas);
            CurrentGameWorld = _gameWorld;
            CurrentLevelIndex = levelIdx;
            if (_hero.Sprite != null && _hero.Sprite.Parent is Canvas oldCanvas)
                oldCanvas.Children.Remove(_hero.Sprite);
            _gameWorld.AddObject(_hero);
            _hero.X = 200; _hero.Y = 200;
            SpawnLevelObjects(_selectedDifficulty, levelIdx, coef);
            _gameWorld.Start();
            currentPortal = null;
            portalSpawned = false;
        }

        private void SpawnLevelObjects(DifficultyLevel level, int levelIdx, double coef)
        {
            if (levelIdx == 0) // MAP
            {
                for (int i = 0; i < 5; i++)
                    _gameWorld.AddObject(new EnemyBeast(_hero, 300 + i*100, 400, coef));
                for (int i = 0; i < 3; i++)
                    _gameWorld.AddObject(new Zombi(_hero, 500 + i*120, 600, coef));
            }
            else if (levelIdx == 1) // MAP3
            {
                for (int i = 0; i < 4; i++)
                    _gameWorld.AddObject(new CosmoEnemy(_hero, 300 + i*120, 400, coef));
                for (int i = 0; i < 2; i++)
                    _gameWorld.AddObject(new ShotgunEnemy(_hero, 500 + i*180, 600, coef));
            }
            else if (levelIdx == 2) // MAP4
            {
                for (int i = 0; i < 4; i++)
                    _gameWorld.AddObject(new RocketEnemy(_hero, 300 + i*120, 400, coef));
            }
            else if (levelIdx == 3) // MAP5 (босс)
            {
                _gameWorld.AddObject(new Boss(_hero, _gameWorld._canvas.ActualWidth/2-64, 100, coef));
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
            // Только смена оружия и перезарядка
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
            // --- Инфо-блок ---
            HealthBar.Value = Math.Max(0, Math.Min(_hero.Health, _hero.MaxHealth));
            AmmoLabel.Content = $"Ammo: {_hero.RifleAmmo}/{_hero.MaxRifleAmmo}";
            WeaponLabel.Content = $"Weapon: {_hero.CurrentWeapon}";
            var elapsed = DateTime.Now - _startTime;
            TimerLabel.Content = $"Time: {elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
            ScoreLabel.Content = $"Score: {_gameWorld.Score}";
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
            MessageLabel.Content = "YOU DIED";
            MessageLabel.Visibility = Visibility.Visible;
        }
    }
}