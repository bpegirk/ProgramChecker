using System.Diagnostics;
using System.Linq;
using System.Text;
using ProgramChecker.classes;

namespace ProgramChecker.Languages
{
    class VB : Language
    {
        private new static string nameScript = "msvb2013.cmd";
        
        public VB(Check check) : base(check)
        {
            pathCompile += @"\check_" + check.checkId;
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
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    StandardOutputEncoding = Encoding.Default,
                }
            };

            return compile;
        }
        
        protected override void checkError()
        {
            base.checkError();
            errors = errors
                .Where(x => x.Contains("error BC"))
                .ToArray();
        }
    }
}