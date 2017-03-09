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
        private ISpreadhseet window;

        // The model being used
        private AbstractSpreadsheet backingSS;

        /// <summary>
        /// Begins controlling window.
        /// </summary>
        public Controller(ISpreadhseet window) : this(window,null)
        {
        }

        public Controller(ISpreadhseet window, string filePath)
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
        }


        private void HandleFileSave()
        {
            if (window.SaveDialog() != null)
            {
                backingSS.Save(File.CreateText(window.SaveDialog()));
            }
            
        }

        private void HandleFileOpen(string fileName)
        {
            try
            {
                window.OpenSaved(fileName);
                
            }
            catch (Exception ex)
            {
                window.PrintMessage("Unable to open file\n" + ex);
            }
        }

        private void HandleKeyPress(char key)
        {
            if (Regex.IsMatch("" + key, "\r"))
            {
                backingSS.SetContentsOfCell(window.GetSelectedCellName(), window.GetCellContentsDisplay());
                GetPanelUpdated();
                GetTextBoxUpdated();
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
            if (backingSS.Changed)
            {
                var result = MessageBox.Show("Unsaved data will be lost if you continue with this operation.", "Warning", MessageBoxButtons.OKCancel);
                
                if (result == DialogResult.OK)
                {
                    window.DoClose();
                }
            }
            else
            {
                window.DoClose();
            }

            
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
