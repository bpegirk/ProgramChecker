using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramChecker.classes;
using System.IO;
using IniParser;
using IniParser.Model;
using Newtonsoft.Json;

namespace ProgramChecker
{
    class Program
    {
        private static IniData globalConfig;
        private static Check activeCheck;
        static void Main(string[] args)
        {

            var mainConfig = new IniData();
            String currDir = Directory.GetCurrentDirectory() + "\\";
            String configFile = currDir + "config.ini";
            mainConfig["paths"]["src"] = currDir + "src\\";
            mainConfig["paths"]["queue"] = currDir + "queue\\";
            mainConfig["paths"]["results"] = currDir + "results\\";
            // init config
            var parser = new FileIniDataParser();
            if (!File.Exists(configFile))
            {
                // write default setting               
                parser.WriteFile(configFile, mainConfig);
            }
            IniData fileConfig = parser.ReadFile(configFile);
            mainConfig.Merge(fileConfig);

            globalConfig = mainConfig;

            // create folders if not exists

            if (!Directory.Exists(globalConfig["paths"]["src"]))
            {
                Directory.CreateDirectory(globalConfig["paths"]["src"]);
            }

            if (!Directory.Exists(globalConfig["paths"]["queue"]))
            {
                Directory.CreateDirectory(globalConfig["paths"]["queue"]);
            }
            if (!Directory.Exists(globalConfig["paths"]["results"]))
            {
                Directory.CreateDirectory(globalConfig["paths"]["results"]);
            }
            // run folder listiner
            listenFolder();

            // wait - not to end
            new System.Threading.AutoResetEvent(false).WaitOne();
        }

        private static void listenFolder()
        {
            String srcFolder = globalConfig["paths"]["src"];
            string queueFolder = globalConfig["paths"]["queue"];

            FileSystemWatcher watcher = new FileSystemWatcher();

            watcher.Path = queueFolder;
            watcher.NotifyFilter = NotifyFilters.FileName;
            watcher.Filter = "*.new";
            watcher.Created += new FileSystemEventHandler(fileRecived);
            watcher.EnableRaisingEvents = true;
            Console.WriteLine("Start watching files in folder " + queueFolder + "...");
        }

        private static void fileRecived(object source, FileSystemEventArgs e)
        {
            Console.Write("File " + Path.GetFileName(e.FullPath) + " come. Try parse...");
            // try parse file
            try
            {
                String fileContent = File.ReadAllText(e.FullPath);
                Check param = JsonConvert.DeserializeObject<Check>(fileContent);

                String compileStatus = compileCode(param);

                if (compileStatus == "ok")
                {
                    runTests(param);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                Console.Write("Can't read file " + e.FullPath + ": " + ex.Message);
            }
            Console.WriteLine("Done");
        }

        private static String compileCode(Check param)
        {
            // there is compiling ^^
            String file = globalConfig["paths"]["src"] + param.fileName;
            if (File.Exists(file))
            {
                Compiller cp = new Compiller(param.language, file);

                bool cpStatus = cp.compile();
                if (cpStatus)
                {
                    return "ok";
                }
                return cp.getError();
            }
            else
            {
                return "file not found";
            }
        }
    }
}
