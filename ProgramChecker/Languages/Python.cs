using System.Diagnostics;
using System.IO;
using ProgramChecker.classes;

namespace ProgramChecker.Languages
{
    class Python : Language
    {
        private static string nameScript = "python.cmd";

        public Python(Check check) : base(check, "py")
        {
        }

        public override bool compile()
        {
            Directory.CreateDirectory(pathExe);
            string testFile = $"{pathExe}check_{check.checkId}.py";
            if (!File.Exists(testFile))
                File.Copy(pathFile, testFile);
            File.Create($"{pathExe}input.txt").Close();
            File.Create($"{pathExe}output.txt").Close();

            return afterCompile();
        }

        public override string prepareTesting(Test test)
        {
            int checkId = check.checkId;

            string pathSrc = Program.globalConfig["paths"]["src"] + $@"check_{checkId}\";
            string testSrc = pathSrc + $@"test_{test.id}\";
            File.Create($"{testSrc}output.txt").Close();
            return base.prepareTesting(test);
        }

        public override Process createTestProcess(string testExe, string testSrc)
        {
            var compile = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pathScript + "python.cmd",
                    Arguments = testExe,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WorkingDirectory = testSrc
                }
            };

            return compile;
        }
    }
}