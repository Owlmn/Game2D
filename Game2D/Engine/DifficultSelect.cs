using System.Windows.Controls;
using System.Windows;

namespace Game2D.Engine
{
    public class DifficultSelect : UserControl
    {
        public DifficultyButton EasyButton { get; private set; }
        public DifficultyButton MediumButton { get; private set; }
        public DifficultyButton HardButton { get; private set; }
        public DifficultSelect()
        {
            var grid = new Grid();
            var stack = new StackPanel { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            EasyButton = new DifficultyButton("Легко");
            MediumButton = new DifficultyButton("Средне");
            HardButton = new DifficultyButton("Сложно");
            stack.Children.Add(EasyButton);
            stack.Children.Add(MediumButton);
            stack.Children.Add(HardButton);
            grid.Children.Add(stack);
            this.Content = grid;
        }
    }
} 