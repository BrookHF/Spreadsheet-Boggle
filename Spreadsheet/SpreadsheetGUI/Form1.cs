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
    public partial class Form1 : Form
    {
        Spreadsheet backingSS = new Spreadsheet();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void spreadsheetPanel1_Load(object sender, EventArgs e)
        {
            
        }

        private void fileNew_Click(object sender, EventArgs e)
        {
            new Form1();
        }

        private void fileClose_Click(object sender, EventArgs e)
        {
            if(backingSS.Changed)
            {
                var result = System.Windows.Forms.MessageBox.Show("Unsaved data will be lost if you continue with this operation.", "Warning", MessageBoxButtons.OKCancel);
                if(result == DialogResult.OK)
                {
                    Close();
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }


        private void spreadsheetPanel1_SelectionChanged(SSGui.SpreadsheetPanel sender)
        {
            getTextBoxUpdated();
        }



        private void spreadsheetPanel1_Load_1(object sender, EventArgs e)
        {
            
            cellNameDisplay.Text = getSelectedCellName();
            cellValueDisplay.Text = backingSS.GetCellValue(getSelectedCellName()).ToString();

        }

        private void cellContentsDisplay_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if (Regex.IsMatch("" + e.KeyChar, "\r"))
            {
                int col, row;
                spreadsheetPanel1.GetSelection(out col, out row);
                backingSS.SetContentsOfCell(getSelectedCellName(), cellContentsDisplay.Text);
                getPanelUpdated();
                getTextBoxUpdated();
            }
            
            //cellContentsDisplay.Text = inputString;

        }
        /// <summary>
        /// update all cell's value in panel.
        /// </summary>
        private void getPanelUpdated()
        {
            // update cell's value
            foreach (string name in backingSS.GetNamesOfAllNonemptyCells())
            {
                spreadsheetPanel1.SetValue(getCol(name), getRow(name), backingSS.GetCellValue(name).ToString());
            }
        }
        /// <summary>
        /// update all text boxes in panel.
        /// </summary>
        private void getTextBoxUpdated()
        {
            // update text boxes
            cellNameDisplay.Text = getSelectedCellName();
            cellValueDisplay.Text = backingSS.GetCellValue(getSelectedCellName()).ToString();
            if(backingSS.GetCellContents(getSelectedCellName()) is Formula)
            {
                cellContentsDisplay.Text = "=" + backingSS.GetCellContents(getSelectedCellName()).ToString();
            }
            else
            {
                cellContentsDisplay.Text = backingSS.GetCellContents(getSelectedCellName()).ToString();
            }
            
        }
        private string getSelectedCellName()
        {
            int col, row;
            spreadsheetPanel1.GetSelection(out col, out row);
            return (char)(col + 'A') + (row + 1).ToString();
        }

        private int getCol(string s)
        {
            return (s[0]-'A');
        }
        private int getRow(string s)
        {
            int result;
            int.TryParse(s.Substring(1), out result);
            return result-1;
        }

        private void fileSave_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Spreadsheet files (*.ss)|*.ss|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save Spreadsheet File";
            saveFileDialog1.ShowDialog();
            while (saveFileDialog1.CheckFileExists)
            {
                var result = MessageBox.Show("This file will be overwritten, do you want to continue?", "Warning", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    break;
                }
                else
                {
                    saveFileDialog1.ShowDialog();
                }
            }

            if (saveFileDialog1.FileName != null)
            {
                StreamWriter sw = File.CreateText(saveFileDialog1.FileName);
                backingSS.Save(sw);
            }
            
        }

        private void helpSelection_Click(object sender, EventArgs e)
        {
            MessageBox.Show("In order to change the selected cell, you must click on the cell you would like to select.", "Changing Selection", MessageBoxButtons.OK);
        }

        private void helpCellContents_Click(object sender, EventArgs e)
        {
            MessageBox.Show("In order to change the contents of a cell, you must select the cell, type the new contents in the upper right contents box, and press enter.", "Editing Cell Contents", MessageBoxButtons.OK);
        }
    }
}
