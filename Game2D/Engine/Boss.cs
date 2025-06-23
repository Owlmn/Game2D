using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media;
using System;

namespace Game2D.Engine
{
    public class Boss : GameObject
    {
        private Hero _hero;
        private double koef = 1;
        private int maxHealth = 2000;
        private int health;
        private int attackCooldown = 120; // 2 секунды
        private int attackTimer = 0;
        private Image bossSprite;
        private Rectangle hpBar;
        private double hpBarWidth = 400;
        private double hpBarHeight = 24;
        public bool IsDead => health <= 0;

        public Boss(Hero hero, double x, double y, double coef = 1)
        {
            X = x;
            Y = y;
            _hero = hero;
            koef = coef;
            maxHealth = (int)(2000 * koef);
            health = maxHealth;
            bossSprite = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Project/images/boss.png")),
                Width = 128,
                Height = 128
            };
            Sprite = bossSprite;
            hpBar = new Rectangle
            {
                Width = hpBarWidth,
                Height = hpBarHeight,
                Fill = Brushes.Red,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
        }

        public override void Update()
        {
            // Атака (например, ракеты)
            attackTimer++;
            if (attackTimer >= attackCooldown)
            {
                attackTimer = 0;
                Attack();
            }
        }
        private void Attack()
        {
            // Спавним ракету в сторону героя
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
        public void TakeDamage(int dmg)
        {
            health -= dmg;
            if (health < 0) health = 0;
        }
        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);
            // Рисуем полоску HP босса поверх Canvas
            if (hpBar != null)
            {
                double percent = (double)health / maxHealth;
                hpBar.Width = hpBarWidth * percent;
                Canvas.SetLeft(hpBar, (canvas.ActualWidth - hpBarWidth) / 2);
                Canvas.SetTop(hpBar, 30);
                if (!canvas.Children.Contains(hpBar))
                    canvas.Children.Add(hpBar);
            }
        }
    }
} 