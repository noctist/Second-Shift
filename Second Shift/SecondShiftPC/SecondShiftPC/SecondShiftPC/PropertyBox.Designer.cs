namespace SecondShiftMobile
{
    partial class PropertyBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.name = new System.Windows.Forms.Label();
            this.valueBox = new System.Windows.Forms.TextBox();
            this.info = new System.Windows.Forms.Label();
            this.applyButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // name
            // 
            this.name.AutoSize = true;
            this.name.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.name.Location = new System.Drawing.Point(0, 0);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(58, 24);
            this.name.TabIndex = 0;
            this.name.Text = "name";
            // 
            // valueBox
            // 
            this.valueBox.Location = new System.Drawing.Point(4, 21);
            this.valueBox.Name = "valueBox";
            this.valueBox.Size = new System.Drawing.Size(171, 20);
            this.valueBox.TabIndex = 1;
            this.valueBox.TextChanged += new System.EventHandler(this.valueBox_TextChanged);
            this.valueBox.Enter += new System.EventHandler(this.valueBox_Enter);
            this.valueBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.valueBox_KeyDown);
            // 
            // info
            // 
            this.info.AutoSize = true;
            this.info.Location = new System.Drawing.Point(4, 44);
            this.info.Name = "info";
            this.info.Size = new System.Drawing.Size(24, 13);
            this.info.TabIndex = 2;
            this.info.Text = "info";
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(181, 21);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(43, 23);
            this.applyButton.TabIndex = 3;
            this.applyButton.Text = "apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // PropertyBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.info);
            this.Controls.Add(this.valueBox);
            this.Controls.Add(this.name);
            this.Name = "PropertyBox";
            this.Size = new System.Drawing.Size(227, 60);
            this.Load += new System.EventHandler(this.PropertyBox_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label name;
        private System.Windows.Forms.TextBox valueBox;
        private System.Windows.Forms.Label info;
        private System.Windows.Forms.Button applyButton;
    }
}
