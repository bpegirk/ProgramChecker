using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgramChecker.classes
{
    class Check
    {
        public const int PASCAL = 1;
        public const int C_SHARP_2008 = 2;
        public const int VB_2008 = 3;
        public const int C_PLUS_2008 = 4;
        public const int DELPHI = 5;
        public const int C_BUILDER = 6;
        public const int C_SHARP_2013 = 7;
        public const int VB_2013 = 8;
        public const int C_PLUS_2013 = 9;


        public int checkId { get; set; }
        public int language { get; set; }
        public string fileName { get; set; }
        public int parseDec { get; set; }
        public bool isChecked { get; set; }
        public int passCount { get; set; }
        public List<Test> tests { get; set; }
    }

    class Test
    {
        public int id { get; set; }
        public int exerciseId { get; set; }
        public string input { get; set; }
        public string output { get; set; }
        public float balls { get; set; }
        public bool isPassed { get; set; }
        public DateTime checkTime { get; set; }
        public string compileError { get; set; }
        public string result { get; set; }

    }
}
