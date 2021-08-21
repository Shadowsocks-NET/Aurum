using System;
using System.Reactive;
using System.Reactive.Linq;
using Aurum.Models;
using ReactiveUI;

namespace Aurum.ViewModels
{
    class AddConnectionViewModel : ViewModelBase
    {
        string name;

        public AddConnectionViewModel()
        {
            var okEnabled = this.WhenAnyValue(
                x => x.Name,
                x => !string.IsNullOrWhiteSpace(x)
            );

            Ok = ReactiveCommand.Create(
                () => new ConnectionItem { ConnectionName = Name },
                okEnabled);
            Cancel = ReactiveCommand.Create(() => { });
        }

        public string Name
        {
            get => name; 
            set => this.RaiseAndSetIfChanged(ref name, value);
        }
        public ReactiveCommand<Unit, ConnectionItem> Ok { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }
    }
}