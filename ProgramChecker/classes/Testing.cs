using System;
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
        private bool isForceKill;
        private bool isMemoryLimit;
        private int peakMemory;
        private int timeOut;
        private long spentTime;
        private int spentMemory;
        private int isParseDec;


        public Testing(Test test, int checkId, int peakMemory, int timeOut, int isParseDec)
        {
            this.test = test;
            this.checkId = checkId;
            this.peakMemory = peakMemory;
            this.timeOut = timeOut * 1000;
            this.isParseDec = isParseDec;
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
            int st = 0;

            if (!isMemoryLimit && !isForceKill)
            {
                using (StreamReader sr = File.OpenText(testSrc + "/output.txt"))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        if (isParseDec == 1)
                        {
                            outtext = s.Replace(',', '.');
                        }
                        else
                        {
                            outtext = s;
                        }
                    }
                }
                outtext = outtext.Trim();
                test.output = test.output.Trim();
                if (outtext.Equals(test.output))
                {
                    st = Check.PASS_SUCCESS;
                }
                else
                {
                    st = Check.PASS_FAIL;
                }
            }
            else if (isMemoryLimit)
            {
                st = Check.PASS_MEMORY_LIMIT;
                outtext = "Memory Limit";
            }
            else
            {
                st = Check.PASS_TIMEOUT;
                outtext = "Timeout";
            }

            return new Result()
            {
                test_id = test.id,
                status = st,
                outtext = outtext,
                memory = spentMemory,
                time = spentTime
            };
        }

        private void runTestFile(string testExe, string testSrc)
        {
            isForceKill = false;
            isMemoryLimit = false;

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
                spentTime = 0;
                Stopwatch w = new Stopwatch();
                compile.Start();
                w.Start();
                do
                {
                    if (compile.PeakPagedMemorySize64 / 1024.0 > peakMemory) isMemoryLimit = true;
                    if (!compile.WaitForExit(timeOut))
                    {
                        isForceKill = true;
                        compile.Kill();
                    }

                } while (!compile.WaitForExit(timeOut));
                w.Stop();
                spentTime = w.ElapsedMilliseconds;
                spentMemory = (int)(compile.PeakPagedMemorySize64 / 1024.0);
            });


            runTesTask.Start();

            Task.WaitAll(runTesTask);
        }


        public bool getTimeout()
        {
            return isForceKill;
        }

        public bool getMemeryLimit()
        {
            return isMemoryLimit;
        }
    }
}