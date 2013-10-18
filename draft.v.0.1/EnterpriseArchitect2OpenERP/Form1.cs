using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace EnterpriseArchitect2OpenERP
{
    public partial class Form1 : Form
    {

        string modelsDirectory = PathData.Models;
        List<CheckBox> ListOfCheckBox = new List<CheckBox>();

        public Form1()
        {
            InitializeComponent();

            textBox1.Text = (new FileInfo(PathData.Addons + "/../toto.xml")).FullName;
            textBox2.Text = PathData.Addons;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Text = "EnterpriseArchitect to OpenERP : Ongoing ...";
            SetTaskText("Generating ...");

            List<string> listOfModule = new List<string>();
            foreach (CheckBox checkBox in ListOfCheckBox)
            {
                if (checkBox.Checked)
                {
                    listOfModule.Add(checkBox.Text);
                }
            }

            Program.ProjectData.AddonsPath = textBox2.Text;

            Program.ProjectData.Create(listOfModule);
            
            this.Text = "EnterpriseArchitect to OpenERP";
            SetTaskText("Finished.");
        }

        private string openerp_class_view(string openerp_class_viewContent, Dictionary<string, string> values)
        {
            openerp_class_viewContent = Utils.replaceValues(openerp_class_viewContent, values);

            return openerp_class_viewContent;
        }

        private string openerp_class(string openerp_classContent, Dictionary<string, string> values)
        {
            openerp_classContent = Utils.replaceValues(openerp_classContent, values);

            return openerp_classContent;
        }

        private string __init__(string __init__Content, Dictionary<string, string> values)
        {
            __init__Content = Utils.replaceValues(__init__Content, values);

            return __init__Content;
        }

        private string __openerp__(string __openerp__Content, Dictionary<string, string> values)
        {
            __openerp__Content = Utils.replaceValues(__openerp__Content, values);

            return __openerp__Content;
        }

        public void SetTaskText(string text)
        {
            label3.Text = text;
        }

        public string GetTaskText()
        {
            return label3.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (ListOfCheckBox == null)
            {
                ListOfCheckBox = new List<CheckBox>();
            }
            else
            {
                foreach (CheckBox checkBox in ListOfCheckBox)
                {
                    Controls.Remove(checkBox);
                }

                ListOfCheckBox.Clear();
            }
            Height = 242;
            
            
            this.Text = "EnterpriseArchitect to OpenERP : Ongoing ...";
            SetTaskText("Initialization ...");

            string file = textBox1.Text.Trim();
            string folder = textBox2.Text.Trim();

            if (XmlUtility.FileExists(file))
            {
                XmlNode xmlNode = XmlUtility.GetXmlDocument(file);

                List<XmlNode> lst = XmlUtility.GetXmlNode(xmlNode, "EAID_F10F202F_3A57_464a_B98A_8DA512A5BE07");

                Program.ProjectData = new ProjectData(xmlNode);
            }

            int i = 0;
            foreach (ModuleData moduleData in Program.ProjectData.ListOfModuleData)
            {
                if (!moduleData.Native)
                {
                    CheckBox checkBox = new CheckBox();
                    checkBox.Text = moduleData.Name;
                    checkBox.Location = new Point(12, 138 + i);

                    if (moduleData.ListOfClass.Count == 0)
                    {
                        checkBox.Enabled = false;
                    }

                    Controls.Add(checkBox);
                    ListOfCheckBox.Add(checkBox);

                    Height = 242 + i;

                    i += 20;
                }
            }

            button2.Enabled = true;

            this.Text = "EnterpriseArchitect to OpenERP";
            SetTaskText("Finished loading.");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
