using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Net;
using System.IO.Compression;

#pragma warning disable CS8600
#pragma warning disable SYSLIB0021
#pragma warning disable IL3000
#pragma warning disable SYSLIB0014

namespace YunOS
{

    // TODO: Add a way to run .NET programs
    // TODO: IF checks for variable types, add string and bool.


    class CLI
    {

        static string[] cache = new String[99];
        static int yunScriptLineNum = 0;
        static string _username;
        static string syspath = "C:\\yunos\\";
        static string nanopath = syspath + "exe\\nano.exe";
        static string pypath = syspath + "exe\\python\\python.exe";
        static string nodepath = syspath + "exe\\node\\node-v18.15.0-win-x64\\node.exe";
        static string temppath = syspath + "temp";
        static Dictionary<string, string> commands = new Dictionary<string, string>(){
            {"help", "Displays a list of Commands"},
            {"man", "Displays the manual for a command%man <command>"},
            {"newuser", "Creates a new user%newuser <username>"},
            {"remuser", "Deletes a user and home directory%remuser <username>"},
            {"exit", "Exits YunOS"},
            {"clear", "Clears the Console"},
            {"echo", "Prints arguments to the Console%echo <text> / echo $<key>"},
            {"sha", "Prints SHA256 of arguments%sha <text> <key>"},
            {"mkdir", "Creates a directory%mkdir <directory>"},
            {"rmdir", "Removes a directory%rmdir <directory>"},
            {"cd", "Changes the current directory%cd <directory>"},
            {"ls", "Lists the contents of the current directory"},
            {"rm", "Removes a file%rm <file>"},
            {"touch", "Creates a file%touch <file>"},
            {"cat", "Displays the contents of a file%cat <file>"},
            {"store", "Stores data in cache%store <key> <data>"},
            {"read", "Reads data from cache%read <key>"},
            {"write", "Writes text to a file%write <file> <text>"},
            {"append", "Appends text to a file%append <file> <text>"},
            {"edit", "Edits a file using nano%edit <file>"},
            {"cp", "Copies a file%cp <file> <destination>"},
            {"mv", "Moves a file%mv <file> <destination>"},
            {"rename", "Renames a file%rename <file> <name>"},
            {"pwd", "Displays the current directory"},
            {"date", "Displays the current date and time"},
            {"run", "runs a .yun applet%run <file>"},
            {"python", "runs a python3 script%python <file>"},
            {"node", "runs a nodejs script%node <file>"},
        };

        public static void Main(string[] args)
        {
            Console.Clear();
            //checkIfAdmin();
            checkifSetup();
        }

        static void throwSuccess(string message = "Action successful.")
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nSUCCESS: {message}");
            Console.ResetColor();
        }

