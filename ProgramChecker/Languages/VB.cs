using System.Diagnostics;
using System.Text;
using ProgramChecker.classes;

namespace ProgramChecker.Languages
{
    class VB : Language
    {
        private static string nameScript = "msvb2013.cmd";
        
        public VB(Check check) : base(check)
        {
        }

        public override bool compile()
        {
            return runScriptCompile(nameScript, true);
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
                    UseShellExecute = false,
                    StandardOutputEncoding = Encoding.Default,
                }
            };

            return compile;
        }
    }
}