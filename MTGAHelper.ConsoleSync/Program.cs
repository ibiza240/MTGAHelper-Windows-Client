using System;
using System.Collections.Generic;
using MTGAHelper.ConsoleSync.Services;
using MTGAHelper.Entity;
using MTGAHelper.Lib;
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
            if (args.Length != 2)
            {
                ShowHelp();
            }
            else
            {
                var container = CreateContainer();

                Console.WriteLine("Initializing...");
                container.Verify();

                var userId = args[0];
                var logFilePath = args[1];

                Console.WriteLine($"Preparing to update data for user {userId}");
                container.GetInstance<LogFileProcessor>().Process(userId, logFilePath);
            }
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
            container.RegisterMapperConfig();
            return container;
        }

        static void ShowHelp()
        {
            Console.WriteLine("==============================================");
            Console.WriteLine("MTGAHelper.ConsoleSync.exe usage:");
            Console.WriteLine("");
            Console.WriteLine("Call the program with two arguments:");
            Console.WriteLine("   1. Your MTGAHelper user id");
            Console.WriteLine("   2. The file location of the log file");
            Console.WriteLine("");
            Console.WriteLine("For example (on Windows):");
            Console.WriteLine("MTGAHelper.ConsoleSync.exe cb4163864000000000061dcf728051d4 \"C:\\Users\\MyUser\\AppData\\LocalLow\\Wizards Of The Coast\\MTGA\\Player.log\"");
            Console.WriteLine("");
            Console.WriteLine("The program will process your log file and upload the parsed content to the MTGAHelper server.");
            Console.WriteLine("You can then go to the MTGAHelper website to browse your latest data.");
            Console.WriteLine("==============================================");
        }
    }
}
