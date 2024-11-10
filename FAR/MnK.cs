using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FAR
{
    internal class MnK
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.Dll")]
        public static extern short GetKeyState(uint nVirtKey);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern short GetAsyncKeyState(int nVirtKey);

        public static void Move(int x, int y)
        {
            mouse_event(1, x, y, 0, 0);
        }
    }
}
