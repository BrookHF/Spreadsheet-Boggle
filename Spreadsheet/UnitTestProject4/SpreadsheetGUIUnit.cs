using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetGUI;
using System.Windows.Forms;

namespace SpreadsheetGUITest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void PublicFunctionTest1()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            stub.PrintMessage("hello");
            Assert.IsTrue(stub.CalledPrintMessage);
        }
        [TestMethod]
        public void PublicFunctionTest2()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            stub.OpenNew();
            Assert.IsTrue(stub.CalledOpenNew);
        }
        [TestMethod]
        public void PublicFunctionTest3()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            stub.DoClose();
            Assert.IsTrue(stub.Closed);
        }
        [TestMethod]
        public void PublicFunctionTest4()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            string selectedCellName = stub.GetSelectedCellName();
            Assert.AreEqual(selectedCellName, stub.SelectedCell);
        }
        [TestMethod]
        public void PublicFunctionTest5()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            string saveDialog = stub.SaveDialog();
            Assert.AreEqual(saveDialog, "newFile.ss");
        }
        [TestMethod]
        public void PublicFunctionTest6()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            stub.SetCellNameDisplay("abc");
            Assert.AreEqual(stub.CellNameDisplay, "abc");
        }
        [TestMethod]
        public void PublicFunctionTest7()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            stub.SetCellValueDisplay("abc");
            Assert.AreEqual(stub.CellValueDisplay, "abc");
        }
        [TestMethod]
        public void PublicFunctionTest8()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            stub.SetCellContentsDisplay("abc");
            Assert.AreEqual(stub.CellContentsDisplay, "abc");
        }
        [TestMethod]
        public void PublicFunctionTest9()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            stub.SetValueOfPanel(1, 1, "hello");
            Assert.AreEqual(stub.PanelGrid[1,1], "hello");
        }
        [TestMethod]
        public void PublicFunctionTest10()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            stub.OpenSaved("newFile.ss");
            Assert.IsTrue(stub.CalledOpenSaved);
        }
        [TestMethod]
        public void PublicFunctionTest11()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            stub.UnsavedData(new FormClosingEventArgs(new CloseReason(), true));
            Assert.IsTrue(stub.CalledUnsavedData);
        }
    }
}
