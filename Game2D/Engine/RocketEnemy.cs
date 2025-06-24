using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;
using System.Windows;

namespace Game2D.Engine
{
    public class RocketEnemy : GameObject
    {
        private Hero _hero;
        private double koef = 1;
        private int shootCooldown = 0;
        private int shootDelay = 40; 
        private int health = 150;
        private const int DETECTION_RANGE = 400;
        private const int SHOOTING_RANGE = 250;
        private const int MIN_DISTANCE = 200;
        private const int FORGET_RANGE = 450;
        private double speed = 2 / 2.3;
        private BitmapImage[] shootFrames;
        private int currentFrame = 0;
        private int animationDelay = 10;
        private int animationTimer = 0;
        private bool isAnimating = false;
        private bool isActive = false;
        private bool isInShootingRange = false;

        public RocketEnemy(Hero hero, double x, double y, double hpCoef, double dmgCoef, double speedCoef)
            : this(hero, x, y)
        {
            this.health = (int)(150 * hpCoef);
            this.speed = (2 / 2.3) * speedCoef;
        }

        public RocketEnemy(Hero hero, double x, double y, double coef = 1)
        {
            X = x;
            Y = y;
            _hero = hero;
            koef = coef;
            health = (int)(150 * koef);
            shootFrames = new BitmapImage[3];
            for (int i = 0; i < 3; i++)
                shootFrames[i] = new BitmapImage(new Uri($"pack://application:,,,/Project/images/launcher_{i + 1}.png"));
            Sprite = new Image
            {
                Source = shootFrames[0],
                Width = 48,
                Height = 48
            };
        }

        public override Rect GetBounds()
        {
            return new Rect(X, Y, Sprite.Width, Sprite.Height);
        }

        public override void Update()
        {
            if (!IsActive && Sprite.Parent is Canvas canvas) { canvas.Children.Remove(Sprite); return; }
            if (_hero == null || !_hero.IsAlive) return;
            double dx = _hero.X - X;
            double dy = _hero.Y - Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            if (!isActive && distance <= DETECTION_RANGE) isActive = true;
            else if (isActive && distance > FORGET_RANGE) ResetBehavior();
            isInShootingRange = (distance <= SHOOTING_RANGE);
            if (isActive)
            {
                if (distance > MIN_DISTANCE)
                {
                    double stepX = speed * dx / distance;
                    double stepY = speed * dy / distance;
                    X += stepX;
                    Y += stepY;
                    double angle = Math.Atan2(dy, dx) * 180 / Math.PI;
                    Sprite.RenderTransform = new System.Windows.Media.RotateTransform(angle, Sprite.Width/2, Sprite.Height/2);
                }
                if (isInShootingRange && !isAnimating)
                {
                    shootCooldown++;
                    if (shootCooldown >= shootDelay)
                    {
                        shootCooldown = 0;
                        isAnimating = true;
                        currentFrame = 0;
                        animationTimer = 0;
                    }
                }
                if (isAnimating)
                {
                    AnimateShoot();
                }
            }
        }
        private void AnimateShoot()
        {
            animationTimer++;
            if (animationTimer >= animationDelay)
            {
                animationTimer = 0;
                currentFrame++;
                if (currentFrame < shootFrames.Length)
                {
                    Sprite.Source = shootFrames[currentFrame];
                    if (currentFrame == 2)
                    {
                        SpawnRocket();
                    }
                }
                else
                {
                    currentFrame = 0;
                    isAnimating = false;
                    Sprite.Source = shootFrames[0];
                }
            }
        }
        private void SpawnRocket()
        {
            double dx = _hero.X + _hero.Sprite.Width / 2 - (X + Sprite.Width / 2);
            double dy = _hero.Y + _hero.Sprite.Height / 2 - (Y + Sprite.Height / 2);
            double angle = Math.Atan2(dy, dx);
            var world = Game2D.MainWindow.CurrentGameWorld;
            if (world != null)
            {
                var canvas = world._canvas;
                var rocket = new Rocket(canvas, X + Sprite.Width / 2, Y + Sprite.Height / 2, angle);
                world.AddObject(rocket);
            }
        }
        private void ResetBehavior()
        {
            isActive = false;
            isAnimating = false;
            isInShootingRange = false;
            Sprite.Source = shootFrames[0];
        }
        public void TakeDamage(int dmg)
        {
            health -= dmg;
            if (health <= 0)
            {
                if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                var world = Game2D.MainWindow.CurrentGameWorld;
                world?.AddScore(130);
                IsActive = false;
            }
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
    }
} 