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
        private static readonly string HashGold105Exe = "C9-56-0B-8C-EE-A9-75-5B-74-24-36-23-CA-63-8E-B1";
        private static readonly string HashGold106Exe = "32-C5-0B-75-BE-89-42-7F-FA-1C-A2-07-AC-A8-B9-06";

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

        internal bool IsGold105(string path)
        {
            return IsValid(path) && HashGold105Exe.Equals(GetFileHash(Path.Combine(path, ValidFile)));
        }

        internal bool IsGold106(string path)
        {
            return IsValid(path) && HashGold106Exe.Equals(GetFileHash(Path.Combine(path, ValidFile)));
        }

        internal static string GetFileHash(string path)
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
