using Formulas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestMain
{
    class Program
    {

        public delegate double Lookup(string var);
        static void Main(string[] args)
        {
            Formula f = new Formula("x+y+z", s => s + s, s => true);
            var actual = f.GetVariables();
            foreach(string s in actual)
            {
                Console.WriteLine(s);
            }

            while (true) ;
        }
        public static double Lookup4(String v)
        {
            switch (v)
            {
                case "x": return 4.0;
                case "y": return 6.0;
                case "z": return 8.0;
                default: throw new UndefinedVariableException(v);
            }
        }
    }
}
