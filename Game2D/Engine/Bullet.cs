using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Game2D.Engine
{
    public class Bullet : GameObject
    {
        private double dx, dy;
        private const double Speed = 15;

        public Bullet(Canvas canvas, double x, double y, double angle)
            : base(canvas)
        {
            X = x;
            Y = y;
            dx = System.Math.Cos(angle) * Speed;
            dy = System.Math.Sin(angle) * Speed;
            Sprite = new Image
            {
                Source = new BitmapImage(new System.Uri("pack://application:,,,/Project/images/bullet.png")),
                Width = 24,
                Height = 24
            };
            Sprite.RenderTransform = new System.Windows.Media.RotateTransform(angle * 180 / System.Math.PI, 12, 12);
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
                    if (!obj.IsActive) continue;
                    if (obj is Wall || obj is Wall_gorizont || obj is Wall_vertical)
                    {
                        if (this.GetBounds().IntersectsWith(obj.GetBounds()))
                        {
                            if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                            this.IsActive = false;
                            break;
                        }
                    }
                    if (obj is EnemyBeast enemy && enemy.IsActive && this.GetBounds().IntersectsWith(enemy.GetBounds()))
                    {
                        enemy.TakeDamage(50);
                        if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                        this.IsActive = false;
                        break;
                    }
                    if (obj is CosmoEnemy cosmo && cosmo.IsActive && this.GetBounds().IntersectsWith(cosmo.GetBounds()))
                    {
                        cosmo.TakeDamage(50);
                        if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                        this.IsActive = false;
                        break;
                    }
                    if (obj is ShotgunEnemy shot && shot.IsActive && this.GetBounds().IntersectsWith(shot.GetBounds()))
                    {
                        shot.TakeDamage(50);
                        if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                        this.IsActive = false;
                        break;
                    }
                    if (obj is RocketEnemy rocket && rocket.IsActive && this.GetBounds().IntersectsWith(rocket.GetBounds()))
                    {
                        rocket.TakeDamage(50);
                        if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                        this.IsActive = false;
                        break;
                    }
                    if (obj is Zombi zombi && zombi.IsActive && this.GetBounds().IntersectsWith(zombi.GetBounds()))
                    {
                        zombi.TakeDamage(50);
                        if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                        this.IsActive = false;
                        break;
                    }
                    if (obj is Boss boss && this.GetBounds().IntersectsWith(boss.GetBounds()))
                    {
                        boss.TakeDamage(50);
                        if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                        this.IsActive = false;
                        break;
                    }
                }
            }
        }
    }
} 