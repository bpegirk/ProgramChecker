using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ProgramChecker.classes;

namespace ProgramChecker.Languages
{
    class VB : Language
    {
        private new static string nameScript = "msvb2013.cmd";
        
        public VB(Check check) : base(check)
        {
        }

        public override bool compile()
        {
            Directory.CreateDirectory(pathCompile);
            string compileFile = $"{pathCompile}check_{check.checkId}.vb";
            if (!File.Exists(compileFile))
            {
                File.Copy(pathFile, compileFile);
            }
            string vbprojFile = $"{pathCompile}check_{check.checkId}.vbproj";
            if (!File.Exists(vbprojFile))
            {
                File.Create(vbprojFile).Close();
                File.WriteAllText(vbprojFile,  Program.globalConfig["build"]["net"]);
            }
            return runScriptCompile(nameScript);
        }

        public override bool checkException()
        {
            string str = string.Empty;
            using (StreamReader reader = File.OpenText(pathFile))
            {
                str = reader.ReadToEnd();
            }

            string pattern = @"(\bThrow|throw\b)";
            Regex rgx = new Regex(pattern);
            string find = rgx.Match(str).ToString();

            return !find.Equals("");
        }

        protected override Process createProcess()
        {
            var compile = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pathScript + nameScript,
                    Arguments = pathCompile + " " + pathCompile,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    StandardOutputEncoding = Encoding.UTF8,
                }
            };

            return compile;
        }
        
        protected override void checkError()
        {
            base.checkError();
            errors = errors
                .Where(x => x.Contains("error BC"))
                .ToList();
        }
    }
}