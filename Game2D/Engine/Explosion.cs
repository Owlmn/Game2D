using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;

namespace Game2D.Engine
{
    public class Explosion : GameObject
    {
        private int frame = 0;
        private const int maxFrames = 5;
        private BitmapImage[] frames;
        public Explosion(double x, double y)
        {
            X = x;
            Y = y;
            frames = new BitmapImage[maxFrames];
            for (int i = 0; i < maxFrames; i++)
                frames[i] = new BitmapImage(new Uri($"pack://application:,,,/Project/images/boom.png")); // Можно заменить на разные кадры
            Sprite = new Image
            {
                Source = frames[0],
                Width = 48,
                Height = 48
            };
        }
        public override void Update()
        {
            frame++;
            if (frame < maxFrames)
                Sprite.Source = frames[frame];
            else
                IsActive = false;
        }
    }
} 