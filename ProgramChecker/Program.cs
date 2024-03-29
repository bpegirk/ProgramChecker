﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using ProgramChecker.classes;
using System.IO;
using System.Threading;
using IniParser;
using IniParser.Model;
using Newtonsoft.Json;
using ProgramChecker.Languages;

namespace ProgramChecker
{
    class Program
    {
        public static IniData globalConfig;
        private static Check param;

        //private static Check activeCheck;
        static void Main(string[] args)
        {
            var mainConfig = new IniData();
            String currDir = Directory.GetCurrentDirectory() + "\\";
            String configFile = currDir + "config.ini";
            mainConfig["paths"]["src"] = currDir + "src\\";
            mainConfig["paths"]["queue"] = currDir + "queue\\";
            mainConfig["paths"]["results"] = currDir + "results\\";
            mainConfig["paths"]["scripts"] = currDir + "scripts\\";
            mainConfig["build"]["net"] =
                @"<Project Sdk=""Microsoft.NET.Sdk""><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net6.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable><PublishSingleFile>true</PublishSingleFile><SelfContained>true</SelfContained></PropertyGroup></Project>";
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

            if (!Directory.Exists(globalConfig["paths"]["scripts"]))
            {
                Directory.CreateDirectory(globalConfig["paths"]["scripts"]);
            }

            // check exist files
            foreach (String file in Directory.GetFiles(globalConfig["paths"]["queue"], "*.new"))
            {
                runCheck(file);
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
            runCheck(e.FullPath);
        }

        private static void parseJSON(String file)
        {
            String fileContent = File.ReadAllText(file);
            Console.Write("==> Try parse JSON ... ");
            param = JsonConvert.DeserializeObject<Check>(fileContent);
            Console.WriteLine("DONE.");
        }

        private static void runCheck(String file)
        {
            Console.WriteLine("File " + Path.GetFileName(file) + " come. Start work");
            // try parse file
            Thread.Sleep(1000);
            try
            {
                parseJSON(file);
                Console.Write("==> Try parse compile... ");
                String compileStatus = compileCode(param);

                string resultFolder = globalConfig["paths"]["results"];
                OutResult outResult;
                if (compileStatus == "ok")
                {
                    Console.WriteLine("DONE.");

                    Console.Write("==> Run tests...");
                    outResult = runTests(param);
                    Console.WriteLine("DONE.");
                }
                else
                {
                    Console.WriteLine("error: " + compileStatus);
                    outResult = new OutResult()
                    {
                        checkId = param.checkId,
                        isError = true,
                        error = compileStatus
                    };
                }

                Console.Write("==> Write data to result.txt ... ");

                writeResult(param.checkId, outResult);

                Console.WriteLine("DONE.");
            }
            catch (Exception ex)
            {
                Console.Write("==> Write data to result.txt ... ");

                OutResult outResult = new OutResult();
                outResult.checkId = param.checkId;
                outResult.isError = true;
                outResult.parse_dec = param.parseDec;
                outResult.error = ex.Message;
                outResult.results = null;

                writeResult(param.checkId, outResult);

                Console.Write("Can't read file " + file + ": " + ex.Message);
            }

            Console.WriteLine("CheckDone");
            // rename file
            File.Move(file, file.Substring(0, file.Length - 3) + "old");
            Console.WriteLine("####### Wait next #########");
        }

        private static void writeResult(int checkId, OutResult outResult)
        {
            string resultFolder = globalConfig["paths"]["results"];
            if (checkId >= 0 && !File.Exists(resultFolder + $@"result_{checkId}.txt"))
            {
                using (FileStream fs = File.Create(resultFolder + $@"result_{checkId}.txt"))
                {
                    Byte[] input = new UTF8Encoding(true).GetBytes(JsonConvert.SerializeObject(outResult));
                    fs.Write(input, 0, input.Length);
                }
            }
        }

        private static OutResult runTests(Check param)
        {
            var tests = param.tests;
            int checkId = param.checkId;
            int memoryLimit = param.memoryLimit;
            int timeOut = param.timeout;
            int parseDec = param.parseDec;
            bool timeout = false;
            bool memeryLimit = false;
            bool testingError = false;
            string error = "";
            List<Result> results = new List<Result>();
            foreach (var test in tests)
            {
                Testing testing = new Testing(test, checkId, memoryLimit, timeOut, parseDec);
                results.Add(testing.testing());
                if (!timeout) timeout = testing.getTimeout();
                if (!memeryLimit) memeryLimit = testing.getMemeryLimit();
                if (!testingError) testingError = testing.getTestingError();
            }

            if (timeout) error = "Timeout";
            if (memeryLimit) error = "Memory Limit";
            if (testingError) error = Compiller.language.getError();
            return new OutResult()
            {
                checkId = checkId,
                isError = timeout || memeryLimit || testingError,
                error = error,
                parse_dec = param.parseDec,
                results = results
            };
        }

        private static String compileCode(Check param)
        {
            // there is compiling ^^
            String file = globalConfig["paths"]["src"] + param.fileName;
            if (File.Exists(file))
            {
                Compiller cp = new Compiller(param);

                if (cp.checkException())
                {
                    return "Использование Exception запрещено!";
                }

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