using System;
using System.Text;

namespace AnotherExternalMemoryLibrary
{
    public class WindowController
    {
        public IntPtrEx WindowHandle { get; set; }
        public int X
        {
            get
            {
                Win32.GetWindowRect(WindowHandle, out Win32.RECT rect);
                return rect.Left;
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
                return rect.Top;
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
                return rect.Right - rect.Left;
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
                return rect.Bottom - rect.Top;
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
        public WindowController(IntPtrEx _WindowHandle)
        {
            WindowHandle = _WindowHandle;
        }
    }
}
