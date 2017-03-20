namespace BoggleClient
{
    partial class GameTimeForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.PlayTimeTextBox = new System.Windows.Forms.TextBox();
            this.PlayButton = new System.Windows.Forms.Button();
            this.CancelJoinButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(89, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(255, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please enter desired play time.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(105, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(199, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "between 5 to 120 seconds";
            // 
            // PlayTimeTextBox
            // 
            this.PlayTimeTextBox.Location = new System.Drawing.Point(145, 137);
            this.PlayTimeTextBox.Name = "PlayTimeTextBox";
            this.PlayTimeTextBox.Size = new System.Drawing.Size(100, 25);
            this.PlayTimeTextBox.TabIndex = 2;
            // 
            // PlayButton
            // 
            this.PlayButton.Location = new System.Drawing.Point(92, 183);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(75, 23);
            this.PlayButton.TabIndex = 3;
            this.PlayButton.Text = "Play";
            this.PlayButton.UseVisualStyleBackColor = true;
            this.PlayButton.Click += new System.EventHandler(this.PlayButton_Click);
            // 
            // CancelJoinButton
            // 
            this.CancelJoinButton.Location = new System.Drawing.Point(229, 183);
            this.CancelJoinButton.Name = "CancelJoinButton";
            this.CancelJoinButton.Size = new System.Drawing.Size(75, 23);
            this.CancelJoinButton.TabIndex = 4;
            this.CancelJoinButton.Text = "Cancel";
            this.CancelJoinButton.UseVisualStyleBackColor = true;
            this.CancelJoinButton.Click += new System.EventHandler(this.CancelJoinButton_Click);
            // 
            // GameTimeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(427, 357);
            this.Controls.Add(this.CancelJoinButton);
            this.Controls.Add(this.PlayButton);
            this.Controls.Add(this.PlayTimeTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "GameTimeForm";
            this.Text = "GameTime";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox PlayTimeTextBox;
        private System.Windows.Forms.Button PlayButton;
        private System.Windows.Forms.Button CancelJoinButton;
    }
}