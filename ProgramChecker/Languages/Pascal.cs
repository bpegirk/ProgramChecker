using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
            removeUsesCrt();
            return runScriptCompile(nameScript);
        }

        private void removeUsesCrt()
        {
            string str = string.Empty;
            using (StreamReader reader = File.OpenText(pathFile))
            {
                str = reader.ReadToEnd();
            }

            string pattern = @"uses crt;";
            Regex rgx = new Regex(pattern);
            string usesCrt = rgx.Match(str).ToString();

            str = Regex.Replace(str, pattern, "");
            if (!usesCrt.Equals(""))
                str = str.Replace(usesCrt, "");
            using (StreamWriter file = new StreamWriter(pathFile))
            {
                file.Write(str);
            }
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