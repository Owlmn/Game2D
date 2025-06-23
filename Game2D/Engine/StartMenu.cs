using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Game2D.Engine
{
    public class StartMenu : UserControl
    {
        public Button StartButton { get; private set; }
        public Button ExitButton { get; private set; }
        public Button ControlsButton { get; private set; }
        public StartMenu()
        {
            var grid = new Grid();
            var bg = new Image
            {
                Source = new BitmapImage(new System.Uri("pack://application:,,,/Project/images/MainMenu.png")),
                Stretch = System.Windows.Media.Stretch.Fill
            };
            grid.Children.Add(bg);
            var stack = new StackPanel { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            StartButton = new Button { Content = "Старт", Width = 200, Height = 60, Margin = new Thickness(0, 10, 0, 10) };
            ExitButton = new Button { Content = "Выход", Width = 200, Height = 60, Margin = new Thickness(0, 10, 0, 10) };
            ControlsButton = new Button { Content = "Управление", Width = 200, Height = 60, Margin = new Thickness(0, 10, 0, 10) };
            stack.Children.Add(StartButton);
            stack.Children.Add(ControlsButton);
            stack.Children.Add(ExitButton);
            grid.Children.Add(stack);
            this.Content = grid;
        }
    }
} 