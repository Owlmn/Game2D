using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;

namespace Game2D.Engine
{
    public class ShooterHand2 : GameObject
    {
        public ShooterHand2(double x, double y)
        {
            X = x;
            Y = y;
            Sprite = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Project/images/boss_hand2.png")),
                Width = 48,
                Height = 48
            };
        }
    }
} 