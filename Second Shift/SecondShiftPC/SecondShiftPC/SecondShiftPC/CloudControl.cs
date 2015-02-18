using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SecondShiftMobile
{
    public partial class CloudControl : UserControl
    {
        Color col = Color.White;
        public CloudControl()
        {
            InitializeComponent();
            cloudColor.SetColor(Color.White);
        }
        void Create()
        {
            int numberOfClouds = 300;
            int.TryParse(numBox.Text, out numberOfClouds);
            string[] box1 = skyBox0.Text.Split(',');
            string[] box2 = skyBox1.Text.Split(',');
            float left = -10000, right = 10000, top = -30000, bottom = -15000, front = 0, back = 50000, minScale = 200, maxScale = 300, alpha = 0;
            float.TryParse(scaleBox0.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out minScale);
            float.TryParse(scaleBox1.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out maxScale);
            float.TryParse(alphaBox.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out alpha);
            for (int i = 0; i < 3; i++)
            {
                if (i < box1.Length)
                {
                    if (i == 0)
                    {
                        float.TryParse(box1[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out left);
                    }
                    else if (i == 1)
                    {
                        float.TryParse(box1[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out top);
                    }
                    else if (i == 2)
                    {
                        float.TryParse(box1[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out front);
                    }
                }
                if (i < box2.Length)
                {
                    if (i == 0)
                    {
                        float.TryParse(box2[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out right);
                    }
                    else if (i == 1)
                    {
                        float.TryParse(box2[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out bottom);
                    }
                    else if (i == 2)
                    {
                        float.TryParse(box2[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out back);
                    }
                }
            }
            LevelBuilder.CreateClouds(numberOfClouds, left, right, top, bottom, front, back, minScale, maxScale, alpha, col);

        }
        private void createButton_Click(object sender, EventArgs e)
        {
            Create();
        }

        private void scaleBox0_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void skyBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void skyBox0_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void numBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void scaleBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void colorChanged(object sender, ColorChangedEventArgs e)
        {
            col = e.Color;
        }
    }
}
