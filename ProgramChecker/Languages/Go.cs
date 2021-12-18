using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ProgramChecker.classes;

namespace ProgramChecker.Languages
{
    public class Go : Language
    {
        private new static string nameScript = "golang.cmd";
        
        public Go(Check check) : base(check)
        {
        }
        
        public override bool checkException()
        {
            string str = string.Empty;
            using (StreamReader reader = File.OpenText(pathFile))
            {
                str = reader.ReadToEnd();
            }

            string pattern = @"(\bpanic\b)";
            Regex rgx = new Regex(pattern);
            string find = rgx.Match(str).ToString();

            return !find.Equals("");
        }
        
        protected override void checkError()
        {
            base.checkError();
            errors = errors
                .Where(x => x.Contains($"check_{check.checkId}.go"))
                .ToArray();
        }

        public override bool compile()
        {
            return runScriptCompile(nameScript);
        }
    }
}