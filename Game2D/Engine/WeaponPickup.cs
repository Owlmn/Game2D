using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;

namespace Game2D.Engine
{
    public class WeaponPickup : GameObject
    {
        public Hero.WeaponType WeaponType { get; }
        public WeaponPickup(double x, double y, Hero.WeaponType type)
        {
            X = x;
            Y = y;
            WeaponType = type;
            string img = type switch
            {
                Hero.WeaponType.Rifle => "rifle.png",
                Hero.WeaponType.Shotgun => "shotgun.png",
                Hero.WeaponType.Lasergun => "lasergun.png",
                _ => "rifle.png"
            };
            Sprite = new Image
            {
                Source = new BitmapImage(new Uri($"pack://application:,,,/Project/images/{img}")),
                Width = 32,
                Height = 32
            };
        }
        public override void Update()
        {
            var world = Game2D.MainWindow.CurrentGameWorld;
            if (world != null)
            {
                foreach (var obj in world.Objects)
                {
                    if (obj is Hero hero && hero.IsAlive && this.GetBounds().IntersectsWith(hero.GetBounds()))
                    {
                        hero.PickupWeapon(WeaponType);
                        this.IsActive = false;
                        break;
                    }
                }
            }
        }
    }
} 