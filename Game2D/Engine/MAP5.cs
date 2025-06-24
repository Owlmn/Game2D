using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Media;

namespace Game2D.Engine
{
    public class MAP5 : GameWorld
    {
        public MAP5(Canvas canvas) : base(canvas)
        {
            SetBackground("MAP5.jpg");
        }
        private void SetBackground(string imageName)
        {
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/Project/images/{imageName}"));
            brush.Stretch = System.Windows.Media.Stretch.UniformToFill;
            _canvas.Background = brush;
        }
    }
} 