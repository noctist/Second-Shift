using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SecondShiftMobile
{
    public partial class BuildForm : Form
    {
        bool changeCamera = false, changeName = false;
        public BuildForm()
        {
            InitializeComponent();
            var l = LevelBuilder.Types;
            foreach (var t in l)
            {
                var ss = t.ToString().Split('.');
                typeBox.Items.Add(ss.Last());
            }
            editBox.Checked = LevelBuilder.EditMode;
            this.Shown += new EventHandler(BuildForm_Shown);
            this.GotFocus += new EventHandler(BuildForm_GotFocus);
            this.Activated += new EventHandler(BuildForm_Activated);
            ContrastColor.SwitchToVector4(0, 2);
        }
        public void SetSelectedObjProperties(StageObjectData sd)
        {
            changeCamera = changeName = false;
            nameBox.Text = sd.Name;
            cameraBox.Checked = sd.CameraTarget;
            changeCamera = changeName = true;
            var sops = sd.Properties;
            layoutPanel.Controls.Clear();
            for (int i = 0; i < sops.Length; i++)
            {
                var pBox = new PropertyBox();
                pBox.SetData(sd, sops[i]);
                layoutPanel.Controls.Add(pBox);
            }
        }
        public void ChangedChanged(bool val)
        {
            saveButton.Enabled = true;
        }
        public void EditModeChanged()
        {
            editBox.Checked = LevelBuilder.EditMode;
        }
        void BuildForm_Activated(object sender, EventArgs e)
        {
            saveButton.Enabled = LevelBuilder.Changed;
            editBox.Checked = LevelBuilder.EditMode;
            objPanel.Visible = editBox.Checked;
        }

        void BuildForm_GotFocus(object sender, EventArgs e)
        {
            saveButton.Enabled = LevelBuilder.Changed;
        }

        void BuildForm_Shown(object sender, EventArgs e)
        {
            saveNameBox.Text = LevelBuilder.LastName;
            saveButton.Enabled = LevelBuilder.Changed;
        }

        private void typeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelBuilder.ChangeIndex(typeBox.SelectedIndex);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (saveNameBox.Text.Length > 0)
            {
                LevelBuilder.Save(saveNameBox.Text);
            }
            else
            {
                MessageBox.Show("Need to enter name");
            }
            saveButton.Enabled = LevelBuilder.Changed;
        }

        private void editBox_CheckedChanged(object sender, EventArgs e)
        {
            LevelBuilder.EditMode = editBox.Checked;
            objPanel.Visible = editBox.Checked;
        }

        private void cameraBox_CheckedChanged(object sender, EventArgs e)
        {
            if (LevelBuilder.SelectedObj != null && changeCamera)
            {
                LevelBuilder.SetCameraTarget(LevelBuilder.SelectedObj, cameraBox.Checked);
                saveButton.Enabled = LevelBuilder.Changed;
            }
        }

        private void nameBox_TextChanged(object sender, EventArgs e)
        {
            if (LevelBuilder.SelectedObj != null && changeName)
            {
                LevelBuilder.SetName(LevelBuilder.SelectedObj, nameBox.Text);
                saveButton.Enabled = LevelBuilder.Changed;
            }
        }

        private void SkyColorChanged(object sender, ColorChangedEventArgs e)
        {
            LevelBuilder.SetSkyColor(e.Color);
        }

        public void SetSkyColor(Microsoft.Xna.Framework.Color c)
        {
            skyColor.SetColor(c);
        }

        public void SetContrast(Microsoft.Xna.Framework.Vector4 v)
        {
            ContrastColor.SetVector4(v);
        }

        private void contrastChanged(object sender, Vector4ChangedEventArgs e)
        {
            LevelBuilder.SetContrast(e.Value);
        }
        
    }
}
