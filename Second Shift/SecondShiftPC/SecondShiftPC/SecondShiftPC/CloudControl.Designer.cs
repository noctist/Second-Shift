namespace SecondShiftMobile
{
    partial class CloudControl
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
            this.numBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.skyBox0 = new System.Windows.Forms.TextBox();
            this.skyBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.scaleBox0 = new System.Windows.Forms.TextBox();
            this.scaleBox1 = new System.Windows.Forms.TextBox();
            this.createButton = new System.Windows.Forms.Button();
            this.Clouds = new System.Windows.Forms.Label();
            this.alphaBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cloudColor = new SecondShiftMobile.ColorControl();
            this.SuspendLayout();
            // 
            // numBox
            // 
            this.numBox.Location = new System.Drawing.Point(3, 53);
            this.numBox.Name = "numBox";
            this.numBox.Size = new System.Drawing.Size(34, 20);
            this.numBox.TabIndex = 0;
            this.numBox.Text = "300";
            this.numBox.TextChanged += new System.EventHandler(this.numBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Number";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(55, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Sky Box Range";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // skyBox0
            // 
            this.skyBox0.Location = new System.Drawing.Point(58, 54);
            this.skyBox0.Name = "skyBox0";
            this.skyBox0.Size = new System.Drawing.Size(123, 20);
            this.skyBox0.TabIndex = 3;
            this.skyBox0.Text = "-100000, -15000, 0";
            this.skyBox0.TextChanged += new System.EventHandler(this.skyBox0_TextChanged);
            // 
            // skyBox1
            // 
            this.skyBox1.Location = new System.Drawing.Point(58, 81);
            this.skyBox1.Name = "skyBox1";
            this.skyBox1.Size = new System.Drawing.Size(123, 20);
            this.skyBox1.TabIndex = 4;
            this.skyBox1.Text = "100000, -30000, 50000";
            this.skyBox1.TextChanged += new System.EventHandler(this.skyBox1_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(188, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Scale";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // scaleBox0
            // 
            this.scaleBox0.Location = new System.Drawing.Point(188, 53);
            this.scaleBox0.Name = "scaleBox0";
            this.scaleBox0.Size = new System.Drawing.Size(45, 20);
            this.scaleBox0.TabIndex = 6;
            this.scaleBox0.Text = "200";
            this.scaleBox0.TextChanged += new System.EventHandler(this.scaleBox0_TextChanged);
            // 
            // scaleBox1
            // 
            this.scaleBox1.Location = new System.Drawing.Point(188, 81);
            this.scaleBox1.Name = "scaleBox1";
            this.scaleBox1.Size = new System.Drawing.Size(45, 20);
            this.scaleBox1.TabIndex = 7;
            this.scaleBox1.Text = "300";
            this.scaleBox1.TextChanged += new System.EventHandler(this.scaleBox1_TextChanged);
            // 
            // createButton
            // 
            this.createButton.Location = new System.Drawing.Point(238, 200);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(75, 85);
            this.createButton.TabIndex = 8;
            this.createButton.Text = "Create";
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler(this.createButton_Click);
            // 
            // Clouds
            // 
            this.Clouds.AutoSize = true;
            this.Clouds.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Clouds.Location = new System.Drawing.Point(2, 0);
            this.Clouds.Name = "Clouds";
            this.Clouds.Size = new System.Drawing.Size(70, 25);
            this.Clouds.TabIndex = 9;
            this.Clouds.Text = "Clouds";
            // 
            // alphaBox
            // 
            this.alphaBox.Location = new System.Drawing.Point(261, 53);
            this.alphaBox.Name = "alphaBox";
            this.alphaBox.Size = new System.Drawing.Size(52, 20);
            this.alphaBox.TabIndex = 10;
            this.alphaBox.Text = "0.5";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(261, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Alpha";
            // 
            // cloudColor
            // 
            this.cloudColor.Location = new System.Drawing.Point(3, 119);
            this.cloudColor.Name = "cloudColor";
            this.cloudColor.Size = new System.Drawing.Size(197, 292);
            this.cloudColor.TabIndex = 12;
            this.cloudColor.ColorChanged += new System.EventHandler<SecondShiftMobile.ColorChangedEventArgs>(this.colorChanged);
            // 
            // CloudControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cloudColor);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.alphaBox);
            this.Controls.Add(this.Clouds);
            this.Controls.Add(this.createButton);
            this.Controls.Add(this.scaleBox1);
            this.Controls.Add(this.scaleBox0);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.skyBox1);
            this.Controls.Add(this.skyBox0);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numBox);
            this.Name = "CloudControl";
            this.Size = new System.Drawing.Size(339, 427);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox numBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox skyBox0;
        private System.Windows.Forms.TextBox skyBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox scaleBox0;
        private System.Windows.Forms.TextBox scaleBox1;
        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.Label Clouds;
        private System.Windows.Forms.TextBox alphaBox;
        private System.Windows.Forms.Label label4;
        private ColorControl cloudColor;
    }
}
