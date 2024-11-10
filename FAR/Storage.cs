using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAR
{
    internal class Storage
    {
        private static readonly string FARPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\FAR";

        public static void SaveSettings()
        {
            File.WriteAllText(FARPath + "\\keybind.txt", Settings.Keybind);
        }
    }
}
