using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;

namespace Game2D.Engine
{
    public class Boss_Hand1 : GameObject
    {
        private Hero _hero;
        private double speed = 1.2;
        private int damage = 8;
        private int health = 120;
        private int moveType = 0;
        private int direction = 1;
        private double patrolRadius = 80;
        private double patrolCenterX, patrolCenterY;
        private double patrolAngle = 0;

        public bool IsDead => health <= 0;

        public Boss_Hand1(double x, double y, Hero hero)
        {
            X = x;
            Y = y;
            _hero = hero;
            patrolCenterX = x;
            patrolCenterY = y;
            
            Sprite = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Project/images/boss_hand1.png")),
                Width = 120,
                Height = 120
            };
        }

        public void SetDamage(int coef)
        {
            damage = coef;
        }

        public void SetSpeed(int speed)
        {
            this.speed = speed;
        }

        public void SetMoveType(int type)
        {
            moveType = type;
        }

        public void SetDirection(int dir)
        {
            direction = dir;
        }

        public override void Update()
        {
            if (_hero == null || !_hero.IsAlive) return;

            MovePattern();
            CheckOutOfBounds();
            CheckHeroCollision();
        }

        public void MovePattern()
        {
            switch (moveType)
            {
                case 0: 
                    ChaseHero();
                    break;
                case 1: 
                    PatrolMovement();
                    break;
                case 2: 
                    AttackMovement();
                    break;
            }
        }

        private void ChaseHero()
        {
            double dx = _hero.X - X;
            double dy = _hero.Y - Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            if (dist > 1)
            {
                X += dx / dist * speed;
                Y += dy / dist * speed;
            }
        }

        private void PatrolMovement()
        {
            patrolAngle += 0.02 * direction;
            X = patrolCenterX + Math.Cos(patrolAngle) * patrolRadius;
            Y = patrolCenterY + Math.Sin(patrolAngle) * patrolRadius;
        }

        private void AttackMovement()
        {
            double dx = _hero.X - X;
            double dy = _hero.Y - Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            if (dist > 1)
            {
                X += dx / dist * speed * 2;
                Y += dy / dist * speed * 2;
            }
        }

        public bool CheckOutOfBounds()
        {
            var world = MainWindow.CurrentGameWorld;
            if (world == null) return false;

            double maxX = world._canvas.ActualWidth - Sprite.Width;
            double maxY = world._canvas.ActualHeight - Sprite.Height;

            if (X < 0 || Y < 0 || X > maxX || Y > maxY)
            {
                X = Math.Max(0, Math.Min(X, maxX));
                Y = Math.Max(0, Math.Min(Y, maxY));
                return true;
            }
            return false;
        }

        public void CheckHeroCollision()
        {
            if (_hero != null && this.GetBounds().IntersectsWith(_hero.GetBounds()))
            {
                _hero.Health -= damage;
            }
        }

        public Hero GetNearestHero()
        {
            return _hero;
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