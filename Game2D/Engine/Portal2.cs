using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;

namespace Game2D.Engine
{
    public class Portal2 : GameObject
    {
        public Portal2(double x, double y)
        {
            X = x;
            Y = y;
            Sprite = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Project/images/portal2.jpg")),
                Width = 64,
                Height = 64
            };
        }
    }
} 