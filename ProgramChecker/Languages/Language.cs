using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgramChecker.classes;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ProgramChecker.Languages
{
    public abstract class Language
    {
        protected string pathScript = Program.globalConfig["paths"]["scripts"];
        protected bool isForceKill;

        public static Dictionary<int, string> languages = new Dictionary<int, string>
        {
            { Check.PASCAL, "Pascal" },
            { Check.DELPHI, "Delphi" },
            { Check.C_BUILDER, "Cpp" },
            { Check.C_SHARP_2013, "CSharp" },
            { Check.VB_2013, "VB" },
            { Check.C_PLUS_2013, "VCpp" },
            { Check.JAVA, "Java" },
            { Check.PYTHON, "Python" },
            { Check.GO, "Go" },
            { Check.NODE_JS, "NodeJs" }
        };

        protected readonly Check check;
        protected string lastError;
        protected string pathCompile;
        private readonly string pathCheck;
        protected string nameScript;
        protected readonly string pathFile;
        protected readonly string checkFile;
        protected Process compileProcess;
        protected string outString;
        protected List<string> errors = new List<string>();
        private readonly string extension;

        protected Language(Check check, string extension = "exe")
        {
            this.check = check;
            pathCompile = Program.globalConfig["paths"]["src"] + "check_" + check.checkId + @"\";
            pathCheck = Program.globalConfig["paths"]["src"] + "check_" + check.checkId + @"\";
            pathFile = Program.globalConfig["paths"]["src"] + check.fileName;
            checkFile = pathCompile + "\\check_" + check.checkId + "." + extension;
            this.extension = extension;
        }

        public abstract bool compile();

        public virtual bool checkException()
        {
            string str = string.Empty;
            using (StreamReader reader = File.OpenText(pathFile))
            {
                str = reader.ReadToEnd();
            }

            string pattern = @"(\bthrow\b)";
            Regex rgx = new Regex(pattern);
            string find = rgx.Match(str).ToString();

            return !find.Equals("");
        }

        public virtual string prepareTesting(Test test)
        {
            int checkId = check.checkId;

            string pathSrc = Program.globalConfig["paths"]["src"] + $@"check_{checkId}\";
            string testSrc = pathSrc + $@"test_{test.id}\";
            string testExe = testSrc + $"check_{checkId}.{extension}";

            if (File.Exists(pathSrc + $"check_{checkId}.{extension}"))
            {
                if (!File.Exists(testExe))
                    File.Copy(pathSrc + $"check_{checkId}.{extension}", testExe);
            }

            return testExe;
        }

        public virtual Process createTestProcess(string testFile, string testSrc)
        {
            var compile = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = testFile,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    StandardErrorEncoding = Encoding.UTF8,
                    StandardOutputEncoding = Encoding.UTF8,
                    WorkingDirectory = testSrc
                }
            };

            return compile;
        }

        protected bool runScriptCompile(string nameScript)
        {
            this.nameScript = nameScript;

            Directory.CreateDirectory(pathCheck);

            compileProcess = createProcess();
            compileProcess.Start();

            do
            {
                if (compileProcess.HasExited) break;
                if (!compileProcess.WaitForExit(10000))
                {
                    isForceKill = true;
                    if (!compileProcess.HasExited)
                    {
                        compileProcess.Kill();
                    }
                }
            } while (!compileProcess.WaitForExit(10000));

            checkError();

            return afterCompile();
        }

        protected virtual Process createProcess()
        {
            var compile = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pathScript + nameScript,
                    Arguments = pathFile + " " + pathCompile,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    StandardOutputEncoding = Encoding.UTF8,
                }
            };

            return compile;
        }

        protected virtual void checkError()
        {
            if (isForceKill)
            {
                errors.Add("Timeout");
            }
            else
            {
                string outString = compileProcess.StandardOutput.ReadToEnd();
                errors = outString.Split('\n').ToList();
            }
        }

        protected virtual bool afterCompile()
        {
            bool isFileExist = File.Exists(checkFile);

            if (!isFileExist)
            {
                lastError = errors.Count == 0 ? outString : errors.Last();
            }

            return isFileExist;
        }

        public string getError()
        {
            return lastError;
        }

        public void setErrors(string errors)
        {
            lastError = errors;
        }

        public static Language getClass(Check check)
        {
            Type TestType = Type.GetType($"ProgramChecker.Languages.{languages[check.language]}", false, true);
            Language language = null;

            if (TestType != null)
            {
                System.Reflection.ConstructorInfo ci = TestType.GetConstructor(new Type[] { typeof(Check) });

                language = (Language)ci.Invoke(new object[] { check });
            }

            return language;
        }
    }
}