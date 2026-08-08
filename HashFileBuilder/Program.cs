

using System.Security.Cryptography;

namespace HashFileBuilder;

public static class CLI
{
    private static string? GetFileHash(string path)
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
    
    static void Main(string[] args)
    {
        for (int i = 0; i < args.Length - 1; i += 2)
        {
            if (i != 0)
                Console.WriteLine();
            var file = args[i];
            var path = args[i + 1];
            Console.Write($"{file} {GetFileHash(path)}");
        }
    }
}