using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoggleClient
{
    public partial class GameTimeForm : Form
    {
        public event Action<string> PlayEvent;
        public event Action CancelJoinEvent;
        public event Action GameTimeFormClosingEvent;
        public GameTimeForm()
        {
            InitializeComponent();
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (PlayEvent != null)
            {
                PlayEvent(PlayTimeTextBox.Text);
            }
        }

        private void CancelJoinButton_Click(object sender, EventArgs e)
        {
            if (CancelJoinEvent != null)
            {
                CancelJoinEvent();
            }
        }

        public void SearchingMessageVisible(bool state)
        {
            SearchingLabel.Visible = state;
            LoadingGif.Visible = state;
        }

        private void GameTimeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (GameTimeFormClosingEvent != null)
            {
                GameTimeFormClosingEvent();
            }
        }
    }
}
