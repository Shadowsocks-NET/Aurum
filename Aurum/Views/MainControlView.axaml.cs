using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Aurum.Views
{
    public class MainControlView : UserControl
    {
        public MainControlView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
