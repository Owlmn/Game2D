using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;

namespace Game2D.Engine
{
    public class Portal3 : GameObject
    {
        public Portal3(double x, double y)
        {
            X = x;
            Y = y;
            Sprite = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Project/images/portal3.png")),
                Width = 64,
                Height = 64
            };
        }
    }
} 