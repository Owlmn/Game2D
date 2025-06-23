using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;

namespace Game2D.Engine
{
    public class Wall_gorizont : GameObject
    {
        public Wall_gorizont(double x, double y)
        {
            X = x;
            Y = y;
            Sprite = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Project/images/wall_.png")),
                Width = 48,
                Height = 48
            };
        }
    }
} 