namespace YunOS
{
    class FileSystem
    {
        public static string SystemPath = "C:\\yunos\\";
        public static string ExePath = SystemPath + "exe\\";
        public static string NanoPath = ExePath + "nano.exe";
        public static string PythonPath = ExePath + "python\\python.exe";
        public static string NodePath = ExePath + "node\\node-v18.15.0-win-x64\\node.exe";
        public static string TempPath = SystemPath + "temp";

        public static void CreateDirectoryIfNotExists(string path)
        {
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}