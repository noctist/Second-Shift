using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecondShiftMobile
{
    public partial class TextForm : Form
    {
        TaskCompletionSource<string> tcs;
        public Task<string> Task { get { return tcs.Task;}}
        public TextForm()
        {
            tcs = new TaskCompletionSource<string>();
            InitializeComponent();
            FormClosed += TextForm_FormClosed;
        }

        void TextForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            tcs.SetResult(textBox.Text);
            Dispose();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox_Enter(object sender, EventArgs e)
        {
            //if (!string.IsNullOrWhiteSpace(textBox.Text))
                //Close();
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrWhiteSpace(textBox.Text))
                    Close();
            }
        }
    }
}
