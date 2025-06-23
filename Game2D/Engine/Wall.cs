using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;

namespace Game2D.Engine
{
    public class Wall : GameObject
    {
        public Wall(double x, double y, string imageName)
        {
            X = x;
            Y = y;
            Sprite = new Image
            {
                Source = new BitmapImage(new Uri($"pack://application:,,,/Project/images/{imageName}")),
                Width = 12,
                Height = 48
            };
        }
    }
} 