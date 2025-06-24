using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Game2D.Engine
{
    public class Portal2 : GameObject
    {
        public Portal2(double x, double y)
        {
            X = x;
            Y = y;
            Sprite = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Project/images/portal2.png")),
                Width = 64,
                Height = 64
            };
        }

        public Action OnEnter { get; set; }

        public override void Update()
        {
            var world = Game2D.MainWindow.CurrentGameWorld;
            if (world != null)
            {
                foreach (var obj in world.Objects)
                {
                    if (obj is Hero hero && hero.IsAlive && this.GetBounds().IntersectsWith(hero.GetBounds()))
                    {
                        OnEnter?.Invoke();
                        this.IsActive = false;
                        break;
                    }
                }
            }
        }
    }
} 