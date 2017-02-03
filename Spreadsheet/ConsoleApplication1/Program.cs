using Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{


    class Program
    {

        static void Main(string[] args)
        {
            DependencyGraph test = new DependencyGraph();
            List<string> list = new List<string>();
            list.Add("1");
            list.Add("2");
            list.Add("3");
            IEnumerable<string> replace = list;
            test.AddDependency("b", "a");
            test.AddDependency("c", "a");
            test.AddDependency("d", "a");
            test.ReplaceDependees("a", replace);
            IEnumerable<string> enumerable = test.GetDependees("a");

            //string[] str = { "1", "2", "3" };
            //int index = 0;
            foreach (string s2 in enumerable)
            {
                Console.WriteLine(s2);
            }
            while (true) ;

        }
    }
}
