using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProgramChecker.classes;

namespace ProgramChecker.Languages
{
    class CSharp : Language
    {
        private new static string nameScript = "msvcs2013.cmd";
        public CSharp(Check check) : base(check)
        {
            pathCompile += @"\check_" + check.checkId;
        }

        public override bool compile()
        {
            return runScriptCompile(nameScript);
        }

        protected override void checkError()
        {
            base.checkError();
            errors = errors
                .Where(x => x.Contains("error CS"))
                .ToList();
        }
    }
}