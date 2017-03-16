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

        public event Action LoadPlayFormEvent;
        public event Action<string> EnterEvent;
        public event Action LeaveEvent;
        public event Action HelpGameRulesEvent;
        public event Action HelpHowToPlayEvent;


        private void PlayForm_Load(object sender, EventArgs e)
        {
            if(LoadPlayFormEvent != null)
            {
                LoadPlayFormEvent();
            }
        }

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

        private void HelpGameRules_Click(object sender, EventArgs e)
        {
            if (HelpGameRulesEvent != null)
            {
                HelpGameRulesEvent();
            }
        }

        private void HelpHowToPlay_Click(object sender, EventArgs e)
        {
            if (HelpHowToPlayEvent != null)
            {
                HelpHowToPlayEvent();
            }
        }


    }
}
