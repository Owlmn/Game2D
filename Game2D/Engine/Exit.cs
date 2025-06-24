using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;

namespace Game2D.Engine
{
    public class Exit : Button
    {
        public Exit()
        {
            this.Width = 300;
            this.Height = 60;
            this.Margin = new Thickness(0, 10, 0, 0);
            
            this.Template = CreateButtonTemplate();
        }
        
        private ControlTemplate CreateButtonTemplate()
        {
            var template = new ControlTemplate(typeof(Button));
            
            var image = new FrameworkElementFactory(typeof(System.Windows.Controls.Image));
            image.SetValue(System.Windows.Controls.Image.SourceProperty, new BitmapImage(new System.Uri("pack://application:,,,/Project/images/exit.png")));
            image.SetValue(System.Windows.Controls.Image.StretchProperty, Stretch.Fill);
            image.Name = "img";
            
            template.VisualTree = image;
            
            var trigger = new Trigger();
            trigger.Property = IsMouseOverProperty;
            trigger.Value = true;
            
            var setter = new Setter();
            setter.TargetName = "img";
            setter.Property = System.Windows.Controls.Image.SourceProperty;
            setter.Value = new BitmapImage(new System.Uri("pack://application:,,,/Project/images/exit_hover.png"));
            
            trigger.Setters.Add(setter);
            template.Triggers.Add(trigger);
            
            return template;
        }
    }
} 