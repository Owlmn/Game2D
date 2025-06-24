using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;
using System.Windows;

namespace Game2D.Engine
{
    public class CosmoEnemy : GameObject
    {
        private Hero _hero;
        private double koef = 1;
        private int shootCooldown = 0;
        private int shootDelay = 30;
        private int health = 120;
        private const int DETECTION_RANGE = 320;
        private const int SHOOTING_RANGE = 250;
        private double speed = 2 / 2.3;
        private BitmapImage[] walkFrames;
        private int currentFrame = 0;
        private int animationDelay = 5;
        private int animationTimer = 0;
        private bool isActive = false;
        private bool isInShootingRange = false;
        private int frameDelay = 8;

        public CosmoEnemy(Hero hero, double x, double y, double hpCoef, double dmgCoef, double speedCoef)
            : this(hero, x, y)
        {
            this.health = (int)(120 * hpCoef);
            this.speed = (2 / 2.3) * speedCoef;
        }

        public CosmoEnemy(Hero hero, double x, double y, double coef = 1)
        {
            X = x;
            Y = y;
            _hero = hero;
            koef = coef;
            health = (int)(120 * koef);
            walkFrames = new BitmapImage[8];
            for (int i = 0; i < 8; i++)
                walkFrames[i] = new BitmapImage(new Uri($"pack://application:,,,/Project/images/cosmo_walk_{i + 1}.png"));
            Sprite = new Image
            {
                Source = walkFrames[0],
                Width = 48,
                Height = 48
            };
        }
        
        private bool CanMove(double dx, double dy)
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as Game2D.MainWindow;
            if (mainWindow == null || mainWindow.GameCanvas == null) return false;
            double newX = X + dx;
            double newY = Y + dy;
            double maxX = mainWindow.GameCanvas.ActualWidth - Sprite.Width;
            double maxY = mainWindow.GameCanvas.ActualHeight - Sprite.Height;
            if (newX < 0 || newY < 0 || newX > maxX || newY > maxY)
                return false;
            var world = Game2D.MainWindow.CurrentGameWorld;
            if (world != null)
            {
                var futureRect = new System.Windows.Rect(newX, newY, Sprite.Width, Sprite.Height);
                foreach (var obj in world.Objects)
                {
                    if (!obj.IsActive) continue;
                    if (obj is Wall || obj is Wall_gorizont || obj is Wall_vertical)
                    {
                        if (futureRect.IntersectsWith(obj.GetBounds()))
                            return false;
                    }
                }
            }
            return true;
        }
        
        private const double AnimationSpeed = 0.1;
        public override void Update()
        {
            if (!IsActive && Sprite.Parent is Canvas canvas) { canvas.Children.Remove(Sprite); return; }
            if (_hero == null || !_hero.IsAlive) return;
            double dx = _hero.X - X;
            double dy = _hero.Y - Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
    
            if (!isActive && distance <= DETECTION_RANGE) isActive = true;
            isInShootingRange = (distance <= SHOOTING_RANGE);
    
            if (isActive)
            {
                if (distance > 1)
                {
                    double stepX = speed * dx / distance;
                    double stepY = speed * dy / distance;
                    if (CanMove(stepX, stepY))
                    {
                        X += stepX;
                        Y += stepY;
                        double angle = Math.Atan2(dy, dx) * 180 / Math.PI;
                        Sprite.RenderTransform = new System.Windows.Media.RotateTransform(angle, Sprite.Width/2, Sprite.Height/2);
                        animationTimer += (int)AnimationSpeed;
                        if (animationTimer >= walkFrames.Length)
                            animationTimer = 0;
                        Sprite.Source = walkFrames[(int)animationTimer];
                    }
                }
        
                // Стрельба
                int level = Game2D.MainWindow.CurrentLevelIndex;
                int localShootDelay = shootDelay;
                double bulletSpeed = 6;
                if (level == 1 || level == 2) { localShootDelay = 90; bulletSpeed = 3.5; } // MAP3, MAP4
                if (isInShootingRange)
                {
                    shootCooldown++;
                    if (shootCooldown >= localShootDelay)
                    {
                        shootCooldown = 0;
                        ShootAtHero(bulletSpeed);
                    }
                }
            }
        }
        
        private void ShootAtHero(double bulletSpeed = 6)
        {
            double dx = _hero.X + _hero.Sprite.Width / 2 - (X + Sprite.Width / 2);
            double dy = _hero.Y + _hero.Sprite.Height / 2 - (Y + Sprite.Height / 2);
            double angle = Math.Atan2(dy, dx);
            var world = Game2D.MainWindow.CurrentGameWorld;
            if (world != null)
            {
                var canvas = world._canvas;
                var bullet = new CosmoBullet(canvas, X + Sprite.Width / 2, Y + Sprite.Height / 2, angle, bulletSpeed);
                world.AddObject(bullet);
            }
        }
        public void TakeDamage(int dmg)
        {
            health -= dmg;
            if (health <= 0)
            {
                if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                if (walkFrames != null)
                {
                    foreach (var frame in walkFrames)
                    {
                        if (frame != null && Sprite.Parent is Canvas c) c.Children.Remove(Sprite);
                    }
                }
                var world = Game2D.MainWindow.CurrentGameWorld;
                world?.AddScore(150);
                IsActive = false;
            }
        }

        public override Rect GetBounds()
        {
            return new Rect(X, Y, Sprite.Width, Sprite.Height);
        }
    }
} 