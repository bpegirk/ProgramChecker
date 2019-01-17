using System.Linq;
using ProgramChecker.classes;

namespace ProgramChecker.Languages
{
    class Pascal : Language
    {
        private new static string nameScript = "pasabc.cmd";
        
        public Pascal(Check check) : base(check)
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
                .Where(x => x.Contains($"check_{check.checkId}.pas:"))
                .ToArray();
        }
    }
}