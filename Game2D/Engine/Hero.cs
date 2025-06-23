using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Game2D.Engine
{
    public class Hero : GameObject
    {
        public int MaxHealth { get; set; } = 3000;
        public int Health { get; set; }
        public bool IsAlive => Health > 0;

        private int rifleAmmo = 30;
        private int maxRifleAmmo = 30;
        public int RifleAmmo => rifleAmmo;
        public int MaxRifleAmmo => maxRifleAmmo;

        public enum WeaponType { Rifle, Shotgun, Lasergun }
        private WeaponType currentWeapon = WeaponType.Rifle;
        private int shotgunAmmo = 5, maxShotgunAmmo = 5;
        private int lasergunAmmo = 10, maxLasergunAmmo = 10;
        private int shootCooldown = 0;
        private int rifleCooldown = 10, shotgunCooldown = 40, lasergunCooldown = 20;

        public Hero(double x, double y)
        {
            X = x;
            Y = y;
            Health = MaxHealth;
            Sprite = new Image
            {
                Source = new BitmapImage(new System.Uri("pack://application:,,,/Project/images/hero.png")),
                Width = 48,
                Height = 48
            };
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

        public void MoveByKeys()
        {
            double speed = 2; // Было 4, теперь в 2 раза меньше
            double dx = 0, dy = 0;
    
            if (Keyboard.IsKeyDown(Key.W)) dy -= 1;
            if (Keyboard.IsKeyDown(Key.S)) dy += 1;
            if (Keyboard.IsKeyDown(Key.A)) dx -= 1;
            if (Keyboard.IsKeyDown(Key.D)) dx += 1;

            double length = Math.Sqrt(dx * dx + dy * dy);
            if (length > 0)
            {
                dx = dx / length * speed;
                dy = dy / length * speed;
            }

            if (CanMove(dx, dy))
            {
                X += dx;
                Y += dy;
            }
            else
            {
                if (dx != 0 && CanMove(dx, 0)) X += dx;
                if (dy != 0 && CanMove(0, dy)) Y += dy;
            }
        }

        public void ShootTo(double targetX, double targetY, GameWorld world)
        {
            double dx = targetX - (X + Sprite.Width / 2);
            double dy = targetY - (Y + Sprite.Height / 2);
            double angle = System.Math.Atan2(dy, dx);
            var canvas = world._canvas;
            switch (currentWeapon)
            {
                case WeaponType.Rifle:
                    if (rifleAmmo > 0 && shootCooldown == 0)
                    {
                        var bullet = new Bullet(canvas, X + Sprite.Width / 2, Y + Sprite.Height / 2, angle);
                        world.AddObject(bullet);
                        rifleAmmo--;
                        shootCooldown = rifleCooldown;
                    }
                    break;
                case WeaponType.Shotgun:
                    if (shotgunAmmo > 0 && shootCooldown == 0)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            var bullet = new Bullet(canvas, X + Sprite.Width / 2, Y + Sprite.Height / 2, angle + i * 0.2);
                            world.AddObject(bullet);
                        }
                        shotgunAmmo--;
                        shootCooldown = shotgunCooldown;
                    }
                    break;
                case WeaponType.Lasergun:
                    if (lasergunAmmo > 0 && shootCooldown == 0)
                    {
                        var laser = new Laser(canvas, X + Sprite.Width / 2, Y + Sprite.Height / 2, angle);
                        world.AddObject(laser);
                        lasergunAmmo--;
                        shootCooldown = lasergunCooldown;
                    }
                    break;
            }
        }

        public void Reload()
        {
            switch (currentWeapon)
            {
                case WeaponType.Rifle:
                    rifleAmmo = maxRifleAmmo;
                    break;
                case WeaponType.Shotgun:
                    shotgunAmmo = maxShotgunAmmo;
                    break;
                case WeaponType.Lasergun:
                    lasergunAmmo = maxLasergunAmmo;
                    break;
            }
        }

        public void PickupWeapon(WeaponType type)
        {
            currentWeapon = type;
            switch (type)
            {
                case WeaponType.Rifle:
                    rifleAmmo = maxRifleAmmo;
                    break;
                case WeaponType.Shotgun:
                    shotgunAmmo = maxShotgunAmmo;
                    break;
                case WeaponType.Lasergun:
                    lasergunAmmo = maxLasergunAmmo;
                    break;
            }
        }

        public void SwitchWeapon(int weaponIndex)
        {
            switch (weaponIndex)
            {
                case 1:
                    currentWeapon = WeaponType.Rifle;
                    Sprite.Source = new BitmapImage(new System.Uri("pack://application:,,,/Project/images/hero.png"));
                    break;
                case 2:
                    currentWeapon = WeaponType.Shotgun;
                    Sprite.Source = new BitmapImage(new System.Uri("pack://application:,,,/Project/images/hero_shotgun.png"));
                    break;
                case 3:
                    currentWeapon = WeaponType.Lasergun;
                    Sprite.Source = new BitmapImage(new System.Uri("pack://application:,,,/Project/images/hero_lasergun.png"));
                    break;
            }
        }

        private void TurnTowardsMouse()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as Game2D.MainWindow;
            if (mainWindow == null || mainWindow.GameCanvas == null) return;
            var pos = System.Windows.Input.Mouse.GetPosition(mainWindow.GameCanvas);
            double centerX = X + (Sprite?.Width ?? 0) / 2;
            double centerY = Y + (Sprite?.Height ?? 0) / 2;
            double dx = pos.X - centerX;
            double dy = pos.Y - centerY;
            double angle = Math.Atan2(dy, dx) * 180 / Math.PI;
            if (Sprite != null)
                Sprite.RenderTransform = new System.Windows.Media.RotateTransform(angle, Sprite.Width/2, Sprite.Height/2);
        }

        public override void Update()
        {
            MoveByKeys();
            TurnTowardsMouse();
            if (shootCooldown > 0) shootCooldown--;
        }

        public WeaponType CurrentWeapon => currentWeapon;
        public int ShotgunAmmo => shotgunAmmo;
        public int MaxShotgunAmmo => maxShotgunAmmo;
        public int LasergunAmmo => lasergunAmmo;
        public int MaxLasergunAmmo => maxLasergunAmmo;

        public void ResetAmmo()
        {
            rifleAmmo = maxRifleAmmo;
            shotgunAmmo = maxShotgunAmmo;
            lasergunAmmo = maxLasergunAmmo;
        }
    }
} 