using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Game2D.Engine
{
    public class DifficultyButton : Button
    {
        public string DifficultyLevel { get; }
        
        public DifficultyButton(string level, string normalColor, string hoverColor, string hoverBorderColor, string pressedColor)
        {
            DifficultyLevel = level;
            this.Width = 300;
            this.Height = 60;
            this.Margin = new Thickness(0, 10, 0, 0);
            
            this.Template = CreateButtonTemplate(normalColor, hoverColor, hoverBorderColor, pressedColor);
        }
        
        private ControlTemplate CreateButtonTemplate(string normalColor, string hoverColor, string hoverBorderColor, string pressedColor)
        {
            var template = new ControlTemplate(typeof(Button));
            
            var border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.BackgroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString(normalColor)));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(8));
            border.SetValue(Border.BorderBrushProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString(hoverColor)));
            border.SetValue(Border.BorderThicknessProperty, new Thickness(2));
            border.Name = "border";
            
            var textBlock = new FrameworkElementFactory(typeof(TextBlock));
            textBlock.SetValue(TextBlock.TextProperty, DifficultyLevel);
            textBlock.SetValue(TextBlock.ForegroundProperty, Brushes.White);
            textBlock.SetValue(TextBlock.FontSizeProperty, 24.0);
            textBlock.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            textBlock.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            textBlock.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            
            border.AppendChild(textBlock);
            template.VisualTree = border;
            
            var hoverTrigger = new Trigger();
            hoverTrigger.Property = IsMouseOverProperty;
            hoverTrigger.Value = true;
            
            var hoverSetter = new Setter();
            hoverSetter.TargetName = "border";
            hoverSetter.Property = Border.BackgroundProperty;
            hoverSetter.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hoverColor));
            hoverTrigger.Setters.Add(hoverSetter);
            
            var hoverBorderSetter = new Setter();
            hoverBorderSetter.TargetName = "border";
            hoverBorderSetter.Property = Border.BorderBrushProperty;
            hoverBorderSetter.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hoverBorderColor));
            hoverTrigger.Setters.Add(hoverBorderSetter);
            
            var pressedTrigger = new Trigger();
            pressedTrigger.Property = IsPressedProperty;
            pressedTrigger.Value = true;
            
            var pressedSetter = new Setter();
            pressedSetter.TargetName = "border";
            pressedSetter.Property = Border.BackgroundProperty;
            pressedSetter.Value = new SolidColorBrush((Color)ColorConverter.ConvertFromString(pressedColor));
            pressedTrigger.Setters.Add(pressedSetter);
            
            template.Triggers.Add(hoverTrigger);
            template.Triggers.Add(pressedTrigger);
            
            return template;
        }
    }
} 