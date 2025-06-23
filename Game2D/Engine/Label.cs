using System.Windows.Controls;
using System.Windows.Media;

namespace Game2D.Engine
{
    public class Label : TextBlock
    {
        public Label(string text, double fontSize = 32)
        {
            this.Text = text;
            this.FontSize = fontSize;
            this.Foreground = Brushes.White;
            this.FontWeight = System.Windows.FontWeights.Bold;
            this.TextAlignment = System.Windows.TextAlignment.Center;
        }
    }
} 