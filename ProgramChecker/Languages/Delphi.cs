using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        public override bool checkException()
        {
            string str = string.Empty;
            using (StreamReader reader = File.OpenText(pathFile))
            {
                str = reader.ReadToEnd();
            }

            string pattern = @"(\braise|Raise\b)";
            Regex rgx = new Regex(pattern);
            string find = rgx.Match(str).ToString();

            return !find.Equals("");
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