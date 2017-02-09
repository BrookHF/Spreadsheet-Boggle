// Written by Joe Zachary for CS 3500, January 2017.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;

namespace FormulaTestCases
{
    /// <summary>
    /// These test cases are in no sense comprehensive!  They are intended to show you how
    /// client code can make use of the Formula class, and to show you how to create your
    /// own (which we strongly recommend).  To run them, pull down the Test menu and do
    /// Run > All Tests.
    /// </summary>
    [TestClass]
    public class UnitTests
    {
        /// <summary>
        /// This tests that a syntactically incorrect parameter to Formula results
        /// in a FormulaFormatException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct1()
        {
            Formula f = new Formula("_");
        }

        /// <summary>
        /// This is another syntax error
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct2()
        {
            Formula f = new Formula("2++3");
        }

        /// <summary>
        /// Another syntax error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct3()
        {
            Formula f = new Formula("2 3");
        }

        /// <summary>
        /// This is another syntax error
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct4()
        {
            Formula f = new Formula("2 5 + 3");
        }

        /// <summary>
        /// This is another syntax error
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct5()
        {
            Formula f = new Formula("2.56 5 + 3");
        }

        /// <summary>
        /// This tests that a syntactically incorrect parameter to Formula results
        /// in a FormulaFormatException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct6()
        {
            Formula f = new Formula("1 _ + 3");
        }


        /// <summary>
        /// Makes sure that "2+3" evaluates to 5.  Since the Formula
        /// contains no variables, the delegate passed in as the
        /// parameter doesn't matter.  We are passing in one that
        /// maps all variables to zero.
        /// </summary>
        [TestMethod]
        public void Evaluate1()
        {
            Formula f = new Formula("2+3");
            Assert.AreEqual(f.Evaluate(v => 0), 5.0, 1e-6);
        }

        /// <summary>
        /// The Formula consists of a single variable (x5).  The value of
        /// the Formula depends on the value of x5, which is determined by
        /// the delegate passed to Evaluate.  Since this delegate maps all
        /// variables to 22.5, the return value should be 22.5.
        /// </summary>
        [TestMethod]
        public void Evaluate2()
        {
            Formula f = new Formula("x5");
            Assert.AreEqual(f.Evaluate(v => 22.5), 22.5, 1e-6);
        }

        /// <summary>
        /// Here, the delegate passed to Evaluate always throws a
        /// UndefinedVariableException (meaning that no variables have
        /// values).  The test case checks that the result of
        /// evaluating the Formula is a FormulaEvaluationException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate3()
        {
            Formula f = new Formula("x + y");
            f.Evaluate(v => { throw new UndefinedVariableException(v); });
        }

        /// <summary>
        /// The delegate passed to Evaluate is defined below.  We check
        /// that evaluating the formula returns in 10.
        /// </summary>
        [TestMethod]
        public void Evaluate4()
        {
            Formula f = new Formula("x + y");
            Assert.AreEqual(f.Evaluate(Lookup4), 10.0, 1e-6);
        }

        /// <summary>
        /// This uses one of each kind of token.
        /// </summary>
        [TestMethod]
        public void Evaluate5 ()
        {
            Formula f = new Formula("(x + y) * (z / x) * 1.0");
            Assert.AreEqual(f.Evaluate(Lookup4), 20.0, 1e-6);
        }

        /// <summary>
        /// Here, the delegate passed to Evaluate always throws a
        /// UndefinedVariableException (meaning that no variables have
        /// values).  The test case checks that the result of
        /// evaluating the Formula is a FormulaEvaluationException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate6()
        {
            Formula f = new Formula("x + y * y1 ");
            f.Evaluate(v => { throw new UndefinedVariableException(v); });
        }

        /// <summary>
        /// This uses one of each kind of token.
        /// </summary>
        [TestMethod]
        public void Evaluate7()
        {
            Formula f = new Formula("x + y * ((z / x) * 1.0)");
            Assert.AreEqual(f.Evaluate(Lookup4), 16.0, 1e-6);
        }

        /// <summary>
        /// This uses one of each kind of token.
        /// </summary>
        [TestMethod]
        public void Evaluate8()
        {
            Formula f = new Formula("2.2e3/100");
            Assert.AreEqual(f.Evaluate(Lookup4), 22.0, 1e-6);
        }

