using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ProgramChecker.classes
{
    class Compiller
    {

        private int lang;
        private string file;
        private string lastError;
        private string pathScript = @"C:\Users\iumag\Desktop\scripts\";

        public Compiller(int lang, string file)
        {
            if (lang > 0)
            {
                this.lang = lang;
            }
            if (file != null)
            {
                this.file = file;
            }

        }

        public bool compile()
        {
            bool result = false;
            switch (lang)
            {
                case Check.PASCAL:
                    result = comlpilePascal();
                    break;

                case Check.C_SHARP_2008:
                    result = comlpileCSharp(2008);
                    break;
                case Check.C_SHARP_2013:
                    result = comlpileCSharp(2013);
                    break;

                case Check.VB_2008:
                    result = comlpileVB(2008);
                    break;
                case Check.VB_2013:
                    result = comlpileVB(2013);
                    break;

                case Check.C_PLUS_2008:
                    result = comlpileCPlus(2008);
                    break;
                case Check.C_PLUS_2013:
                    result = comlpileCPlus(2013);
                    break;

                case Check.C_BUILDER:
                    result = comlpileCBuilder();
                    break;
                case Check.DELPHI:
                    result = comlpileDelphi();
                    break;
            }

            return result;
        }

        public string getError() {
            return lastError;
        }

        private bool comlpilePascal()
        {  
            return runScriptCompile("pasabc.cmd"); ;
        }

        private bool comlpileCSharp(int ver)
        {
            return runScriptCompile(ver == 2008 ? "msvcs.cmd" : "msvcs2013.cmd");
        }

        private bool comlpileVB(int ver)
        {
            return runScriptCompile(ver == 2008 ? "msvb.cmd" : "msvb2013.cmd");
        }

        private bool comlpileCPlus(int ver)
        {
            return runScriptCompile(ver == 2008 ? "msvcpp.cmd" : "msvcpp2013.cmd");
        }

        private bool comlpileCBuilder()
        {   
            return runScriptCompile("cpp.cmd");
        }
        private bool comlpileDelphi()
        {
            return runScriptCompile("delphi.cmd");
        }

        private bool runScriptCompile(string path)
        {
            var compile = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pathScript + path,
                    Arguments = file,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };

            compile.Start();

            string[] errors = compile.StandardOutput.ReadToEnd().Split('\n');
            lastError = errors[errors.Length - 2];

            return lastError.Trim().Equals("");
        }

    }
}
