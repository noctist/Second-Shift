namespace SecondShiftMobile
{
    partial class BuildForm
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
            this.typeBox = new System.Windows.Forms.ListBox();
            this.editBox = new System.Windows.Forms.CheckBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.saveNameBox = new System.Windows.Forms.TextBox();
            this.cameraBox = new System.Windows.Forms.CheckBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.objPanel = new System.Windows.Forms.Panel();
            this.layoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.nameBox = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.cloudControl1 = new SecondShiftMobile.CloudControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.skyColor = new SecondShiftMobile.ColorControl();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ContrastColor = new SecondShiftMobile.ColorControl();
            this.objPanel.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // typeBox
            // 
            this.typeBox.FormattingEnabled = true;
            this.typeBox.Location = new System.Drawing.Point(3, 0);
            this.typeBox.Name = "typeBox";
            this.typeBox.Size = new System.Drawing.Size(120, 420);
            this.typeBox.TabIndex = 1;
            this.typeBox.SelectedIndexChanged += new System.EventHandler(this.typeBox_SelectedIndexChanged);
            // 
            // editBox
            // 
            this.editBox.AutoSize = true;
            this.editBox.Location = new System.Drawing.Point(13, 1);
            this.editBox.Name = "editBox";
            this.editBox.Size = new System.Drawing.Size(74, 17);
            this.editBox.TabIndex = 2;
            this.editBox.Text = "Edit Mode";
            this.editBox.UseVisualStyleBackColor = true;
            this.editBox.CheckedChanged += new System.EventHandler(this.editBox_CheckedChanged);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(402, 565);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 3;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // saveNameBox
            // 
            this.saveNameBox.Location = new System.Drawing.Point(129, 567);
            this.saveNameBox.Name = "saveNameBox";
            this.saveNameBox.Size = new System.Drawing.Size(267, 20);
            this.saveNameBox.TabIndex = 4;
            // 
            // cameraBox
            // 
            this.cameraBox.AutoSize = true;
            this.cameraBox.Location = new System.Drawing.Point(6, 3);
            this.cameraBox.Name = "cameraBox";
            this.cameraBox.Size = new System.Drawing.Size(96, 17);
            this.cameraBox.TabIndex = 5;
            this.cameraBox.Text = "Camera Target";
            this.cameraBox.UseVisualStyleBackColor = true;
            this.cameraBox.CheckedChanged += new System.EventHandler(this.cameraBox_CheckedChanged);
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(3, 23);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(38, 13);
            this.nameLabel.TabIndex = 6;
            this.nameLabel.Text = "Name:";
            // 
            // objPanel
            // 
            this.objPanel.Controls.Add(this.layoutPanel);
            this.objPanel.Controls.Add(this.nameBox);
            this.objPanel.Controls.Add(this.nameLabel);
            this.objPanel.Controls.Add(this.cameraBox);
            this.objPanel.Location = new System.Drawing.Point(122, 0);
            this.objPanel.Name = "objPanel";
            this.objPanel.Size = new System.Drawing.Size(338, 407);
            this.objPanel.TabIndex = 7;
            // 
            // layoutPanel
            // 
            this.layoutPanel.AutoScroll = true;
            this.layoutPanel.Location = new System.Drawing.Point(4, 67);
            this.layoutPanel.Name = "layoutPanel";
            this.layoutPanel.Size = new System.Drawing.Size(331, 353);
            this.layoutPanel.TabIndex = 8;
            // 
            // nameBox
            // 
            this.nameBox.Location = new System.Drawing.Point(4, 40);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(195, 20);
            this.nameBox.TabIndex = 7;
            this.nameBox.TextChanged += new System.EventHandler(this.nameBox_TextChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(548, 535);
            this.tabControl1.TabIndex = 9;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.typeBox);
            this.tabPage1.Controls.Add(this.objPanel);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(540, 509);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Objects";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.cloudControl1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(540, 509);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Clouds";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // cloudControl1
            // 
            this.cloudControl1.Location = new System.Drawing.Point(6, 6);
            this.cloudControl1.Name = "cloudControl1";
            this.cloudControl1.Size = new System.Drawing.Size(341, 401);
            this.cloudControl1.TabIndex = 8;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.skyColor);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.label3);
            this.tabPage3.Controls.Add(this.ContrastColor);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(540, 509);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Display";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // skyColor
            // 
            this.skyColor.Location = new System.Drawing.Point(3, 35);
            this.skyColor.Name = "skyColor";
            this.skyColor.Size = new System.Drawing.Size(243, 292);
            this.skyColor.TabIndex = 10;
            this.skyColor.ColorChanged += new System.EventHandler<SecondShiftMobile.ColorChangedEventArgs>(this.SkyColorChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Sky Color";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(338, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Contrast";
            // 
            // ContrastColor
            // 
            this.ContrastColor.Location = new System.Drawing.Point(338, 35);
            this.ContrastColor.Name = "ContrastColor";
            this.ContrastColor.Size = new System.Drawing.Size(197, 292);
            this.ContrastColor.TabIndex = 0;
            this.ContrastColor.Vector4Changed += new System.EventHandler<SecondShiftMobile.Vector4ChangedEventArgs>(this.contrastChanged);
            // 
            // BuildForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 616);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.saveNameBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.editBox);
            this.Name = "BuildForm";
            this.Text = "BuildForm";
            this.objPanel.ResumeLayout(false);
            this.objPanel.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox typeBox;
        private System.Windows.Forms.CheckBox editBox;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TextBox saveNameBox;
        private System.Windows.Forms.CheckBox cameraBox;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Panel objPanel;
        private System.Windows.Forms.TextBox nameBox;
        private CloudControl cloudControl1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private ColorControl skyColor;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label3;
        private ColorControl ContrastColor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel layoutPanel;
    }
}