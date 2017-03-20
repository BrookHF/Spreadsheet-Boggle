namespace BoggleClient
{
    partial class RegisterForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.DomainNameTextBox = new System.Windows.Forms.TextBox();
            this.UserNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.LoginButton = new System.Windows.Forms.Button();
            this.CancelRegisterButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // DomainNameTextBox
            // 
            this.DomainNameTextBox.Location = new System.Drawing.Point(101, 97);
            this.DomainNameTextBox.Name = "DomainNameTextBox";
            this.DomainNameTextBox.Size = new System.Drawing.Size(100, 25);
            this.DomainNameTextBox.TabIndex = 0;
            // 
            // UserNameTextBox
            // 
            this.UserNameTextBox.Location = new System.Drawing.Point(218, 97);
            this.UserNameTextBox.Name = "UserNameTextBox";
            this.UserNameTextBox.Size = new System.Drawing.Size(100, 25);
            this.UserNameTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(106, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Domain Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(224, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "User Name";
            // 
            // LoginButton
            // 
            this.LoginButton.Location = new System.Drawing.Point(109, 149);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(75, 23);
            this.LoginButton.TabIndex = 4;
            this.LoginButton.Text = "Login";
            this.LoginButton.UseVisualStyleBackColor = true;
            this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // CancelRegisterButton
            // 
            this.CancelRegisterButton.Location = new System.Drawing.Point(228, 149);
            this.CancelRegisterButton.Name = "CancelRegisterButton";
            this.CancelRegisterButton.Size = new System.Drawing.Size(75, 23);
            this.CancelRegisterButton.TabIndex = 5;
            this.CancelRegisterButton.Text = "Cancel";
            this.CancelRegisterButton.UseVisualStyleBackColor = true;
            this.CancelRegisterButton.Click += new System.EventHandler(this.CancelRegisterButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(383, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Welcome to Boggle, Please enter the domain name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(92, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(255, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "and User name to enter the game";
            // 
            // RegisterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(466, 258);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.CancelRegisterButton);
            this.Controls.Add(this.LoginButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.UserNameTextBox);
            this.Controls.Add(this.DomainNameTextBox);
            this.Name = "RegisterForm";
            this.Text = "RegisterForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox DomainNameTextBox;
        private System.Windows.Forms.TextBox UserNameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.Button CancelRegisterButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}