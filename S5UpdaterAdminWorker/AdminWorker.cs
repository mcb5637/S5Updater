using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace S5UpdaterAdminWorker
{
    internal class AdminWorker
    {
        private enum AWExitCode
        {
            Success = 0,
            Invalid = 1,
            Unknown,
            InvalidOS,
            AccessDenied,
        }

        public static int Main(string[] args)
        {
            foreach (string s in args)
                Console.WriteLine(s);
            AWExitCode e = AWExitCode.Unknown;
            if (args.Length == 2 && args[0] == "FullAccess")
                e = FullAccess(args[1]);
            if (args.Length == 5 && args[0] == "SetReg")
                e = SetReg(args[1], args[2], args[3], args[4]);
            Console.ReadKey();
            return (int) e;
        }

        private static AWExitCode FullAccess(string dir)
        {
            if (!OperatingSystem.IsWindows())
            {
                Console.WriteLine("access control not on win");
                return AWExitCode.InvalidOS;
            }
            if (!File.Exists(Path.Combine(dir, "bin\\settlershok.exe")))
            {
                Console.WriteLine("no shok dir?");
                return AWExitCode.Invalid;
            }
            DirectoryInfo inf = new(dir);
            DirectorySecurity sec = inf.GetAccessControl();
            sec.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            inf.SetAccessControl(sec);
            return AWExitCode.Success;
        }

        private static AWExitCode SetReg(string k, string k2, string v, string t)
        {
            if (!OperatingSystem.IsWindows())
            {
                Console.WriteLine("reg not on win");
                return AWExitCode.InvalidOS;
            }
            if (!(k.StartsWith("HKEY_LOCAL_MACHINE\\SOFTWARE\\Blue Byte\\The Settlers - Heritage of Kings")
                || k.StartsWith("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Blue Byte\\The Settlers - Heritage of Kings")))
            {
                Console.WriteLine("reg not shok");
                return AWExitCode.Invalid;
            }
            RegistryValueKind rk = RegistryValueKind.String;
            object o = v;
            if (t == "dword")
            {
                rk = RegistryValueKind.DWord;
                o = int.Parse(v);
            }
            Console.WriteLine($"{k} {k2} -> {v} {t}");
            Registry.SetValue(k, k2, o, rk);
            return AWExitCode.Success;
        }
    }
}
