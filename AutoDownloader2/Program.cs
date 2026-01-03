using System.IO.Compression;

namespace AutoDownloader2
{
    public static class Program
    {
        public static async Task Main(string[] _)
        {
            await DownloadFile("https://github.com/mcb5637/S5Updater/releases/latest/download/Release.zip", "./update.zip");
            string pname = OperatingSystem.IsWindows() ? "./AutoDownloader.exe" : "./AutoDownloader";
            string back = pname + ".bak";
            if (File.Exists(back))
                File.Delete(back);
            if (File.Exists(pname))
                File.Move(pname, back);
            await using (ZipArchive a = await ZipFile.OpenReadAsync("./update.zip"))
            {
                foreach (ZipArchiveEntry en in a.Entries)
                {
                    if (en.FullName.EndsWith('/'))
                    {
                        continue;
                    }
                    string destinationFileName = Path.Combine("./", en.FullName);
                    var d = Path.GetDirectoryName(destinationFileName);
                    if (d != null)
                        Directory.CreateDirectory(d);
                    await en.ExtractToFileAsync(destinationFileName, true);
                    Console.WriteLine(en.FullName);
                }
            }
            File.Delete("./update.zip");
        }

        private static async Task DownloadAsync(string uri, Stream target)
        {
            using HttpClient cl = new();
            using HttpResponseMessage res = await cl.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
            long len = res.Content.Headers.ContentLength ?? -1;
            await using Stream source = await res.Content.ReadAsStreamAsync();
            int buffsize = 1024;
            byte[] buffer = new byte[buffsize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer).ConfigureAwait(false)) != 0)
            {
                await target.WriteAsync(buffer.AsMemory(0, bytesRead)).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                Console.WriteLine($"{(float)totalBytesRead / len * 100.0f }%");
            }
        }

        private static async Task DownloadFile(string uri, string file)
        {
            if (File.Exists(file))
                File.Delete(file);
            await using FileStream s = new(file, FileMode.Create, FileAccess.Write);
            await DownloadAsync(uri, s);
        }
    }
}