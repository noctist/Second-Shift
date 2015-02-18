namespace SecondShiftMobile
{
    partial class CutsceneBuilder
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
            this.propertyPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.saveButton = new System.Windows.Forms.Button();
            this.saveBox = new System.Windows.Forms.TextBox();
            this.actionList = new System.Windows.Forms.ListBox();
            this.loadButton = new System.Windows.Forms.Button();
            this.actionComboBox = new System.Windows.Forms.ComboBox();
            this.addButton = new System.Windows.Forms.Button();
            this.testButton = new System.Windows.Forms.Button();
            this.insertButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.upButton = new System.Windows.Forms.Button();
            this.downButton = new System.Windows.Forms.Button();
            this.loadBox = new System.Windows.Forms.ComboBox();
            this.classBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // propertyPanel
            // 
            this.propertyPanel.AutoScroll = true;
            this.propertyPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.propertyPanel.Location = new System.Drawing.Point(563, 61);
            this.propertyPanel.Margin = new System.Windows.Forms.Padding(4);
            this.propertyPanel.Name = "propertyPanel";
            this.propertyPanel.Size = new System.Drawing.Size(487, 497);
            this.propertyPanel.TabIndex = 0;
            this.propertyPanel.WrapContents = false;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(866, 566);
            this.saveButton.Margin = new System.Windows.Forms.Padding(4);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(88, 26);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // saveBox
            // 
            this.saveBox.Location = new System.Drawing.Point(535, 566);
            this.saveBox.Margin = new System.Windows.Forms.Padding(4);
            this.saveBox.Name = "saveBox";
            this.saveBox.Size = new System.Drawing.Size(323, 23);
            this.saveBox.TabIndex = 3;
            // 
            // actionList
            // 
            this.actionList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.actionList.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.actionList.FormattingEnabled = true;
            this.actionList.ItemHeight = 17;
            this.actionList.Location = new System.Drawing.Point(15, 61);
            this.actionList.Margin = new System.Windows.Forms.Padding(4);
            this.actionList.Name = "actionList";
            this.actionList.Size = new System.Drawing.Size(453, 478);
            this.actionList.TabIndex = 4;
            this.actionList.TabStop = false;
            this.actionList.UseTabStops = false;
            this.actionList.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.actionList_MeasureItem);
            this.actionList.SelectedIndexChanged += new System.EventHandler(this.actionList_SelectedValueChanged);
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(143, 566);
            this.loadButton.Margin = new System.Windows.Forms.Padding(4);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(88, 26);
            this.loadButton.TabIndex = 2;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // actionComboBox
            // 
            this.actionComboBox.FormattingEnabled = true;
            this.actionComboBox.Location = new System.Drawing.Point(15, 28);
            this.actionComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.actionComboBox.Name = "actionComboBox";
            this.actionComboBox.Size = new System.Drawing.Size(140, 23);
            this.actionComboBox.TabIndex = 5;
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(164, 28);
            this.addButton.Margin = new System.Windows.Forms.Padding(4);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(88, 26);
            this.addButton.TabIndex = 6;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // testButton
            // 
            this.testButton.Location = new System.Drawing.Point(961, 566);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(75, 26);
            this.testButton.TabIndex = 7;
            this.testButton.Text = "Test";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // insertButton
            // 
            this.insertButton.Location = new System.Drawing.Point(259, 28);
            this.insertButton.Name = "insertButton";
            this.insertButton.Size = new System.Drawing.Size(75, 26);
            this.insertButton.TabIndex = 8;
            this.insertButton.Text = "Insert";
            this.insertButton.UseVisualStyleBackColor = true;
            this.insertButton.Click += new System.EventHandler(this.insertButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(393, 28);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 26);
            this.deleteButton.TabIndex = 9;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // upButton
            // 
            this.upButton.Location = new System.Drawing.Point(475, 271);
            this.upButton.Name = "upButton";
            this.upButton.Size = new System.Drawing.Size(52, 23);
            this.upButton.TabIndex = 10;
            this.upButton.Text = "up";
            this.upButton.UseVisualStyleBackColor = true;
            this.upButton.Click += new System.EventHandler(this.upButton_Click);
            // 
            // downButton
            // 
            this.downButton.Location = new System.Drawing.Point(475, 301);
            this.downButton.Name = "downButton";
            this.downButton.Size = new System.Drawing.Size(52, 23);
            this.downButton.TabIndex = 11;
            this.downButton.Text = "down";
            this.downButton.UseVisualStyleBackColor = true;
            this.downButton.Click += new System.EventHandler(this.downButton_Click);
            // 
            // loadBox
            // 
            this.loadBox.FormattingEnabled = true;
            this.loadBox.Location = new System.Drawing.Point(15, 567);
            this.loadBox.Name = "loadBox";
            this.loadBox.Size = new System.Drawing.Size(121, 23);
            this.loadBox.TabIndex = 12;
            // 
            // classBox
            // 
            this.classBox.FormattingEnabled = true;
            this.classBox.Location = new System.Drawing.Point(563, 32);
            this.classBox.Name = "classBox";
            this.classBox.Size = new System.Drawing.Size(473, 23);
            this.classBox.TabIndex = 13;
            this.classBox.SelectedIndexChanged += new System.EventHandler(this.classBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(563, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 15);
            this.label1.TabIndex = 14;
            this.label1.Text = "Class";
            // 
            // CutsceneBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1064, 613);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.classBox);
            this.Controls.Add(this.loadBox);
            this.Controls.Add(this.downButton);
            this.Controls.Add(this.upButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.insertButton);
            this.Controls.Add(this.testButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.actionComboBox);
            this.Controls.Add(this.actionList);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.saveBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.propertyPanel);
            this.Font = new System.Drawing.Font("Segoe UI Semilight", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CutsceneBuilder";
            this.Text = "CutsceneBuilder";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel propertyPanel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TextBox saveBox;
        private System.Windows.Forms.ListBox actionList;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.ComboBox actionComboBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button testButton;
        private System.Windows.Forms.Button insertButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button upButton;
        private System.Windows.Forms.Button downButton;
        private System.Windows.Forms.ComboBox loadBox;
        private System.Windows.Forms.ComboBox classBox;
        private System.Windows.Forms.Label label1;
    }
}