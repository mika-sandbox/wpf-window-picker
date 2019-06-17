using System;

using Prism.Services.Dialogs;

using Reactive.Bindings;

using WpfApp1.Views;

namespace WpfApp1.ViewModels
{
    internal class AppShellViewModel : ViewModel
    {
        public ReactiveProperty<string> SelectedWindow { get; }
        public ReactiveCommand SelectWindowCommand { get; }

        public AppShellViewModel(IDialogService dialogService)
        {
            SelectedWindow = new ReactiveProperty<string>();
            SelectWindowCommand = new ReactiveCommand();
            SelectWindowCommand.Subscribe(_ =>
            {
                dialogService.ShowDialog(nameof(WindowPicker), new DialogParameters(), r =>
                {
                    if (r.Parameters.ContainsKey("SelectedWindow"))
                        SelectedWindow.Value = r.Parameters.GetValue<string>("SelectedWindow");
                    else
                        SelectedWindow.Value = null;
                });
            });
        }
    }
}