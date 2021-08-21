using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Aurum.Models;
using ReactiveUI;
using Aurum.Services;
using Aurum.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using DynamicData;

namespace Aurum.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        public MainWindowViewModel(Connections connections)
        {
            List = new MainControlViewModel(connections.GetItems());
        }

        public MainControlViewModel List { get; }

        public async void AddConnection()
        {
            var window = new AddConnectionWindow();
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var connectionName = await window.ShowDialog<string>(desktop.MainWindow);
                if (!string.IsNullOrWhiteSpace(connectionName))
                {
                    List.Items.Add(new ConnectionItem { ConnectionName = connectionName } );
                }
            }
        }
    }
}
