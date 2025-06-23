using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;

namespace Game2D.Engine
{
    public class Boss_Hand2 : GameObject
    {
        private Hero _hero;
        private double speed = 1;
        private int damage = 5;
        private int health = 100;
        public bool IsDead => health <= 0;
        public void TakeDamage(int dmg)
        {
            health -= dmg;
            if (health <= 0)
            {
                if (Sprite.Parent is Canvas canvas) canvas.Children.Remove(Sprite);
                IsActive = false;
            }
        }
        public Boss_Hand2(double x, double y, Hero hero)
        {
            X = x;
            Y = y;
            _hero = hero;
            Sprite = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Project/images/boss_hand2.png")),
                Width = 128,
                Height = 128
            };
        }
        public override void Update()
        {
            if (_hero == null || !_hero.IsAlive) return;
            // Движение к герою
            double dx = _hero.X - X;
            double dy = _hero.Y - Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            if (dist > 1)
            {
                X += dx / dist * speed;
                Y += dy / dist * speed;
            }
            // Проверка коллизии
            if (this.GetBounds().IntersectsWith(_hero.GetBounds()))
            {
                _hero.Health -= damage;
            }
        }
    }
} 