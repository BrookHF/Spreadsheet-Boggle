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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.fileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.fileClose = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.helpCellContents = new System.Windows.Forms.ToolStripMenuItem();
            this.cellValueDisplay = new System.Windows.Forms.TextBox();
            this.cellNameDisplay = new System.Windows.Forms.TextBox();
            this.cellContentsDisplay = new System.Windows.Forms.TextBox();
            this.cellNameLabel = new System.Windows.Forms.Label();
            this.valueLabel = new System.Windows.Forms.Label();
            this.contentsLabel = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.spreadsheetPanel1 = new SSGui.SpreadsheetPanel();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
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
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileNew,
            this.fileOpen,
            this.fileSave,
            this.fileClose});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // fileNew
            // 
            this.fileNew.Name = "fileNew";
            this.fileNew.Size = new System.Drawing.Size(120, 26);
            this.fileNew.Text = "New";
            this.fileNew.Click += new System.EventHandler(this.fileNew_Click);
            // 
            // fileOpen
            // 
            this.fileOpen.Name = "fileOpen";
            this.fileOpen.Size = new System.Drawing.Size(120, 26);
            this.fileOpen.Text = "Open";
            // 
            // fileSave
            // 
            this.fileSave.Name = "fileSave";
            this.fileSave.Size = new System.Drawing.Size(120, 26);
            this.fileSave.Text = "Save";
            // 
            // fileClose
            // 
            this.fileClose.Name = "fileClose";
            this.fileClose.Size = new System.Drawing.Size(120, 26);
            this.fileClose.Text = "Close";
            this.fileClose.Click += new System.EventHandler(this.fileClose_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpSelection,
            this.helpCellContents});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // helpSelection
            // 
            this.helpSelection.Name = "helpSelection";
            this.helpSelection.Size = new System.Drawing.Size(222, 26);
            this.helpSelection.Text = "Changing Selection";
            // 
            // helpCellContents
            // 
            this.helpCellContents.Name = "helpCellContents";
            this.helpCellContents.Size = new System.Drawing.Size(222, 26);
            this.helpCellContents.Text = "Editing Cell Contents";
            // 
            // cellValueDisplay
            // 
            this.cellValueDisplay.Location = new System.Drawing.Point(684, 5);
            this.cellValueDisplay.Margin = new System.Windows.Forms.Padding(4);
            this.cellValueDisplay.Name = "cellValueDisplay";
            this.cellValueDisplay.ReadOnly = true;
            this.cellValueDisplay.Size = new System.Drawing.Size(132, 22);
            this.cellValueDisplay.TabIndex = 2;
            // 
            // cellNameDisplay
            // 
            this.cellNameDisplay.HideSelection = false;
            this.cellNameDisplay.Location = new System.Drawing.Point(385, 5);
            this.cellNameDisplay.Margin = new System.Windows.Forms.Padding(4);
            this.cellNameDisplay.Name = "cellNameDisplay";
            this.cellNameDisplay.ReadOnly = true;
            this.cellNameDisplay.Size = new System.Drawing.Size(132, 22);
            this.cellNameDisplay.TabIndex = 3;
            this.cellNameDisplay.TextChanged += new System.EventHandler(this.cellNameDisplay_TextChanged);
            // 
            // cellContentsDisplay
            // 
            this.cellContentsDisplay.Location = new System.Drawing.Point(987, 5);
            this.cellContentsDisplay.Margin = new System.Windows.Forms.Padding(4);
            this.cellContentsDisplay.Name = "cellContentsDisplay";
            this.cellContentsDisplay.Size = new System.Drawing.Size(132, 22);
            this.cellContentsDisplay.TabIndex = 4;
            this.cellContentsDisplay.TextChanged += new System.EventHandler(this.cellContentsDisplay_TextChanged);
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
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // spreadsheetPanel1
            // 
            this.spreadsheetPanel1.Location = new System.Drawing.Point(16, 33);
            this.spreadsheetPanel1.Margin = new System.Windows.Forms.Padding(5);
            this.spreadsheetPanel1.Name = "spreadsheetPanel1";
            this.spreadsheetPanel1.Size = new System.Drawing.Size(1307, 767);
            this.spreadsheetPanel1.TabIndex = 0;
            this.spreadsheetPanel1.SelectionChanged += new SSGui.SelectionChangedHandler(this.spreadsheetPanel1_SelectionChanged);
            this.spreadsheetPanel1.Load += new System.EventHandler(this.spreadsheetPanel1_Load_1);
            this.spreadsheetPanel1.Click += new System.EventHandler(this.spreadsheetPanel1_Click);
            this.spreadsheetPanel1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.spreadsheetPanel1_MouseClick);
            this.spreadsheetPanel1.MouseEnter += new System.EventHandler(this.spreadsheetPanel1_MouseEnter);
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
            this.Margin = new System.Windows.Forms.Padding(4);
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
        private System.Windows.Forms.ToolStripMenuItem fileNew;
        private System.Windows.Forms.ToolStripMenuItem fileOpen;
        private System.Windows.Forms.ToolStripMenuItem fileSave;
        private System.Windows.Forms.ToolStripMenuItem fileClose;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpSelection;
        private System.Windows.Forms.ToolStripMenuItem helpCellContents;
        private System.Windows.Forms.TextBox cellValueDisplay;
        private System.Windows.Forms.TextBox cellNameDisplay;
        private System.Windows.Forms.TextBox cellContentsDisplay;
        private System.Windows.Forms.Label cellNameLabel;
        private System.Windows.Forms.Label valueLabel;
        private System.Windows.Forms.Label contentsLabel;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}

