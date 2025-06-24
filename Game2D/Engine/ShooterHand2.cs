using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;
using System.Collections.Generic;

namespace Game2D.Engine
{
    public class ShooterHand2 : GameObject
    {
        private Hero _hero;
        private int lifetime = 300;
        private int cooldown = 30;
        private int shootCount = 0;
        private int maxShots = 5;
        private int frameCount = 0;
        private int spreadAngle = 30;
        private double koef = 1;
        private static int activeBulletCount = 0;
        private const int maxBullets = 250;

        public ShooterHand2(double x, double y, Hero hero, double coefficient = 1.0)
        {
            X = x;
            Y = y;
            _hero = hero;
            koef = coefficient;
            
            Sprite = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Project/images/boss_hand2.png")),
                Width = 120,
                Height = 120
            };
        }

        public void SetCoefficient(double coefficient)
        {
            koef = coefficient;
        }

        public override void Update()
        {
            if (_hero == null || !_hero.IsAlive) return;

            frameCount++;
            lifetime--;

            if (lifetime <= 0 || shootCount >= maxShots)
            {
                if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                IsActive = false;
                return;
            }

            if (frameCount % cooldown == 0)
            {
                ShootFanAtHero();
                shootCount++;
            }
        }

        private void ShootFanAtHero()
        {
            if (Shoot.GetActiveBulletCount() > 250 + (int)(koef * 50)) return;

            double deltaX = _hero.X - X;
            double deltaY = _hero.Y - Y;
            double baseAngle = Math.Atan2(deltaY, deltaX) * 180 / Math.PI;
            int randomSpread = 30 + new Random().Next(16);

            int bulletsInFan = (int)(6 * koef);
            for (int i = 0; i < bulletsInFan; i++)
            {
                if (Shoot.GetActiveBulletCount() >= 250 + (int)(koef * 50)) break;

                int angle = (int)(baseAngle - randomSpread / 2 + (randomSpread * i) / (bulletsInFan - 1));
                var bullet = new Shoot(angle);
                bullet.X = X;
                bullet.Y = Y;
                
                var world = MainWindow.CurrentGameWorld;
                if (world != null)
                {
                    world.AddGameObject(bullet);
                }
            }
        }

        public static void DecreaseBulletCount()
        {
            activeBulletCount--;
        }

        public static int GetActiveBulletCount()
        {
            return activeBulletCount;
        }
    }
} 