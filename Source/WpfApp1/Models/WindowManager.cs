using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;

using WpfApp1.Interop;

namespace WpfApp1.Models
{
    public class WindowManager : IDisposable
    {
        private readonly ObservableCollection<Window> _windows;
        public ReadOnlyObservableCollection<Window> Windows { get; }

        public WindowManager()
        {
            _windows = new ObservableCollection<Window>();
            Windows = new ReadOnlyObservableCollection<Window>(_windows);
        }

        public void Dispose()
        {
            foreach (var window in Windows)
                window.Dispose();
        }

        public void FetchWindows()
        {
            _windows.Clear();

            NativeMethods.EnumWindows((hWnd, _) =>
            {
                // remove invisible window
                if (!NativeMethods.IsWindowVisible(hWnd))
                    return true;

                // remove UWP (invisible or background) window
                NativeMethods.DwmGetWindowAttributeBool(hWnd, DWMWINDOWATTRIBUTE.Cloaked, out var isCloaked, Marshal.SizeOf<bool>());
                if (isCloaked)
                    return true;

                // remove popup window
                var ws = NativeMethods.GetWindowLongPtr(hWnd, (int) GWL.GWL_STYLE);
                if ((ws.ToInt64() & (long) WindowStyles.WS_POPUP) != 0)
                    return true;

                // remove untitled window
                var sb = new StringBuilder(1024);
                NativeMethods.GetWindowText(hWnd, sb, sb.Capacity);
                if (string.IsNullOrEmpty(sb.ToString()))
                    return true;

                if (sb.ToString() == "ChromeWindow")
                    return true;

                _windows.Add(new Window(hWnd));

                return true;
            }, IntPtr.Zero);
        }
    }
}