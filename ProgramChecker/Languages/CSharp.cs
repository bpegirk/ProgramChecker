using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        }

        public override bool compile()
        {
            Directory.CreateDirectory(pathCompile);
            string compileFile = $"{pathCompile}check_{check.checkId}.cs";
            if (!File.Exists(compileFile))
            {
                File.Copy(pathFile, compileFile);
            }
            string csprojFile = $"{pathCompile}check_{check.checkId}.csproj";
            if (!File.Exists(csprojFile))
            {
                File.Create(csprojFile).Close();
                File.WriteAllText(csprojFile,  Program.globalConfig["build"]["net"]);
            }
            return runScriptCompile(nameScript);
        }
        
        protected override Process createProcess()
        {
            var compile = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pathScript + nameScript,
                    Arguments = pathCompile + " " + pathCompile,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    StandardOutputEncoding = Encoding.UTF8,
                }
            };

            return compile;
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