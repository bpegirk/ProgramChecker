using ProgramChecker.classes;

namespace ProgramChecker.Languages
{
    class Delphi : Language
    {
        private static string nameScript = "delphi.cmd";
        
        public Delphi(Check check) : base(check)
        {
        }

        public override bool compile()
        {
            return runScriptCompile(nameScript);
        }
    }
}