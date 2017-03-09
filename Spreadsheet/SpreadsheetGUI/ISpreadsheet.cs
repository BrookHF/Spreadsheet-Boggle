using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    public interface ISpreadsheet
    {
        /// <summary>
        /// Event thrown to close file.
        /// </summary>
        event Action FileCloseEvent;

        /// <summary>
        /// Event thrown to open new file.
        /// </summary>
        event Action FileNewEvent;

        /// <summary>
        /// Event thrown to change selected cell.
        /// </summary>
        event Action SelectionChangedEvent;

        /// <summary>
        /// Event thrown to update a Panel when it is loading.
        /// </summary>
        event Action PanelLoadEvent;

        /// <summary>
        /// Event thrown when a key is pressed in text box.
        /// </summary>
        event Action<char> KeyPressEvent;

        /// <summary>
        /// Event thrown to save a file.
        /// </summary>
        event Action FileSaveEvent;

        /// <summary>
        /// Event thrown to update a form when it is loading.
        /// </summary>
        event Action FormLoadEvent;

        /// <summary>
        /// Event throw to open a file.
        /// </summary>
        event Action<string> FileOpenEvent;

        /// <summary>
        /// Event thrown to close a form.
        /// </summary>
        event Action<FormClosingEventArgs> FormCloseEvent;

        /// <summary>
        /// Takes a message as a string and prints it in a window.
        /// </summary>
        /// <param name="message"></param>
        void PrintMessage(string message);

        /// <summary>
        /// Opens an empty spreadsheet in a new window.
        /// </summary>
        void OpenNew();

        /// <summary>
        /// Closes current window.
        /// </summary>
        void DoClose();

        /// <summary>
        /// Returns the text from the cell contents display box.
        /// </summary>
        /// <returns></returns>
        string GetCellContentsDisplay();

        /// <summary>
        /// Returns the name of the current selected cell.
        /// </summary>
        /// <returns></returns>
        string GetSelectedCellName();

        /// <summary>
        /// Creates a dialog for saving a file, returns the file name.
        /// </summary>
        /// <returns></returns>
        string SaveDialog();

        /// <summary>
        /// Takes cell name as parameter and updates the name display box.
        /// </summary>
        /// <param name="name"></param>
        void SetCellNameDisplay(string name);

        /// <summary>
        /// Takes cell value as parameter and updates the value display box.
        /// </summary>
        /// <param name="value"></param>
        void SetCellValueDisplay(string value);

        /// <summary>
        /// Takes cell content as parameter and updates the content display box.
        /// </summary>
        /// <param name="content"></param>
        void SetCellContentsDisplay(string content);

        /// <summary>
        /// Takes in collumn, row number, and value and uses value to update the display on that cell on the panel.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="value"></param>
        void SetValueOfPanel(int col, int row, string value);

        /// <summary>
        /// Takes file name as input and opens that file in a new window.
        /// </summary>
        /// <param name="fileName"></param>
        void OpenSaved(string fileName);

        /// <summary>
        /// Prompts user that there is unsaved data and allows them to cancel the event by pressing cancel in prompt.
        /// </summary>
        /// <param name="e"></param>
        void UnsavedData(FormClosingEventArgs e);
    }
}
