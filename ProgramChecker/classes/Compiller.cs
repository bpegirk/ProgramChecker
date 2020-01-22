using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ProgramChecker.Languages;

namespace ProgramChecker.classes
{
    class Compiller
    {
        private Check check;
        public static Language language;

        public Compiller(Check check)
        {
            this.check = check;
            language = Language.getClass(check);
        }

        public bool compile()
        {
            return language.compile();
        }

        public bool checkException()
        {
            return  language.checkException();;
        }

        public string getError()
        {
            return language.getError();
        }
    }
}