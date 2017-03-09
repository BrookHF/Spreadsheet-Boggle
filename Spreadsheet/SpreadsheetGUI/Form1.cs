using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SS;
using System.Text.RegularExpressions;
using Formulas;
using System.IO;

namespace SpreadsheetGUI
{
    public partial class Form1 : Form, ISpreadsheet
    {
        /// <summary>
        /// Constructs a new Form1
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event thrown to close file.
        /// </summary>
        public event Action FileCloseEvent;

        /// <summary>
        /// Event thrown to open new file.
        /// </summary>
        public event Action FileNewEvent;

        /// <summary>
        /// Event thrown to change selected cell.
        /// </summary>
        public event Action SelectionChangedEvent;

        /// <summary>
        /// Event thrown to update a Panel when it is loading.
        /// </summary>
        public event Action PanelLoadEvent;

        /// <summary>
        /// Event thrown when a key is pressed in text box.
        /// </summary>
        public event Action<char> KeyPressEvent;

        /// <summary>
        /// Event thrown to save a file.
        /// </summary>
        public event Action FileSaveEvent;

        /// <summary>
        /// Event thrown to update a form when it is loading.
        /// </summary>
        public event Action FormLoadEvent;

        /// <summary>
        /// Event throw to open a file.
        /// </summary>
        public event Action<string> FileOpenEvent;

        /// <summary>
        /// Event thrown to close a form.
        /// </summary>
        public event Action<FormClosingEventArgs> FormCloseEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileNew_Click(object sender, EventArgs e)
        {
            if (FileNewEvent != null)
            {
                FileNewEvent();
            }
        }

        /// <summary>
        /// Opens an empty spreadsheet in a new window.
        /// </summary>
        public void OpenNew()
        {
            SpreadsheetApplicationContext.GetContext().RunNew();
        }

        /// <summary>
        /// Throws a FileCloseEvent when the Close menu option is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileClose_Click(object sender, EventArgs e)
        {
            if (FileCloseEvent != null)
            {
                FileCloseEvent();
            }
        }

        /// <summary>
        /// Closes current window.
        /// </summary>
        public void DoClose()
        {
            Close();
        }

        /// <summary>
        /// Throws a SelectionChangedEvent when a new cell is selected in spreadsheet panel.
        /// </summary>
        /// <param name="sender"></param>
        private void spreadsheetPanel1_SelectionChanged(SSGui.SpreadsheetPanel sender)
        {
            if(SelectionChangedEvent != null)
            {
                SelectionChangedEvent();
            }
            
        }

        /// <summary>
        /// Returns the name of the current selected cell.
        /// </summary>
        /// <returns></returns>
        public string GetSelectedCellName()
        {
            int col, row;
            spreadsheetPanel1.GetSelection(out col, out row);
            return (char)(col + 'A') + (row + 1).ToString();
        }

        /// <summary>
        /// Throws a PanelLoadEvent when a spreadsheet panel loads.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void spreadsheetPanel1_Load_1(object sender, EventArgs e)
        {
            if (PanelLoadEvent != null)
            {
                PanelLoadEvent();
            }
            
        }

        /// <summary>
        /// Throws a KeyPressEvent when a key is pressed in the cell contents display box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cellContentsDisplay_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressEvent != null)
            {
                KeyPressEvent(e.KeyChar);
            }
        }

        /// <summary>
        /// Returns the text from the cell contents display box.
        /// </summary>
        /// <returns></returns>
        public string GetCellContentsDisplay()
        {
            return cellContentsDisplay.Text;
        }


        /// <summary>
        /// Throws a FileSaveEvent when the Save menu option is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileSave_Click(object sender, EventArgs e)
        {

            if (FileSaveEvent != null)
            {
                FileSaveEvent();
            }  
        }

        /// <summary>
        /// Creates a dialog for saving a file, returns the file name.
        /// </summary>
        /// <returns></returns>
        public string SaveDialog()
        {
            saveFileDialog1.Filter = "Spreadsheet files (*.ss)|*.ss|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save Spreadsheet File";
            saveFileDialog1.ShowDialog();
            
            return saveFileDialog1.FileName;
        }

        /// <summary>
        /// Takes cell name as parameter and updates the name display box.
        /// </summary>
        /// <param name="name"></param>
        public void SetCellNameDisplay(string name)
        {
            cellNameDisplay.Text = name;
        }

        /// <summary>
        /// Takes cell value as parameter and updates the value display box.
        /// </summary>
        /// <param name="value"></param>
        public void SetCellValueDisplay(string value)
        {
            cellValueDisplay.Text = value;
        }

        /// <summary>
        /// Takes cell content as parameter and updates the content display box.
        /// </summary>
        /// <param name="content"></param>
        public void SetCellContentsDisplay(string content)
        {
            cellContentsDisplay.Text = content;
        }

        /// <summary>
        /// Takes in collumn, row number, and value and uses value to update the display on that cell on the panel.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="value"></param>
        public void SetValueOfPanel(int col, int row, string value)
        {
            spreadsheetPanel1.SetValue(col, row, value);
        }

        /// <summary>
        /// Shows informative message when Changing Selection help menu button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpSelection_Click(object sender, EventArgs e)
        {
            MessageBox.Show("In order to change the selected cell, you must click on the cell you would like to select.", "Changing Selection", MessageBoxButtons.OK);
        }

        /// <summary>
        /// Shows informative message when Editing Cell Contents help menu button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpCellContents_Click(object sender, EventArgs e)
        {
            MessageBox.Show("In order to change the contents of a cell, you must select the cell, type the new contents in the upper right contents box, and press enter.", "Editing Cell Contents", MessageBoxButtons.OK);
        }

        /// <summary>
        /// Throws a FormLoadEvent when a new form is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            if (FormLoadEvent != null)
            {
                FormLoadEvent();
            }
        }

        /// <summary>
        /// When the Open menu option is clicked, an open file dialog is shown and if a file is selected to open, 
        /// throws a FileOpenEvent with the selected file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileOpen_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (FileOpenEvent != null)
                {
                    FileOpenEvent(openFileDialog1.FileName);
                }
            }        
        }

        /// <summary>
        /// Takes a message as a string and prints it in a window.
        /// </summary>
        /// <param name="message"></param>
        public void PrintMessage(string message)
        {
            MessageBox.Show(message);
        }

        /// <summary>
        /// Takes file name as input and opens that file in a new window.
        /// </summary>
        /// <param name="fileName"></param>
        public void OpenSaved(string fileName)
        {
            SpreadsheetApplicationContext.GetContext().RunNew(fileName);
        }

        /// <summary>
        /// When a form is closing, throw FormCloseEvent.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(FormCloseEvent != null)
            {
                FormCloseEvent(e);
            }
        }

        /// <summary>
        /// Prompts user that there is unsaved data and allows them to cancel the event by pressing cancel in prompt.
        /// </summary>
        /// <param name="e"></param>
        public void UnsavedData(FormClosingEventArgs e)
        {
            var result = MessageBox.Show("Unsaved data will be lost if you continue with this operation.", "Warning", MessageBoxButtons.OKCancel);

            if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }
    }
}
