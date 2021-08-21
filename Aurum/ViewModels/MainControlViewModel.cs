using System.Collections.Generic;
using System.Collections.ObjectModel;
using Aurum.Models;

namespace Aurum.ViewModels
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
