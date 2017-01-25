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
        static void Main(string[] args)
        {
            String s = "3", ss = "4";
            if (Regex.IsMatch(s, @"^[a-zA-Z0-9)]+$") && !Regex.IsMatch(ss, @"^[\+\-*/)]+$"))
            {
                Console.WriteLine("true");
            }
            while (true) ;
        }
        
    }
}
