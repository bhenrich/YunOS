using System.Diagnostics;
using System.Net;
using System.IO.Compression;

#pragma warning disable SYSLIB0021
#pragma warning disable SYSLIB0014

namespace YunOS
{
    class Data
    {
        public static void RunProcess(string program, string path, bool announceExit = true, bool replaceSlashes = true)
        {
            if(replaceSlashes && path != null && !(path.Contains("\\") || path.Contains("/")))
            {
                path = $"{Directory.GetCurrentDirectory()}\\{path}";
            }
            var process = Process.Start(program, path);
            process.WaitForExit();
            if(announceExit)
            {
                int exitCode = process.ExitCode;
                if (exitCode == 0)
                    Console.WriteLine("\nProgram exited with Exit Code 0.\n");
                else
                    ThrowError($"Program exited with Exit Code {exitCode}.");
            }
        }
        public static bool Prompt(string message, ConsoleColor col)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{message} (Y/n): ");
            Console.ResetColor();

            // For a T/F Prompt, read a Char instead of a String for faster Setup.
            char input = Char.ToLower(Console.ReadKey().KeyChar);

            if (input != 'y' && input != 'n')
            {
                ThrowError("Invalid Input. Please try again.");
                return Prompt(message, col);
            }

            Console.WriteLine();
            Console.ForegroundColor = col;
            return input == 'y';
        }

        public static void ThrowSuccess(string message = "Action successful.")
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nSUCCESS: {message}");
            Console.ResetColor();
        }

        public static void ThrowError(string message = "An unexpected error occurred.")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nERROR: {message}\n");
            Console.ResetColor();

        }

        public static string ShaEncrypt(string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        public static void InstallProgram(string url, string path, string name, bool zip)
        {
            WebClient client = new WebClient();
            Console.Write($"Downloading {name}...");

            string destination = FileSystem.TempPath + "\\temp.zip";
            if(!zip) destination = $"{path}\\{name}.exe";

            client.DownloadFile(url, destination);
            while (client.IsBusy)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }

            client.Dispose();

            if(zip)
            {
                Console.Write($"\nExtracting {name}...");
                ZipFile.ExtractToDirectory(FileSystem.TempPath + "\\temp.zip", path);
                try { File.Delete(FileSystem.TempPath + "\\temp.zip"); }
                catch (Exception e) { Data.ThrowError(e.Message); }
            }
            Console.WriteLine($"\nFinished Installing {name}!");
        }

    }

}