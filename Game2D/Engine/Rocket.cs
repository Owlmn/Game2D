using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;

namespace Game2D.Engine
{
    public class Rocket : GameObject
    {
        private double dx, dy;
        private const double Speed = 10;
        private int damage = 40;
        public Rocket(double x, double y, double angle)
        {
            X = x;
            Y = y;
            dx = Math.Cos(angle) * Speed;
            dy = Math.Sin(angle) * Speed;
            Sprite = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Project/images/rocket.png")),
                Width = 32,
                Height = 32
            };
            Sprite.RenderTransform = new System.Windows.Media.RotateTransform(angle * 180 / Math.PI, 16, 16);
        }
        public override void Update()
        {
            X += dx;
            Y += dy;
            var mainWindow = System.Windows.Application.Current.MainWindow as Game2D.MainWindow;
            if (mainWindow != null && mainWindow.GameCanvas != null)
            {
                double w = mainWindow.GameCanvas.ActualWidth;
                double h = mainWindow.GameCanvas.ActualHeight;
                if (X < -Sprite.Width || X > w || Y < -Sprite.Height || Y > h)
                    IsActive = false;
            }
            var world = Game2D.MainWindow.CurrentGameWorld;
            if (world != null)
            {
                foreach (var obj in world.Objects)
                {
                    if (obj is Wall wall && wall.IsActive && this.GetBounds().IntersectsWith(wall.GetBounds()))
                    {
                        this.IsActive = false;
                        break;
                    }
                    if (obj is Hero hero && hero.IsAlive && this.GetBounds().IntersectsWith(hero.GetBounds()))
                    {
                        hero.Health -= damage;
                        this.IsActive = false;
                        break;
                    }
                }
            }
        }
    }
} 