// https://stackoverflow.com/questions/621577/clipboard-event-c-sharp

namespace CVMagic
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;

    public class ClipboardManager
    {
        public event ClipboardEventHandler ClipboardChanged;
        public event ClipboardEventHandler ClipboardChangedFromOutside;

        public ClipboardManager(Window windowSource)
        {
            if (PresentationSource.FromVisual(windowSource) is HwndSource source)
            {
                source.AddHook(WndProc);
            }
            else
            {
                throw new ArgumentException(
                    "Window source MUST be initialized first, such as in the Window's OnSourceInitialized handler."
                    , nameof(windowSource));
            }

            // get window handle for interop
            IntPtr windowHandle = new WindowInteropHelper(windowSource).Handle;

            // register for clipboard events
            NativeMethods.AddClipboardFormatListener(windowHandle);
        }

        private void OnClipboardChanged()
        {
            this.ClipboardChanged?.Invoke(this, new ClipboardEventArgs { Text = "" });
        }

        private static readonly IntPtr WndProcSuccess = IntPtr.Zero;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_CLIPBOARDUPDATE)
            {
                this.OnClipboardChanged();
                handled = true;
            }

            return WndProcSuccess;
        }
    }

    public class ClipboardEventArgs : EventArgs
    {
        public string Text { get; set; }
    }

    public delegate void ClipboardEventHandler(object sender, ClipboardEventArgs e);

    internal static class NativeMethods
    {
        // See http://msdn.microsoft.com/en-us/library/ms649021%28v=vs.85%29.aspx
        public const int WM_CLIPBOARDUPDATE = 0x031D;
        public static IntPtr HWND_MESSAGE = new IntPtr(-3);

        // See http://msdn.microsoft.com/en-us/library/ms632599%28VS.85%29.aspx#message_only
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);
    }
}
