﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProgramChecker.classes
{
    public class Testing
    {
        private Test test;
        private int checkId;

        public Testing(Test test, int checkId)
        {
            this.test = test;
            this.checkId = checkId;
        }

        public Result testing()
        {
            string pathSrc = Program.globalConfig["paths"]["src"] + $@"check_{checkId}\";
            string testSrc = pathSrc + $@"test_{test.id}\";
            string testExe = testSrc + $"check_{checkId}.exe";
            string outtext = "";
            if (!Directory.Exists(testSrc))
            {
                Directory.CreateDirectory(testSrc);
            }

            if (File.Exists(pathSrc + $"check_{checkId}.exe"))
            {
                File.Copy(pathSrc + $"check_{checkId}.exe", testExe);
            }

            using (FileStream fs = File.Create(testSrc + "/input.txt"))
            {
                Byte[] input = new UTF8Encoding(true).GetBytes(test.input);
                fs.Write(input, 0, input.Length);
            }


            runTestFile(testExe, testSrc);

            using (StreamReader sr = File.OpenText(testSrc + "/output.txt"))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    outtext = s;
                }
            }

            return new Result()
            {
                test_id = test.id,
                status = outtext.Equals(test.output),
                outtext = outtext
            };
        }

        private void runTestFile(string testExe, string testSrc)
        {
            Task runTesTask = new Task(() =>
            {
                var compile = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = testExe,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        WorkingDirectory = testSrc
                    }
                };
                Console.WriteLine("test");
                compile.Start();
                compile.WaitForExit(3000);
            });

            runTesTask.Start();

           Task.WaitAll(runTesTask);
        }
    }
}