using System.Windows;

using Prism.Ioc;
using Prism.Unity;

using WpfApp1.ViewModels;
using WpfApp1.Views;

namespace WpfApp1
{
    /// <summary>
    ///     App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<WindowPicker, WindowPickerViewModel>();
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<AppShell>();
        }
    }
}