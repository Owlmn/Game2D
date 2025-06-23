using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;
using System.Windows;

namespace Game2D.Engine
{
    public class Zombi : GameObject
    {
        private Hero _hero;
        private int health = 80;
        private double speed = 2 / 2.3;
        private int damage = 10;
        private int attackCooldown = 30;
        private int attackTimer = 0;
        private int frameDelay = 8; // задержка между кадрами анимации
        private BitmapImage[] walkFrames;
        private int currentFrame = 0;
        private int animationTimer = 0;
        public Zombi(Hero hero, double x, double y)
        {
            X = x;
            Y = y;
            _hero = hero;
            // Если есть несколько кадров, используем их, иначе один
            walkFrames = new BitmapImage[1];
            walkFrames[0] = new BitmapImage(new Uri("pack://application:,,,/Project/images/zombi.png"));
            Sprite = new Image
            {
                Source = walkFrames[0],
                Width = 48,
                Height = 48
            };
        }
        public Zombi(Hero hero, double x, double y, double coef)
            : this(hero, x, y)
        {
            this.health = (int)(120 * coef);
            this.damage = (int)(10 * coef);
            // Можно добавить другие параметры, если нужно
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
                // Анимация (если есть несколько кадров)
                animationTimer++;
                if (animationTimer >= frameDelay)
                {
                    animationTimer = 0;
                    currentFrame = (currentFrame + 1) % walkFrames.Length;
                    Sprite.Source = walkFrames[currentFrame];
                }
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
            health -= dmg;
            if (health <= 0)
            {
                if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                var world = Game2D.MainWindow.CurrentGameWorld;
                world?.AddScore(50);
                IsActive = false;
            }
        }
    }
} 