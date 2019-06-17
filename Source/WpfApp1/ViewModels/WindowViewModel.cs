using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using WpfApp1.Extensions;

using Window = WpfApp1.Models.Window;

namespace WpfApp1.ViewModels
{
    internal class WindowViewModel : ViewModel
    {
        private readonly Window _window;
        public int Height => 125;
        public int Width { get; }
        public ReadOnlyReactiveProperty<string> Title { get; }
        public ReactiveProperty<IntPtr> WindowHandle { get; }
        public ReactiveProperty<Rect> DisplayPosition { get; }
        public ReactiveProperty<Rect> RenderPosition { get; }

        public WindowViewModel(Window window)
        {
            _window = window;

            var (x, y) = GetAspectRatio(_window.Width, _window.Height);
            Width = (int) Math.Floor(x / (double) y * Height);
            Title = _window.ObserveProperty(w => w.Name).ToReadOnlyReactiveProperty().AddTo(this);
            WindowHandle = new ReactiveProperty<IntPtr>();
            WindowHandle.Where(w => w != IntPtr.Zero).Subscribe(_ => RenderPreview()).AddTo(this);
            DisplayPosition = new ReactiveProperty<Rect>();
            DisplayPosition.Where(w => !w.IsEmpty).Subscribe(_ => RenderPreview()).AddTo(this);
            RenderPosition = new ReactiveProperty<Rect>();
            RenderPosition.Where(w => !w.IsEmpty).Subscribe(_ => RenderPreview()).AddTo(this);
        }

        private (int, int) GetAspectRatio(int x, int y)
        {
            int CalcGcd(int a, int b)
            {
                return b == 0 ? a : CalcGcd(b, a % b);
            }

            var gcd = CalcGcd(x, y);
            return (x / gcd, y / gcd);
        }

        private void RenderPreview()
        {
            if (WindowHandle == null || RenderPosition == null || DisplayPosition == null)
                return;
            if (DisplayPosition.Value.Height != 0)
                Debug.WriteLine("");
            _window.RenderThumbnail(WindowHandle.Value, RenderPosition.Value, DisplayPosition.Value);
        }
    }
}