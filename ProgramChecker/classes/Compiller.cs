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
        }

        public bool compile()
        {
            language = Language.getClass(check);

            return language.compile();
        }

        public string getError()
        {
            return language.getError();
        }

    }
}
