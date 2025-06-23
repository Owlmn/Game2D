using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Media;

namespace Game2D.Engine
{
    public class MAP3 : GameWorld
    {
        public MAP3(Canvas canvas) : base(canvas)
        {
            SetBackground("MAP3.jpg");
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