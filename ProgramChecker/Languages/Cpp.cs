using System.Linq;
using ProgramChecker.classes;

namespace ProgramChecker.Languages
{
    class Cpp : Language
    {
        private static string nameFile = "cpp.cmd";
        
        public Cpp(Check check) : base(check)
        {
        }

        public override bool compile()
        {
            return runScriptCompile(nameFile);
        }
        
        protected override void checkError()
        {
            base.checkError();
            errors = errors
                .Where(x => x.Contains("Error"))
                .ToList();
        }
    }
}