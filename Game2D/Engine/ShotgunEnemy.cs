using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;
using System.Windows;

namespace Game2D.Engine
{
    public class ShotgunEnemy : GameObject
    {
        private Hero _hero;
        private double koef = 1;
        private int shootCooldown = 0;
        private int shootDelay = 45; // ~0.75 сек при 60 FPS
        private int health = 150;
        private const int DETECTION_RANGE = 250;
        private const int SHOOTING_RANGE = 200;
        private double speed = 2 / 2.3;
        private bool isActive = false;
        private int frameDelay = 8; // задержка между кадрами анимации
        private BitmapImage[] walkFrames;
        private int currentFrame = 0;
        private int animationTimer = 0;
        private double PrevX;
        private double PrevY;

        public ShotgunEnemy(Hero hero, double x, double y, double coef = 1)
        {
            X = x;
            Y = y;
            _hero = hero;
            koef = coef;
            health = (int)(150 * koef);
            // Если есть несколько кадров, используем их, иначе один
            walkFrames = new BitmapImage[1];
            walkFrames[0] = new BitmapImage(new Uri("pack://application:,,,/Project/images/shouterenemy.png"));
            Sprite = new Image
            {
                Source = walkFrames[0],
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
            if (isActive)
            {
                if (distance > 1)
                {
                    double stepX = speed * dx / distance;
                    double stepY = speed * dy / distance;
                    X += stepX;
                    Y += stepY;
                    // Анимация (если есть несколько кадров)
                    animationTimer++;
                    if (animationTimer >= frameDelay)
                    {
                        animationTimer = 0;
                        currentFrame = (currentFrame + 1) % walkFrames.Length;
                        Sprite.Source = walkFrames[currentFrame];
                    }
                }
                // Стрельба
                if (distance <= SHOOTING_RANGE)
                {
                    shootCooldown++;
                    if (shootCooldown >= shootDelay)
                    {
                        shootCooldown = 0;
                        ShootAtHero();
                    }
                }
            }
        }
        private void ShootAtHero()
        {
            double dx = _hero.X + _hero.Sprite.Width / 2 - (X + Sprite.Width / 2);
            double dy = _hero.Y + _hero.Sprite.Height / 2 - (Y + Sprite.Height / 2);
            double angle = Math.Atan2(dy, dx);
            var world = Game2D.MainWindow.CurrentGameWorld;
            if (world != null)
            {
                for (int i = -1; i <= 1; i++)
                {
                    var pellet = new ShotgunPellet(X + Sprite.Width / 2, Y + Sprite.Height / 2, angle + i * 0.25);
                    world.AddObject(pellet);
                }
            }
        }
        public void TakeDamage(int dmg)
        {
            health -= dmg;
            if (health <= 0)
            {
                if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                IsActive = false;
            }
        }
    }
} 