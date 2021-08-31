using System.Diagnostics;
using Aurum.Micro.Services;
using Aurum.Micro.ViewModels;
using Aurum.Micro.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Controls;

namespace Aurum.Micro
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