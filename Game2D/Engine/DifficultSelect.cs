using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Game2D.Engine
{
    public class DifficultSelect : UserControl
    {
        public DifficultyButton EasyButton { get; private set; }
        public DifficultyButton MediumButton { get; private set; }
        public DifficultyButton HardButton { get; private set; }
        public DifficultyButton HardcoreButton { get; private set; }
        
        public DifficultSelect()
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
            
            var titleText = new TextBlock
            {
                Text = "ВЫБЕРИТЕ СЛОЖНОСТЬ",
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 36,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 30)
            };
            stack.Children.Add(titleText);
            
            EasyButton = new DifficultyButton("ЛЁГКИЙ", "#4CAF50", "#66BB6A", "#81C784", "#388E3C");
            MediumButton = new DifficultyButton("СРЕДНИЙ", "#FF9800", "#FFB74D", "#FFCC02", "#F57C00");
            HardButton = new DifficultyButton("СЛОЖНЫЙ", "#F44336", "#EF5350", "#E57373", "#D32F2F");
            HardcoreButton = new DifficultyButton("ХАРДКОР", "#9C27B0", "#BA68C8", "#CE93D8", "#7B1FA2");
            
            stack.Children.Add(EasyButton);
            stack.Children.Add(MediumButton);
            stack.Children.Add(HardButton);
            stack.Children.Add(HardcoreButton);
            
            var escText = new TextBlock
            {
                Text = "ESC — назад в меню",
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 20,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };
            stack.Children.Add(escText);
            
            grid.Children.Add(stack);
            this.Content = grid;
        }
    }
} 