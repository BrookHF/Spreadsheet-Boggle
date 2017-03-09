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

        event Action FileCloseEvent;
        event Action FileNewEvent;
        event Action SelectionChangedEvent;
        event Action PanelLoadEvent;
        event Action<char> KeyPressEvent;
        event Action FileSaveEvent;
        event Action FormLoadEvent;
        event Action<string> FileOpenEvent;
        event Action<FormClosingEventArgs> FormCloseEvent;

        void PrintMessage(string message);
        void OpenNew();
        void DoClose();
        string GetCellContentsDisplay();
        string GetSelectedCellName();
        string SaveDialog();
        void SetCellNameDisplay(string name);
        void SetCellValueDisplay(string value);
        void SetCellContentsDisplay(string content);
        void SetValueOfPanel(int col, int row, string value);
        void OpenSaved(string fileName);
        void UnsavedData(FormClosingEventArgs e);
    }
}
