using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace FAR
{
    internal class Settings
    {
        public class Recoil
        {
            public static bool Enabled;

            public static bool isSpray = false;

            public static int X = 0;

            public static int Y = 3;

            public static int Sleep = 20;
        }

        public static ConfigManager.Config currentConfig = new ConfigManager.Config();

        public static string currentConfigName = "UNIVERSAL";

        public static string Mode = "UNIVERSAL";

        public static string Keybind = "CAPS LOCK";

        public static bool TimeSmoothing = true;

        public static bool YSmoothing = false;

        public static int SmoothingValue = 2;

        public static bool WebServerEnabled = false;

        public static void Load()
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\FAR\\keybind.txt";
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "CAPS LOCK");
            }
            else
            {
                Keybind = File.ReadAllText(filePath);
            }
        }
    }
}