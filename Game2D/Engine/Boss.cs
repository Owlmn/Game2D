using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.Generic;
using System;

namespace Game2D.Engine
{
    public class Boss : GameObject
    {
        private Hero _hero;
        private double koef = 1;
        private int maxHealth = 1500;
        private int health;
        private int attackCooldown = 180;
        private int attackTimer = 0;
        private Image bossSprite;
        private Rectangle hpBar;
        private double hpBarWidth = 400;
        private double hpBarHeight = 24;
        public bool IsDead => health <= 0;
        
        private int difficultyLevel = 1;
        private double attackSpeedMultiplier = 1.0;
        private Queue<string> attackQueue = new Queue<string>();
        private int currentAttackPhase = 0;
        private int phaseTimer = 0;
        private int phaseDuration = 900;
        private double floatOffset = 0;
        private double floatSpeed = 0.05;
        
        private readonly string[] attackTypes = { "vertical", "horizontal", "diagonal", "circle", "bulletSpam", "specialCombo" };

        public Boss(Hero hero, double x, double y, double coef = 1)
        {
            X = x;
            Y = y;
            _hero = hero;
            koef = coef;
            maxHealth = (int)(1500 * koef);
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

            InitializeAttackQueue();
        }

        private void InitializeAttackQueue()
        {
            attackQueue.Clear();
            attackQueue.Enqueue("vertical");
            attackQueue.Enqueue("horizontal");
            attackQueue.Enqueue("diagonal");
            attackQueue.Enqueue("circle");
            
            if (difficultyLevel >= 2)
            {
                attackQueue.Enqueue("bulletSpam");
            }
            if (difficultyLevel >= 3)
            {
                attackQueue.Enqueue("specialCombo");
            }
        }

        public override void Update()
        {
            if (IsDead) return;
            
            UpdateDifficulty();
            
            FloatUpDown();
            
            phaseTimer++;
            if (phaseTimer >= phaseDuration)
            {
                phaseTimer = 0;
                currentAttackPhase++;
                StartNextAttack();
            }
            
            attackTimer++;
            if (attackTimer >= attackCooldown * attackSpeedMultiplier)
            {
                attackTimer = 0;
                PerformCurrentAttack();
            }
        }

        private void UpdateDifficulty()
        {
            double healthPercent = (double)health / maxHealth;
            if (healthPercent <= 0.2 && difficultyLevel < 4)
            {
                difficultyLevel = 4;
                attackSpeedMultiplier = 1.5;
            }
            else if (healthPercent <= 0.4 && difficultyLevel < 3)
            {
                difficultyLevel = 3;
                attackSpeedMultiplier = 1.3;
            }
            else if (healthPercent <= 0.6 && difficultyLevel < 2)
            {
                difficultyLevel = 2;
                attackSpeedMultiplier = 1.1;
            }
        }

        private void FloatUpDown()
        {
            floatOffset += floatSpeed;
            double floatY = Y + Math.Sin(floatOffset) * 5;
            Y = floatY;
        }

        private void StartNextAttack()
        {
            if (attackQueue.Count > 0)
            {
                string nextAttack = attackQueue.Dequeue();
                attackQueue.Enqueue(nextAttack);
                
                
                switch (nextAttack)
                {
                    case "vertical":
                        AttackVertical(difficultyLevel);
                        break;
                    case "horizontal":
                        AttackHorizontal(difficultyLevel);
                        break;
                    case "diagonal":
                        AttackDiagonal(difficultyLevel);
                        break;
                    case "circle":
                        AttackCircle(difficultyLevel);
                        break;
                    case "bulletSpam":
                        AttackBulletSpam();
                        break;
                    case "specialCombo":
                        AttackSpecialCombo();
                        break;
                }
            }
        }

        private void PerformCurrentAttack()
        {
            Attack();
        }

        private void AttackVertical(int level)
        {
            var world = MainWindow.CurrentGameWorld;
            if (world == null) return;

            int bulletCount = Math.Min(level + 1, 4);
            double spread = 0.2;
            
            for (int i = 0; i < bulletCount; i++)
            {
                double angle = -Math.PI/2 + (i - bulletCount/2.0) * spread;
                var rocket = new Rocket(world._canvas, X + Sprite.Width/2, Y + Sprite.Height/2, angle);
                world.AddObject(rocket);
            }
        }

