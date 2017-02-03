using Dependencies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DependencyGraphTestCases
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// test constructing graph
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            DependencyGraph test = new DependencyGraph();
        }

        /// <summary>
        /// test adding dependency pair
        /// </summary>
        [TestMethod]
        public void TestMethod2()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("a", "b");
        }

        /// <summary>
        /// test get size
        /// </summary>
        [TestMethod]
        public void TestMethod3()
        {
            DependencyGraph test = new DependencyGraph();
            Assert.AreEqual(test.Size, 0);
        }

        /// <summary>
        /// test get size
        /// </summary>
        [TestMethod]
        public void TestMethod4()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("a", "b");
            Assert.AreEqual(test.Size,1);
        }

        /// <summary>
        /// test remove dependency
        /// </summary>
        [TestMethod]
        public void TestMethod5()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("a", "b");
            test.RemoveDependency("a", "b");
            Assert.AreEqual(test.Size, 0);
        }

        /// <summary>
        /// test remove dependency not exist
        /// </summary>
        [TestMethod]
        public void TestMethod6()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("a", "b");
            test.RemoveDependency("a", "c");
            Assert.AreEqual(test.Size, 1);
        }

        /// <summary>
        /// test remove dependency not exist
        /// </summary>
        [TestMethod]
        public void TestMethod7()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("a", "b");
            test.RemoveDependency("a", "c");
            Assert.AreEqual(test.Size, 1);
        }

        /// <summary>
        /// test has dependees
        /// </summary>
        [TestMethod]
        public void TestMethod8()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("a", "b");
            Assert.IsTrue(test.HasDependees("b"));
        }

        /// <summary>
        /// test has dependees false
        /// </summary>
        [TestMethod]
        public void TestMethod9()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("a", "b");
            Assert.IsFalse(test.HasDependees("a"));
        }

        /// <summary>
        /// test has dependents
        /// </summary>
        [TestMethod]
        public void TestMethod10()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("a", "b");
            Assert.IsTrue(test.HasDependents("a"));
        }

        /// <summary>
        /// test has dependents false
        /// </summary>
        [TestMethod]
        public void TestMethod11()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("a", "b");
            Assert.IsFalse(test.HasDependents("b"));
        }

        /// <summary>
        /// test has dependees not exist
        /// </summary>
        [TestMethod]
        public void TestMethod12()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("a", "b");
            Assert.IsFalse(test.HasDependees("c"));
        }

        /// <summary>
        /// test has dependents not exist
        /// </summary>
        [TestMethod]
        public void TestMethod13()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("a", "b");
            Assert.IsFalse(test.HasDependents("c"));
        }

        /// <summary>
        /// test remove dependents
        /// </summary>
        [TestMethod]
        public void TestMethod14()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("a", "b");
            test.RemoveDependency("a", "b");
            Assert.IsFalse(test.HasDependents("a"));
        }

        /// <summary>
        /// test remove dependents
        /// </summary>
        [TestMethod]
        public void TestMethod15()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("a", "b");
            test.RemoveDependency("a", "b");
            Assert.IsFalse(test.HasDependees("b"));
        }

        /// <summary>
        /// test get dependees
        /// </summary>
        [TestMethod]
        public void TestMethod16()
        {
            DependencyGraph test = new DependencyGraph();
            string[] str = { "b", "c", "d" };
            int index = 0;
            test.AddDependency("a", "b");
            test.AddDependency("a", "c");
            test.AddDependency("a", "d");
            IEnumerable<string> enumerable = test.GetDependees("a");
            foreach(string s2 in enumerable)
            {
                Assert.Equals(s2, str[index++]);
            }
        }

        /// <summary>
        /// test get dependees
        /// </summary>
        [TestMethod]
        public void TestMethod17()
        {
            DependencyGraph test = new DependencyGraph();
            string[] str = { "b", "c", "d" };
            int index = 0;
            test.AddDependency("b", "a");
            test.AddDependency("c", "a");
            test.AddDependency("d", "a");
            IEnumerable<string> enumerable = test.GetDependents("a");
            foreach (string s2 in enumerable)
            {
                Assert.Equals(s2, str[index++]);
            }
        }

        /// <summary>
        /// test get dependees with remove
        /// </summary>
        [TestMethod]
        public void TestMethod18()
        {
            DependencyGraph test = new DependencyGraph();
            string[] str = { "b", "d" };
            int index = 0;
            test.AddDependency("b", "a");
            test.AddDependency("c", "a");
            test.AddDependency("d", "a");
            test.RemoveDependency("c", "a");
            IEnumerable<string> enumerable = test.GetDependees("a");
            foreach (string s2 in enumerable)
            {
                Assert.AreEqual(s2, str[index++]);
            }
            Assert.AreEqual(2, test.Size);
        }

        /// <summary>
        /// test replace dependees
        /// </summary>
        [TestMethod]
        public void TestMethod19()
        {
            DependencyGraph test = new DependencyGraph();
            List<string> list = new List<string>();
            list.Add("1");
            list.Add("2");
            list.Add("3");
            list.Add("4");
            IEnumerable < string > replace = list;
            test.AddDependency("b", "a");
            test.AddDependency("c", "a");
            test.AddDependency("d", "a");
            test.ReplaceDependees("a", replace);
            IEnumerable<string> enumerable = test.GetDependees("a");

            string[] str = { "1", "2", "3" ,"4"};
            int index = 0;
            foreach (string s2 in enumerable)
            {
                Assert.AreEqual(s2, str[index++]);
            }
            Assert.AreEqual(4, test.Size);
        }

        /// <summary>
        /// test replace dependents
        /// </summary>
        [TestMethod]
        public void TestMethod20()
        {
            DependencyGraph test = new DependencyGraph();
            List<string> list = new List<string>();
            list.Add("1");
            list.Add("2");
            list.Add("3");
            list.Add("4");
            IEnumerable<string> replace = list;
            test.AddDependency("b", "a");
            test.AddDependency("c", "a");
            test.AddDependency("d", "a");
            test.ReplaceDependents("b", replace);
            IEnumerable<string> enumerable = test.GetDependents("b");

            string[] str = { "1", "2", "3", "4" };
            int index = 0;
            foreach (string s2 in enumerable)
            {
                Assert.AreEqual(s2, str[index++]);
            }
            Assert.AreEqual(6, test.Size);
        }

        /// <summary>
        /// test replace dependents element not exist
        /// </summary>
        [TestMethod]
        public void TestMethod21()
        {
            DependencyGraph test = new DependencyGraph();
            List<string> list = new List<string>();
            list.Add("1");
            list.Add("2");
            list.Add("3");
            list.Add("4");
            IEnumerable<string> replace = list;
            test.AddDependency("b", "a");
            test.AddDependency("c", "a");
            test.AddDependency("d", "a");
            test.ReplaceDependents("y", replace);
            IEnumerable<string> enumerable = test.GetDependents("y");

            string[] str = { "1", "2", "3", "4" };
            int index = 0;
            foreach (string s2 in enumerable)
            {
                Assert.AreEqual(s2, str[index++]);
            }
            Assert.AreEqual(7, test.Size);
        }

        /// <summary>
        /// test replace dependees element not exist
        /// </summary>
        [TestMethod]
        public void TestMethod22()
        {
            DependencyGraph test = new DependencyGraph();
            List<string> list = new List<string>();
            list.Add("1");
            list.Add("2");
            list.Add("3");
            list.Add("4");
            IEnumerable<string> replace = list;
            test.AddDependency("b", "a");
            test.AddDependency("c", "a");
            test.AddDependency("d", "a");
            test.ReplaceDependees("y", replace);
            IEnumerable<string> enumerable = test.GetDependees("y");

            string[] str = { "1", "2", "3", "4" };
            int index = 0;
            foreach (string s2 in enumerable)
            {
                Assert.AreEqual(s2, str[index++]);
            }
            Assert.AreEqual(7, test.Size);
        }

        /// <summary>
        /// test has dependee
        /// </summary>
        [TestMethod]
        public void TestMethod23()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("b", "a");
            Assert.AreEqual(1, test.Size);
            Assert.IsTrue(test.HasDependees("a"));
        }

        /// <summary>
        /// test has dependent
        /// </summary>
        [TestMethod]
        public void TestMethod24()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("b", "c");
            Assert.AreEqual(1, test.Size);
            Assert.IsTrue(test.HasDependents("b"));
        }

        /// <summary>
        /// test add same dependency
        /// </summary>
        [TestMethod]
        public void TestMethod25()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("a", "b");
            test.AddDependency("a", "b");
            test.AddDependency("a", "b");
            Assert.AreEqual(1, test.Size);
            Assert.IsTrue(test.HasDependees("b"));
        }
    }
}
