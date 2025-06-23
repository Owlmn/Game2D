using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;

namespace Game2D.Engine
{
    public class Poison : GameObject
    {
        public Poison(double x, double y)
        {
            X = x;
            Y = y;
            Sprite = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Project/images/кислота.png")),
                Width = 48,
                Height = 48
            };
        }
    }
} 