        /// <summary>
        /// This uses one of each kind of token.
        /// </summary>
        [TestMethod]
        public void Evaluate9()
        {
            Formula f = new Formula("x + i * ((z / i) * 1.0)");
            Assert.AreEqual(f.Evaluate(Lookup4), 12.0, 1e-6);
        }

        /// <summary>
        /// This uses one of each kind of token.
        /// </summary>
        [TestMethod]
        public void Evaluate10()
        {
            Formula f = new Formula("e23 + 12");
            Assert.AreEqual(f.Evaluate(v=>8.0), 20.0, 1e-6);
        }

        /// <summary>
        /// This uses one of each kind of token.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Evaluate11()
        {
            Formula f = new Formula("3er3 + 12");
            Assert.AreEqual(f.Evaluate(v => 8.0), 20.0, 1e-6);
        }

        /// <summary>
        /// This uses one of each kind of token.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate12()
        {
            Formula f = new Formula("e23/0");
            Assert.AreEqual(f.Evaluate(v => 8.0), 20.0, 1e-6);
        }

        /// <summary>
        /// This uses one of each kind of token.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate13()
        {
            Formula f = new Formula("3/e23");
            f.Evaluate(v => 0.0);
        }

        /// <summary>
        /// This uses one of each kind of token.
        /// </summary>
        [TestMethod]
        public void Evaluate14()
        {
            Formula f = new Formula("3/(e23+4)");
            f.Evaluate(v => 3.0);
        }

        /// <summary>
        /// test the Normalizer  
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void newConstructor1()
        {
            Formula f = new Formula("1+1", s => "2d", s=> true);
        }

        /// <summary>
        /// test the Validator  
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void newConstructor2()
        {
            Formula f = new Formula("1+1", s => s, s => (s.Length>10));
        }

        /// <summary>
        /// test the Normalizer 
        /// </summary>
        [TestMethod]
        public void newConstructor3()
        {
            Formula f = new Formula("1+1", s => "2e3", s => true);
        }

        /// <summary>
        /// test the Validator  
        /// </summary>
        [TestMethod]
        public void newConstructor4()
        {
            Formula f = new Formula("1+1", s => s, s => (s.Contains("+")));
        }

        /// <summary>
        /// test the to string  
        /// </summary>
        [TestMethod]
        public void newConstructor5()
        {
            Formula f = new Formula("1+1", s => s, s => (s.Contains("+")));
            Assert.AreEqual("1+1", f.ToString());
        }

        /// <summary>
        /// test Normalizer  
        /// </summary>
        [TestMethod]
        public void newConstructor6()
        {
            Formula f = new Formula("1+1", s => "40*23", s => true);
            Assert.AreEqual("40*23", f.ToString());
        }

        /// <summary>
        /// test the ArgumentNullException  
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void newConstructor7()
        {
            Formula f = new Formula("1+1", null, s => (s.Length > 10));
        }

        /// <summary>
        /// test the ArgumentNullException  
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void newConstructor8()
        {
            Formula f = new Formula("1+1", null, null);
        }

        /// <summary>
        /// test the ArgumentNullException  
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void newConstructor9()
        {
            Formula f = new Formula("1+1", s => s, null);
        }

        /// <summary>
        /// test the ArgumentNullException  
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void newConstructor10()
        {
            Formula f = new Formula(null, s => s, s => true);
        }

        /// <summary>
        /// test the ArgumentNullException  
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void newConstructor11()
        {
            Formula f = new Formula(null);
        }

        /// <summary>
        /// test the ArgumentNullException  
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void newConstructor12()
        {
            Formula f = new Formula("1+1");
            f.Evaluate(null);
        }

        /// <summary>
        /// A Lookup method that maps x to 4.0, y to 6.0, and z to 8.0.
        /// All other variables result in an UndefinedVariableException.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Lookup4(String v)
        {
            switch (v)
            {
                case "x": return 4.0;
                case "y": return 6.0;
                case "z": return 8.0;
                case "i": return 3e2;
                default: throw new UndefinedVariableException(v);
            }
        }
    }
}
