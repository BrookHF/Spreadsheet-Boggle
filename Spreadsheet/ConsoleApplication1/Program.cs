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
            Dictionary<string, List<string>> hashMap = new Dictionary<string, List<string>>();
            List<string> tempList = new List<string>();
            List<string> list1 = new List<string>();
            tempList.Add("c");
            hashMap.Add("d",tempList);

            
            Console.WriteLine(hashMap.TryGetValue("c", out list1));
            while (true) ;


        }


        class Dependency
        {
            public string _name { get; set; }
            public List<string> _depend { get; set; }

            public Dependency(string name, List<string> depend)
            {
                _name = name;
                _depend = depend;
            }
        }
    }

    
}
