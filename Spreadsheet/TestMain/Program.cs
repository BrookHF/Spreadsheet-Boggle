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
           Formula f = new Formula("1+3d * 44");
            Console.WriteLine(f.Evaluate(v => 3.0));
            //Lookup look = Lookup4;
            //Console.WriteLine(look("x"));
            //Console.WriteLine(Regex.IsMatch("x", @"^[a-zA-Z0-9]+$"));
            //Console.WriteLine(1.0 / 0);
            //double i=3;
            //Console.WriteLine(i);
            //Console.WriteLine(Double.TryParse("e4", out i));
            //Console.WriteLine(i);
            

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
