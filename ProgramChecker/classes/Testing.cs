using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

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

        public void testing()
        {
            string pathSrc = Program.globalConfig["paths"]["src"] + $@"check_{checkId}\";
            string testSrc = pathSrc + $@"test_{test.id}\";
            string testExe = testSrc + $"check_{checkId}.exe";
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
        }

        private void runTestFile(string testExe, string testSrc)
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

            compile.Start();
        }
    }
}