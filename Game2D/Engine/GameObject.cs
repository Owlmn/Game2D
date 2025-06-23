using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Game2D.Engine
{
    /// <summary>Base class for everything that lives in the world.</summary>
    public abstract class GameObject
    {
        protected readonly Canvas Canvas;
        public Image Sprite { get; protected set; }
        public   Point Position { get; protected set; }
        public   Vector Velocity { get; protected set; }
        public   double Radius { get; protected set; } = 16;
        public   bool   IsAlive { get; private set; } = true;
        public double X { get; set; }
        public double Y { get; set; }
        public bool IsActive { get; set; } = true;
        public double PrevX { get; set; }
        public double PrevY { get; set; }

        protected GameObject(Canvas canvas, string asset, Point startPos)
        {
            Canvas   = canvas;
            Position = startPos;
            Sprite   = new Image
            {
                Source = new BitmapImage(new Uri($"pack://application:,,,/Assets/{asset}", UriKind.Absolute)),
                Width  = Radius*2,
                Height = Radius*2,
                RenderTransformOrigin = new Point(.5,.5)
            };
            Canvas.Children.Add(Sprite);
            RepositionSprite();
        }

        protected GameObject(Canvas canvas)
        {
            Canvas = canvas;
        }

        protected GameObject() { }

        protected void RepositionSprite()
        {
            Canvas.SetLeft(Sprite, Position.X - Radius);
            Canvas.SetTop (Sprite, Position.Y - Radius);
        }

        public virtual void Update() {
            PrevX = X;
            PrevY = Y;
            Position += Velocity;
            RepositionSprite();
        }

        public virtual void Kill()
        {
            if (!IsAlive) return;
            IsAlive = false;
            Canvas.Children.Remove(Sprite);
        }

        public bool Collides(GameObject other)
            => (Position - other.Position).LengthSquared < (Radius + other.Radius)*(Radius + other.Radius);

        public virtual void Draw(Canvas canvas)
        {
            if (Sprite != null && IsActive)
            {
                Canvas.SetLeft(Sprite, X);
                Canvas.SetTop(Sprite, Y);
                if (!canvas.Children.Contains(Sprite))
                    canvas.Children.Add(Sprite);
            }
        }

        public virtual Rect GetBounds()
        {
            if (Sprite != null)
                return new Rect(X, Y, Sprite.Width, Sprite.Height);
            return Rect.Empty;
        }
    }
}