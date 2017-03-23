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
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        public event Action<string, string> LoginEvent;
        public event Action CancelRegisterEvent;
        private void LoginButton_Click(object sender, EventArgs e)
        {
            if (LoginEvent != null)
            {
                LoginEvent(DomainNameTextBox.Text, UserNameTextBox.Text);
            }
        }

        private void CancelRegisterButton_Click(object sender, EventArgs e)
        {
            if (CancelRegisterEvent != null)
            {
                CancelRegisterEvent();
            }
        }

        public void EnableControls(bool state)
        {
            LoginButton.Enabled = state;
            DomainNameTextBox.ReadOnly = !state;
            UserNameTextBox.ReadOnly = !state;
            CancelRegisterButton.Enabled = !state;
        }

        private void HelpGameRules_Click(object sender, EventArgs e)
        {

        }

        private void HelpHowToPlay_Click(object sender, EventArgs e)
        {

        }
    }
}
