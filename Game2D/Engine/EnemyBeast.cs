using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;
using System.Windows;

namespace Game2D.Engine
{
    public class EnemyBeast : GameObject
    {
        public int Health { get; set; } = 100;
        private double speed = 2.5 / 2.3;
        private Hero _hero;
        private int damage = 20;
        private int attackCooldown = 30;
        private int attackTimer = 0;
        private BitmapImage[] walkFrames;
        private int animationIndex = 0;
        private int animationSpeed = 1;
        private int frameCounter = 0;
        private int frameDelay = 8; // задержка между кадрами анимации

        public EnemyBeast(Hero hero, double x, double y)
        {
            X = x;
            Y = y;
            _hero = hero;
            walkFrames = new BitmapImage[4];
            for (int i = 0; i < 4; i++)
            {
                walkFrames[i] = new BitmapImage(new Uri($"pack://application:,,,/Project/images/enemy_go_{i + 1}.png"));
            }
            Sprite = new Image
            {
                Source = walkFrames[0],
                Width = 48,
                Height = 48
            };
        }

        public EnemyBeast(Hero hero, double x, double y, double coef)
            : this(hero, x, y)
        {
            this.Health = (int)(120 * coef);
            this.damage = (int)(50 * coef);
            // Можно добавить другие параметры, если нужно
        }

        private double animationTimer = 0;
        private const double AnimationSpeed = 0.1; // Скорость анимации

        public override Rect GetBounds()
        {
            return new Rect(X, Y, Sprite.Width, Sprite.Height);
        }

        public override void Update()
        {
            if (!IsActive && Sprite.Parent is Canvas canvas) { canvas.Children.Remove(Sprite); return; }
            if (_hero == null || !_hero.IsAlive) return;
            attackTimer = Math.Max(0, attackTimer - 1);
    
            double dx = _hero.X - X;
            double dy = _hero.Y - Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);
    
            if (dist > 1)
            {
                double stepX = speed * dx / dist;
                double stepY = speed * dy / dist;
                X += stepX;
                Y += stepY;
                // Анимация только при движении
                animationTimer += AnimationSpeed;
                if (animationTimer >= walkFrames.Length)
                    animationTimer = 0;
                Sprite.Source = walkFrames[(int)animationTimer];
            }
    
            if (IsCollidingWith(_hero) && attackTimer == 0)
            {
                _hero.Health -= damage;
                attackTimer = attackCooldown;
            }
        }

        private bool IsCollidingWith(GameObject other)
        {
            return GetBounds().IntersectsWith(other.GetBounds());
        }

        public void TakeDamage(int dmg)
        {
            Health -= dmg;
            if (Health <= 0)
            {
                if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                // Удаляем все кадры анимации, если они вдруг были добавлены
                if (walkFrames != null)
                {
                    foreach (var frame in walkFrames)
                    {
                        if (frame != null && Sprite.Parent is Canvas c) c.Children.Remove(Sprite);
                    }
                }
                var world = Game2D.MainWindow.CurrentGameWorld;
                world?.AddScore(100);
                IsActive = false;
            }
        }

        // Проверка возможности движения по смещению dx, dy
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
            // Убираем любые проверки на стены и препятствия
            return true;
        }
    }
} 