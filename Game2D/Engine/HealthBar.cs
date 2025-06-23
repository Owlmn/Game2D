using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Game2D.Engine
{
    public class HealthBar : Canvas
    {
        private Rectangle _bar;
        private double _maxWidth;
        public HealthBar(double maxWidth, double height)
        {
            _maxWidth = maxWidth;
            this.Width = maxWidth;
            this.Height = height;
            _bar = new Rectangle
            {
                Width = maxWidth,
                Height = height,
                Fill = Brushes.Red
            };
            this.Children.Add(_bar);
        }
        public void SetValue(double value, double max)
        {
            _bar.Width = _maxWidth * (value / max);
        }
    }
} 