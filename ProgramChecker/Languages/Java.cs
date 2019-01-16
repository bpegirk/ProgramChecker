using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgramChecker.classes;
using System.Diagnostics;
using System.IO;

namespace ProgramChecker.Languages
{
    class Java : Language
    {
        private static string nameFile = "java.cmd";

        public Java(Check check) : base(check, "class")
        {

        }
        public override bool compile()
        {
           return runScriptCompile(nameFile);
        }

        public override Process createProcess()
        {
            var compile = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pathScript + nameScript,
                    Arguments = pathFile + " " + pathExe,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    StandardOutputEncoding = null,
                }
            };

            return compile;
        }

        public override string[] checkError()
        {
            outString = compileProcess.StandardOutput.ReadToEnd();
            string error = compileProcess.StandardError.ReadToEnd();
            string[] allMessage = outString.Split('\n').Concat(error.Split('\n')).ToArray();
            errors = allMessage
                .Where(x => x.Contains("Warning") || x.Contains($"check_{check.checkId}.pas:") || x.Contains("error CS") ||
                x.Contains("error BC") || x.Contains("Error") || x.Contains("error"))
                .ToArray();

            return errors;
        }

        public override bool afterCompile()
        {
            bool isFileExist = File.Exists(checkFile); ;
            if (!isFileExist)
            {
                lastError = errors.Length == 0 ? outString : errors.Last();
            }
            else
            {
                File.Copy(pathFile, pathExe + "\\check_" + check.checkId + ".java");
            }

            return isFileExist;
        }

        public override string prepareTesting(Test test)
        {
            int checkId = check.checkId;

            string pathSrc = Program.globalConfig["paths"]["src"] + $@"check_{checkId}\";
            string testSrc = pathSrc + $@"test_{test.id}\";

            if (File.Exists(pathSrc + $"check_{checkId}.java"))
            {
                File.Copy(pathSrc + $"check_{checkId}.java", testSrc + $"check_{checkId}.java");
                File.Copy(pathSrc + $"check_{checkId}.class", testSrc + $"check_{checkId}.class");
            }

            return $"check_{checkId}";
        }

        public override Process createTestProcess(string testExe, string testSrc)
        {
            var compile = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pathScript + "javarun.cmd",
                    Arguments = testExe,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WorkingDirectory = testSrc
                }
            };

            return compile;
        }

    }
}
