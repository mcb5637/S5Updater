using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace S5Updater2
{
    internal class Settings
    {
        public Dictionary<string, bool> SelectedModPacks = [], SelectedMaps = [], SelectedMapPacks = [];


        private const string FilePath = "./settings.json";
        private static readonly JsonSerializerOptions Options = new()
        {
            IncludeFields = true,
            WriteIndented = true,
        };

        internal static Settings Load()
        {
            try
            {
                return JsonSerializer.Deserialize<Settings>(File.ReadAllText(FilePath), Options) ?? new Settings();
            }
            catch (IOException)
            {
                return new Settings();
            }
        }
        internal void Save()
        {
            File.WriteAllText(FilePath, JsonSerializer.Serialize(this, Options));
        }
    }
}
