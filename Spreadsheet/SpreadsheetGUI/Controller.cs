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
    public class Controller 
    {
        // The window being controlled
        private ISpreadsheet window;

        // The model being used
        private AbstractSpreadsheet backingSS;

        /// <summary>
        /// Begins controlling a new window.
        /// </summary>
        public Controller(ISpreadsheet window) : this(window,null)
        {
        }

        /// <summary>
        /// Begins controlling a new window by loading spreadsheet from filePath.
        /// </summary>
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
            
            //Gets event from the window and handles them with controller methods.
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
        /// <summary>
        /// Handles the FormCloseEvent, if the spreadhseet is changed then uses UnsavedData method to 
        /// ask user if they want to continue and lose unsaved changes, otherwise it continues with the close.
        /// </summary>
        /// <param name="e"></param>
        private void HandleFormClose(FormClosingEventArgs e)
        {
            if(backingSS.Changed == true)
            {
                window.UnsavedData(e);
            }
        }

        /// <summary>
        /// Handles the file save event, creates a save dialog, if file name isn't empty or null, it will use the backing 
        /// Spreadsheet's save method to generate a file.
        /// </summary>
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

        /// <summary>
        /// Handles the file open event, takes a file name as parameter and calls the OpenSaved method on it. If it doesnt 
        /// successfully open the file, prints a message to notify user.
        /// </summary>
        /// <param name="fileName"></param>
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

        /// <summary>
        /// Handles the KeyPress event when typing in the Content display box. If the "enter" key is pressed, 
        /// updates the contents of the cell to the string in the display box. If this results in any exceptions, 
        /// it will instead print an informative message based on the exception.
        /// </summary>
        /// <param name="key"></param>
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

        /// <summary>
        /// Handles the panel load event, updates the display boxes based on the selected cell when the panel loads. 
        /// </summary>
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

        /// <summary>
        /// Handles the SelectionChanged event, updates the values displayed in the Panel and the display boxes.
        /// </summary>
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

            //if the contents are a formula, then concatinate "=" before the rest of the formula for the display.
            if (backingSS.GetCellContents(window.GetSelectedCellName()) is Formula)
            {
                window.SetCellContentsDisplay("=" + backingSS.GetCellContents(window.GetSelectedCellName()).ToString());
            }
            else
            {
                window.SetCellContentsDisplay(backingSS.GetCellContents(window.GetSelectedCellName()).ToString());
            }

        }

        /// <summary>
        /// Helper method to return the collumn number from the input cell name.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private int getCol(string s)
        {
            return (s[0] - 'A');
        }

        /// <summary>
        /// Helper method to obtain row number from a cell name.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private int getRow(string s)
        {
            int result;
            int.TryParse(s.Substring(1), out result);
            return result - 1;
        }
    }
}
