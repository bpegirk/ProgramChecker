using ProgramChecker.classes;

namespace ProgramChecker.Languages
{
    class Pascal : Language
    {
        private static string nameScript = "pasabc.cmd";
        
        public Pascal(Check check) : base(check)
        {
        }

        public override bool compile()
        {
            return runScriptCompile(nameScript);
        }
    }
}