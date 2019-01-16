using ProgramChecker.classes;

namespace ProgramChecker.Languages
{
    class VCpp : Language
    {
        private static string nameScript = "msvcpp2013.cmd";
        
        public VCpp(Check check) : base(check)
        {
        }

        public override bool compile()
        {
            return runScriptCompile(nameScript);
        }
    }
}