using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;

namespace S5Updater2
{
    internal class Settings
    {
        public Dictionary<string, bool> SelectedModPacks = [], SelectedMaps = [], SelectedMapPacks = [];
        public bool DebuggerVSCAdaptor = true;
        public string GoldPath = "";
        public string HEPath = "";
        public string WinePath = "";


        private const string FilePath = "./settings.json";
        internal static Settings Load()
        {
            try
            {
                return JsonSerializer.Deserialize(File.ReadAllText(FilePath), SourceGenerationContext.Default.Settings) ?? new Settings();
            }
            catch (IOException)
            {
                return new Settings();
            }
        }
        internal void Save()
        {
            File.WriteAllText(FilePath, JsonSerializer.Serialize(this, SourceGenerationContext.Default.Settings));
        }
    }
    [JsonSourceGenerationOptions(WriteIndented = true, IncludeFields = true)]
    [JsonSerializable(typeof(Settings))]
    internal partial class SourceGenerationContext : JsonSerializerContext
    {
    }
}
