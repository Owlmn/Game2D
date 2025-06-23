using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;

namespace Game2D.Engine
{
    public class Lasergun : GameObject
    {
        public Lasergun(double x, double y)
        {
            X = x;
            Y = y;
            Sprite = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Project/images/lasergun.png")),
                Width = 48,
                Height = 48
            };
        }
    }
} 