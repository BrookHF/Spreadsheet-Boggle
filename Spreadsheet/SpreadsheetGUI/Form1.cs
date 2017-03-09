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

        public Form1()
        {
            InitializeComponent();
        }

        public event Action FileCloseEvent;
        public event Action FileNewEvent;
        public event Action SelectionChangedEvent;
        public event Action PanelLoadEvent;
        public event Action<char> KeyPressEvent;
        public event Action FileSaveEvent;
        public event Action FormLoadEvent;
        public event Action<string> FileOpenEvent;
        public event Action<FormClosingEventArgs> FormCloseEvent;

        private void fileNew_Click(object sender, EventArgs e)
        {
            if (FileNewEvent != null)
            {
                FileNewEvent();
            }
        }

        public void OpenNew()
        {
            SpreadsheetApplicationContext.GetContext().RunNew();
        }



        private void fileClose_Click(object sender, EventArgs e)
        {
            if (FileCloseEvent != null)
            {
                FileCloseEvent();
            }
        }

        public void DoClose()
        {
            Close();
        }


        private void spreadsheetPanel1_SelectionChanged(SSGui.SpreadsheetPanel sender)
        {
            if(SelectionChangedEvent != null)
            {
                SelectionChangedEvent();
            }
            
        }

        public string GetSelectedCellName()
        {
            int col, row;
            spreadsheetPanel1.GetSelection(out col, out row);
            return (char)(col + 'A') + (row + 1).ToString();
        }

        private void spreadsheetPanel1_Load_1(object sender, EventArgs e)
        {
            if (PanelLoadEvent != null)
            {
                PanelLoadEvent();
            }
            
        }

        private void cellContentsDisplay_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (KeyPressEvent != null)
            {
                KeyPressEvent(e.KeyChar);
            }

            
            //cellContentsDisplay.Text = inputString;

        }
 
  
        public string GetCellContentsDisplay()
        {
            return cellContentsDisplay.Text;
        }



        private void fileSave_Click(object sender, EventArgs e)
        {

            if (FileSaveEvent != null)
            {
                FileSaveEvent();
            }  
        }

        public string SaveDialog()
        {
            saveFileDialog1.Filter = "Spreadsheet files (*.ss)|*.ss|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save Spreadsheet File";
            saveFileDialog1.ShowDialog();
            
            return saveFileDialog1.FileName;
        }
        public void SetCellNameDisplay(string name)
        {
            cellNameDisplay.Text = name;
        }

        public void SetCellValueDisplay(string value)
        {
            cellValueDisplay.Text = value;
        }
        public void SetCellContentsDisplay(string content)
        {
            cellContentsDisplay.Text = content;
        }

        public void SetValueOfPanel(int col, int row, string value)
        {
            spreadsheetPanel1.SetValue(col, row, value);
        }

        private void helpSelection_Click(object sender, EventArgs e)
        {
            MessageBox.Show("In order to change the selected cell, you must click on the cell you would like to select.", "Changing Selection", MessageBoxButtons.OK);
        }

        private void helpCellContents_Click(object sender, EventArgs e)
        {
            MessageBox.Show("In order to change the contents of a cell, you must select the cell, type the new contents in the upper right contents box, and press enter.", "Editing Cell Contents", MessageBoxButtons.OK);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (FormLoadEvent != null)
            {
                FormLoadEvent();
            }
        }

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


        public void PrintMessage(string message)
        {
            MessageBox.Show(message);
        }

        public void OpenSaved(string fileName)
        {
            SpreadsheetApplicationContext.GetContext().RunNew(fileName);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(FormCloseEvent != null)
            {
                FormCloseEvent(e);
            }

        }
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
