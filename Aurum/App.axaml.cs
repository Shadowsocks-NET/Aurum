using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Aurum.ViewModels;
using Aurum.Views;
using Aurum.Services;
using Avalonia.Controls;

namespace Aurum
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var connections = new Connections();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(connections)
                };
            }
        }
    }
}