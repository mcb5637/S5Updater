using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;

namespace S5Updater2
{
    class InstallValidator
    {
        private const string ValidFile = "bin/settlershok.exe";
        private const string ValidGold = "base/data.bba";
        private const string ValidHE = "base/lng";

        private const string HashGoldExe = "37-A5-45-96-7A-62-1F-9A-AB-3C-4A-3C-E2-07-E0-DC";
        private const string HashGold105Exe = "C9-56-0B-8C-EE-A9-75-5B-74-24-36-23-CA-63-8E-B1";
        private const string HashGold106Exe = "32-C5-0B-75-BE-89-42-7F-FA-1C-A2-07-AC-A8-B9-06";
        private const string HashGold106ExeOldPatch = "D0-77-66-1D-00-3A-2B-2E-9F-DC-EF-53-A5-59-64-51";

        internal static bool IsValid([NotNullWhen(false)] string? path)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(Path.Combine(path, ValidFile));
        }

        internal bool IsValidGold([NotNullWhen(false)] string? path)
        {
            if (path == null)
                return false;
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

        internal bool IsValidHE([NotNullWhen(false)] string? path)
        {
            if (path == null)
                return false;
            return IsValid(path) && Directory.Exists(Path.Combine(path, ValidHE));
        }

        internal bool IsValidHENotConverted([NotNullWhen(false)] string? path)
        {
            if (path == null)
                return false;
            return IsValidHE(path) && !IsExeGold(path);
        }

        internal bool IsExeGold(string path)
        {
            return IsValid(path) && HashGold106ExeOldPatch.Equals(GetFileHash(Path.Combine(path, ValidFile)));
        }

        internal bool IsGold105(string path)
        {
            return IsValid(path) && HashGold105Exe.Equals(GetFileHash(Path.Combine(path, ValidFile)));
        }

        internal bool IsGold106(string path)
        {
            string? h = GetFileHash(Path.Combine(path, ValidFile));
            return IsValid(path) && (HashGold106Exe.Equals(h) || HashGoldExe.Equals(h));
        }

        internal static string? GetFileHash(string path)
        {
            using MD5 md5 = MD5.Create();
            try
            {
                using FileStream str = File.OpenRead(path);
                return BitConverter.ToString(md5.ComputeHash(str));
            }
            catch (IOException)
            {
                return null;
            }
        }
    }
}
