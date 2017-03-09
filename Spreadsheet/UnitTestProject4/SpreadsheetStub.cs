using SpreadsheetGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUITest
{
    class SpreadsheetStub : ISpreadsheet
    {
        public void FireFileCloseEvent()
        {
            if (FileCloseEvent != null)
            {
                FileCloseEvent();
            }
        }
        public void FireFileNewEvent()
        {
            if (FileNewEvent != null)
            {
                FileNewEvent();
            }
        }
        public void FireFileOpenEvent(string fileName)
        {
            if (FileOpenEvent != null)
            {
                FileOpenEvent(fileName);
            }
        }
        public void FireFileSaveEvent()
        {
            if (FileSaveEvent != null)
            {
                FileSaveEvent();
            }
        }
        public void FireFormCloseEvent(FormClosingEventArgs e)
        {
            if (FormCloseEvent != null)
            {
                FormCloseEvent(e);
            }
        }
        public void FireFormLoadEvent()
        {
            if (FormLoadEvent != null)
            {
                FormLoadEvent();
            }
        }
        public void FireKeyPressEvent(char key)
        {
            if (KeyPressEvent != null)
            {
                KeyPressEvent(key);
            }
        }
        public void FirePanelLoadEvent()
        {
            if (PanelLoadEvent != null)
            {
                PanelLoadEvent();
            }
        }
        public void FireSelectionChangedEvent()
        {
            if (SelectionChangedEvent != null)
            {
                SelectionChangedEvent();
            }
        }

        public event Action FileCloseEvent;
        public event Action FileNewEvent;
        public event Action<string> FileOpenEvent;
        public event Action FileSaveEvent;
        public event Action<FormClosingEventArgs> FormCloseEvent;
        public event Action FormLoadEvent;
        public event Action<char> KeyPressEvent;
        public event Action PanelLoadEvent;
        public event Action SelectionChangedEvent;


        public bool Closed { get; set; }
        public void DoClose()
        {
            Closed = true;
        }

        public string GetCellContentsDisplay()
        {
            return CellContentsDisplay;
        }
        public string SelectedCell { get; private set; }
        public string GetSelectedCellName()
        {
            return SelectedCell;
        }

        public bool CalledOpenNew { get; private set; }
        public void OpenNew()
        {
            CalledOpenNew = true;
        }

        public bool CalledOpenSaved { get; private set; }
        public void OpenSaved(string fileName)
        {
            CalledOpenSaved = true;
        }

        public bool CalledPrintMessage { get; private set; }
        public void PrintMessage(string message)
        {
            CalledPrintMessage = true;
        }
        public string SaveDialog()
        {
            return "newFile.ss";
        }
        public string CellContentsDisplay{ get; private set; }
        public void SetCellContentsDisplay(string content)
        {
            CellContentsDisplay = content;
        }
        public string CellNameDisplay { get; private set; }
        public void SetCellNameDisplay(string name)
        {
            CellNameDisplay = name;
        }
        public string CellValueDisplay { get; private set; }
        public void SetCellValueDisplay(string value)
        {
            CellValueDisplay = value;
        }

        private string[,] panelGrid = new string[99,26]; 
        public string[,] PanelGrid { get; private set; }
        public void SetValueOfPanel(int col, int row, string value)
        {
            PanelGrid[col, row] = value;
        }

        public bool CalledUnsavedData { get; private set; }
        public void UnsavedData(FormClosingEventArgs e)
        {
            CalledUnsavedData = true;
        }
    }
}
