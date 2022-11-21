using System.Text;

namespace AnotherExternalMemoryLibrary.Core
{
    public class Window
    {
        public PointerEx WindowHandle { get; set; }
        public int X
        {
            get
            {
                Win32.GetWindowRect(WindowHandle, out Win32.RECT rect);
                return rect.left;
            }
            set
            {
                Win32.SetWindowPos(WindowHandle, IntPtr.Zero, value, Y, Width, Height, Win32.SetWindowPosFlags.SWP_SHOWWINDOW);
            }
        }
        public int Y
        {
            get
            {
                Win32.GetWindowRect(WindowHandle, out Win32.RECT rect);
                return rect.top;
            }
            set
            {
                Win32.SetWindowPos(WindowHandle, IntPtr.Zero, Y, value, Width, Height, Win32.SetWindowPosFlags.SWP_SHOWWINDOW);
            }
        }
        public int Width
        {
            get
            {
                Win32.GetWindowRect(WindowHandle, out Win32.RECT rect);
                return rect.right - rect.left;
            }
            set
            {
                Win32.SetWindowPos(WindowHandle, IntPtr.Zero, X, Y, value, Height, Win32.SetWindowPosFlags.SWP_SHOWWINDOW);
            }
        }
        public int Height
        {
            get
            {
                Win32.GetWindowRect(WindowHandle, out Win32.RECT rect);
                return rect.bottom - rect.top;
            }
            set
            {
                Win32.SetWindowPos(WindowHandle, IntPtr.Zero, X, Y, Width, value, Win32.SetWindowPosFlags.SWP_SHOWWINDOW);
            }
        }
        public string TitleBar
        {
            get
            {
                int length = Win32.GetWindowTextLength(WindowHandle);
                StringBuilder sb = new StringBuilder(length + 1);
                Win32.GetWindowText(WindowHandle, sb, sb.Capacity);
                return sb.ToString();
            }
        }
        public Window(PointerEx _WindowHandle)
        {
            WindowHandle = _WindowHandle;
        }
    }
}
