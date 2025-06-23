using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Game2D.Engine
{
    public class Controls : UserControl
    {
        public Button BackButton { get; private set; }
        public Controls()
        {
            var grid = new Grid();
            var bg = new Image
            {
                Source = new BitmapImage(new System.Uri("pack://application:,,,/Project/images/controls.png")),
                Stretch = System.Windows.Media.Stretch.Fill
            };
            grid.Children.Add(bg);
            BackButton = new Button { Content = "Назад", Width = 200, Height = 60, VerticalAlignment = VerticalAlignment.Bottom, Margin = new Thickness(0,0,0,40) };
            grid.Children.Add(BackButton);
            this.Content = grid;
        }
    }
} 