using Dependencies;
using SS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleApplication1
{


    class Program
    {

        static void Main(string[] args)
        {

            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("su1", "= a1 + a2");
            int i;
            int depth = 100;
            for (i = 1; i <= depth * 2; i += 2)
            {
                s.SetContentsOfCell("a" + i, "= a" + (i + 2) + " + a" + (i + 3));
                s.SetContentsOfCell("a" + (i + 1), "= a" + (i + 2) + "+ a" + (i + 3));
            }
            s.SetContentsOfCell("a" + i, "1");
            s.SetContentsOfCell("a" + (i + 1), "1");
            Console.WriteLine((double)s.GetCellValue("su1"));
            s.SetContentsOfCell("a" + i, "0");
            Console.WriteLine((double)s.GetCellValue("su1"));
            s.SetContentsOfCell("a" + (i + 1), "0");
            Console.WriteLine((double)s.GetCellValue("su1"));

            Console.Read();

        }
    }
}
