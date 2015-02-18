using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SecondShiftMobile
{
    public partial class PropertyBox : UserControl
    {
        StageObjectProperty prop;
        StageObjectData dat;
        public PropertyBox()
        {
            InitializeComponent();
            name.Text = info.Text = "null";
            valueBox.Enabled = false;
            applyButton.Enabled = false;
        }

        private void PropertyBox_Load(object sender, EventArgs e)
        {

        }
        public void SetData(StageObjectData stageObjectData, StageObjectProperty stageObjectProperty)
        {
            prop = stageObjectProperty;
            dat = stageObjectData;
            if (prop != null)
            {
                name.Text = prop.Name;
                info.Text = prop.Info;
                valueBox.Enabled = true;
                valueBox.Text = prop.XML.Value;
                applyButton.Enabled = false;
            }
            else
            {
                name.Text = info.Text = "null";
                valueBox.Enabled = false;
                applyButton.Enabled = false;
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            Apply();
        }

        private void Apply()
        {
            if (prop != null && dat != null && dat.Obj != null)
            {
                prop.SetValue(valueBox.Text);
                dat.Obj.SetStageProperties(prop);
                LevelBuilder.SetProperties(dat, dat.Obj);
                foreach (var p in dat.Properties)
                {
                    if (p.Name == prop.Name)
                        prop = p;
                }
                SetData(dat, prop);
            }
        }

        private void valueBox_TextChanged(object sender, EventArgs e)
        {
            applyButton.Enabled = true;
        }

        private void valueBox_Enter(object sender, EventArgs e)
        {
            Apply();
        }

        private void valueBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                Apply();
        }
    }
}
