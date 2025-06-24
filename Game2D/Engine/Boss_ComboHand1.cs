using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;

namespace Game2D.Engine
{
    public class Boss_ComboHand1 : GameObject
    {
        private Hero _hero;
        private double speed = 1.5;
        private int damage = 10;
        private int health = 150;
        private int difficultyLevel = 1;
        private int attackOrder = 1;
        private double targetX, targetY;
        private int moveTimer = 0;
        private int moveDuration = 240;
        private bool isAttacking = false;

        public bool IsDead => health <= 0;

        public Boss_ComboHand1(double x, double y, Hero hero, int order)
        {
            X = x;
            Y = y;
            _hero = hero;
            attackOrder = order;
            targetX = x;
            targetY = y;
            
            Sprite = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Project/images/boss_hand1.png")),
                Width = 64,
                Height = 64
            };
        }

        public void SetDamage(int coef)
        {
            damage = coef;
        }

        public void SetDifficultyLevel(int level)
        {
            difficultyLevel = level;
            speed = 1.5 + level * 0.5;
        }

        public void SetAttackOrder(int order)
        {
            attackOrder = order;
        }

        public void MoveTowards(int tx, int ty)
        {
            targetX = tx;
            targetY = ty;
            moveTimer = 0;
            isAttacking = true;
        }

        public double DistanceTo(int x, int y)
        {
            double dx = X - x;
            double dy = Y - y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public Hero GetNearestHero()
        {
            return _hero;
        }

        public override void Update()
        {
            if (IsDead || _hero == null || !_hero.IsAlive) return;

            if (isAttacking)
            {
                // Движение к цели
                moveTimer++;
                if (moveTimer < moveDuration)
                {
                    double dx = targetX - X;
                    double dy = targetY - Y;
                    double dist = Math.Sqrt(dx * dx + dy * dy);
                    if (dist > 1)
                    {
                        X += dx / dist * speed;
                        Y += dy / dist * speed;
                    }
                }
                else
                {
                    isAttacking = false;
                    CheckHeroCollision();
                }
            }
            else
            {
                PatrolMovement();
            }

            CheckHeroCollision();
        }

        private void PatrolMovement()
        {
            double time = DateTime.Now.Ticks / 10000000.0;
            double radius = 150;
            double centerX = 600;
            double centerY = 100;
            
            X = centerX + Math.Cos(time * 0.5) * radius;
            Y = centerY + Math.Sin(time * 0.3) * radius;
        }

        public void CheckHeroCollision()
        {
            if (_hero != null && this.GetBounds().IntersectsWith(_hero.GetBounds()))
            {
                _hero.Health -= damage;
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