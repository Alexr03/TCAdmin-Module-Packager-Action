using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace TCAdmin_Module_Packager_Action
{
    class Program
    {
        static void Main(string[] args)
        {
            SetupEnvironment();
            Log.Information("TCAdmin - The Game Hosting Control Panel");
            Log.Information("Running Module Packager");

            // Base Directory
            Directory.CreateDirectory("./outputdeployment/Monitor");
            var config = JsonConvert.DeserializeObject<JObject>("Module.json");
            if (Directory.Exists("./Views/Default"))
            {
                CopyDirectory("./Views", "./outputdeployment/Views");
            }
            else
            {
                CopyDirectory("./Views/Default", "./outputdeployment/Views/Default");
            }
            
            Log.Information("Copying Module.json");
            File.Copy("./Module.json", "./outputdeployment/Module.json", true);
            File.Copy($"./bin/Debug/{config["Dll"]}", $"./outputdeployment/{config["Dll"]}", true);
            File.Copy($"./bin/Debug/{config["Dll"]}", $"./outputdeployment/Monitor/{config["Dll"]}", true);
            Log.Information("Copying DLL - " + config["Dll"]);

            var dependencies = config["Dependencies"]?.ToObject<List<string>>();
            if (dependencies != null)
                foreach (var dependency in dependencies)
                {
                    Log.Information("Copying Dependency - " + Path.GetFileName(dependency));
                    File.Copy(dependency, $"./outputdeployment/{Path.GetFileName(dependency)}", true);
                    File.Copy(dependency, $"./outputdeployment/Monitor/{Path.GetFileName(dependency)}", true);
                }

            foreach (var file in Directory.GetFiles("./", "*.sql"))
            {
                Log.Information("Copying SQL - " + Path.GetFileName(file));
                File.Copy(file, $"./outputdeployment/{Path.GetFileName(file)}", true);
                File.Copy(file, $"./outputdeployment/Monitor/{Path.GetFileName(file)}", true);
            }
            
            ZipFile.CreateFromDirectory("./outputdeployment", "./output.zip", CompressionLevel.Optimal, false);
            Log.Information("Created zip artifact");
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

        private static void CopyDirectory(string source, string destination)
        {
            Directory.CreateDirectory(destination);
            //Now Create all of the directories
            foreach (var dirPath in Directory.GetDirectories(source, "*", 
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(source, destination));

            //Copy all the files & Replaces any files with the same name
            foreach (var newPath in Directory.GetFiles(source, "*.*", 
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(source, destination), true);
        }
    }
}