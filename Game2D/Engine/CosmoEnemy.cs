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
        private int shootDelay = 30; // ~0.5 сек при 60 FPS
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
        private int frameDelay = 8; // задержка между кадрами анимации

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

        // Проверка возможности движения по смещению dx, dy
        private bool CanMove(double dx, double dy)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow?.GameCanvas == null) return false;
    
            double newX = X + dx;
            double newY = Y + dy;
    
            // Проверка границ canvas
            if (newX < 0 || newY < 0 || 
                newX > mainWindow.GameCanvas.ActualWidth - Sprite.Width || 
                newY > mainWindow.GameCanvas.ActualHeight - Sprite.Height)
                return false;
    
            // Проверка столкновений со стенами
            var world = MainWindow.CurrentGameWorld;
            if (world != null)
            {
                Rect futureBounds = new Rect(newX, newY, Sprite.Width, Sprite.Height);
                foreach (var obj in world.Objects)
                {
                    if (obj is Wall wall && wall.IsActive && futureBounds.IntersectsWith(wall.GetBounds()))
                        return false;
                }
            }
            return true;
        }
        
        private const double AnimationSpeed = 0.1; // Скорость анимации
        public override void Update()
        {
            if (_hero == null || !_hero.IsAlive) return;
            double dx = _hero.X - X;
            double dy = _hero.Y - Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
    
            if (!isActive && distance <= DETECTION_RANGE) isActive = true;
            isInShootingRange = (distance <= SHOOTING_RANGE);
    
            if (isActive)
            {
                // Движение к герою
                if (distance > 1)
                {
                    double stepX = speed * dx / distance;
                    double stepY = speed * dy / distance;
            
                    if (CanMove(stepX, stepY))
                    {
                        X += stepX;
                        Y += stepY;
                
                        // Плавная анимация
                        animationTimer += (int)AnimationSpeed;
                        if (animationTimer >= walkFrames.Length)
                            animationTimer = 0;
                
                        Sprite.Source = walkFrames[(int)animationTimer];
                    }
                }
        
                // Стрельба
                if (isInShootingRange)
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
                var bullet = new CosmoBullet(X + Sprite.Width / 2, Y + Sprite.Height / 2, angle);
                world.AddObject(bullet);
            }
        }
        public void TakeDamage(int dmg)
        {
            health -= dmg;
            if (health <= 0) IsActive = false;
        }

        public override Rect GetBounds()
        {
            return new Rect(X, Y, Sprite.Width, Sprite.Height);
        }
    }
} 