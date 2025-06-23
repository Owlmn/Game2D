using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Game2D.Engine;
using System.Collections.Generic;
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
            _gameWorld = new GameWorld(GameCanvas);
            CurrentGameWorld = _gameWorld;
            _hero = new Hero(200, 200);
            _gameWorld.AddObject(_hero);
            SpawnLevelObjects(_selectedDifficulty);
            _gameWorld.Start();
            _score = 0;
            _startTime = DateTime.Now;
            _gameOver = false;
            GameCanvas.Focus();
        }

        private void SpawnLevelObjects(DifficultyLevel level)
        {
            // --- Стены горизонтальные ---
            int[,] wallPositions = new int[,] {
                {423, 283}, {463, 283}, {383, 323}, {343, 363}, {343, 481},
                {383, 521}, {423, 561}, {463, 561}, {147, 283}, {187, 283}, {227, 283}, {267, 283}, {147, 127},
                {267, 127}, {382, 206}, {422, 206}, {462, 206}, {502, 206}, {582, 88},{622, 88},{662, 88},{702, 88},{699, 246},
                {739, 246},{779, 246},{819, 246},{859, 246}, {934, 324},{974, 324},{1014, 324},{934, 599},{974, 599},
                {1014, 599},{1054,599},{1132, 324},{1172, 324}, {107, 639},{147, 639},{186, 716},{226, 716},{266,716},
                {305, 639},{345, 639}, {502, 756},{542, 756},{582, 756}, {1307, 639},{1347, 639},{1386, 716},{1426, 716},{1466, 716},
                {187, 10},{227, 10},{267, 10},{307, 10},{347, 10}, {387, 10},{427, 10},{467, 10},{507, 10}, {547, 10},
                {587, 10},{627, 10},{667, 10},{707, 10},{747, 10}, {787, 10},{827, 10},{867, 10}, {907, 10},{947, 10},{987, 10},{972, 168},
                {1012, 168},{1052, 168},{1092, 168}, {1347, 285},{1387, 285}, {1427, 285},{1467, 285}, {1347, 127},{1467, 127},{10,320}, {10, 515}
            };
            for (int i = 0; i < wallPositions.GetLength(0); i++)
                _gameWorld.AddObject(new Wall(wallPositions[i,0], wallPositions[i,1], "wall_.png"));

            // --- Стены вертикальные ---
            int[,] wallPositions2 = new int[,] {
                {10, 10}, {10, 50}, {10, 90}, {10, 130},{10, 170}, {10, 210},
                {10, 250}, {10, 290}, {10, 540}, {10, 580}, {10, 620}, {10, 660},
                {10, 700}, {10, 740},{10, 780}, {127, 148}, {127, 188}, {127, 228},
                {127, 268}, {285, 148}, {285, 188}, {285, 228}, {285, 268},
                {363, 30}, {363, 190}, {363, 340}, {403, 300}, {481, 301}, {363, 500},
                {403, 540}, {481, 540}, {87, 660}, {87, 700},{167, 660}, {167, 700},
                {284, 660}, {284, 700}, {364, 660}, {364, 700}, {600, 580}, {600, 620},
                {600, 660}, {600, 700},{600, 740}, {600, 105}, {600, 145}, {600, 185},
                {600, 225}, {600, 265}, {717, 382}, {717, 422},{717, 462}, {796, 580},
                {796, 620}, {796, 660}, {796, 780}, {914, 580}, {993, 620}, {993, 660},
                {993, 700}, {914, 343}, {914, 383}, {914, 423}, {874, 223}, {874, 183},
                {954, 147}, {954, 107}, {363, 30}, {1190, 20}, {1210, 20}, {1190, 60}, {1210, 60},
                {1190, 100}, {1210, 100}, {1190, 140}, {1210, 140}, {1190, 180}, {1210, 180},
                {1190, 220}, {1210, 220}, {1190, 260}, {1210, 260}, {1190, 300}, {1210, 300},
                {1190, 540}, {1210, 540}, {1190, 580}, {1210, 580}, {1190, 620}, {1210, 620},
                {1190, 660}, {1210, 660}, {1190, 700}, {1210, 700}, {1190, 740}, {1210, 740}, {1190, 780}, {1210, 780},
                {1287, 700}, {1287, 660}, {1366, 660}, {1366, 700}, {1486, 660}, {1486, 700},
                {1327, 266}, {1327, 226}, {1327, 186}, {1327, 146}, {1485, 146}, {1485, 186}, {1485, 226}, {1485, 266}
            };
            for (int i = 0; i < wallPositions2.GetLength(0); i++)
                _gameWorld.AddObject(new Wall(wallPositions2[i,0], wallPositions2[i,1], "wall_.png"));

            // --- Кубы ---
            int[,] wallPositions3 = new int[,] {
                {108, 422}, {148, 422},{188, 382},{188, 462},
                {776, 224}, {776, 620}, {1308, 422}, {1348, 422},{1388, 382},{1388, 462}
            };
            for (int i = 0; i < wallPositions3.GetLength(0); i++)
                _gameWorld.AddObject(new Wall(wallPositions3[i,0], wallPositions3[i,1], "Cube.png"));

            // --- Герой ---
            _hero.X = 200; _hero.Y = 190;

            // --- Враги ---
            for (int i = 0; i < 5; i++)
                _gameWorld.AddObject(new EnemyBeast(_hero, 300 + i*100, 400));
            for (int i = 0; i < 3; i++)
                _gameWorld.AddObject(new Zombi(_hero, 500 + i*120, 600));
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
            if (_gameOver) return;
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
            if (_hero != null)
            {
                HealthBar.Value = Math.Max(0, Math.Min(_hero.Health, _hero.MaxHealth));
                AmmoLabel.Content = $"Ammo: {_hero.RifleAmmo}/{_hero.MaxRifleAmmo}";
            }
            // Счет: считаем количество убитых врагов (неактивных)
            var killed = _gameWorld.Objects.FindAll(o => o is EnemyBeast enemy && !enemy.IsActive).Count;
            if (killed != _score)
            {
                _score = killed;
                ScoreLabel.Content = $"Score: {_score}";
            }
            // Таймер
            var elapsed = DateTime.Now - _startTime;
            TimerLabel.Content = $"Time: {elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
            // Проверка конца игры
            if (_hero != null && _hero.Health <= 0 && !_gameOver)
            {
                GameOver();
            }
        }
        private void GameOver()
        {
            _gameOver = true;
            MessageLabel.Content = "Game Over";
            _gameWorld.Stop();
        }
    }
}