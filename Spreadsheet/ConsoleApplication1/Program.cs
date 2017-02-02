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
            //string[] str = { "b", "d" };
            //int index = 0;
            test.AddDependency("b", "a");
            test.AddDependency("c", "a");
            test.AddDependency("d", "a");

            //Console.WriteLine(str[0]);
            //Console.WriteLine(str[1]);
            Console.WriteLine(test.Size);

            //Console.WriteLine(hashMap.TryGetValue("c", out list1));
            while (true) ;


        }
    }
}
