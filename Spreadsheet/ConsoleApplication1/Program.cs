using Dependencies;
using SS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleApplication1
{


    class Program
    {

        static void Main(string[] args)
        {

            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A2", "2.35");
            //sheet.SetContentsOfCell("B4", "fang");
            //sheet.SetContentsOfCell("C1", "=1+1");

            sheet.Save(File.CreateText("abc.txt"));
            Console.Read();

        }
    }
}
