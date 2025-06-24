using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;

namespace Game2D.Engine
{
    public class Shoot : GameObject
    {
        private double dx, dy;
        private int lifetime = 300;
        private int damage = 25;
        private static int activeBulletCount = 0;

        public Shoot(int angle)
        {
            Sprite = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Project/images/bullet.png")),
                Width = 20,
                Height = 20
            };

            double rad = angle * Math.PI / 180;
            dx = Math.Cos(rad) * 4;
            dy = Math.Sin(rad) * 4;
            activeBulletCount++;
        }

        public static int GetActiveBulletCount()
        {
            return activeBulletCount;
        }

        public static void DecreaseBulletCount()
        {
            activeBulletCount--;
        }

        public override void Update()
        {
            lifetime--;
            if (lifetime <= 0)
            {
                RemoveSelf();
                return;
            }

            X += dx;
            Y += dy;

            if (IsOutOfBounds())
            {
                RemoveSelf();
                return;
            }

            CheckHeroCollision();
        }

        private bool IsOutOfBounds()
        {
            var world = MainWindow.CurrentGameWorld;
            if (world == null) return true;

            return X < 0 || Y < 0 || X > world._canvas.ActualWidth || Y > world._canvas.ActualHeight;
        }

        private void CheckHeroCollision()
        {
            var world = MainWindow.CurrentGameWorld;
            if (world == null) return;

            foreach (var gameObject in world.gameObjects)
            {
                if (gameObject is Hero hero && hero.IsAlive)
                {
                    if (this.GetBounds().IntersectsWith(hero.GetBounds()))
                    {
                        hero.Health -= damage;
                        RemoveSelf();
                        return;
                    }
                }
            }
        }

        private void RemoveSelf()
        {
            DecreaseBulletCount();
            if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
            IsActive = false;
        }
    }
} 