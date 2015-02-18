namespace SecondShiftMobile
{
    partial class ColorControl
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
            this.rBar = new System.Windows.Forms.TrackBar();
            this.gBar = new System.Windows.Forms.TrackBar();
            this.bBar = new System.Windows.Forms.TrackBar();
            this.aBar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.rText = new System.Windows.Forms.Label();
            this.gText = new System.Windows.Forms.Label();
            this.bText = new System.Windows.Forms.Label();
            this.aText = new System.Windows.Forms.Label();
            this.colorLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.rBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aBar)).BeginInit();
            this.SuspendLayout();
            // 
            // rBar
            // 
            this.rBar.LargeChange = 1;
            this.rBar.Location = new System.Drawing.Point(3, 0);
            this.rBar.Maximum = 255;
            this.rBar.Name = "rBar";
            this.rBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.rBar.Size = new System.Drawing.Size(45, 237);
            this.rBar.TabIndex = 0;
            this.rBar.ValueChanged += new System.EventHandler(this.colorChanged);
            // 
            // gBar
            // 
            this.gBar.LargeChange = 1;
            this.gBar.Location = new System.Drawing.Point(54, 0);
            this.gBar.Maximum = 255;
            this.gBar.Name = "gBar";
            this.gBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.gBar.Size = new System.Drawing.Size(45, 237);
            this.gBar.TabIndex = 0;
            this.gBar.ValueChanged += new System.EventHandler(this.colorChanged);
            // 
            // bBar
            // 
            this.bBar.LargeChange = 1;
            this.bBar.Location = new System.Drawing.Point(105, 0);
            this.bBar.Maximum = 255;
            this.bBar.Name = "bBar";
            this.bBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.bBar.Size = new System.Drawing.Size(45, 237);
            this.bBar.TabIndex = 0;
            this.bBar.ValueChanged += new System.EventHandler(this.colorChanged);
            // 
            // aBar
            // 
            this.aBar.LargeChange = 1;
            this.aBar.Location = new System.Drawing.Point(156, 0);
            this.aBar.Maximum = 255;
            this.aBar.Name = "aBar";
            this.aBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.aBar.Size = new System.Drawing.Size(45, 237);
            this.aBar.TabIndex = 0;
            this.aBar.ValueChanged += new System.EventHandler(this.colorChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 240);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Red";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 240);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Green";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(102, 240);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Blue";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(153, 240);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Alpha";
            // 
            // rText
            // 
            this.rText.AutoSize = true;
            this.rText.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.rText.Location = new System.Drawing.Point(3, 257);
            this.rText.Name = "rText";
            this.rText.Size = new System.Drawing.Size(13, 13);
            this.rText.TabIndex = 2;
            this.rText.Text = "0";
            // 
            // gText
            // 
            this.gText.AutoSize = true;
            this.gText.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.gText.Location = new System.Drawing.Point(51, 257);
            this.gText.Name = "gText";
            this.gText.Size = new System.Drawing.Size(13, 13);
            this.gText.TabIndex = 2;
            this.gText.Text = "0";
            // 
            // bText
            // 
            this.bText.AutoSize = true;
            this.bText.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.bText.Location = new System.Drawing.Point(102, 257);
            this.bText.Name = "bText";
            this.bText.Size = new System.Drawing.Size(13, 13);
            this.bText.TabIndex = 2;
            this.bText.Text = "0";
            // 
            // aText
            // 
            this.aText.AutoSize = true;
            this.aText.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.aText.Location = new System.Drawing.Point(153, 257);
            this.aText.Name = "aText";
            this.aText.Size = new System.Drawing.Size(13, 13);
            this.aText.TabIndex = 2;
            this.aText.Text = "0";
            // 
            // colorLabel
            // 
            this.colorLabel.AutoSize = true;
            this.colorLabel.BackColor = System.Drawing.Color.Black;
            this.colorLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colorLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.colorLabel.Location = new System.Drawing.Point(4, 274);
            this.colorLabel.Name = "colorLabel";
            this.colorLabel.Size = new System.Drawing.Size(48, 21);
            this.colorLabel.TabIndex = 3;
            this.colorLabel.Text = "color";
            // 
            // ColorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.colorLabel);
            this.Controls.Add(this.aText);
            this.Controls.Add(this.bText);
            this.Controls.Add(this.gText);
            this.Controls.Add(this.rText);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.aBar);
            this.Controls.Add(this.bBar);
            this.Controls.Add(this.gBar);
            this.Controls.Add(this.rBar);
            this.Name = "ColorControl";
            this.Size = new System.Drawing.Size(197, 302);
            ((System.ComponentModel.ISupportInitialize)(this.rBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar rBar;
        private System.Windows.Forms.TrackBar gBar;
        private System.Windows.Forms.TrackBar bBar;
        private System.Windows.Forms.TrackBar aBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label rText;
        private System.Windows.Forms.Label gText;
        private System.Windows.Forms.Label bText;
        private System.Windows.Forms.Label aText;
        private System.Windows.Forms.Label colorLabel;
    }
}
