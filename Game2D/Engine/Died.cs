using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Game2D.Engine
{
    public class Died : UserControl
    {
        public Button ExitButton { get; private set; }
        public Died()
        {
            var grid = new Grid();
            var bg = new Image
            {
                Source = new BitmapImage(new System.Uri("pack://application:,,,/Project/images/endgame.png")),
                Stretch = System.Windows.Media.Stretch.Fill
            };
            grid.Children.Add(bg);
            ExitButton = new Button { Content = "Выход", Width = 200, Height = 60, VerticalAlignment = VerticalAlignment.Bottom, Margin = new Thickness(0,0,0,40) };
            grid.Children.Add(ExitButton);
            this.Content = grid;
        }
    }
} 