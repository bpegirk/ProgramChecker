using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ProgramChecker.classes;

namespace ProgramChecker.Languages
{
    public class Go : Language
    {
        private new static string nameScript = "golang.cmd";

        public Go(Check check) : base(check)
        {
        }

        public override bool checkException()
        {
            string str = string.Empty;
            using (StreamReader reader = File.OpenText(pathFile))
            {
                str = reader.ReadToEnd();
            }

            string pattern = @"(\bpanic\b)";
            Regex rgx = new Regex(pattern);
            string find = rgx.Match(str).ToString();

            return !find.Equals("");
        }

        protected override void checkError()
        {
            string error = compileProcess.StandardError.ReadToEnd();
            errors = error.Split('\n').ToList()
                .Where(x => x.Contains($"check_{check.checkId}.go"))
                .ToList();
        }

        public override bool compile()
        {
            return runScriptCompile(nameScript);
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
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    StandardOutputEncoding = Encoding.UTF8,
                }
            };

            return compile;
        }
    }
}