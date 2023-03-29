using System.Text;

namespace YunOS 
{
    class Setup
    {

        public static bool IsSetup = Directory.Exists(FileSystem.SystemPath);

        public static void StartSetup()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("Performing First-Time Setup...");
            Console.WriteLine("One Moment Please...");
            Console.WriteLine();

            FileSystem.CreateDirectoryIfNotExists(FileSystem.SystemPath);
            FileSystem.CreateDirectoryIfNotExists(FileSystem.ExePath);
            FileSystem.CreateDirectoryIfNotExists(FileSystem.TempPath);

            if (Data.Prompt("Would you like to install nano?", ConsoleColor.Yellow))
                Data.InstallProgram("https://www.nano-editor.org/dist/win32-support/nano-git-0d9a7347243.exe", FileSystem.NanoPath.Substring(0, FileSystem.NanoPath.Length-8), "nano", false);
            else
                Console.WriteLine("Skipping nano Installation...");

            if (Data.Prompt("Would you like to install Python (3.10)?", ConsoleColor.Yellow))
            {
                string PythonFolder = FileSystem.PythonPath.Substring(0, FileSystem.PythonPath.Length-10);
                Data.InstallProgram("https://www.python.org/ftp/python/3.10.10/python-3.10.10-embed-amd64.zip", PythonFolder, "Python", true);
                Data.InstallProgram("https://bootstrap.pypa.io/get-pip.py", PythonFolder, "Pip", false);
                File.Move($"{PythonFolder}Pip.exe", $"{PythonFolder}get-pip.py");
                File.AppendAllText($"{PythonFolder}python310._pth", "import site\r\n");
                Data.RunProcess($"{FileSystem.PythonPath}", $"{PythonFolder}get-pip.py", false);
                File.Delete($"{PythonFolder}get-pip.py");
            }
            else
                Console.WriteLine("Skipping Python Installation...");

            if (Data.Prompt("Would you like to install NodeJS 18.15.0?", ConsoleColor.Yellow))
                Data.InstallProgram("https://nodejs.org/dist/v18.15.0/node-v18.15.0-win-x64.zip", FileSystem.NodePath.Substring(0, FileSystem.NodePath.Length-30), "NodeJS", true);
            else
                Console.WriteLine("Skipping NodeJS Installation...");

            Console.WriteLine();

            Users.CreateUser("root");
            Data.ThrowSuccess("\nFirst-Time Setup successful.\nYou may now login.\nIt is recommended to create another User, however, this is not necessary.\nFor a list of commands, type 'help', for instructions on how to use a command type 'man <command>'.\n");
        }
    }
}