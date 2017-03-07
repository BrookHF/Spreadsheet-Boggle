namespace SpreadsheetGUI
{
    partial class Form1
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
            this.spreadsheetPanel1 = new SSGui.SpreadsheetPanel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changingSelectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editingCellContentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cellValueDisplay = new System.Windows.Forms.TextBox();
            this.cellNameDisplay = new System.Windows.Forms.TextBox();
            this.cellContentsDisplay = new System.Windows.Forms.TextBox();
            this.cellNameLabel = new System.Windows.Forms.Label();
            this.valueLabel = new System.Windows.Forms.Label();
            this.contentsLabel = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // spreadsheetPanel1
            // 
            this.spreadsheetPanel1.Location = new System.Drawing.Point(16, 33);
            this.spreadsheetPanel1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.spreadsheetPanel1.Name = "spreadsheetPanel1";
            this.spreadsheetPanel1.Size = new System.Drawing.Size(1307, 767);
            this.spreadsheetPanel1.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1339, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.newToolStripMenuItem.Text = "New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(120, 26);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(120, 26);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(120, 26);
            this.closeToolStripMenuItem.Text = "Close";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changingSelectionsToolStripMenuItem,
            this.editingCellContentsToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // changingSelectionsToolStripMenuItem
            // 
            this.changingSelectionsToolStripMenuItem.Name = "changingSelectionsToolStripMenuItem";
            this.changingSelectionsToolStripMenuItem.Size = new System.Drawing.Size(222, 26);
            this.changingSelectionsToolStripMenuItem.Text = "Changing Selection";
            // 
            // editingCellContentsToolStripMenuItem
            // 
            this.editingCellContentsToolStripMenuItem.Name = "editingCellContentsToolStripMenuItem";
            this.editingCellContentsToolStripMenuItem.Size = new System.Drawing.Size(222, 26);
            this.editingCellContentsToolStripMenuItem.Text = "Editing Cell Contents";
            // 
            // cellValueDisplay
            // 
            this.cellValueDisplay.Location = new System.Drawing.Point(684, 5);
            this.cellValueDisplay.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cellValueDisplay.Name = "cellValueDisplay";
            this.cellValueDisplay.ReadOnly = true;
            this.cellValueDisplay.Size = new System.Drawing.Size(132, 22);
            this.cellValueDisplay.TabIndex = 2;
            // 
            // cellNameDisplay
            // 
            this.cellNameDisplay.HideSelection = false;
            this.cellNameDisplay.Location = new System.Drawing.Point(385, 5);
            this.cellNameDisplay.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cellNameDisplay.Name = "cellNameDisplay";
            this.cellNameDisplay.ReadOnly = true;
            this.cellNameDisplay.Size = new System.Drawing.Size(132, 22);
            this.cellNameDisplay.TabIndex = 3;
            // 
            // cellContentsDisplay
            // 
            this.cellContentsDisplay.Location = new System.Drawing.Point(987, 5);
            this.cellContentsDisplay.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cellContentsDisplay.Name = "cellContentsDisplay";
            this.cellContentsDisplay.Size = new System.Drawing.Size(132, 22);
            this.cellContentsDisplay.TabIndex = 4;
            // 
            // cellNameLabel
            // 
            this.cellNameLabel.AutoSize = true;
            this.cellNameLabel.Location = new System.Drawing.Point(304, 9);
            this.cellNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.cellNameLabel.Name = "cellNameLabel";
            this.cellNameLabel.Size = new System.Drawing.Size(72, 17);
            this.cellNameLabel.TabIndex = 5;
            this.cellNameLabel.Text = "Cell Name";
            // 
            // valueLabel
            // 
            this.valueLabel.AutoSize = true;
            this.valueLabel.Location = new System.Drawing.Point(629, 9);
            this.valueLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.valueLabel.Name = "valueLabel";
            this.valueLabel.Size = new System.Drawing.Size(44, 17);
            this.valueLabel.TabIndex = 6;
            this.valueLabel.Text = "Value";
            // 
            // contentsLabel
            // 
            this.contentsLabel.AutoSize = true;
            this.contentsLabel.Location = new System.Drawing.Point(913, 9);
            this.contentsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.contentsLabel.Name = "contentsLabel";
            this.contentsLabel.Size = new System.Drawing.Size(64, 17);
            this.contentsLabel.TabIndex = 7;
            this.contentsLabel.Text = "Contents";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1339, 815);
            this.Controls.Add(this.contentsLabel);
            this.Controls.Add(this.valueLabel);
            this.Controls.Add(this.cellNameLabel);
            this.Controls.Add(this.cellContentsDisplay);
            this.Controls.Add(this.cellNameDisplay);
            this.Controls.Add(this.cellValueDisplay);
            this.Controls.Add(this.spreadsheetPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SSGui.SpreadsheetPanel spreadsheetPanel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changingSelectionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editingCellContentsToolStripMenuItem;
        private System.Windows.Forms.TextBox cellValueDisplay;
        private System.Windows.Forms.TextBox cellNameDisplay;
        private System.Windows.Forms.TextBox cellContentsDisplay;
        private System.Windows.Forms.Label cellNameLabel;
        private System.Windows.Forms.Label valueLabel;
        private System.Windows.Forms.Label contentsLabel;
    }
}

