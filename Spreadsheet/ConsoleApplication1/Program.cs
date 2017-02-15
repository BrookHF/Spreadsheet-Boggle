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
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 10;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 2; j < SIZE; j += 3)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Replace a bunch of dependees
            for (int i = 0; i < SIZE; i += 2)
            {
                HashSet<string> newDees = new HashSet<String>();
                for (int j = 0; j < SIZE; j += 9)
                {
                    newDees.Add(letters[j]);
                }
                Console.Write(i+" ");
                foreach(string s in newDees)
                {
                    Console.Write(s + " ");
                }
                Console.WriteLine();

                t.ReplaceDependees(letters[i], newDees);

                Console.Write(i + " ");
                foreach (string s in t.GetDependees(letters[i]))
                {
                    Console.Write(s + " ");
                }
                Console.WriteLine();

                foreach (string s in dees[i])
                {
                    dents[s[0] - 'a'].Remove(letters[i]);
                }

                foreach (string s in newDees)
                {
                    dents[s[0] - 'a'].Add(letters[i]);
                }

                dees[i] = newDees;
            }

            //// Make sure everything is right
            //for (int i = 0; i < SIZE; i++)
            //{
            //    Console.WriteLine("dees: " + i + dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            //    foreach (string str in t.GetDependees(letters[i]))
            //    {
            //        Console.Write(str + " ");
            //    }
            //    Console.WriteLine("");
            //    foreach (string str in dees[i])
            //    {
            //        Console.Write(str + " ");
            //    }
            //    Console.WriteLine("");

            //    Console.WriteLine("dents: " + dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
            //    foreach (string str in t.GetDependents(letters[i]))
            //    {
            //        Console.Write(str + " ");
            //    }
            //    Console.WriteLine("");
            //    foreach (string str in dents[i])
            //    {
            //        Console.Write(str + " ");
            //    }
            //    Console.WriteLine("");
            //}
            while (true) ;

        }
    }
}
