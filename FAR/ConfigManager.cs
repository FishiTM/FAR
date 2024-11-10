using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace FAR
{
    internal class ConfigManager
    {

        public class Config
        {
            public int[][] Data { get; set; }

            public int firstBullet { get; set; }
        }

        private static string FARPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\FAR";

        private static readonly string configsDir = FARPath + "\\Configs\\";

        public static List<string> GetAll(bool noDefault = true)
        {
            List<string> ret = new List<string>();
            DirectoryInfo dinfo = new DirectoryInfo(configsDir);
            FileInfo[] Files = dinfo.GetFiles("*.json");
            FileInfo[] array = Files;
            foreach (FileInfo file in array)
            {
                if (!(file.Name == "Default.json" && noDefault))
                {
                    ret.Add(file.Name.Replace(".json", string.Empty));
                }
            }
            return ret;
        }

        public static Config Get(string cfg)
        {
            try
            {
                Settings.currentConfigName = cfg;
                string filePath = configsDir + cfg + ".json";
                if (!File.Exists(filePath))
                {
                    return null;
                }
                return JsonConvert.DeserializeObject<Config>(File.ReadAllText(filePath));
            }
            catch (Exception e)
            {
                throw new Exception("Config Is Corrupt.\nExact Error: " + e.Message);
            }
        }

        public static void Init()
        {
            if (!Directory.Exists(configsDir))
            {
                Directory.CreateDirectory(configsDir);
            }
            if (!File.Exists(configsDir + "UNIVERSAL.json"))
            {
                Config config = new Config();
                config.Data = new int[1][] { new int[3] { 0, 3, 20 } };
                config.firstBullet = 3;
                Config defaultConfig = config;
                File.WriteAllText(configsDir + "UNIVERSAL.json", JsonConvert.SerializeObject(defaultConfig));
            }
        }

        public static void Create(string name, Config newConfig)
        {
            File.WriteAllText(configsDir + name + ".json", JsonConvert.SerializeObject(newConfig));
        }
    }
}
