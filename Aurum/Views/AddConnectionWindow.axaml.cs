using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Aurum.Views
{
    public class AddConnectionWindow : Window
    {
        public AddConnectionWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}