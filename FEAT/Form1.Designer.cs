namespace Fire_Emblem_Awakening_Archive_Tool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.RTB_Output = new System.Windows.Forms.RichTextBox();
            this.TB_FilePath = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.B_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.B_Go = new System.Windows.Forms.ToolStripMenuItem();
            this.B_Help = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.B_AutoExtract = new System.Windows.Forms.ToolStripMenuItem();
            this.B_BuildTexture = new System.Windows.Forms.ToolStripMenuItem();
            this.B_ArcPadding = new System.Windows.Forms.ToolStripMenuItem();
            this.aRCFileAlignmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.B_Align0 = new System.Windows.Forms.ToolStripMenuItem();
            this.B_Align16 = new System.Windows.Forms.ToolStripMenuItem();
            this.B_Align32 = new System.Windows.Forms.ToolStripMenuItem();
            this.B_Align64 = new System.Windows.Forms.ToolStripMenuItem();
            this.B_Align128 = new System.Windows.Forms.ToolStripMenuItem();
            this.B_RubyScript = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // RTB_Output
            // 
            this.RTB_Output.BackColor = System.Drawing.SystemColors.Control;
            this.RTB_Output.Location = new System.Drawing.Point(7, 28);
            this.RTB_Output.Name = "RTB_Output";
            this.RTB_Output.ReadOnly = true;
            this.RTB_Output.Size = new System.Drawing.Size(450, 301);
            this.RTB_Output.TabIndex = 12;
            this.RTB_Output.Text = "Open a file, or Drag/Drop several! Click this box for more options.\n";
            this.RTB_Output.Click += new System.EventHandler(this.RTB_Output_Click);
            // 
            // TB_FilePath
            // 
            this.TB_FilePath.Location = new System.Drawing.Point(113, 4);
            this.TB_FilePath.Name = "TB_FilePath";
            this.TB_FilePath.ReadOnly = true;
            this.TB_FilePath.Size = new System.Drawing.Size(344, 20);
            this.TB_FilePath.TabIndex = 11;
            this.TB_FilePath.TextChanged += new System.EventHandler(this.TB_FilePath_TextChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripDropDownButton2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(469, 25);
            this.toolStrip1.TabIndex = 15;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.B_Open,
            this.B_Go,
            this.B_Help});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(38, 22);
            this.toolStripDropDownButton1.Text = "File";
            // 
            // B_Open
            // 
            this.B_Open.Name = "B_Open";
            this.B_Open.Size = new System.Drawing.Size(103, 22);
            this.B_Open.Text = "Open";
            this.B_Open.Click += new System.EventHandler(this.B_Open_Click);
            // 
            // B_Go
            // 
            this.B_Go.Name = "B_Go";
            this.B_Go.Size = new System.Drawing.Size(103, 22);
            this.B_Go.Text = "Go";
            this.B_Go.Click += new System.EventHandler(this.B_Go_Click);
            // 
            // B_Help
            // 
            this.B_Help.Name = "B_Help";
            this.B_Help.Size = new System.Drawing.Size(103, 22);
            this.B_Help.Text = "Help";
            this.B_Help.Click += new System.EventHandler(this.B_Help_Click);
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.B_AutoExtract,
            this.B_BuildTexture,
            this.B_ArcPadding,
            this.aRCFileAlignmentToolStripMenuItem,
            this.B_RubyScript});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(62, 22);
            this.toolStripDropDownButton2.Text = "Options";
            // 
            // B_AutoExtract
            // 
            this.B_AutoExtract.Checked = true;
            this.B_AutoExtract.CheckOnClick = true;
            this.B_AutoExtract.CheckState = System.Windows.Forms.CheckState.Checked;
            this.B_AutoExtract.Name = "B_AutoExtract";
            this.B_AutoExtract.Size = new System.Drawing.Size(180, 22);
            this.B_AutoExtract.Text = "Auto Extract";
            // 
            // B_BuildTexture
            // 
            this.B_BuildTexture.CheckOnClick = true;
            this.B_BuildTexture.Name = "B_BuildTexture";
            this.B_BuildTexture.Size = new System.Drawing.Size(180, 22);
            this.B_BuildTexture.Text = "Build Textures";
            // 
            // B_ArcPadding
            // 
            this.B_ArcPadding.Checked = true;
            this.B_ArcPadding.CheckOnClick = true;
            this.B_ArcPadding.CheckState = System.Windows.Forms.CheckState.Checked;
            this.B_ArcPadding.Name = "B_ArcPadding";
            this.B_ArcPadding.Size = new System.Drawing.Size(180, 22);
            this.B_ArcPadding.Text = "ARC Padding";
            // 
            // aRCFileAlignmentToolStripMenuItem
            // 
            this.aRCFileAlignmentToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.B_Align0,
            this.B_Align16,
            this.B_Align32,
            this.B_Align64,
            this.B_Align128});
            this.aRCFileAlignmentToolStripMenuItem.Name = "aRCFileAlignmentToolStripMenuItem";
            this.aRCFileAlignmentToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aRCFileAlignmentToolStripMenuItem.Text = "ARC File Alignment";
            // 
            // B_Align0
            // 
            this.B_Align0.CheckOnClick = true;
            this.B_Align0.Name = "B_Align0";
            this.B_Align0.Size = new System.Drawing.Size(123, 22);
            this.B_Align0.Text = "0 bytes";
            this.B_Align0.Click += new System.EventHandler(this.B_Align0_Click);
            // 
            // B_Align16
            // 
            this.B_Align16.CheckOnClick = true;
            this.B_Align16.Name = "B_Align16";
            this.B_Align16.Size = new System.Drawing.Size(123, 22);
            this.B_Align16.Text = "16 bytes";
            this.B_Align16.Click += new System.EventHandler(this.B_Align16_Click);
            // 
            // B_Align32
            // 
            this.B_Align32.CheckOnClick = true;
            this.B_Align32.Name = "B_Align32";
            this.B_Align32.Size = new System.Drawing.Size(123, 22);
            this.B_Align32.Text = "32 bytes";
            this.B_Align32.Click += new System.EventHandler(this.B_Align32_Click);
            // 
            // B_Align64
            // 
            this.B_Align64.CheckOnClick = true;
            this.B_Align64.Name = "B_Align64";
            this.B_Align64.Size = new System.Drawing.Size(123, 22);
            this.B_Align64.Text = "64 bytes";
            this.B_Align64.Click += new System.EventHandler(this.B_Align64_Click);
            // 
            // B_Align128
            // 
            this.B_Align128.Checked = true;
            this.B_Align128.CheckOnClick = true;
            this.B_Align128.CheckState = System.Windows.Forms.CheckState.Checked;
            this.B_Align128.Name = "B_Align128";
            this.B_Align128.Size = new System.Drawing.Size(123, 22);
            this.B_Align128.Text = "128 bytes";
            this.B_Align128.Click += new System.EventHandler(this.B_Align128_Click);
            // 
            // B_RubyScript
            // 
            this.B_RubyScript.CheckOnClick = true;
            this.B_RubyScript.Name = "B_RubyScript";
            this.B_RubyScript.Size = new System.Drawing.Size(180, 22);
            this.B_RubyScript.Text = "Enable Ruby Script";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 341);
            this.Controls.Add(this.RTB_Output);
            this.Controls.Add(this.TB_FilePath);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(485, 380);
            this.MinimumSize = new System.Drawing.Size(485, 380);
            this.Name = "Form1";
            this.Text = "Fire Emblem Archive Tool";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox RTB_Output;
        private System.Windows.Forms.TextBox TB_FilePath;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem B_Open;
        private System.Windows.Forms.ToolStripMenuItem B_Go;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem B_AutoExtract;
        private System.Windows.Forms.ToolStripMenuItem B_BuildTexture;
        private System.Windows.Forms.ToolStripMenuItem B_Help;
        private System.Windows.Forms.ToolStripMenuItem B_ArcPadding;
        private System.Windows.Forms.ToolStripMenuItem aRCFileAlignmentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem B_Align0;
        private System.Windows.Forms.ToolStripMenuItem B_Align16;
        private System.Windows.Forms.ToolStripMenuItem B_Align32;
        private System.Windows.Forms.ToolStripMenuItem B_Align64;
        private System.Windows.Forms.ToolStripMenuItem B_Align128;
        private System.Windows.Forms.ToolStripMenuItem B_RubyScript;
    }
}

