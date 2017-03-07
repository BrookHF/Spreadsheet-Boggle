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

        private void cellNameDisplay_TextChanged(object sender, EventArgs e)
        {
            //backingSS.SetContentsOfCell("A1", "123");
        }

        private void spreadsheetPanel1_Click(object sender, EventArgs e)
        {
            int col, row;
            spreadsheetPanel1.GetSelection(out col, out row);
            //spreadsheetPanel1.SetSelection(col, row);
            cellNameDisplay.Text = col.ToString() + row.ToString();
            MessageBox.Show(col.ToString() + row.ToString());
        }

        private void spreadsheetPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            int col, row;
            spreadsheetPanel1.GetSelection(out col, out row);
            //spreadsheetPanel1.SetSelection(col, row);
            cellNameDisplay.Text = col.ToString() + row.ToString();
            MessageBox.Show(col.ToString() + row.ToString());
        }

        private void spreadsheetPanel1_MouseEnter(object sender, EventArgs e)
        {
            int col, row;
            spreadsheetPanel1.GetSelection(out col, out row);
            //spreadsheetPanel1.SetSelection(col, row);
            cellNameDisplay.Text = col.ToString() + row.ToString();
            MessageBox.Show(col.ToString() + row.ToString());
        }

        private void cellContentsDisplay_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void spreadsheetPanel1_SelectionChanged(SSGui.SpreadsheetPanel sender)
        {
            int col, row;
            spreadsheetPanel1.GetSelection(out col, out row);
            cellNameDisplay.Text = (char)(col + 'A') + (row + 1).ToString();
            
        }

        private void spreadsheetPanel1_Load_1(object sender, EventArgs e)
        {
            int col, row;
            spreadsheetPanel1.GetSelection(out col, out row);
            string cellName = (char)(col + 'A') + (row + 1).ToString();
            cellNameDisplay.Text = cellName;
            cellValueDisplay.Text = backingSS.GetCellValue(cellName).ToString();

        }
    }
}
