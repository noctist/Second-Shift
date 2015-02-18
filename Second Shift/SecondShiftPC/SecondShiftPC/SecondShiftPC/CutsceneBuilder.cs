using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SecondShiftMobile.Cutscenes;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace SecondShiftMobile
{
    public partial class CutsceneBuilder : Form
    {
        
        Cutscene cutscene;
        CutsceneAction action;
        string[] fileNames;
        BindingList<string> FileNames;
        List<Type> cutsceneTypes;
        public CutsceneBuilder()
        {
            InitializeComponent();
            foreach (var v in CutsceneAction.ActionTypeConverter.Values)
            {
                actionComboBox.Items.Add(v);
            }
            /*var enums = Enum.GetNames(typeof(ActionType));
            foreach (var act in enums)
            {
                ActionType ty = (ActionType)Enum.Parse(typeof(ActionType), act);
                if (CutsceneAction.ActionTypeConverter.ContainsKey(ty))
                {
                    actionComboBox.Items.Add(CutsceneAction.ActionTypeConverter[ty]);
                }
            }*/
            if (Cutscene.LastLoadedName.Length > 0)
            {
                loadBox.Text = Cutscene.LastLoadedName;
                Load();
            }
            else SetCutscene(new Cutscene());
            FileNames = new BindingList<string>();
            GetFilesList();
            loadBox.DataSource = FileNames;
            var theList = Assembly.GetExecutingAssembly().GetTypes().ToList().Where(t => t.Namespace == "SecondShiftMobile.Cutscenes.Scenes");
            cutsceneTypes = theList.ToList();
            cutsceneTypes.Insert(0, typeof(Cutscene));
            classBox.DataSource = cutsceneTypes;
        }
        public void GetFilesList()
        {
            if (Directory.Exists("Content/Cutscenes"))
            {
                fileNames = Directory.GetFiles("Content/Cutscenes");
            }
            else
            {
                fileNames = new string[0];
            }
            FileNames.Clear();
            foreach (var name in fileNames)
            {
                var n = name.Split('/');
                string filename = n.Last();
                filename = filename.Split('\\').Last();
                filename = filename.Replace(".xml", "");
                FileNames.Add(filename);
            }
            
        }
        public void SetCutscene(Cutscene cut)
        {
            //actionList.Items.Clear();
#if DEBUG
            if (cutscene != null)
                cutscene.Actions.ListChanged -= Actions_ListChanged;
#endif
            cutscene = cut;
            foreach (var a in cut.Actions)
            {
                //actionList.Items.Add(a);
            }
            actionList.DataSource = cut.Actions;
            classBox.SelectedItem = cut.Class;
            //actionList.DataBindings.Add("Text", cut.Actions, "Time");
#if DEBUG
            cut.Actions.ListChanged += Actions_ListChanged;
#endif
        }

        void Actions_ListChanged(object sender, ListChangedEventArgs e)
        {
            
        }

        private void actionList_SelectedValueChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("Selected");
            if (actionList.SelectedItem != null && actionList.SelectedItem is CutsceneAction)
            {
                setAction((CutsceneAction)actionList.SelectedItem);
            }
            //actionList.Refresh();
        }
        void setAction(CutsceneAction ca)
        {
            if (actionList == null || action != ca)
            {
                action = ca;
                propertyPanel.Controls.Clear();
                var acs = ca.GetContracts();
                foreach (var ac in acs)
                {
                    addLabel(ac.Property);
                    if (ac.Type == typeof(string) || ac.Type == typeof(int) || ac.Type == typeof(float) || ac.Type == typeof(Vector3) || ac.Type == typeof(Vector2))
                    {
                        ContractTextBox ct = new ContractTextBox(ca, ac);
                        propertyPanel.Controls.Add(ct);
                    }
                    else if (ac.Type == typeof(bool))
                    {
                        propertyPanel.Controls.Add(new ContractCheckBox(ca, ac));
                    }
                    else if (ac.Type.IsEnum)
                    {
                        propertyPanel.Controls.Add(new ContractComboBox(ca, ac));
                    }
                }
            }
        }
        void addLabel(string Text)
        {
            propertyPanel.Controls.Add(new Label() { Text = Text, Margin = new Padding(-18, 30, 0, 0) });
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            if (loadBox.Text.Length > 0)
            {
                var c = Cutscene.Load(loadBox.Text);
                SetCutscene(c);
                saveBox.Text = loadBox.Text;
            }
        }

        public void Load()
        {
            if (loadBox.Text.Length > 0)
            {
                var c = Cutscene.Load(loadBox.Text);
                SetCutscene(c);
                saveBox.Text = loadBox.Text;
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (cutscene != null && actionComboBox.SelectedItem != null)
            {
                //ActionType ty = (ActionType)Enum.Parse(typeof(ActionType), (string)actionComboBox.SelectedItem);
                //CutsceneAction ca = (CutsceneAction)Activator.CreateInstance(CutsceneAction.ActionTypeConverter[ty]);
                CutsceneAction ca = (CutsceneAction)Activator.CreateInstance(((CutsceneAction.TypeAndName)actionComboBox.SelectedItem).Type);
                int maxTime = 0;
                for (int i = 0; i < cutscene.Actions.Count; i++)
                {
                    if (cutscene.Actions[i].Time > maxTime)
                    {
                        maxTime = cutscene.Actions[i].Time;
                    }
                }
                //ca.Time = maxTime;

                cutscene.Actions.Add(ca);
                actionList.SelectedItem = ca;
                //actionList.Refresh();
            }
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            if (cutscene != null)
            {
                Cutscene c = Cutscene.CreateCutscene(cutscene.GetXML());
                Global.Cutscene = c;
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (cutscene != null && saveBox.Text.Length > 0)
            {
                Cutscene.Save(cutscene, saveBox.Text);

            }
            GetFilesList();
        }

        private void insertButton_Click(object sender, EventArgs e)
        {
            if (cutscene != null && actionComboBox.SelectedItem != null)
            {
                //ActionType ty = (ActionType)Enum.Parse(typeof(ActionType), (string)actionComboBox.SelectedItem);
                //CutsceneAction ca = (CutsceneAction)Activator.CreateInstance(CutsceneAction.ActionTypeConverter[ty]);
                CutsceneAction ca = (CutsceneAction)Activator.CreateInstance(((CutsceneAction.TypeAndName)actionComboBox.SelectedItem).Type);
                if (actionList.SelectedIndex >= 0)
                {
                    cutscene.Actions.Insert(actionList.SelectedIndex + 1, ca);
                    actionList.SelectedItem = ca;
                }
                else
                {
                    cutscene.Actions.Insert(0, ca);
                    actionList.SelectedItem = ca;
                }
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (cutscene != null && actionList.SelectedIndex >= 0)
            {
                cutscene.Actions.RemoveAt(actionList.SelectedIndex);
            }
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            ChangePosition(-1);
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            ChangePosition(1);
        }
        public void ChangePosition(int change)
        {
            if (actionList.SelectedIndex >= 0)
            {
                int ind = actionList.SelectedIndex;
                var ca = cutscene.Actions[ind];
                cutscene.Actions.RemoveAt(ind);
                ind += change;
                if (ind < 0)
                    ind = 0;
                if (ind > cutscene.Actions.Count)
                    ind = cutscene.Actions.Count;
                cutscene.Actions.Insert(ind, ca);
                actionList.SelectedItem = ca;
            }
        }

        private void classBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cutscene != null && classBox.SelectedValue != null)
            {
                cutscene.Class = (Type)classBox.SelectedValue;
            }
        }

        private void actionList_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 200;
            e.Graphics.TranslateTransform(100, 100);
        }

        
    }
    public class ContractTextBox : TextBox
    {
        ActionContract contract;
        CutsceneAction action;
        public ContractTextBox(CutsceneAction ca, ActionContract con)
        {
            Text = StageObjectPropertyConverter.SetValue(con.GetValue(ca));
            contract = con;
            action = ca;
            Width = 400;
            if (con.Type == typeof(string))
            {
                this.ResizeRedraw = true;
                this.
                Multiline = true;
                
            }
            //this.AcceptsReturn = true;
            //this.WordWrap = true;
        }
        protected override void OnTextChanged(EventArgs e)
        {
            using (System.Drawing.Graphics g = CreateGraphics())
            {
                int height = 0;
                //foreach (var line in Lines)
                {
                    SizeF size = g.MeasureString(Text, Font, Width, new StringFormat());
                    for (int i = 0; i < size.Width; i+=Height)
                    {
                        
                    }
                    height += (int)size.Height;
                }
                height = (int)MathHelper.Clamp(height, 25, 1000) + 10;
                Height = height;
            }
            
            if (contract.Type == typeof(string))
            {
                contract.SetValue(action, Text);
            }
            else if (contract.Type == typeof(int))
            {
                int i = 0;
                if (int.TryParse(Text, out i))
                {
                    contract.SetValue(action, i);
                }
            }
            else if (contract.Type == typeof(float))
            {
                float t = 0;
                float f = 0;
                if (float.TryParse(Text, out f))
                {
                    contract.SetValue(action, f);
                }
            }
            else if (contract.Type == typeof(Vector3))
            {
                Vector3 vec = StageObjectPropertyConverter.GetVector3(Text);
                contract.SetValue(action, vec);
            }
            else if (contract.Type == typeof(Vector2))
            {
                Vector2 vec = StageObjectPropertyConverter.GetVector2(Text);
                contract.SetValue(action, vec);
            }
            base.OnTextChanged(e);
        }
        
    }
    public class ContractCheckBox : CheckBox
    {
        ActionContract contract;
        CutsceneAction action;
        public ContractCheckBox(CutsceneAction ca, ActionContract con)
        {
            contract = con;
            action = ca;
            Checked = (bool)con.GetValue(ca);
            
        }
        protected override void OnCheckedChanged(EventArgs e)
        {
            contract.SetValue(action, Checked);
            base.OnCheckedChanged(e);
        }
    }
    public class ContractComboBox : ComboBox
    {
        CutsceneAction action;
        ActionContract contract;
        public ContractComboBox(CutsceneAction act, ActionContract con)
        {
            action = act;
            contract = con;
            string[] vals = Enum.GetNames(con.Type);
            foreach (var val in vals)
            {
                Items.Add(val);
            }
            SelectedIndex = Items.IndexOf(con.GetValue(act).ToString());
        }
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            object en;
            if (SelectedIndex > -1)
            {
                try
                {
                    en = Enum.Parse(contract.Type, (string)Items[SelectedIndex]);
                    contract.SetValue(action, en);
                }
                catch (Exception ex)
                {

                }
            }
            base.OnSelectedIndexChanged(e);
        }
    }

}
