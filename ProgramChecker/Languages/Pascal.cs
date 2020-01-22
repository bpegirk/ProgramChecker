using System.IO;
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

        public override bool checkException()
        {
            string str = string.Empty;
            using (StreamReader reader = File.OpenText(pathFile))
            {
                str = reader.ReadToEnd();
            }

            return str.Contains("raise");
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