        private void AttackHorizontal(int level)
        {
            var world = MainWindow.CurrentGameWorld;
            if (world == null) return;

            int bulletCount = Math.Min(level + 2, 5);
            double spread = 0.3;
            
            for (int i = 0; i < bulletCount; i++)
            {
                double angle = (i - bulletCount/2.0) * spread;
                var rocket = new Rocket(world._canvas, X + Sprite.Width/2, Y + Sprite.Height/2, angle);
                world.AddObject(rocket);
            }
        }

        private void AttackDiagonal(int level)
        {
            var world = MainWindow.CurrentGameWorld;
            if (world == null) return;
            double[] angles = { Math.PI/4, 3*Math.PI/4, 5*Math.PI/4, 7*Math.PI/4 };
            
            foreach (double angle in angles)
            {
                var rocket = new Rocket(world._canvas, X + Sprite.Width/2, Y + Sprite.Height/2, angle);
                world.AddObject(rocket);
            }
        }

        private void AttackCircle(int level)
        {
            var world = MainWindow.CurrentGameWorld;
            if (world == null) return;
            
            int bulletCount = 6 + level;
            double angleStep = 2 * Math.PI / bulletCount;
            
            for (int i = 0; i < bulletCount; i++)
            {
                double angle = i * angleStep;
                var rocket = new Rocket(world._canvas, X + Sprite.Width/2, Y + Sprite.Height/2, angle);
                world.AddObject(rocket);
            }
        }

        private void AttackBulletSpam()
        {
            var world = MainWindow.CurrentGameWorld;
            if (world == null) return;

            int pattern = new Random().Next(3) + 1;
            int offset = 400;

            switch (pattern)
            {
                case 1:
                    var hand1 = new ShooterHand1(X + offset, Y, _hero, koef);
                    world.AddGameObject(hand1);

                    var hand2 = new ShooterHand2(X - offset, Y, _hero, koef);
                    world.AddGameObject(hand2);
                    break;

                case 2:
                    var diagHand1 = new ShooterHand1(X - offset, Y + offset, _hero, koef);
                    world.AddGameObject(diagHand1);

                    var diagHand2 = new ShooterHand2(X + offset, Y + offset, _hero, koef);
                    world.AddGameObject(diagHand2);
                    break;

                case 3:
                    var topLeft = new ShooterHand2(X - offset, Y, _hero, koef);
                    world.AddGameObject(topLeft);

                    var topRight = new ShooterHand1(X + offset, Y, _hero, koef);
                    world.AddGameObject(topRight);

                    var bottomLeft = new ShooterHand1(X - offset, Y + offset + 50, _hero, koef);
                    world.AddGameObject(bottomLeft);

                    var bottomRight = new ShooterHand2(X + offset, Y + offset + 50, _hero, koef);
                    world.AddGameObject(bottomRight);
                    break;
            }
        }

        private void AttackSpecialCombo()
        {
            var world = MainWindow.CurrentGameWorld;
            if (world == null) return;
            
            double bossX = X + Sprite.Width/2;
            double bossY = Y + Sprite.Height/2;
            
            var comboHand1 = new Boss_ComboHand1(bossX - 100, bossY + 50, _hero, 1);
            var comboHand2 = new Boss_ComboHand2(bossX + 100, bossY + 50, _hero, 2);
            
            world.AddObject(comboHand1);
            world.AddObject(comboHand2);
        }

        private void Attack()
        {
            double dx = _hero.X + _hero.Sprite.Width / 2 - (X + Sprite.Width / 2);
            double dy = _hero.Y + _hero.Sprite.Height / 2 - (Y + Sprite.Height / 2);
            double angle = Math.Atan2(dy, dx);
            var world = MainWindow.CurrentGameWorld;
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

        public int GetDifficultyLevel() => difficultyLevel;
        public double GetAttackSpeedMultiplier() => attackSpeedMultiplier;

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);
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