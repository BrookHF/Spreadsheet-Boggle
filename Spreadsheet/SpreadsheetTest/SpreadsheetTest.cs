using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using Formulas;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;

namespace SpreadsheetTest
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// test argument null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void NullArgument1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents(null);
        }

        /// <summary>
        /// test argument null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullArgument2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("aa", null);
        }
        /// <summary>
        /// test argument null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void NullArgument3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell(null, "1.0");
        }
        /// <summary>
        /// test argument null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void NullArgument4()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell(null, "dd");
        }
        /// <summary>
        /// test argument null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void NullArgument5()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell(null, "=1");
        }
        /// <summary>
        /// test name is not valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("A");
        }

        /// <summary>
        /// test name is not valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("AB4a");
        }
        /// <summary>
        /// test name is not valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("1");
        }
        /// <summary>
        /// test name is not valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName4()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("7d");
        }
        /// <summary>
        /// test name is not valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName5()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("AAA");
        }
        /// <summary>
        /// test name is not valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName6()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A", "1.0");
        }

        /// <summary>
        /// test name is not valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName7()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("AB4a", "d");
        }
        /// <summary>
        /// test name is not valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName8()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("1", "=2");
        }
        /// <summary>
        /// test get cell contents
        /// </summary>
        [TestMethod()]
        public void GetCellContent1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "1.0");
            Assert.AreEqual(1.0, s.GetCellContents("Z7"));
        }
        /// <summary>
        /// test get cell contents
        /// </summary>
        [TestMethod()]
        public void GetCellContent2()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "hello");
            Assert.AreEqual("hello", s.GetCellContents("Z7"));
        }
        /// <summary>
        /// test get cell contents
        /// </summary>
        [TestMethod()]
        public void GetCellContent3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "=1.0");
            Formula f = (Formula)s.GetCellContents("Z7");
            Assert.AreEqual(1.0, f.Evaluate(v => 1.0));
        }

        /// <summary>
        /// test get all null empty cell
        /// </summary>
        [TestMethod()]
        public void nullEmptyCell1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "");
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        /// <summary>
        /// test get all null empty cell
        /// </summary>
        [TestMethod()]
        public void nullEmptyCell2()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "A2");
            Assert.IsTrue(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }
        /// <summary>
        /// test get all null empty cell
        /// </summary>
        [TestMethod()]
        public void nullEmptyCell3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "A2");
            s.SetContentsOfCell("C3", "Hello");

            List<string> names = new List<string>(s.GetNamesOfAllNonemptyCells());
            Assert.AreEqual("B1", names[0]);
            Assert.AreEqual("C3", names[1]);
        }
        /// <summary>
        /// test get all null empty cell
        /// </summary>
        [TestMethod()]
        public void MoreContents1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=C1+C2");
            s.SetContentsOfCell("B1", "=D1 /D2");
            s.SetContentsOfCell("C2", "=B3*B4");
        }
        /// <summary>
        /// test get all null empty cell
        /// </summary>
        [TestMethod()]
        public void MoreContents2()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=C1+C2");
            s.SetContentsOfCell("B1", "=D1 /D2");
            s.SetContentsOfCell("C2", "=B3*B4");
            Assert.AreEqual(s.GetCellContents("R3"), "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestProtectedMethods1()
        {
            PrivateObject sampleAccessor = new PrivateObject(new Spreadsheet());
            object[] parameters = { null };
            sampleAccessor.Invoke("GetDirectDependents", parameters);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestProtectedMethods2()
        {
            PrivateObject sampleAccessor = new PrivateObject(new Spreadsheet());
            object[] parameters = { "AAA" };
            sampleAccessor.Invoke("GetDirectDependents", parameters);
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestProtectedMethods3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=C1+C2");
            s.SetContentsOfCell("B1", "=D1 /D2");
            s.SetContentsOfCell("C2", "=A1*B4");
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNewConstructor1()
        {
            AbstractSpreadsheet s = new Spreadsheet(new Regex("^[b-zB-Z]{1}[1-9]{1}[0-9]*$"));
            s.SetContentsOfCell("A1", "=C1+C2");
        }
        [TestMethod]
        public void TestNewConstructor2()
        {
            AbstractSpreadsheet s = new Spreadsheet(new Regex(".*"));
            s.SetContentsOfCell("A9", "n8");
            Assert.AreEqual("n8", s.GetCellValue("A9"));
        }
        [TestMethod]
        public void TestNewConstructor3()
        {
            AbstractSpreadsheet s = new Spreadsheet(new Regex(".*"));
            s.SetContentsOfCell("A1", "=B1");
            Assert.IsInstanceOfType(s.GetCellValue("A1"), typeof(FormulaError));
        }

        [TestMethod]
        public void TestNewConstructor4()
        {
            AbstractSpreadsheet s = new Spreadsheet(new Regex(".*"));
            s.SetContentsOfCell("a1", "=b1");
            s.SetContentsOfCell("b1", "=1+2");
            s.SetContentsOfCell("c2", "=R2");
            Assert.IsInstanceOfType(s.GetCellValue("C2"), typeof(FormulaError));
            Assert.AreEqual(s.GetCellValue("A1"), 3);
        }
        [TestMethod]
        public void TestNewConstructor5()
        {
            AbstractSpreadsheet s = new Spreadsheet(new Regex(".*"));
            s.SetContentsOfCell("a1", "=b1");
            s.SetContentsOfCell("b1", "=C2+2");
            s.SetContentsOfCell("c2", "hello");
            Assert.IsInstanceOfType(s.GetCellValue("a1"), typeof(FormulaError));
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNewConstructor6()
        {
            AbstractSpreadsheet s = new Spreadsheet(new Regex(".*"));
            s.SetContentsOfCell(null, "=b1");
            Assert.IsInstanceOfType(s.GetCellValue("a1"), typeof(FormulaError));
        }
        [TestMethod]
        public void TestNewConstructor7()
        {
            AbstractSpreadsheet s = new Spreadsheet(new Regex(".*"));
            Assert.AreEqual(s.GetCellValue("a1"), "");
        }
        [TestMethod]
        public void TestNewConstructor8()
        {
            AbstractSpreadsheet s = new Spreadsheet(new Regex(".*"));
            s.SetContentsOfCell("a1", "=b1");
            s.SetContentsOfCell("b1", "=C2+2");
            Assert.IsInstanceOfType(s.GetCellValue("a1"), typeof(FormulaError));
        }
        [TestMethod]
        public void TestNewConstructor9()
        {
            AbstractSpreadsheet s = new Spreadsheet(new Regex(".*"));
            s.SetContentsOfCell("a1", "=b1");
            s.SetContentsOfCell("b1", "=C2+2");
            s.SetContentsOfCell("c2", "=a3");
            Assert.IsInstanceOfType(s.GetCellValue("a1"), typeof(FormulaError));
        }

        [TestMethod]
        public void TestNewConstructor10()
        {
            AbstractSpreadsheet s = new Spreadsheet(new Regex(".*"));
            s.SetContentsOfCell("a1", "=b1");
            s.SetContentsOfCell("b1", "2");
            s.SetContentsOfCell("c2", "hello");
            StringWriter sw = new StringWriter();
            s.Save(sw);
            AbstractSpreadsheet ss = new Spreadsheet(new StringReader(sw.ToString()), new Regex(".*"));
            Assert.AreEqual(s.GetCellContents("A1"), s.GetCellContents("A1"));
            Assert.AreEqual(s.GetCellContents("B1"), s.GetCellContents("B1"));
            Assert.AreEqual(s.GetCellContents("C2"), s.GetCellContents("C2"));
        }
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void TestNewConstructor11()
        {
            StringWriter sw = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sw))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteStartElement("cell");
                writer.WriteAttributeString("name", "A1");
                writer.WriteAttributeString("contents", "hello");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            AbstractSpreadsheet ss = new Spreadsheet(new StringReader(sw.ToString()), new Regex(".*"));
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNewConstructor12()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("D6", "42");
            s.GetCellValue("sss3");
        }
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void TestNewConstructor13()
        {
            StringWriter sw = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sw))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("isValid", "[^[b-d]]");
                writer.WriteStartElement("cell");
                writer.WriteAttributeString("name", "A1");
                writer.WriteAttributeString("contents", "hello");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            AbstractSpreadsheet ss = new Spreadsheet(new StringReader(sw.ToString()), new Regex(".*"));
        }
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetVersionException))]
        public void TestNewConstructor14()
        {
            StringWriter sw = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sw))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("isValid", ".*");
                writer.WriteStartElement("cell");
                writer.WriteAttributeString("name", "A1");
                writer.WriteAttributeString("contents", "hello");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            AbstractSpreadsheet ss = new Spreadsheet(new StringReader(sw.ToString()), new Regex("[^[0-9]]"));
        }
        
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void TestNewConstructor15()
        {
            StringWriter sw = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sw))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("isValid", "[^]");
                writer.WriteStartElement("cell");
                writer.WriteAttributeString("name", "A1");
                writer.WriteAttributeString("contents", "hello");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            AbstractSpreadsheet ss = new Spreadsheet(new StringReader(sw.ToString()), new Regex(".*"));
        }
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void TestNewConstructor16()
        {
            StringWriter sw = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sw))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("isValid", "[^]");
                writer.WriteStartElement("cell");
                writer.WriteAttributeString("name", "A1");
                writer.WriteAttributeString("contents", "hello");
                writer.WriteEndElement();
                writer.WriteStartElement("cell");
                writer.WriteAttributeString("name", "A1");
                writer.WriteAttributeString("contents", "hello");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            AbstractSpreadsheet ss = new Spreadsheet(new StringReader(sw.ToString()), new Regex(".*"));
        }
    }

}
