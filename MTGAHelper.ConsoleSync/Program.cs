using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MTGAHelper.ConsoleSync.Services;
using MTGAHelper.Entity;
using MTGAHelper.Lib;
using MTGAHelper.Lib.CacheLoaders;
using MTGAHelper.Lib.IoC;
using MTGAHelper.Lib.OutputLogParser;
using MTGAHelper.Lib.OutputLogParser.IoC;
using SimpleInjector;

namespace MTGAHelper.ConsoleSync
{
    public class Program
    {
        static void Main(string[] args)
        {
            var (userId, logFilePath) = ParseArgs(args);
            if (userId == default)
                return;

            var container = CreateContainer();

            Console.WriteLine("Initializing...");
            container.Verify();

            Console.WriteLine($"Preparing to update data for user {userId}");
            container.GetInstance<LogFileProcessor>().Process(userId, logFilePath);

            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }

        private static (string userId, string logFilePath) ParseArgs(string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    return (GetUserId(), GetPath());
                case 1 when args[0].Length < 7 && args[0].Contains("help"):
                    // --help; /help; -help, ...
                    return ShowHelp();
                case 1 when args[0].Contains("."):
                    // userId never contains a dot, file path always does
                    return (GetUserId(), args[0]);
                case 1:
                    return (args[0], GetPath());
                case 2:
                    return (args[0], args[1]);
                default:
                    return ShowHelp();
            }
        }

        private static readonly string[] defaultPaths =
        {
            // https://docs.unity3d.com/Manual/LogFiles.html
            $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/../LocalLow/Wizards Of The Coast/MTGA/Player.log", // Windows
            $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Library/Logs/Wizards Of The Coast/MTGA/Player.log",    // Mac
            $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.config/unity3d/CompanyName/ProductName/Player.log",   // Linux
        };

        private static string GetPath()
        {
            var path = AskForPath();
            // for use with drag and dropping the log file, windows surrounds the path with quotes, which we don't want.
            var pathNoQuotes = path.StartsWith("\"") && path.EndsWith("\"") ? path[1..^1] : path;
            return Path.GetFullPath(string.IsNullOrWhiteSpace(pathNoQuotes) ? "." : pathNoQuotes);
        }

        private static string AskForPath()
        {
            Console.WriteLine("No log file path given in command line arguments.");

            var defaultPath = defaultPaths.FirstOrDefault(File.Exists);
            if (defaultPath != default)
            {
                Console.WriteLine($"Log file found at default path ({Path.GetFullPath(defaultPath)})");
                Console.WriteLine("Press <Enter> to use this path. You can also provide a different path (tip: drag and drop file to paste location):");
                var path = Console.ReadLine();

                return string.IsNullOrWhiteSpace(path)
                    ? defaultPath
                    : path;
            }
            Console.WriteLine("Please type the path to use (tip: drag and drop file to paste location):");
            return Console.ReadLine();
        }

        private static string GetUserId()
        {
            Console.WriteLine("No MTGAHelper User id given in command line arguments.");
            Console.WriteLine("(your user id can be found on the website at https://mtgahelper.com/profile)");
            Console.WriteLine("Please enter your user id:");
            return Console.ReadLine();
        }

        public static Container CreateContainer()
        {
            var container = new Container()
                .RegisterServicesLibOutputLogParser()
                .RegisterServicesShared();

            container.RegisterSingleton<IPossibleDateFormats, DateFormatsHardCoded>();
            container.Register<LogFileProcessor>();
            container.RegisterSingleton<ServerApiCaller>();
            container.RegisterSingleton<ICacheLoader<Dictionary<int, Card>>, CacheLoaderCardsByApi>();
            container.RegisterDecorator<ICacheLoader<Dictionary<int, Card>>, CardLoaderAddLinkedFaceCardDecorator>(Lifestyle.Singleton);
            container.RegisterMapperConfig();
            return container;
        }

        static (string, string) ShowHelp()
        {
            Console.WriteLine("==============================================");
            Console.WriteLine("MTGAHelper.ConsoleSync.exe usage:");
            Console.WriteLine("");
            Console.WriteLine("Call the program with two arguments:");
            Console.WriteLine("   1. Your MTGAHelper user id (can be found in profile on the website)");
            Console.WriteLine("   2. The file location of the log file");
            Console.WriteLine("");
            Console.WriteLine("For example (on Windows, assuming your user id is 'user1234id'):");
            Console.WriteLine("MTGAHelper.ConsoleSync.exe user1234id \"C:\\Users\\MyUser\\AppData\\LocalLow\\Wizards Of The Coast\\MTGA\\Player.log\"");
            Console.WriteLine("");
            Console.WriteLine("The program will process your log file and upload the parsed content to the MTGAHelper server.");
            Console.WriteLine("You can then go to the MTGAHelper website to browse your latest data.");
            Console.WriteLine("==============================================");

            return default;
        }
    }
}
