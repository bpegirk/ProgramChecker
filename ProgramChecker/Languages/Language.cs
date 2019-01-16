using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgramChecker.classes;
using System.IO;
using System.Diagnostics;

namespace ProgramChecker.Languages
{
    public abstract class Language
    {
        public static Dictionary<int, string> languages = new Dictionary<int, string>
        {
            {Check.PASCAL, "Pascal"},
            {Check.DELPHI, "Delphi"},
            {Check.C_BUILDER, "Cpp"},
            {Check.C_SHARP_2013, "CSharp"},
            {Check.VB_2013, "VB"},
            {Check.C_PLUS_2013, "VCpp"},
            {Check.JAVA, "Java"},
            {Check.PYTHON, "Python"}
        };

        protected Check check;
        protected string lastError;
        protected string pathScript = Program.globalConfig["paths"]["scripts"];
        protected string pathExe;
        protected string nameScript;
        protected string pathFile;
        protected string checkFile;
        protected Process compileProcess;
        protected string outString;
        protected string[] errors;
        protected string checkExtension;
        public Language(Check check, string checkExtension = "exe")
        {
            this.check = check;
            pathExe = Program.globalConfig["paths"]["src"] + "check_" + check.checkId + @"\";
            pathFile = Program.globalConfig["paths"]["src"] + check.fileName;
            checkFile = pathExe + "\\check_" + check.checkId + "." + checkExtension;
            this.checkExtension = checkExtension;
        }

        public abstract bool compile();

        public virtual string prepareTesting(Test test)
        {
            int checkId = check.checkId;

            string pathSrc = Program.globalConfig["paths"]["src"] + $@"check_{checkId}\";
            string testSrc = pathSrc + $@"test_{test.id}\";
            string testExe = testSrc + $"check_{checkId}.{checkExtension}";

            if (File.Exists(pathSrc + $"check_{checkId}.{checkExtension}"))
            {
                File.Copy(pathSrc + $"check_{checkId}.{checkExtension}", testExe);
            }

            return testExe;
        }

        public virtual Process createTestProcess(string testExe, string testSrc)
        {
            var compile = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = testExe,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WorkingDirectory = testSrc
                }
            };

            return compile;
        }

        protected bool runScriptCompile(string nameScript, bool isExe = false, bool isEncoderVB = false)
        {
            this.nameScript = nameScript;
            int checkId = check.checkId;

            Directory.CreateDirectory(pathExe);

            if (isExe)
            {
                pathExe = pathExe + @"\check_" + checkId;
            }

            compileProcess = createProcess();
            compileProcess.Start();

            string[] errors = checkError();

            return afterCompile();
        }
        
        public virtual Process createProcess()
        {
            var compile = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pathScript + nameScript,
                    Arguments = pathFile + " " + pathExe,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    StandardOutputEncoding = null,
                }
            };

            return compile;
        }
        
        public virtual string[] checkError()
        {
            string outString = compileProcess.StandardOutput.ReadToEnd();
            string[] allMessage = outString.Split('\n');
            errors = allMessage
                .Where(x => x.Contains("Warning") || x.Contains($"check_{check.checkId}.pas:") ||
                            x.Contains("error CS") ||
                            x.Contains("error BC") || x.Contains("Error") || x.Contains("error"))
                .ToArray();

            return errors;
        }
        
        public virtual bool afterCompile()
        {
            bool isFileExist = File.Exists(checkFile);
            ;
            if (!isFileExist)
            {
                lastError = errors.Length == 0 ? outString : errors.Last();
            }

            return isFileExist;
        }

        public string getError()
        {
            return lastError;
        }

        public static Language getClass(Check check)
        {
            Type TestType = Type.GetType($"ProgramChecker.Languages.{languages[check.language]}", false, true);
            Language language = null;
           
            if (TestType != null)
            { 
                System.Reflection.ConstructorInfo ci = TestType.GetConstructor(new Type[] {typeof(Check)});

                language = (Language) ci.Invoke(new object[] {check});
            }

            return language;
        }
    }
}