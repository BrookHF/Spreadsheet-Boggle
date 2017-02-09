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
            Formula f = new Formula("(7e-5+b-c)", s => s.ToUpper(), s => true);
            //"(((((2+3*a)/(7e-5+b-c))*d+.0005e+92)-8.2)*3.14159) * ((e+3.1)-.00000000008)"
            //Console.WriteLine((double)f.Evaluate(s => (s == "x") ? 1 : 4));


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
