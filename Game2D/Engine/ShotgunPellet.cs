using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;

namespace Game2D.Engine
{
    public class ShotgunPellet : GameObject
    {
        private double dx, dy;
        private double speed;
        private int damage = 15;
        public ShotgunPellet(Canvas canvas, double x, double y, double angle, double speed = 8)
            : base(canvas)
        {
            X = x;
            Y = y;
            this.speed = speed;
            dx = Math.Cos(angle) * speed;
            dy = Math.Sin(angle) * speed;
            Sprite = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Project/images/shotgun_pellet.png")),
                Width = 16,
                Height = 16
            };
            Sprite.RenderTransform = new System.Windows.Media.RotateTransform(angle * 180 / Math.PI, 8, 8);
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
                        if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                        this.IsActive = false;
                        break;
                    }
                    if (obj is Hero hero && hero.IsAlive && this.GetBounds().IntersectsWith(hero.GetBounds()))
                    {
                        hero.Health -= damage;
                        if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                        this.IsActive = false;
                        break;
                    }
                }
            }
        }
    }
} 