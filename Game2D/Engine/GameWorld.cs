using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Imaging;

namespace Game2D.Engine
{
    public class GameWorld
    {
        public List<GameObject> Objects { get; } = new List<GameObject>();
        public Canvas _canvas;
        private DispatcherTimer _timer;
        private int zombieSpawnTimer = 0;
        private int zombieSpawnInterval = 600; // 10 секунд при 60 FPS
        private int maxZombies = 10;
        private Hero _heroCache;
        private bool isRendering = false;
        public int Score { get; private set; } = 0;

        public GameWorld(Canvas canvas)
        {
            _canvas = canvas;
            _timer = new DispatcherTimer();
            _timer.Interval = System.TimeSpan.FromMilliseconds(1000.0 / 60.0); // 60 FPS, как в Greenfoot
            _timer.Tick += (s, e) => GameLoop();
        }

        public void Start()
        {
            if (!isRendering)
            {
                CompositionTarget.Rendering += OnRendering;
                isRendering = true;
            }
        }

        public void Stop()
        {
            if (isRendering)
            {
                CompositionTarget.Rendering -= OnRendering;
                isRendering = false;
            }
        }

        private void OnRendering(object sender, System.EventArgs e)
        {
            GameLoop();
        }

        private void GameLoop()
        {
            // 1. Update logic
            foreach (var obj in Objects.ToArray())
                obj.Update();
            Objects.RemoveAll(o => !o.IsActive);
            // 2. Draw (только перемещаем спрайты, не очищаем Canvas)
            foreach (var obj in Objects)
                obj.Draw(_canvas);
        }

        public void AddObject(GameObject obj)
        {
            Objects.Add(obj);
        }

        public void AddScore(int value)
        {
            Score += value;
        }
    }
} 