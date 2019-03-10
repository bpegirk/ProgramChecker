using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ProgramChecker.classes;

namespace ProgramChecker.Languages
{
    class Python : Language
    {
        private new static string nameScript = "python.cmd";

        public Python(Check check) : base(check, "py")
        {
        }

        public override bool compile()
        {
            Directory.CreateDirectory(pathCompile);
            string testFile = $"{pathCompile}check_{check.checkId}.py";
            if (!File.Exists(testFile))
                File.Copy(pathFile, testFile);
            using (FileStream fs = File.Create(pathCompile + "input.txt"))
            {
                Byte[] input = new UTF8Encoding(true).GetBytes(check.tests[0].input);
                fs.Write(input, 0, input.Length);
            }

            File.Create($"{pathCompile}output.txt").Close();

            return runScriptCompile(nameScript);
        }

        protected override Process createProcess()
        {
            var compile = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pathScript + nameScript,
                    Arguments = checkFile + " " + pathCompile,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    StandardOutputEncoding = null,
                    WorkingDirectory = pathCompile
                }
            };

            return compile;
        }

        protected override bool afterCompile()
        {
            return lastError == "";
        }

        protected override void checkError()
        {
            lastError = compileProcess.StandardError.ReadToEnd();
        }

        public override string prepareTesting(Test test)
        {
            int checkId = check.checkId;

            string pathSrc = Program.globalConfig["paths"]["src"] + $@"check_{checkId}\";
            string testSrc = pathSrc + $@"test_{test.id}\";
            if (!File.Exists($"{testSrc}output.txt"))
                File.Create($"{testSrc}output.txt").Close();
            return base.prepareTesting(test);
        }

        public override Process createTestProcess(string testFile, string testSrc)
        {
            var compile = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pathScript + "python.cmd",
                    Arguments = testFile,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WorkingDirectory = testSrc
                }
            };

            return compile;
        }
    }
}