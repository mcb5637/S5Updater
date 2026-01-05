using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace S5Updater2
{
    internal partial class UserScriptManager
    {
        internal bool Zoom = false;
        internal int PlayerColor = -1;
        private static string FileHE => Path.Combine(FolderHE, "Script/UserScript.lua");
        private static string FolderHE => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "THE SETTLERS 5 - History Edition");

        internal static readonly PlayerColor[] PlayerColors = [ new PlayerColor("default", -1),
            new PlayerColor("blue", 1), new PlayerColor("red", 2), new PlayerColor("yellow", 3), new PlayerColor("teal", 4),
            new PlayerColor("orange", 5), new PlayerColor("purple", 6), new PlayerColor("pink", 7), new PlayerColor("light green", 8),
            new PlayerColor("dark green", 9), new PlayerColor("light gray", 10), new PlayerColor("brown", 11), new PlayerColor("gray", 12),
            new PlayerColor("white", 13), new PlayerColor("black", 14), new PlayerColor("teal 2", 15), new PlayerColor("pink 2", 16),
        ];

        private string? FileGold(RegistryHandler reg)
        {
            var g = reg.GoldDocuments;
            if (g == null)
                return null;
            return Path.Combine(g, "Script/UserScript.lua");
        }

        internal void Update(RegistryHandler reg, Action<string> log)
        {
            if (MainUpdater.IsControlledFolderAccessForbidden())
                MainUpdater.SetControlledFolderAccessException();
            if (Directory.Exists(reg.GoldDocuments))
                WriteTo(FileGold(reg), log);
            if (Directory.Exists(FolderHE))
                WriteTo(FileHE, log);
        }

        internal void Read(RegistryHandler reg)
        {
            if (!ReadFrom(FileGold(reg)))
                ReadFrom(FileHE);
        }

        private void WriteTo(string? file, Action<string> log)
        {
            if (file == null)
                return;
            string? folder = Path.GetDirectoryName(file);
            if (folder != null && !Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            log(string.Format(Res.TaskUserScript_WriteLog, file));
            using StreamWriter w = new(file, false);
            w.Write($"UserScriptSettings = {{Weather = {(Zoom ? "true" : "false")}, Zoom = {(Zoom ? "2" : "nil")}, PlayerColor = {(PlayerColor > 0 ? PlayerColor.ToString() : "nil")}, Debug = nil}}\r\n");
            using StreamReader r = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("S5Updater2.UserScript.lua")?? throw new NullReferenceException("res not found"));
            for (string? l = r.ReadLine(); l != null; l = r.ReadLine())
            {
                w.Write(l);
                w.Write("\r\n");
            }
        }

        private bool ReadFrom(string? file)
        {
            try
            {
                if (file == null)
                    return false;
                using StreamReader r = new(file);
                string? line = r.ReadLine();
                if (line == null)
                    return false;
                Match m = SettingsRegex().Match(line);
                if (!m.Success)
                    return false;
                Zoom = m.Groups[1].Value == "true";
                PlayerColor = m.Groups[3].Value == "nil" ? -1 : int.Parse(m.Groups[3].Value);
                return true;
            }
            catch (IOException)
            {
            }
            return false;
        }

        [GeneratedRegex("^UserScriptSettings = {Weather = (.*?), Zoom = (.*?), PlayerColor = (.*?), Debug = nil}$")]
        private static partial Regex SettingsRegex();
    }
}
