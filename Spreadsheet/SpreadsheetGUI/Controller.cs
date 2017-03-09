using System;
using SS;
using SSGui;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Formulas;
using System.Text.RegularExpressions;
using System.IO;

namespace SpreadsheetGUI
{
    class Controller 
    {
        // The window being controlled
        private ISpreadsheet window;

        // The model being used
        private AbstractSpreadsheet backingSS;

        /// <summary>
        /// Begins controlling window.
        /// </summary>
        public Controller(ISpreadsheet window) : this(window,null)
        {
        }

        public Controller(ISpreadsheet window, string filePath)
        {
            this.window = window;
            if(filePath == null)
            {
                this.backingSS = new Spreadsheet();
            }
            else
            {
                this.backingSS = new Spreadsheet(new StreamReader(filePath), new Regex(""));
            }
            
            //window.FileChosenEvent += HandleFileChosen;
            window.FileCloseEvent += HandleClose;
            window.FileNewEvent += HandleNew;
            window.SelectionChangedEvent += HandleSelectionChanged;
            window.PanelLoadEvent += HandlePanelLoad;
            window.KeyPressEvent += HandleKeyPress;
            window.FormLoadEvent += HandleSelectionChanged;
            window.FileSaveEvent += HandleFileSave;
            window.FileOpenEvent += HandleFileOpen;
            window.FormCloseEvent += HandleFormClose;
    }

        private void HandleFormClose(FormClosingEventArgs e)
        {
            if(backingSS.Changed == true)
            {
                window.UnsavedData(e);
            }
        }

        private void HandleFileSave()
        {
            string fileName = window.SaveDialog();
            if (fileName != null)
            {
                if(fileName != "")
                {
                    backingSS.Save(File.CreateText(fileName));
                }
            }
        }

        private void HandleFileOpen(string fileName)
        {
            try
            {
                window.OpenSaved(fileName);
                
            }
            catch 
            {
                window.PrintMessage("Unable to open file, invalid File type.\n");
            }
        }

        private void HandleKeyPress(char key)
        {
            if (Regex.IsMatch("" + key, "\r"))
            {
                try
                {
                    backingSS.SetContentsOfCell(window.GetSelectedCellName(), window.GetCellContentsDisplay());
                    GetPanelUpdated();
                    GetTextBoxUpdated();
                }
                catch(Exception e)
                {
                    if(e is CircularException)
                    {
                        window.PrintMessage("This formula will result in a circular dependency.");
                    }
                    else if(e is FormulaFormatException)
                    {
                        window.PrintMessage("Invalid Formula, cannot contain: " + e.Message);
                    }
                    else
                    {
                        window.PrintMessage(e.Message);
                    }
                }
            }
        }
        private void HandlePanelLoad()
        {
            window.SetCellNameDisplay(window.GetSelectedCellName());
            window.SetCellValueDisplay(backingSS.GetCellValue(window.GetSelectedCellName()).ToString());
            window.SetCellContentsDisplay(backingSS.GetCellContents(window.GetSelectedCellName()).ToString());


        }

        /// <summary>
        /// Handles a request to close the window
        /// </summary>
        private void HandleClose()
        {    
            window.DoClose();           
        }

        private void HandleSelectionChanged()
        {
            GetPanelUpdated();
            GetTextBoxUpdated();
        }

        /// <summary>
        /// Handles a request to open a new window.
        /// </summary>
        private void HandleNew()
        {
            window.OpenNew();
        }

        /// <summary>
        /// update all cell's value in panel.
        /// </summary>
        private void GetPanelUpdated()
        {
            // update cell's value
            foreach (string name in backingSS.GetNamesOfAllNonemptyCells())
            {
                window.SetValueOfPanel(getCol(name), getRow(name), backingSS.GetCellValue(name).ToString());
            }
        }
        /// <summary>
        /// update all text boxes in panel.
        /// </summary>
        private void GetTextBoxUpdated()
        {
            // update text boxes
            window.SetCellNameDisplay(window.GetSelectedCellName());
            window.SetCellValueDisplay(backingSS.GetCellValue(window.GetSelectedCellName()).ToString());
            if (backingSS.GetCellContents(window.GetSelectedCellName()) is Formula)
            {
                window.SetCellContentsDisplay("=" + backingSS.GetCellContents(window.GetSelectedCellName()).ToString());
            }
            else
            {
                window.SetCellContentsDisplay(backingSS.GetCellContents(window.GetSelectedCellName()).ToString());
            }

        }


        private int getCol(string s)
        {
            return (s[0] - 'A');
        }
        private int getRow(string s)
        {
            int result;
            int.TryParse(s.Substring(1), out result);
            return result - 1;
        }
    }
}
