using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgramChecker.classes;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

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
            replaceClassName();

            return runScriptCompile(nameFile);
        }

        protected override Process createProcess()
        {
            var compile = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pathScript + nameScript,
                    Arguments = pathFile + " " + pathCompile,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    StandardOutputEncoding = null,
                }
            };

            return compile;
        }

        protected override void checkError()
        {
            string error = compileProcess.StandardError.ReadToEnd();
            errors = error.Split('\n').ToList();
        }

        protected override bool afterCompile()
        {
            bool isFileExist = File.Exists(checkFile);
            if (!isFileExist)
            {
                lastError = errors.Count == 0 ? outString : errors.First();
            }
            else
            {
                if (!File.Exists(pathCompile + "\\check_" + check.checkId + ".java"))
                    File.Copy(pathFile, pathCompile + "\\check_" + check.checkId + ".java");
            }

            return isFileExist;
        }

        private void replaceClassName()
        {
            string str = string.Empty;
            using (StreamReader reader = File.OpenText(pathFile))
            {
                str = reader.ReadToEnd();
            }

            string pattern = @"(?<=public class ).[^[{]+";
            Regex rgx = new Regex(pattern);
            string className = rgx.Match(str).ToString();

            str = Regex.Replace(str, pattern, $"check_{check.checkId}");
            str = str.Replace(className, $"check_{check.checkId}");
            using (StreamWriter file = new StreamWriter(pathFile))
            {
                file.Write(str);
            }
        }

        public override string prepareTesting(Test test)
        {
            int checkId = check.checkId;

            string pathSrc = Program.globalConfig["paths"]["src"] + $@"check_{checkId}\";
            string testSrc = pathSrc + $@"test_{test.id}\";

            if (File.Exists(pathSrc + $"check_{checkId}.java"))
            {
                if (!File.Exists(testSrc + $"check_{checkId}.java"))
                    File.Copy(pathSrc + $"check_{checkId}.java", testSrc + $"check_{checkId}.java");
                if (!File.Exists(testSrc + $"check_{checkId}.class"))
                    File.Copy(pathSrc + $"check_{checkId}.class", testSrc + $"check_{checkId}.class");
            }

            return $"check_{checkId}";
        }

        public override Process createTestProcess(string testFile, string testSrc)
        {
            var compile = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pathScript + "javarun.cmd",
                    Arguments = testFile,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    StandardErrorEncoding = Encoding.GetEncoding(1251),
                    StandardOutputEncoding = Encoding.GetEncoding(1251),
                    WorkingDirectory = testSrc
                }
            };

            return compile;
        }
    }
}