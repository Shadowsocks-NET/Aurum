using System.Collections.Generic;
using System.Collections.ObjectModel;
using Aurum.Micro.Models;

namespace Aurum.Micro.ViewModels
{
    public class MainControlViewModel : ViewModelBase
    {
        public MainControlViewModel(IEnumerable<ConnectionItem> items)
        {
            Items = new ObservableCollection<ConnectionItem>(items);
        }

        public ObservableCollection<ConnectionItem> Items { get; }
    }
}