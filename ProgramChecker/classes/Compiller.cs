using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgramChecker.classes
{
    class Compiller
    {

        private int lang;
        private string file;
        private string lastError;

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
            return true;
        }

        private bool comlpileCSharp(int ver)
        {

            return true;
        }

        private bool comlpileVB(int ver)
        {

            return true;
        }

        private bool comlpileCPlus(int ver)
        {

            return true;
        }

        private bool comlpileCBuilder()
        {

            return true;
        }
        private bool comlpileDelphi()
        {

            return true;
        }

    }
}
