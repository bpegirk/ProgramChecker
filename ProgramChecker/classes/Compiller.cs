using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ProgramChecker.classes
{
    class Compiller
    {

        private int lang;
        private string file;
        private int checkId;
        private string lastError;

        public Compiller(int lang, string file, int checkId)
        {
            if (lang > 0)
            {
                this.lang = lang;
            }
            if (file != null)
            {
                this.file = file;
            }

            if (checkId > 0)
            {
                this.checkId = checkId;
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

        public string getError()
        {
            return lastError;
        }

        private bool comlpilePascal()
        {
            return runScriptCompile("pasabc.cmd"); ;
        }

        private bool comlpileCSharp(int ver)
        {
            return runScriptCompile(ver == 2008 ? "msvcs.cmd" : "msvcs2013.cmd", true);
        }

        private bool comlpileVB(int ver)
        {
            return runScriptCompile(ver == 2008 ? "msvb.cmd" : "msvb2013.cmd", true, true);
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

        private bool runScriptCompile(string path, bool isExe = false, bool isEncoderVB = false)
        {
            string pathExe = Program.globalConfig["paths"]["src"] + "check_" + checkId + @"\";
            string pathScript = Program.globalConfig["paths"]["scripts"];
            Directory.CreateDirectory(pathExe);
            if (isExe)
            {
                pathExe = pathExe + @"\check_" + checkId + ".exe";
            }
            var compile = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pathScript + path,
                    Arguments = file + " " + pathExe,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    StandardOutputEncoding = isEncoderVB ? Encoding.Default : null
                }
            };
            compile.Start();

            string outString = compile.StandardOutput.ReadToEnd();
            string[] allMessage = outString.Split('\n');
            string[] errors = allMessage
                .Where(x => x.Contains("Warning") || x.Contains($"check_{checkId}.pas:") || x.Contains("error CS") ||
                x.Contains("error BC") || x.Contains("Error") || x.Contains("error"))
                .ToArray();

            bool isFileExist = File.Exists(pathExe); ;
            if (!isFileExist)
            {
                lastError = errors.Length == 0 ? outString : errors.Last();
            }

            return isFileExist;
        }

    }
}
