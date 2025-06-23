using System.Windows.Controls;

namespace Game2D.Engine
{
    public class DifficultyButton : Button
    {
        public string DifficultyLevel { get; }
        public DifficultyButton(string level)
        {
            DifficultyLevel = level;
            this.Content = level;
            this.Width = 200;
            this.Height = 60;
        }
    }
} 