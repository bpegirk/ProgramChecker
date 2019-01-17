using System.Linq;
using ProgramChecker.classes;

namespace ProgramChecker.Languages
{
    class VCpp : Language
    {
        private new static string nameScript = "msvcpp2013.cmd";
        
        public VCpp(Check check) : base(check)
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
                .Where(x => x.Contains("error"))
                .ToArray();
        }
    }
}