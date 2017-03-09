using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetGUI;
using System.Windows.Forms;

namespace SpreadsheetGUITest
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Tests to see if the stub is closed if the FileCloseEvent is fired.
        /// </summary>
        [TestMethod]
        public void TestCloseEvent()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            SpreadsheetGUI.Controller controller = new SpreadsheetGUI.Controller(stub);

            stub.FireFileCloseEvent();
            Assert.IsTrue(stub.Closed);
        }

        /// <summary>
        /// Tests to see if OpenNew is called if the FileNewEvent is fired.
        /// </summary>
        [TestMethod]
        public void TestFileNewEvent()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            SpreadsheetGUI.Controller controller = new SpreadsheetGUI.Controller(stub);

            stub.FireFileNewEvent();
            Assert.IsTrue(stub.CalledOpenNew);
        }

        /// <summary>
        /// Tests to see if OpenSaved is called when FileOpenEvent is fired.
        /// </summary>
        [TestMethod]
        public void TestFileOpenEvent()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            SpreadsheetGUI.Controller controller = new SpreadsheetGUI.Controller(stub);
            string fileName = "hello.ss";
            
            stub.FireFileOpenEvent(fileName);
            Assert.IsTrue(stub.CalledOpenSaved);
        }

        /// <summary>
        /// Tests to see if file is saved when FileSaveEvent is fired.
        /// </summary>
        [TestMethod]
        public void TestFileSaveEvent()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            SpreadsheetGUI.Controller controller = new SpreadsheetGUI.Controller(stub);

            stub.FireFileSaveEvent();
            Assert.IsTrue(stub.CalledSaveDialog);
        }

        /// <summary>
        /// Tests to see if the UnsavedData method is called when FormCloseEvent is fired.
        /// </summary>
        [TestMethod]
        public void TestFormCloseEvent()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            SpreadsheetGUI.Controller controller = new SpreadsheetGUI.Controller(stub);
            CloseReason reason = new CloseReason();
            bool cancel = true;
            FormClosingEventArgs e = new FormClosingEventArgs(reason, cancel);

            stub.FireFormCloseEvent(e);
            Assert.IsTrue(stub.CalledSaveDialog);
        }

        /// <summary>
        /// Tests to make sure GetPanelUpdated and GetTextBoxUpdated are called when FormLoadEvent is fired.
        /// </summary>
        [TestMethod]
        public void TestFormLoadEvent()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            SpreadsheetGUI.Controller controller = new SpreadsheetGUI.Controller(stub);

            stub.FireFormLoadEvent();
            Assert.IsTrue(stub.CalledGetPanelUpdated && stub.CalledGetTextBoxUpdated);
        }

        /// <summary>
        /// Tests to make sure GetPanelUpdated and GetTextBoxUpdayed
        /// </summary>
        [TestMethod]
        public void TestKeyPressEnterEvent()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            SpreadsheetGUI.Controller controller = new SpreadsheetGUI.Controller(stub);
        }

        /// <summary>
        /// Tests to make sure circular exception is thrown when key press enter results in circular dependency.
        /// </summary>
        [TestMethod]
        public void TestKeyPressCircularEvent()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            SpreadsheetGUI.Controller controller = new SpreadsheetGUI.Controller(stub);
        }

        [TestMethod]
        public void TestPanelLoadEvent()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            SpreadsheetGUI.Controller controller = new SpreadsheetGUI.Controller(stub);
        }

        [TestMethod]
        public void TestSelectionChangedEvent()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            SpreadsheetGUI.Controller controller = new SpreadsheetGUI.Controller(stub);
        }
    }
}
