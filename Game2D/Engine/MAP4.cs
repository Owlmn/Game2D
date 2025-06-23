using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Media;

namespace Game2D.Engine
{
    public class MAP4 : GameWorld
    {
        public MAP4(Canvas canvas) : base(canvas)
        {
            SetBackground("MAP4.jpg");
            // Не добавляем никаких объектов на фон
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