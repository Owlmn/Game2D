using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;

namespace Game2D.Engine
{
    public class Controls : UserControl
    {
        public Button BackButton { get; private set; }
        public Controls()
        {
            var grid = new Grid();
            
            var backgroundImage = new Image
            {
                Source = new BitmapImage(new System.Uri("pack://application:,,,/Project/images/menuBackground.png")),
                Stretch = Stretch.Fill,
                Opacity = 0.7
            };
            grid.Children.Add(backgroundImage);
            
            var stack = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            
            var controlsImage = new Image
            {
                Source = new BitmapImage(new System.Uri("pack://application:,,,/Project/images/controls.png")),
                Width = 300,
                Margin = new Thickness(0, 0, 0, 20)
            };
            stack.Children.Add(controlsImage);
            
            var wasdImage = new Image
            {
                Source = new BitmapImage(new System.Uri("pack://application:,,,/Project/images/wasd.png")),
                Width = 300,
                Margin = new Thickness(0, 0, 0, 20)
            };
            stack.Children.Add(wasdImage);
            
            var weaponText = new TextBlock
            {
                Text = "1, 2, 3 — переключение оружия",
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 24,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            stack.Children.Add(weaponText);
            
            var escText = new TextBlock
            {
                Text = "ESC — назад в меню",
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 24,
                TextAlignment = TextAlignment.Center
            };
            stack.Children.Add(escText);
            
            grid.Children.Add(stack);
            this.Content = grid;
        }
    }
} 