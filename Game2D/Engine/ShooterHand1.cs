using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;

namespace Game2D.Engine
{
    public class ShooterHand1 : GameObject
    {
        public ShooterHand1(double x, double y)
        {
            X = x;
            Y = y;
            Sprite = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Project/images/boss_hand1.png")),
                Width = 48,
                Height = 48
            };
        }
    }
} 