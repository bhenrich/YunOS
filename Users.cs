using System.Text;

namespace YunOS
{
    class Users
    {
        public static string GetHash(string username) {
            if(!UserExists(username)) return null;
            return File.ReadAllLines($"{FileSystem.SystemPath}{username}.yuser")[0];
        }
        
        public static bool DeleteUser(string username)
        {
            try
            {
                File.Delete($"{FileSystem.SystemPath + username}.yuser");
                Directory.Delete($"{FileSystem.SystemPath + username}", true);
                return true;
            } 
            catch(IOException) { return false; }
        }

        public static void CreateUser(string username)
        {
            Console.Write($"Please Enter a Password for {username}: ");
            StringBuilder passwordBuilder = new StringBuilder();
            bool reading = true;
            while(reading) {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                char entered = keyInfo.KeyChar;
                switch (entered)
                {
                    case '\b':
                        if (passwordBuilder.Length == 0) break;
                        Console.Write(entered + " " + entered);
                        passwordBuilder.Length--;
                        break;
                    case '\r':
                        reading = false;
                        break;
                    default:
                        Console.Write('*');
                        passwordBuilder.Append(entered.ToString());
                        break;
                }
            }
            File.Create($"{FileSystem.SystemPath + username}.yuser").Close();
            File.WriteAllText($"{FileSystem.SystemPath + username}.yuser", Data.ShaEncrypt(passwordBuilder.ToString()).ToLower());
            Directory.CreateDirectory(FileSystem.SystemPath + username);
            Directory.CreateDirectory($"{FileSystem.SystemPath + username}\\home");
        }

        public static bool UserExists(string username) {
            return File.Exists($"{FileSystem.SystemPath}{username}.yuser");
        }
    }
}