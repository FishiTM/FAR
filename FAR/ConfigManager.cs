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

        private static readonly string FARPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\FAR";

        private static readonly string configsDir = FARPath + "\\Configs\\";

        public static List<string> GetAll(bool noDefault = true)
        {
            List<string> returnArray = new List<string>();
            DirectoryInfo dirInfo = new DirectoryInfo(configsDir);
            FileInfo[] Files = dirInfo.GetFiles("*.json");
            foreach (FileInfo File in Files)
            {
                if (!(noDefault && File.Name == "UNIVERSAL.json"))
                {
                    returnArray.Add(File.Name.Replace(".json", string.Empty));
                }
            }
            return returnArray;
        }

        public static Config Get(string configName)
        {
            try
            {
                string filePath = configsDir + configName + ".json";
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

        public static void Create(string newConfigName, Config newConfig)
        {
            File.WriteAllText(configsDir + newConfigName + ".json", JsonConvert.SerializeObject(newConfig));
        }
    }
}
