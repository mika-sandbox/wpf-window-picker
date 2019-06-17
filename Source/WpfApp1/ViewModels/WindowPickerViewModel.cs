using System;
using System.Reactive.Linq;
using System.Windows;

using Prism.Services.Dialogs;

using Reactive.Bindings;

using WpfApp1.Extensions;
using WpfApp1.Models;

namespace WpfApp1.ViewModels
{
    internal class WindowPickerViewModel : ViewModel, IDialogAware
    {
        private readonly WindowManager _windowManager;
        public ReadOnlyReactiveCollection<WindowViewModel> Windows { get; }
        public ReactiveProperty<Rect> RenderPosition { get; }
        public ReactiveProperty<WindowViewModel> SelectedWindow { get; }

        public ReactiveCommand SelectCommand { get; }
        public ReactiveCommand CancelCommand { get; }

        public WindowPickerViewModel()
        {
            _windowManager = new WindowManager().AddTo(this);
            Windows = _windowManager.Windows.ToReadOnlyReactiveCollection(w => new WindowViewModel(w).AddTo(this)).AddTo(this);
            Windows.ToCollectionChanged().Throttle(TimeSpan.FromMilliseconds(100)).Subscribe(_ => UpdateChildDisplayPosition()).AddTo(this);
            RenderPosition = new ReactiveProperty<Rect>();
            RenderPosition.Where(w => !w.IsEmpty).Subscribe(_ => UpdateChildDisplayPosition()).AddTo(this);
            SelectedWindow = new ReactiveProperty<WindowViewModel>();
            SelectCommand = new ReactiveCommand();
            SelectCommand.Subscribe(_ => RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters { { "SelectedWindow", SelectedWindow.Value?.Title?.Value } }))).AddTo(this);
            CancelCommand = new ReactiveCommand();
            CancelCommand.Subscribe(_ => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel))).AddTo(this);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            CompositeDisposable.Dispose();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _windowManager.FetchWindows();
        }

        public string Title => "Select a Window";

        public event Action<IDialogResult> RequestClose;

        private void UpdateChildDisplayPosition()
        {
            foreach (var vm in Windows)
                vm.DisplayPosition.Value = RenderPosition.Value;
        }
    }
}