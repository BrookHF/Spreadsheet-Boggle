using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;
using System.Collections.Generic;
using System;

namespace PS4GradingTests
{
    [TestClass]
    public class GradingTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Null1()
        {
            DependencyGraph d = new DependencyGraph();
            d.AddDependency("a", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Null2()
        {
            DependencyGraph d = new DependencyGraph();
            d.HasDependees(null);
        }

        [TestMethod]
        public void Copy1()
        {
            var d1 = new DependencyGraph();
            var d2 = new DependencyGraph(d1);
            Assert.AreEqual(0, d1.Size);
            Assert.AreEqual(0, d2.Size);
        }

        [TestMethod]
        public void Copy2()
        {
            var d1 = new DependencyGraph();
            var d2 = new DependencyGraph(d1);
            d1.AddDependency("a", "b");
            Assert.AreEqual(1, d1.Size);
            Assert.AreEqual(0, d2.Size);
        }

        [TestMethod]
        public void Copy3()
        {
            var d1 = new DependencyGraph();
            var d2 = new DependencyGraph(d1);
            d1.AddDependency("a", "b");
            d2.AddDependency("c", "d");
            Assert.IsTrue(d1.HasDependents("a"));
            Assert.IsFalse(d1.HasDependents("c"));
            Assert.IsFalse(d2.HasDependents("a"));
            Assert.IsTrue(d2.HasDependents("c"));
        }

        [TestMethod]
        public void Copy4()
        {
            var d1 = new DependencyGraph();
            d1.AddDependency("a", "b");
            var d2 = new DependencyGraph(d1);
            Assert.IsTrue(d1.HasDependees("b"));
            Assert.IsTrue(d2.HasDependees("b"));
        }

        [TestMethod]
        public void Copy5()
        {
            var d1 = new DependencyGraph();
            d1.AddDependency("a", "b");
            d1.AddDependency("d", "e");
            var d2 = new DependencyGraph(d1);
            d1.AddDependency("a", "c");
            d2.AddDependency("d", "f");
            Assert.AreEqual(2, new List<string>(d1.GetDependents("a")).Count);
            Assert.AreEqual(1, new List<string>(d1.GetDependents("d")).Count);
            Assert.AreEqual(2, new List<string>(d2.GetDependents("d")).Count);
            Assert.AreEqual(1, new List<string>(d2.GetDependents("a")).Count);
        }

        [TestMethod]
        public void Copy6()
        {
            var d1 = new DependencyGraph();
            d1.AddDependency("b", "a");
            d1.AddDependency("e", "d");
            var d2 = new DependencyGraph(d1);
            d1.AddDependency("c", "a");
            d2.AddDependency("f", "d");
            Assert.AreEqual(2, new List<string>(d1.GetDependees("a")).Count);
            Assert.AreEqual(1, new List<string>(d1.GetDependees("d")).Count);
            Assert.AreEqual(2, new List<string>(d2.GetDependees("d")).Count);
            Assert.AreEqual(1, new List<string>(d2.GetDependees("a")).Count);
        }

        [TestMethod]
        public void Copy7()
        {
            var d1 = new DependencyGraph();
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    d1.AddDependency(i.ToString(), j.ToString());
                }
            }
            var d2 = new DependencyGraph(d1);

            for (int i = 0; i < 100; i++)
            {
                d1.RemoveDependency(i.ToString(), i.ToString());
                d2.AddDependency(i.ToString(), "x");
            }

            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(i, new List<string>(d1.GetDependents(i.ToString())).Count);
                Assert.AreEqual(i + 2, new List<string>(d2.GetDependents(i.ToString())).Count);
            }

            for (int j = 0; j <= 50; j++)
            {
                Assert.AreEqual(99 - j, new List<string>(d1.GetDependees(j.ToString())).Count);
                Assert.AreEqual(100 - j, new List<string>(d2.GetDependees(j.ToString())).Count);
            }

            Assert.AreEqual(100, new List<string>(d2.GetDependees("x")).Count);

            Assert.AreEqual(5050 - 100, d1.Size);
            Assert.AreEqual(5050 + 100, d2.Size);
        }

        [TestMethod]
        public void Copy8()
        {
            Copy7();
        }
    }
}
