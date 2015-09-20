using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NpDeck.Detectors
{
    public static class WinApi
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
		[DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SendMessage(int hWnd, int Msg, int wparam, int lparam);

		const int WM_GETTEXT = 0x000D;
		const int WM_GETTEXTLENGTH = 0x000E;

		public static string GetControlText(IntPtr hWnd)
		{
			var title = new StringBuilder();
			var size = SendMessage((int)hWnd, WM_GETTEXTLENGTH, 0, 0).ToInt32();
			if (size <= 0) return String.Empty;
			title = new StringBuilder(size + 1);
			SendMessage(hWnd, WM_GETTEXT, title.Capacity, title);
			return title.ToString();
		}

        public static string GetWindowTextByClassName(string className)
        {
            var hwnd = FindWindow(className, null);
            if (hwnd == IntPtr.Zero) return null;
	        return GetControlText(hwnd);
        }
    }
}