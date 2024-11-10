using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FAR
{
    internal class Loop
    {
        private static uint Bind = 0x91;
        public static ConfigManager.Config RunningConfig = ConfigManager.Get("UNIVERSAL");

        public static void UpdateToggle()
        {
            if (Settings.Keybind == "CAPS LOCK")
            {
                Bind = 0x14;
            }
            else if (Settings.Keybind == "SCROLL LOCK")
            {
                Bind = 0x91;
            }
            else if (Settings.Keybind == "NUM LOCK")
            {
                Bind = 0x90;
            }
            else if (Settings.Keybind == "F1")
            {
                Bind = 0x70;
            }
            else if (Settings.Keybind == "F2")
            {
                Bind = 0x71;
            }
            else if (Settings.Keybind == "F3")
            {
                Bind = 0x72;
            }
            else if (Settings.Keybind == "F4")
            {
                Bind = 0x73;
            }
            else if (Settings.Keybind == "F5")
            {
                Bind = 0x74;
            }
            else if (Settings.Keybind == "F6")
            {
                Bind = 0x75;
            }
            else if (Settings.Keybind == "F7")
            {
                Bind = 0x76;
            }
            else if (Settings.Keybind == "F8")
            {
                Bind = 0x77;
            }
            else if (Settings.Keybind == "F9")
            {
                Bind = 0x78;
            }
            else if (Settings.Keybind == "F10")
            {
                Bind = 0x79;
            }
            else if (Settings.Keybind == "F11")
            {
                Bind = 0x7A;
            }
            else if (Settings.Keybind == "F12")
            {
                Bind = 0x7B;
            }
            else if (Settings.Keybind == "Page Up")
            {
                Bind = 0x21;
            }
            else if (Settings.Keybind == "Page Down")
            {
                Bind = 0x22;
            }
            else if (Settings.Keybind == "Right Shift")
            {
                Bind = 0xA1;
            }
        }
        // RECOIL
        public static void RecoilMethod()
        {
            while (true)
            {
                Thread.Sleep(5);
                if (!Settings.Recoil.Enabled)
                {
                    continue;
                }
                int SprayCount = RunningConfig.Data.Length;
                int SprayIndex = 0;
                bool firstBulletDone = false;
                int[][] configLines = RunningConfig.Data;
                while ((MnK.GetAsyncKeyState(1) & 0x8000) == 32768 && (MnK.GetAsyncKeyState(2) & 0x8000) == 32768)
                {
                    int[] sprayLine = configLines[SprayIndex];
                    Settings.Recoil.X = Convert.ToInt32(sprayLine[0]);
                    Settings.Recoil.Y = Convert.ToInt32(sprayLine[1]);
                    try
                    {
                        Settings.Recoil.Sleep = Convert.ToInt32(sprayLine[2]);
                    }
                    catch (Exception)
                    {
                        Settings.Recoil.Sleep = 20;
                    }
                    if (Settings.TimeSmoothing)
                    {
                        Thread.Sleep(Smoothing.Calculate(Settings.Recoil.Sleep, Settings.SmoothingValue));
                    }
                    else
                    {
                        Thread.Sleep(Settings.Recoil.Sleep);
                    }
                    int y = Settings.Recoil.Y;
                    if (!firstBulletDone)
                    {
                        y += RunningConfig.firstBullet;
                        firstBulletDone = true;
                    }
                    if (Settings.YSmoothing)
                    {
                        y = Smoothing.Calculate(y, Settings.SmoothingValue);
                    }
                    MnK.Move(Settings.Recoil.X, y);
                    SprayIndex++;
                    if (SprayIndex >= SprayCount)
                    {
                        SprayIndex = 0;
                    }
                }
            }
        }

        // KEYBINDS TOGGLE
        public static void ToggleMethod()
        {
            while (true)
            {
                Thread.Sleep(50);
                if (((ushort)MnK.GetKeyState(Bind) & 0xFFFFu) != 0)
                {
                    Settings.Recoil.Enabled = true;
                }
                else
                {
                    Settings.Recoil.Enabled = false;
                }
            }
        }
    }
}
