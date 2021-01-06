using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace S5Updater
{
    class InstallValidator
    {
        private static readonly string ValidFile = "bin\\settlershok.exe";
        private static readonly string ValidGold = "base\\data.bba";
        private static readonly string ValidHE = "base\\lng";

        private static readonly string HashGoldExe = "D0-77-66-1D-00-3A-2B-2E-9F-DC-EF-53-A5-59-64-51";

        internal bool IsValid(string path)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(Path.Combine(path, ValidFile));
        }

        internal bool IsValidGold(string path)
        {
            return IsValidGoldNormal(path) || IsValidGoldConverted(path);
        }

        internal bool IsValidGoldNormal(string path)
        {
            return IsValid(path) && File.Exists(Path.Combine(path, ValidGold));
        }

        internal bool IsValidGoldConverted(string path)
        {
            return IsValidHE(path) && IsExeGold(path);
        }

        internal bool IsValidHE(string path)
        {
            return IsValid(path) && Directory.Exists(Path.Combine(path, ValidHE));
        }

        internal bool IsValidHENotConverted(string path)
        {
            return IsValidHE(path) && !IsExeGold(path);
        }

        internal bool IsExeGold(string path)
        {
            return IsValid(path) && HashGoldExe.Equals(GetFileHash(Path.Combine(path, ValidFile)));
        }

        internal string GetFileHash(string path)
        {
            using (MD5 md5 = MD5.Create())
            {
                try
                {
                    using (FileStream str = File.OpenRead(path))
                    {
                        return BitConverter.ToString(md5.ComputeHash(str));
                    }
                }
                catch (IOException)
                {
                    return null;
                }
            }
        }
    }
}
