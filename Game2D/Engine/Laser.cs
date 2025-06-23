using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;

namespace Game2D.Engine
{
    public class Laser : GameObject
    {
        private double dx, dy;
        private const double Speed = 8;
        private int damage = 75;
        public Laser(Canvas canvas, double x, double y, double angle)
            : base(canvas)
        {
            X = x;
            Y = y;
            dx = Math.Cos(angle) * Speed;
            dy = Math.Sin(angle) * Speed;
            Sprite = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Project/images/laser.png")),
                Width = 32,
                Height = 8
            };
            Sprite.RenderTransform = new System.Windows.Media.RotateTransform(angle * 180 / Math.PI, 16, 4);
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
                {
                    if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                    IsActive = false;
                }
            }
            var world = Game2D.MainWindow.CurrentGameWorld;
            if (world != null)
            {
                foreach (var obj in world.Objects)
                {
                    if (obj is EnemyBeast enemy && enemy.IsActive && this.GetBounds().IntersectsWith(enemy.GetBounds()))
                    {
                        enemy.TakeDamage(damage);
                        if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                        this.IsActive = false;
                        break;
                    }
                    if (obj is CosmoEnemy cosmo && cosmo.IsActive && this.GetBounds().IntersectsWith(cosmo.GetBounds()))
                    {
                        cosmo.TakeDamage(damage);
                        if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                        this.IsActive = false;
                        break;
                    }
                    if (obj is ShotgunEnemy shot && shot.IsActive && this.GetBounds().IntersectsWith(shot.GetBounds()))
                    {
                        shot.TakeDamage(damage);
                        if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                        this.IsActive = false;
                        break;
                    }
                    if (obj is RocketEnemy rocket && rocket.IsActive && this.GetBounds().IntersectsWith(rocket.GetBounds()))
                    {
                        rocket.TakeDamage(damage);
                        if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                        this.IsActive = false;
                        break;
                    }
                    if (obj is Zombi zombi && zombi.IsActive && this.GetBounds().IntersectsWith(zombi.GetBounds()))
                    {
                        zombi.TakeDamage(damage);
                        if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                        this.IsActive = false;
                        break;
                    }
                    if (obj is Boss boss && this.GetBounds().IntersectsWith(boss.GetBounds()))
                    {
                        if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                        this.IsActive = false;
                        break;
                    }
                }
            }
        }
    }
} 