        static void throwError(string message = "An unexpected error occurred.")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nERROR: {message}\n");
            Console.ResetColor();

        }

        static void resetConsole()
        {
            // set the directory to C:\\yunos\\root
            checkifSetup();
            Directory.SetCurrentDirectory("C:\\yunos\\root\\home");
            Console.Clear();
            Console.WriteLine("Welcome to YunOS!");
            shellLoop();
        }

        static void resetConsoleFirst()
        {
            Directory.SetCurrentDirectory("C:\\yunos");
            Console.WriteLine("Welcome to YunOS!");
            login();
        }

        static void login()
        {
            Console.Write("Username: ");
            _username = Console.ReadLine();
            Console.Write("Password: ");
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
            Console.WriteLine();
            string _password = passwordBuilder.ToString();

            if (userExists(_username))
            {
                string[] lines = File.ReadAllLines($"C:\\yunos\\{_username}.yuser");
                if (lines[0] == ShaEncrypt(_password).ToLower())
                {
                    _password = null;
                    Console.WriteLine("Login successful!");
                    Console.WriteLine();
                    Directory.SetCurrentDirectory($"C:\\yunos\\{_username}\\home");
                    shellLoop();
                }
                else
                {
                    throwError("Incorrect username or password.");
                    login();
                }
            }
            else
            {
                throwError("This User does not exist.");
                login();
            }
            Console.Clear();
        }

        static void checkifSetup()
        {
            if (!Directory.Exists(syspath))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;


                Console.WriteLine("Performing First-Time Setup...");
                Console.WriteLine("Creating System Directories...");

                Directory.CreateDirectory(syspath);

                Directory.CreateDirectory($"{syspath}exe");

                Directory.CreateDirectory(temppath);

                Console.WriteLine();

                if (prompt("Would you like to install nano?", ConsoleColor.Yellow))
                    installProgram("https://www.nano-editor.org/dist/win32-support/nano-git-0d9a7347243.exe", nanopath.Substring(0, nanopath.Length-8), "nano", false);
                else
                    Console.WriteLine("Skipping nano Installation...");

                if (prompt("Would you like to install Python (3.10)?", ConsoleColor.Yellow))
                    installProgram("https://www.python.org/ftp/python/3.10.10/python-3.10.10-embed-amd64.zip", pypath.Substring(0, pypath.Length-10), "Python", true);
                else
                    Console.WriteLine("Skipping Python Installation...");

                if (prompt("Would you like to install NodeJS 18.15.0?", ConsoleColor.Yellow))
                    installProgram("https://nodejs.org/dist/v18.15.0/node-v18.15.0-win-x64.zip", nodepath.Substring(0, nodepath.Length-30), "NodeJS", true);
                else
                    Console.WriteLine("Skipping NodeJS Installation...");
                Console.WriteLine();

                Console.Write("Please enter a Password for the Root User: ");
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
                Console.WriteLine();
                createUser("root", ShaEncrypt(passwordBuilder.ToString()).ToLower());
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nFirst-Time Setup successful.\nYou may now login.\nIt is recommended to create another User, however, this is not necessary.\nFor a list of commands, type 'help', for instructions on how to use a command type 'man <command>'.\n");
                Console.ResetColor();
            }
            else
            {
                if(!userExists("root"))
                {
                    if(prompt("WARNING: The Root user is missing. Would you like to Create it?", ConsoleColor.Yellow))
                    {
                        Console.Write("Please enter a Password: ");
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
                        Console.WriteLine();
                        createUser("root", ShaEncrypt(passwordBuilder.ToString()).ToLower());
                    }
                }
            }
            resetConsoleFirst();
        }

        static string ShaEncrypt(string text)
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

        static void shellLoop()
        {

            while (true)
            {
                string dir = Directory.GetCurrentDirectory().ToString();
                Console.Write($"[{dir.Substring(3)}] {_username}@YunOS> ");
                string input = Console.ReadLine();
                parseInput(input);
            }
        }

        static void parseInput(string input)
        {
            string[] args = input.Split(' ');
            string cmd = args[0];
            args = args.Skip(1).ToArray();

            if(!commands.ContainsKey(cmd))
        	{
                if (File.Exists(cmd))
                {
                    var proc = Process.Start($"{Directory.GetCurrentDirectory() + "\\" + cmd}");
                    proc.WaitForExit();
                }
                else
                    throwError($"{cmd} is not a recognized command - see 'help'.");
            }

            switch (cmd)
            {
                case "help":
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine();

                    foreach(var pair in commands)
                    {
                        string desc = pair.Value;
                        desc = (desc.Contains("%") ? desc.Split("%")[0] : desc);
                        Console.WriteLine($"\t{pair.Key} - {desc}");
                    }

                    Console.WriteLine("\t\r\n--- YunScript Specific Commands---\r\n");
                    Console.WriteLine("\tmath - performs simple mathematical equasions");
                    Console.WriteLine("\tif - performs an if statement");
                    Console.WriteLine();
                    Console.ResetColor();
                    break;
                case "man":
                    if (args.Length == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine();

                        if(commands.ContainsKey(args[0]))
                        {
                            string description = commands[args[0]];
                            string usage = null;
                            if (description.Contains("%"))
                            {
                                usage = description.Split("%")[1];
                                description = description.Split("%")[0];
                            }

                            Console.WriteLine($"{args[0]} - {description}");
                            if(usage != null) Console.WriteLine($"Usage: {usage}");
                            Console.WriteLine();
                            Console.ResetColor();
                            break;
                        }

                        switch (args[0])
                        {
                            case "math":
                                Console.WriteLine("math - Performs a math operation");
                                Console.WriteLine("Usage: math <operation> <number_1> <number_2> <key>");
                                Console.WriteLine();
                                Console.WriteLine("Operations:");
                                Console.WriteLine("add - Adds two numbers");
                                Console.WriteLine("sub - Subtracts two numbers");
                                Console.WriteLine("mul - Multiplies two numbers");
                                Console.WriteLine("div - Divides two numbers");
                                Console.WriteLine("mod - Finds the remainder of two numbers");
                                Console.WriteLine("usage: math <operation> <number_1> <number_2> <key>");
                                break;
                            case "if":
                                Console.WriteLine("if - check if two values are the same, skips the following line of code if they are not.");
                                Console.WriteLine("Usage: if <value 1> <value 2>");
                                break;
                            default:
                                throwError("This Command is Unknown.");
                                break;
                        }
                        Console.ResetColor();
                    }
                    break;
                case "newuser":
                    if(args.Length == 2)
                    {
                        if (!userExists(args[0])) {
                            Console.WriteLine("Please enter a Password for your new User: ");
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
                            createUser(args[0], ShaEncrypt(passwordBuilder.ToString()).ToString());
                            throwSuccess("User has been created.");
                        }
                        else
                            throwError("User already exists.");
                    }
                    else
                        throwError("newuser requires two arguments - see 'man newuser'");
                    break;

                case "remuser":
                    if(args.Length == 1)
                    {
                        // Prevent Self-Deletion
                        if (userExists(args[0]) && _username != args[0])
                        {
                            if(!deleteUser(args[0]))
                            {
                                throwError("User partially deleted. Some files may be in use.");
                                break;
                            }
                            Console.ForegroundColor = ConsoleColor.Green;
                            throwSuccess("Successfully deleted User.");
                        }
                        else
                            throwError("User doesn't exist.");
                    } 
                    else
                        throwError("remuser requires one argument - see 'man remuser'");
                    break;

                case "exit":
                    Environment.Exit(0);
                    break;
                case "clear":
                    Console.Clear();
                    break;
                case "echo":
                    if (args.Length > 0)
                    {
                        if (args[0].StartsWith("$") && args.Length == 1)
                        {
                            Console.WriteLine(cache[Int16.Parse(args[1].Substring(1))]);
                        }
                        else
                        {
                            for (int i = 0; i < args.Length; i++)
                            {
                                Console.Write(args[i] + " ");
                            }
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        throwError("echo requires at least one argument - see 'man echo'");
                    }
                    break;
                case "sha":
                    if (args.Length > 1)
                    {
                        for (int i = 0; i < args.Length; i++)
                        {
                            cache[Int16.Parse(args[0])] += ShaEncrypt(args[i]).ToLower() + " ";
                        }
                    }
                    else
                    {
                        throwError("sha requires at least one argument - see 'man sha'");
                    }
                    break;
                case "mkdir":
                    if (args.Length > 0)
                    {
                        for (int i = 0; i < args.Length; i++)
                        {
                            Directory.CreateDirectory(args[i]);
                        }
                    }
                    else
                    {
                        throwError("No directory name given - see 'man mkdir'");
                    }
                    break;
                case "rmdir":
                    if (args.Length > 0)
                    {
                        for (int i = 0; i < args.Length; i++)
                        {
                            Directory.Delete(args[i], true);
                        }
                    }
                    else
                    {
                        throwError("No directory name given - see 'man rmdir'");
                    }
                    break;
                case "cd":
                    if (args.Length > 0)
                    {
                        // make sure the directory starts with C:\\yunos
                        if (args[0].StartsWith("C:\\yunos"))
                        {
                            Directory.SetCurrentDirectory(args[0]);
                        }
                        else if (args[0] == "..")
                        {
                            string path = Directory.GetCurrentDirectory();
                            string[] pathSplit = path.Split("\\");
                            string newPath = "";
                            for (int i = 0; i < pathSplit.Length - 1; i++)
                            {
                                newPath += pathSplit[i] + "\\";
                            }
                            Directory.SetCurrentDirectory(newPath.Substring(0, newPath.Length - 1));
                        }
                        else
                        {
                            try
                            {
                                Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + "\\" + args[0]);
                            }
                            catch (Exception e)
                            {
                                string error = e.ToString();
                                Console.WriteLine("Directory not found!");
                            }
                        }
                    }
                    else
                    {
                        throwError("No directory name given - see 'man cd'");
                    }
                    break;
                case "ls":
                    Console.WriteLine();
                    foreach (string dir in Directory.GetDirectories(Directory.GetCurrentDirectory()))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("\t<DIR>  " + dir.Split("\\")[dir.Split("\\").Length - 1]);
                        Console.ResetColor();
                    }
                    foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory()))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("\t<FILE>  " + file.Split("\\")[file.Split("\\").Length - 1]);
                        Console.ResetColor();
                    }
                    Console.WriteLine();
                    break;
                case "rm":
                    if (args.Length > 0)
                    {
                        for (int i = 0; i < args.Length; i++)
                        {
                            File.Delete(args[i]);
                        }
                    }
                    else
                    {
                        throwError("No file name given - see 'man rm'");
                    }
                    break;
                case "touch":
                    if (args.Length > 0)
                    {
                        for (int i = 0; i < args.Length; i++)
                        {
                            File.Create(args[i]).Close();
                        }
                    }
                    else
                    {
                        throwError("No file name given - see 'man touch'");
                    }
                    break;
                case "cat":
                    if (args.Length > 0)
                    {
                        if (File.Exists(args[0]))
                        {
                            Console.WriteLine(File.ReadAllText(args[0]));
                        }
                        else
                        {
                            throwError("File not found.");
                        }
                    }
                    else
                    {
                        throwError("No file name given - see 'man cat'");
                    }
                    break;
                case "store":
                    if (args.Length == 2)
                    {
                        string key = args[0];
                        string value = args[1];
                        cache[Int16.Parse(key)] = value;
                    }
                    else
                    {
                        throwError("Not enough arguments - see 'man store'");
                    }
                    break;
                case "read":
                    if (args.Length == 1)
                    {
                        string key = args[0];
                        Console.WriteLine(cache[Int16.Parse(key)]);
                    }
                    else
                    {
                        throwError("No cache key given - see 'man read'");
                    }
                    break;
                case "write":
                    // YuNii = DUMMY
                    if (args.Length > 2)
                    {
                        string file = args[0];
                        string text = args[1];
                        if (text.StartsWith("$"))
                        {
                            text = cache[Int16.Parse(text.Substring(1))];
                        }
                        else
                        {
                            for (int i = 2; i < args.Length; i++)
                            {
                                text += " " + args[i];
                            }
                        }
                        if (File.Exists(file)) File.WriteAllText(file, text);
                        else throwError("File not found!");
                    }
                    else
                    {
                        throwError("Not enough arguments - see 'man write'");
                    }
                    break;
                case "append":
                    // YuNii = DUMMY
                    if (args.Length > 2)
                    {
                        string file = args[0];
                        string text = args[1];
                        if (text.StartsWith("$"))
                        {
                            text = cache[Int16.Parse(text.Substring(1))];
                        }
                        else
                        {
                            for (int i = 2; i < args.Length; i++)
                            {
                                text += " " + args[i];
                            }
                        }
                        if (File.Exists(file)) File.AppendAllText(file, text + "\r\n");
                        else throwError("File not found!");
                    }
                    else
                    {
                        throwError("Not enough arguments given - see 'man append'");
                    }
                    break;
                case "cp":
                    if (args.Length == 2)
                    {
                        string source = args[0];
                        string destination = args[1];
                        File.Copy(source, destination);
                    }
                    else
                    {
                        throwError("Not enough arguments - see 'man cp'");
                    }
                    break;
                case "edit":
                    if (args.Length == 1)
                        runProcess(nanopath, args[0], false);
                    else
                        throwError("No filename given - see 'man edit'");
                    break;
                case "node":
                case "python":
                    if (args.Length == 1)
                    {
                        if (File.Exists(args[0]))
                            runProcess((cmd == "node" ? nodepath : pypath), $"\"{Directory.GetCurrentDirectory() + "\\" + args[0]}\"");
                        else
                            throwError("File not found.");
                    }
                    else
                        // Interactive Shell
                        runProcess(cmd,null,true);
                    break;
                case "rename":
                case "mv":
                    if (args.Length == 2)
                    {
                        string source = args[0];
                        string destination = args[1];
                        File.Move(source, destination);
                    }
                    else
                    {
                        throwError($"Not enough arguments given - see 'man {cmd}'");
                    }
                    break;
                case "pwd":
                    if (args.Length == 1) { cache[Int16.Parse(args[0])] = Directory.GetCurrentDirectory().Substring(3); }
                    else
                        Console.WriteLine(Directory.GetCurrentDirectory().Substring(3));
                    break;
                case "date":
                    if (args.Length == 1) { cache[Int16.Parse(args[0])] = DateTime.Now.ToString(); }
                    else
                        Console.WriteLine(DateTime.Now.ToString());
                    break;
                case "run":
                    if (args.Length == 1 && args[0].EndsWith(".yun"))
                        runApplet(args[0]);
                    else
                        throwError("No file specified or File is not a .yun File.");
                    break;
                case "reset":
                    if (args.Length == 1 && args[0].ToLower().Equals("confirm")) {
                        Directory.SetCurrentDirectory("C:\\");
                        Directory.Delete(syspath, true);
                        Console.WriteLine("Deleting C:\\yunos\\... See you next time!");
                        Environment.Exit(0);
                    }
                    else Console.WriteLine("WARNING! This command will erase the C:\\yunos\\ Directory! To confirm this, please run \"reset confirm\"!");
                    break;
            }

            shellLoop();
        }

        static void runApplet(string file)
        {
            string[] lines = File.ReadAllLines(file);
            yunScriptLineNum = 0;
            for (yunScriptLineNum = 0; yunScriptLineNum < lines.Length; yunScriptLineNum++)
            {
                string[] args = lines[yunScriptLineNum].Split(' ');
                runCommand(args, yunScriptLineNum, file);
            }
        }

        static void runCommand(string[] line, int lineNum, string _file)
        {
            switch (line[0].Trim())
            {
                case "exit":
                    shellLoop();
                    break;
                case "clear":
                    Console.Clear();
                    break;
                case "echo":
                    if (line.Length > 1)
                    {
                        if (line[1].StartsWith("$") && line.Length == 2)
                        {
                            Console.WriteLine(cache[Int16.Parse(line[1].Substring(1))]);
                        }
                        else
                        {
                            for (int i = 1; i < line.Length; i++)
                            {
                                Console.Write(line[i] + " ");
                            }
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        throwError("echo requires at least one argument - see 'man echo'");
                    }
                    break;
                case "sha":
                    if (line.Length > 2)
                    {
                        for (int i = 1; i < line.Length; i++)
                        {
                            cache[Int16.Parse(line[1])] += ShaEncrypt(line[i]).ToLower() + " ";
                        }
                    }
                    else
                    {
                        throwError("sha requires at least one argument - see 'man sha'");
                    }
                    break;
                case "mkdir":
                    if (line.Length > 1)
                    {
                        for (int i = 1; i < line.Length; i++)
                        {
                            Directory.CreateDirectory(line[i]);
                        }
                    }
                    else
                    {
                        throwError("No directory name given - see 'man mkdir'");
                    }
                    break;
                case "rmdir":
                    if (line.Length > 1)
                    {
                        for (int i = 1; i < line.Length; i++)
                        {
                            Directory.Delete(line[i], true);
                        }
                    }
                    else
                    {
                        throwError("No directory name given - see 'man rmdir'");
                    }
                    break;
                case "cd":
                    if (line.Length > 1)
                    {
                        // make sure the directory starts with C:\\yunos
                        if (line[1].StartsWith("C:\\yunos"))
                        {
                            Directory.SetCurrentDirectory(line[1]);
                        }
                        else if (line[1] == "..")
                        {
                            string path = Directory.GetCurrentDirectory();
                            string[] pathSplit = path.Split("\\");
                            string newPath = "";
                            for (int i = 0; i < pathSplit.Length - 1; i++)
                            {
                                newPath += pathSplit[i] + "\\";
                            }
                            Directory.SetCurrentDirectory(newPath.Substring(0, newPath.Length - 1));
                        }
                        else
                        {
                            try
                            {
                                Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + "\\" + line[1]);
                            }
                            catch (Exception e)
                            {
                                string error = e.ToString();
                                Console.WriteLine("Directory not found!");
                            }
                        }
                    }
                    else
                    {
                        throwError("No directory name given - see 'man cd'");
                    }
                    break;
                case "ls":
                    Console.WriteLine();
                    foreach (string dir in Directory.GetDirectories(Directory.GetCurrentDirectory()))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("\t<DIR>  " + dir.Split("\\")[dir.Split("\\").Length - 1]);
                        Console.ResetColor();
                    }
                    foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory()))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("\t<FILE>  " + file.Split("\\")[file.Split("\\").Length - 1]);
                        Console.ResetColor();
                    }
                    Console.WriteLine();
                    break;
                case "rm":
                    if (line.Length > 1)
                    {
                        for (int i = 1; i < line.Length; i++)
                        {
                            File.Delete(line[i]);
                        }
                    }
                    else
                    {
                        throwError("No file name given - see 'man rm'");
                    }
                    break;
                case "touch":
                    if (line.Length > 1)
                    {
                        for (int i = 1; i < line.Length; i++)
                        {
                            File.Create(line[i]).Close();
                        }
                    }
                    else
                    {
                        throwError("No file name given - see 'man touch'");
                    }
                    break;
                case "cat":
                    if (line.Length > 1)
                    {
                        if (File.Exists(line[1]))
                        {
                            Console.WriteLine(File.ReadAllText(line[1]));
                        }
                        else
                        {
                            throwError("File not found!");
                        }
                    }
                    else
                    {
                        throwError("No file name given - see 'man cat'");
                    }
                    break;
                case "store":
                    if (line.Length > 2)
                    {
                        string key = line[1];
                        string value = line[2];
                        cache[Int16.Parse(key)] = value;
                    }
                    else
                    {
                        throwError("Not enough arguments - see 'man store'");
                    }
                    break;
                case "read":
                    if (line.Length > 1)
                    {
                        string key = line[1];
                        Console.WriteLine(cache[Int16.Parse(key)]);
                    }
                    else
                    {
                        throwError("No cache key given - see 'man read'");
                    }
                    break;
                case "write":
                    if (line.Length > 2)
                    {
                        string file = line[1];
                        string text = line[2];
                        if (text.StartsWith("$"))
                        {
                            text = cache[Int16.Parse(text.Substring(1))];
                        }
                        else
                        {
                            for (int i = 3; i < line.Length; i++)
                            {
                                text += " " + line[i];
                            }
                        }
                        if (File.Exists(file)) File.WriteAllText(file, text);
                        else throwError("File not found!");
                    }
                    else
                    {
                        throwError("Not enough arguments - see 'man write'");
                    }
                    break;
                case "append":
                    if (line.Length > 2)
                    {
                        string file = line[1];
                        string text = line[2];
                        if (text.StartsWith("$"))
                        {
                            text = cache[Int16.Parse(text.Substring(1))];
                        }
                        else
                        {
                            for (int i = 3; i < line.Length; i++)
                            {
                                text += " " + line[i];
                            }
                        }
                        if (File.Exists(file)) File.AppendAllText(file, text + "\r\n");
                        else throwError("File not found!");
                    }
                    else
                    {
                        throwError("Not enough arguments given - see 'man append'");
                    }
                    break;
                case "cp":
                    if (line.Length > 2)
                    {
                        string source = line[1];
                        string destination = line[2];
                        File.Copy(source, destination);
                    }
                    else
                    {
                        throwError("Not enough arguments - see 'man cp'");
                    }
                    break;
                case "edit":
                    if (line.Length > 1)
                        runProcess(nanopath, line[1], false);
                    else
                        throwError("No filename given - see 'man edit'");
                    break;
                case "python":
                    if (line.Length > 1)
                    {
                        if (File.Exists(line[1]))
                        {
                            var proc = Process.Start($"{pypath}", $"\"{Directory.GetCurrentDirectory() + "\\" + line[1]}\"");
                            proc.WaitForExit();
                            var exitCode = proc.ExitCode;
                        }
                        else
                        {
                            throwError("File not found!");
                        }
                    }
                    break;
                case "node":
                    if (line.Length > 1)
                    {
                        if (File.Exists(line[1]))
                        {
                            var proc = Process.Start($"{nodepath}", $"\"{Directory.GetCurrentDirectory() + "\\" + line[1]}\"");
                            proc.WaitForExit();
                            var exitCode = proc.ExitCode;
                        }
                        else
                        {
                            throwError("File not found!");
                        }

                    }
                    break;
                case "mv":
                    if (line.Length > 2)
                    {
                        string source = line[1];
                        string destination = line[2];
                        File.Move(source, destination);
                    }
                    else
                    {
                        throwError("Not enough arguments given - see 'man mv'");
                    }
                    break;
                case "rename":
                    if (line.Length > 2)
                    {
                        string source = line[1];
                        string destination = line[2];
                        File.Move(source, destination);
                    }
                    else
                    {
                        throwError("Not enough arguments given - see 'man rename'");
                    }
                    break;
                case "pwd":
                    if (line.Length > 1) { cache[Int16.Parse(line[1])] = Directory.GetCurrentDirectory().Substring(3); }
                    else
                        Console.WriteLine(Directory.GetCurrentDirectory().Substring(3));
                    break;
                case "date":
                    if (line.Length > 1) { cache[Int16.Parse(line[1])] = DateTime.Now.ToString(); }
                    else
                        Console.WriteLine(DateTime.Now.ToString());
                    break;
                case "run":
                    if (line.Length > 1)
                        runApplet(line[1]);
                    else
                        throwError("No file specified.");
                    break;
                // math functions utiliying cache as variables
                case "math":
                    try
                    {
                        // check if there are 5 args
                        if (line.Length >= 5)
                        {
                            switch (line[1])
                            {
                                // check for the second arg (add, sub, mul, div, sqrt, pow or mod)
                                case "add":
                                    // check if the third arg is a number
                                    if (Int16.TryParse(line[2], out Int16 num1))
                                    {
                                        // check if the fourth arg is a number
                                        if (Int16.TryParse(line[3], out Int16 num2))
                                        {
                                            // check if the fifth arg is a number
                                            if (Int16.TryParse(line[4], out Int16 num3))
                                            {
                                                // add the numbers
                                                cache[num1] = (Int16.Parse(cache[num2]) + Int16.Parse(cache[num3])).ToString();
                                            }
                                            else
                                            {
                                                throwError("Error in mathematical expression.");
                                            }
                                        }
                                        else
                                        {
                                            throwError("Error in mathematical expression.");
                                        }
                                    }
                                    else
                                    {
                                        throwError("Error in mathematical expression.");
                                    }

                                    break;
                                case "sub":
                                    // check if the third arg is a number
                                    if (Int16.TryParse(line[2], out Int16 num4))
                                    {
                                        // check if the fourth arg is a number
                                        if (Int16.TryParse(line[3], out Int16 num5))
                                        {
                                            // check if the fifth arg is a number
                                            if (Int16.TryParse(line[4], out Int16 num6))
                                            {
                                                // subtract the numbers
                                                cache[num4] = (Int16.Parse(cache[num5]) - Int16.Parse(cache[num6])).ToString();
                                            }
                                            else
                                            {
                                                throwError("Error in mathematical expression.");
                                            }
                                        }
                                        else
                                        {
                                            throwError("Error in mathematical expression.");
                                        }
                                    }
                                    else
                                    {
                                        throwError("Error in mathematical expression.");
                                    }
                                    break;
                                case "mul":

                                    // check if the third arg is a number
                                    if (Int16.TryParse(line[2], out Int16 num7))
                                    {
                                        // check if the fourth arg is a number
                                        if (Int16.TryParse(line[3], out Int16 num8))
                                        {
                                            // check if the fifth arg is a number
                                            if (Int16.TryParse(line[4], out Int16 num9))
                                            {
                                                // multiply the numbers
                                                cache[num7] = (Int16.Parse(cache[num8]) * Int16.Parse(cache[num9])).ToString();
                                            }
                                            else
                                            {
                                                throwError("Error in mathematical expression.");
                                            }
                                        }
                                        else
                                        {
                                            throwError("Error in mathematical expression.");
                                        }
                                    }
                                    else
                                    {
                                        throwError("Error in mathematical expression.");
                                    }
                                    break;
                                case "div":
                                    // check if the third arg is a number
                                    if (Int16.TryParse(line[2], out Int16 num10))
                                    {
                                        // check if the fourth arg is a number
                                        if (Int16.TryParse(line[3], out Int16 num11))
                                        {
                                            // check if the fifth arg is a number
                                            if (Int16.TryParse(line[4], out Int16 num12))
                                            {
                                                // divide the numbers
                                                cache[num10] = (Int16.Parse(cache[num11]) / Int16.Parse(cache[num12])).ToString();
                                            }
                                            else
                                            {
                                                throwError("Error in mathematical expression.");
                                            }
                                        }
                                        else
                                        {
                                            throwError("Error in mathematical expression.");
                                        }
                                    }
                                    else
                                    {
                                        throwError("Error in mathematical expression.");
                                    }
                                    break;
                                case "sqrt":
                                    // check if the third arg is a number
                                    if (Int16.TryParse(line[2], out Int16 num13))
                                    {
                                        // check if the fourth arg is a number
                                        if (Int16.TryParse(line[3], out Int16 num14))
                                        {
                                            // square root the number
                                            cache[num13] = Math.Sqrt(Int16.Parse(cache[num14])).ToString();
                                        }
                                        else
                                        {
                                            throwError("Error in mathematical expression.");
                                        }
                                    }
                                    else
                                    {
                                        throwError("Error in mathematical expression.");
                                    }
                                    break;
                                case "pow":
                                    // check if the third arg is a number
                                    if (Int16.TryParse(line[2], out Int16 num15))
                                    {
                                        // check if the fourth arg is a number
                                        if (Int16.TryParse(line[3], out Int16 num16))
                                        {
                                            // check if the fifth arg is a number
                                            if (Int16.TryParse(line[4], out Int16 num17))
                                            {
                                                // raise the number to the power
                                                cache[num15] = Math.Pow(Int16.Parse(cache[num16]), Int16.Parse(cache[num17])).ToString();
                                            }
                                            else
                                            {
                                                throwError("Error in mathematical expression.");
                                            }
                                        }
                                        else
                                        {
                                            throwError("Error in mathematical expression.");
                                        }
                                    }
                                    else
                                    {
                                        throwError("Error in mathematical expression.");
                                    }
                                    break;
                                case "mod":
                                    // check if the third arg is a number
                                    if (Int16.TryParse(line[2], out Int16 num18))
                                    {
                                        // check if the fourth arg is a number
                                        if (Int16.TryParse(line[3], out Int16 num19))
                                        {
                                            // check if the fifth arg is a number
                                            if (Int16.TryParse(line[4], out Int16 num20))
                                            {
                                                // get the remainder
                                                cache[num18] = (Int16.Parse(cache[num19]) % Int16.Parse(cache[num20])).ToString();
                                            }
                                            else
                                            {
                                                throwError("Error in mathematical expression.");
                                            }
                                        }
                                        else
                                        {
                                            throwError("Error in mathematical expression.");
                                        }
                                    }
                                    else
                                    {
                                        throwError("Error in mathematical expression.");
                                    }
                                    break;
                                default:
                                    throwError("Error in mathematical expression.");
                                    break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        string error = e.ToString();
                        throwError("Error in mathematical expression.");
                    }

                    break;
                case "if":
                    // check if cache[line[1]] has the value of [cache[line[2]], if so, excecute the next line. If not, skip it.
                    if (cache[Int16.Parse(line[1])] != cache[Int16.Parse(line[2])])
                    {
                        // get the file that is currently being excecuted
                        yunScriptLineNum++;
                    }
                    break;
                case "print":
                    // print every line[] after line[0]
                    for (int i = 1; i < line.Length; i++)
                    {
                        Console.Write(line[i] + " ");
                    }
                    break;
                case "":
                    // do nothing
                    break;
                default:
                    throwError($"Unknown function '{line[0]}'.");
                    break;

            }
        }

        static bool deleteUser(string username)
        {
            try
            {
                File.Delete($"{syspath + username}.yuser");
                Directory.Delete($"{syspath + username}", true);
                return true;
            } 
            catch(IOException) { return false; }
        }

        static void createUser(string username, string hashedPassword)
        {
            File.Create($"{syspath + username}.yuser").Close();
            File.WriteAllText($"{syspath + username}.yuser", hashedPassword);
            Directory.CreateDirectory(syspath + username);
            Directory.CreateDirectory($"{syspath + username}\\home");
        }

        static bool userExists(string username) {
            return File.Exists($"{syspath}{username}.yuser");
        }

        static void installProgram(string url, string path, string name, bool zip)
        {
            WebClient client = new WebClient();
            Console.Write($"Downloading {name}...");

            string destination = temppath + "\\temp.zip";
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
                ZipFile.ExtractToDirectory(temppath + "\\temp.zip", path);
                try { File.Delete(temppath + "\\temp.zip"); }
                catch (Exception e) { throwError(e.Message); }
            }
            Console.WriteLine($"\nFinished Installing {name}!");
        }

        static void runProcess(string program, string path, bool announceExit = true)
        {
            if(path != null && !(path.Contains("\\") || path.Contains("/")))
            {
                path = $"{Directory.GetCurrentDirectory()}\\{path}";
            }
            var process = Process.Start(program, path);
            process.WaitForExit();
            if(announceExit)
            {
                int exitCode = process.ExitCode;
                if (exitCode != 0)
                    Console.Write("\nProgram exited with Exit Code 0.");
                else
                    throwError($"Program exited with Exit Code {exitCode}.");

            }
        }

        static bool prompt(string message, ConsoleColor col)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{message} (Y/n): ");
            Console.ResetColor();

            // For a T/F Prompt, read a Char instead of a String for faster Setup.
            char input = Char.ToLower(Console.ReadKey().KeyChar);

            if (input != 'y' && input != 'n')
            {
                throwError("Invalid Input. Please try again.");
                return prompt(message, col);
            }

            Console.WriteLine();
            Console.ForegroundColor = col;
            return input == 'y';
        }
    }
}