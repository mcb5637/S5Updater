using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S5Updater
{
    class MainUpdater
    {
        [STAThread]
        public static void Main()
        {
            //CultureInfo i = new CultureInfo("en-us");
            //CultureInfo.DefaultThreadCurrentCulture = i;
            //CultureInfo.DefaultThreadCurrentUICulture = i;
            Application.EnableVisualStyles();
            Application.Run(new MainMenu());
        }


        internal static void Copy(string sourceDirectory, string targetDirectory, IEnumerable<string> exclude)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget, exclude);
        }

        private static void CopyAll(DirectoryInfo source, DirectoryInfo target, IEnumerable<string> exclude)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                if (exclude.Contains(fi.Name))
                    continue;
                //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                if (exclude.Contains(diSourceSubDir.Name))
                    continue;
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir, exclude);
            }
        }
    }
}
