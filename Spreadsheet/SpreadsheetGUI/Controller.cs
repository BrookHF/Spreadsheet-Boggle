using System;
using SS;
using SSGui;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetGUI
{
    class Controller
    {
        // The window being controlled
        private Form1 window;

        // The model being used
        private AbstractSpreadsheet backingSS;

        /// <summary>
        /// Begins controlling window.
        /// </summary>
        public Controller(Form1 window)
        {
            this.window = window;
            this.backingSS = new Spreadsheet();
            window.FileChosenEvent += HandleFileChosen;
            window.fileClose_click += HandleClose;
            window.NewEvent += HandleNew;
            window.CountEvent += HandleCount;
        }

        /// <summary>
        /// Handles a request to open a file.
        /// </summary>
        private void HandleFileChosen(String filename)
        {
            try
            {
                model.ReadFile(filename);
                window.CharCount = model.CountChars();
                window.WordCount = model.CountWords();
                window.LineCount = model.CountLines();
                window.SubstringCount = 0;
                window.SearchString = "";
                window.Title = filename;
            }
            catch (Exception ex)
            {
                window.Message = "Unable to open file\n" + ex.Message;
            }
        }

        /// <summary>
        /// Handles a request to close the window
        /// </summary>
        private void HandleClose()
        {
            window.DoClose();
        }

        /// <summary>
        /// Handles a request to open a new window.
        /// </summary>
        private void HandleNew()
        {
            window.OpenNew();
        }

        /// <summary>
        /// Handles a request to count occurrences of the search string.
        /// </summary>
        /// <param name="searchString">The search string.</param>
        private void HandleCount(string searchString)
        {
            window.SubstringCount = model.CountSubstrings(searchString);
        }
    }
}
