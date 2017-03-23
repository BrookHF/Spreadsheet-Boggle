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
    public partial class PlayForm : Form
    {

        public PlayForm()
        {
            InitializeComponent();
        }

        public event Action<string> EnterEvent;
        public event Action LeaveEvent;
        public event Action PlayFormClosingEvent;



        private void EnterButton_Click(object sender, EventArgs e)
        {
            if (EnterEvent != null)
            {
                EnterEvent(InputTextBox.Text);
                InputTextBox.Text = "";
            }
        }

        private void LeaveButton_Click(object sender, EventArgs e)
        {
            if (LeaveEvent != null)
            {
                LeaveEvent();
            }
        }

        public void UpdateNamesAndBoard(string your, string opponent, string board)
        {
            OpponentsNameDisplayTextBox.Text = opponent;
            YourNameDisplayTextBox.Text = your;
            Display1_1.Text = board[0]=='Q' ? "QU" : "" + board[0];
            Display1_2.Text = board[1] == 'Q' ? "QU" : "" + board[1];
            Display1_3.Text = board[2] == 'Q' ? "QU" : "" + board[2];
            Display1_4.Text = board[3] == 'Q' ? "QU" : "" + board[3];
            Display2_1.Text = board[4] == 'Q' ? "QU" : "" + board[4];
            Display2_2.Text = board[5] == 'Q' ? "QU" : "" + board[5];
            Display2_3.Text = board[6] == 'Q' ? "QU" : "" + board[6];
            Display2_4.Text = board[7] == 'Q' ? "QU" : "" + board[7];
            Display3_1.Text = board[8] == 'Q' ? "QU" : "" + board[8];
            Display3_2.Text = board[9] == 'Q' ? "QU" : "" + board[9];
            Display3_3.Text = board[10] == 'Q' ? "QU" : "" + board[10];
            Display3_4.Text = board[11] == 'Q' ? "QU" : "" + board[11];
            Display4_1.Text = board[12] == 'Q' ? "QU" : "" + board[12];
            Display4_2.Text = board[13] == 'Q' ? "QU" : "" + board[13];
            Display4_3.Text = board[14] == 'Q' ? "QU" : "" + board[14];
            Display4_4.Text = board[15] == 'Q' ? "QU" : "" + board[15];
        }
        public void UpdateScoresAndTime(string yourScore, string opponentScore, string timeRemain, string domain)
        {
            YourScoreDisplayTextBox.Text = yourScore;
            OpponentScoreDisplayTextBox.Text = opponentScore;
            TimerDisplayTextBox.Text = timeRemain;
            ServerDisplayTextBox.Text = domain;
        }

        private void PlayForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (PlayFormClosingEvent != null)
            {
                PlayFormClosingEvent();
            }
        }
    }
}
