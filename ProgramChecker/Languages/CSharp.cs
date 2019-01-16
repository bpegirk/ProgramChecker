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
        private static string nameScript = "msvcs2013.cmd";
        public CSharp(Check check) : base(check)
        {
        }

        public override bool compile()
        {
            return runScriptCompile(nameScript, true);
        }
    }
}