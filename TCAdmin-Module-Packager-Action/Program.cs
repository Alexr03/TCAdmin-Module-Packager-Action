using System;
using System.IO;
using Serilog;

namespace TCAdmin_Module_Packager_Action
{
    class Program
    {
        static void Main(string[] args)
        {
            SetupEnvironment();
            Log.Information("Hello World!");
            Log.Information("Directory: " + Directory.GetCurrentDirectory());
        }

        private static void SetupEnvironment()
        {
            DotNetEnv.Env.TraversePath().Load();
            CreateLogger();
            
        }

        private static void CreateLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .CreateLogger();
        }
    }
}