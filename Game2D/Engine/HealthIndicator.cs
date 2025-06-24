using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Game2D.Engine
{
    public class HealthIndicator
    {
        private Polygon _indicator;
        private Canvas _canvas;
        private GameObject _target;
        private bool _isHero;

        public HealthIndicator(Canvas canvas, GameObject target, bool isHero = false)
        {
            _canvas = canvas;
            _target = target;
            _isHero = isHero;
            
            CreateIndicator();
        }

        private void CreateIndicator()
        {
            _indicator = new Polygon();
            
            // Создаем треугольник
            PointCollection points = new PointCollection();
            points.Add(new Point(0, 0));
            points.Add(new Point(8, 0));
            points.Add(new Point(4, 8));
            _indicator.Points = points;
            
            if (_isHero)
            {
                _indicator.Fill = Brushes.Yellow;
                _indicator.Stroke = Brushes.Orange;
            }
            else
            {
                _indicator.Fill = Brushes.Red;
                _indicator.Stroke = Brushes.DarkRed;
            }
            
            _indicator.StrokeThickness = 1;
            _indicator.Width = 8;
            _indicator.Height = 8;
            
            _canvas.Children.Add(_indicator);
        }

        public void Update()
        {
            if (_target == null || _target.Sprite == null || !_target.IsActive)
            {
                Remove();
                return;
            }
            
            double x = _target.X + (_target.Sprite.Width / 2) - (_indicator.Width / 2);
            double y = _target.Y - _indicator.Height - 5;
            
            Canvas.SetLeft(_indicator, x);
            Canvas.SetTop(_indicator, y);
        }

        public void Remove()
        {
            if (_indicator != null && _canvas.Children.Contains(_indicator))
            {
                _canvas.Children.Remove(_indicator);
            }
        }
    }
} 