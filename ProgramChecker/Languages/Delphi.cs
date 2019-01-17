using System.Linq;
using ProgramChecker.classes;

namespace ProgramChecker.Languages
{
    class Delphi : Language
    {
        private new static string nameScript = "delphi.cmd";
        
        public Delphi(Check check) : base(check)
        {
        }

        public override bool compile()
        {
            return runScriptCompile(nameScript);
        }
        
        protected override void checkError()
        {
            base.checkError();
            errors = errors
                .Where(x => x.Contains("Error"))
                .ToArray();
        }
    }
}