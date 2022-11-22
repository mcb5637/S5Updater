﻿using S5Updater.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace S5Updater
{
    internal class UserScriptManager
    {
        internal bool Zoom = false;
        internal int PlayerColor = -1;
        private string FileGold => Path.Combine(FolderGold, "Script\\UserScript.lua");
        private string FileHE => Path.Combine(FolderHE, "Script\\UserScript.lua");
        private string FolderGold => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents\\DIE SIEDLER - DEdK");
        private string FolderHE => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents\\THE SETTLERS 5 - History Edition");

        internal void Update()
        {
            if (Directory.Exists(FolderGold))
                WriteTo(FileGold);
            if (Directory.Exists(FolderHE))
                WriteTo(FileHE);
        }

        internal void Read()
        {
            if (!ReadFrom(FileGold))
                ReadFrom(FileHE);
        }

        private void WriteTo(string file)
        {
            if (!Directory.Exists(Path.GetDirectoryName(file)))
                Directory.CreateDirectory(Path.GetDirectoryName(file));
            using (StreamWriter w = new StreamWriter(file, false))
            {
                w.WriteLine($"UserScriptSettings = {{Weather = {(Zoom ? "true" : "false")}, Zoom = {(Zoom ? "2" : "nil")}, PlayerColor = {(PlayerColor > 0 ? PlayerColor.ToString() : "nil")}, Debug = nil}}");
                w.Flush();
                Assembly.GetExecutingAssembly().GetManifestResourceStream("S5Updater.UserScript.lua").CopyTo(w.BaseStream);
            }
        }

        private bool ReadFrom(string file)
        {
            try
            {
                using (StreamReader r = new StreamReader(file))
                {
                    string line = r.ReadLine();
                    Match m = new Regex("^UserScriptSettings = {Weather = (.*?), Zoom = (.*?), PlayerColor = (.*?), Debug = nil}$").Match(line);
                    if (!m.Success)
                        return false;
                    Zoom = m.Groups[1].Value == "true";
                    PlayerColor = m.Groups[3].Value == "nil" ? -1 : int.Parse(m.Groups[3].Value);
                    return true;
                }
            }
            catch (IOException)
            {

            }
            return false;
        }
    }
}