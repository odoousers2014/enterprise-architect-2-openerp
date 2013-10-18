using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace EnterpriseArchitect2OpenERP
{
    public class ModuleData
    {
        public ModuleData(ProjectData projectData, XmlNode xmlNode, XmlNode Elements)
        {
            XmlNode = xmlNode;

            Parent = projectData;
            Native = false;

            Name = xmlNode.Attributes["name"].Value;
            ClassicName = Name.Replace("/", "_").Replace(" ", "_").ToLower();

            if (xmlNode.Attributes["xmi:id"] != null)
            {
                ID = xmlNode.Attributes["xmi:id"].Value;
            }
            else
            {
                ID = xmlNode.Attributes["xmi:idref"].Value;
            }

            NameDirectory = xmlNode.Attributes["name"].Value;
            NameDirectory = NameDirectory.Replace("/", "_");
            NameDirectory = NameDirectory.ToLower();

            Data = new Dictionary<string, object>();
            
            Depends = new List<string>();
            Depends.Add("base");
            Depends.Add("board");


            Documentation = "";

            ListOfClass = new List<ClassData>();

            ListOfMenu = new Dictionary<string, string>();

            Menus = "";
            Dashboard = "";
            menu_count = 1;

            ModuleName = Utils.getExtra(xmlNode, "module", ClassicName)[0];
            ModulePath = Utils.getExtra(xmlNode, "modulepath", ModuleName.Replace('.', '_'))[0];


            foreach (XmlNode element in Elements.ChildNodes)
            {
                if (element.Attributes["name"] != null)
                {
                    if (element.Attributes["xmi:idref"].Value.Equals(ID))
                    {
                        if (element["properties"] != null)
                        {
                            Documentation = (element["properties"].Attributes["documentation"] != null) ? element["properties"].Attributes["documentation"].Value : "Gestion du module : " + Utils.firstCharUpper(Name.Replace("_", " "));
                            Alias = (element["properties"].Attributes["alias"] != null) ? element["properties"].Attributes["alias"].Value : Utils.firstCharUpper(Name.Replace("_", " "));

                            Menus = "<menuitem name=\"" + Alias + "\" id=\"menu_" + ClassicName + "\" web_icon=\"images/icon.png\" web_icon_hover=\"images/icon-hover.png\"/>";


                            Menus += "\r\n\t\t" + "<menuitem name=\"" + Alias + "\" parent=\"menu_" + ClassicName + "\" id=\"leftmenu_" + ClassicName + "\" sequence=\"" + menu_count.ToString() + "\"/>";

                            //Menus = "<menuitem name=\"" + Alias + "\" parent=\"menu_" + ClassicName + "\" id=\"leftmenu_" + ClassicName + "\" sequence=\"" + menu_count.ToString() + "\"/>";

                            if (!ListOfMenu.ContainsKey("default"))
                            {
                                ListOfMenu["default"] = "leftmenu_" + ClassicName;
                            }

                            ListOfMenu["leftmenu_" + ClassicName] = "leftmenu_" + ClassicName;

                            menu_count++;


                            //------------------------


                            Menus += "\r\n\t\t" + "<menuitem name=\"Rapports\" parent=\"menu_" + ClassicName + "\" id=\"leftmenu_rapport_" + ClassicName + "\" sequence=\"%sequence1%\"/>";

                            ListOfMenu["leftmenu_rapport_" + ClassicName] = "leftmenu_rapport_" + ClassicName;



                            Menus += "\r\n\t\t" + "<menuitem name=\"" + Alias + "\" parent=\"leftmenu_rapport_" + ClassicName + "\" id=\"leftsubmenu_rapport_" + ClassicName + "\" sequence=\"%sequence2%\"/>";

                            ListOfMenu["leftsubmenu_rapport_" + ClassicName] = "leftsubmenu_rapport_" + ClassicName;
                        }
                        
                        if (element["tags"] != null)
                        {
                            foreach (XmlNode tagNode in element["tags"].ChildNodes)
                            {
                                switch (tagNode.Attributes["name"].Value)
                                {
                                    case "category":
                                    {
                                        Category = tagNode.Attributes["value"].Value;
                                        break;
                                    }

                                    case "menu":
                                    {
                                        /*
                                        string[] menu = tagNode.Attributes["value"].Value.Split(new char[]{ ':' });

                                        Menus += "\r\n\t\t" + "<menuitem name=\"" + menu[1].Trim() + "\" parent=\"menu_" + ClassicName + "\" id=\"leftmenu_" + menu[0].Trim() + "\" sequence=\"" + menu_count.ToString() + "\"/>";

                                        if (!ListOfMenu.ContainsKey("default"))
                                        {
                                            ListOfMenu["default"] = "leftmenu_" + menu[0].Trim();
                                        }

                                        ListOfMenu["leftmenu_" + menu[0].Trim()] = "leftmenu_" + menu[0].Trim();

                                        menu_count++;
                                        
                                         */



                                        break;
                                    } 
                                }
                            }
                        }
                    }
                    else if (element.Attributes["name"].Value.Equals("Class Model"))
                    {
                        if (element["model"].Attributes["package"].Value.Equals(ID))
                        {
                            string ClassModelID = element.Attributes["xmi:idref"].Value;

                            foreach (XmlNode subElementXmlNode in Elements.ChildNodes)
                            {
                                if (subElementXmlNode.Attributes["xmi:type"] != null)
                                {
                                    if (subElementXmlNode.Attributes["xmi:type"].Value.Equals("uml:Class"))
                                    {
                                        if (subElementXmlNode["model"].Attributes["package"].Value.Equals(ClassModelID))
                                        {
                                            ClassData classData = new ClassData(this, subElementXmlNode);

                                            ListOfClass.Add(classData);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (xmlNode.Attributes["xmi:id"] == null)
                    {
                        if (element.Attributes["xmi:type"] != null)
                        {
                            if (element.Attributes["xmi:type"].Value.Equals("uml:Class"))
                            {
                                if (element["model"].Attributes["package"].Value.Equals(ID))
                                {
                                    ClassData classData = new ClassData(this, element);

                                    ListOfClass.Add(classData);
                                }
                            }
                        }
                    }
                }
            }

            if (!Menus.Equals(""))
            {
                Menus = Menus.Replace("%sequence1%", menu_count.ToString());
                menu_count++;

                Menus = Menus.Replace("%sequence2%", menu_count.ToString());
                menu_count++;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public string Create()
        {
            string AddonPath = this.Parent.AddonsPath;

            string moduleDirectory = AddonPath + "/" + NameDirectory;

            if (Directory.Exists(moduleDirectory)) Directory.Delete(moduleDirectory, true);

            Directory.CreateDirectory(moduleDirectory);

            DirectoryInfo_Base = Directory.CreateDirectory(moduleDirectory);
            DirectoryInfo_I18n = Directory.CreateDirectory(moduleDirectory + "/i18n");
            DirectoryInfo_Process = Directory.CreateDirectory(moduleDirectory + "/process");
            DirectoryInfo_Report = Directory.CreateDirectory(moduleDirectory + "/report");
            DirectoryInfo_Security = Directory.CreateDirectory(moduleDirectory + "/security");
            DirectoryInfo_Static = Directory.CreateDirectory(moduleDirectory + "/static/src/img");
            DirectoryInfo_Wizard = Directory.CreateDirectory(moduleDirectory + "/wizard");
            DirectoryInfo_View = Directory.CreateDirectory(moduleDirectory + "/view");
            DirectoryInfo_Demo = Directory.CreateDirectory(moduleDirectory + "/demo");
            DirectoryInfo_Workflow = Directory.CreateDirectory(moduleDirectory + "/workflow");
            DirectoryInfo_Images = Directory.CreateDirectory(moduleDirectory + "/images");
            DirectoryInfo_Sequences = Directory.CreateDirectory(moduleDirectory + "/data");

            Utils.MakeFileFromModel("openerp_security.xml.model", DirectoryInfo_Security.FullName + "/groups.xml");
            Utils.MakeFileFromModel("ir.model.access.csv.model", DirectoryInfo_Security.FullName + "/ir.model.access.csv");

            Dashboard = "";

            foreach (ClassData classData in ListOfClass)
            {
                if (classData.InDashBoard)
                {
                    Dashboard += "\t\t<record id=\"" + classData.DashboardAction["id"] + "\" model=\"ir.actions.act_window\">\r\n";
                    Dashboard += "\t\t\t<field name=\"name\">" + classData.DashboardAction["name"] + "</field>\r\n";
                    Dashboard += "\t\t\t<field name=\"type\">ir.actions.act_window</field>\r\n";
                    Dashboard += "\t\t\t<field name=\"res_model\">" + classData.DashboardAction["res_model"] + "</field>\r\n";
                    Dashboard += "\t\t\t<field name=\"view_type\">" + classData.DashboardAction["view_type"] + "</field>\r\n";
                    Dashboard += "\t\t\t<field name=\"view_mode\">" + classData.DashboardAction["view_mode"] + "</field>\r\n";
                    //Dashboard += "\t\t\t<field name=\"context\">" + classData.DashboardAction["context"] + "</field>\r\n";
                    Dashboard += "\t\t\t<field name=\"view_id\" ref=\"" + classData.DashboardAction["view_id"] + "\"/>\r\n";
                    //Dashboard += "\t\t\t<field name=\"domain\">" + classData.DashboardAction["domain"] + "</field>\r\n";
                    Dashboard += "\t\t</record>\r\n";

                    Dashboard += "\r\n";
                }
            }

            string board_style = "1";
            foreach (ClassData classData in ListOfClass)
            {
                if (classData.InDashBoard)
                {
                    if (!classData.DashboardAction["position"].Equals("1"))
                    {
                        board_style = "2-1";
                    }
                }
            }


            Dashboard += "\t\t<record id=\"dashboard_" + ClassicName + "_form\" model=\"ir.ui.view\">\r\n";
            Dashboard += "\t\t\t<field name=\"name\">dashboard." + ClassicName + ".form</field>\r\n";
            Dashboard += "\t\t\t<field name=\"model\">board.board</field>\r\n";
            Dashboard += "\t\t\t<field name=\"type\">form</field>\r\n";
            Dashboard += "\t\t\t<field name=\"arch\" type=\"xml\">\r\n";

            Dashboard += "\t\t\t\t<form string=\"Tableau de bord : " + Alias + "\">\r\n";
            Dashboard += "\t\t\t\t\t<board style=\"" + board_style + "\">\r\n";

            Dashboard += "\t\t\t\t\t\t<column>\r\n";
            foreach (ClassData classData in ListOfClass)
            {
                if ((classData.InDashBoard) && (classData.DashboardAction["position"].Equals("1")))
                {
                    Dashboard += "\t\t\t\t\t\t\t<action name=\"%(" + classData.DashboardAction["id"] + ")d\" creatable=\"true\" string=\"" + classData.Alias + "\"/>\r\n";
                }
            }
            Dashboard += "\t\t\t\t\t\t</column>\r\n";

            if (board_style.Equals("2-1"))
            {
                Dashboard += "\t\t\t\t\t\t<column>\r\n";
                foreach (ClassData classData in ListOfClass)
                {
                    if ((classData.InDashBoard) && (classData.DashboardAction["position"].Equals("2")))
                    {
                        Dashboard += "\t\t\t\t\t\t\t<action name=\"%(" + classData.DashboardAction["id"] + ")d\" creatable=\"true\" string=\"" + classData.Alias + "\"/>\r\n";
                    }
                }
                Dashboard += "\t\t\t\t\t\t</column>\r\n";
            }

            Dashboard += "\t\t\t\t\t</board>\r\n";
            Dashboard += "\t\t\t\t</form>\r\n";

            Dashboard += "\t\t\t</field>\r\n";
            Dashboard += "\t\t</record>\r\n";

            Dashboard += "\r\n";

            Dashboard += "\t\t<record id=\"open_dashboard_" + ClassicName + "\" model=\"ir.actions.act_window\">\r\n";
            Dashboard += "\t\t\t<field name=\"name\">Tableau de bord : " + Alias + "</field>\r\n";
            Dashboard += "\t\t\t<field name=\"res_model\">board.board</field>\r\n";
            Dashboard += "\t\t\t<field name=\"view_type\">form</field>\r\n";
            Dashboard += "\t\t\t<field name=\"view_mode\">form</field>\r\n";
            Dashboard += "\t\t\t<field name=\"usage\">menu</field>\r\n";
            Dashboard += "\t\t\t<field name=\"view_id\" ref=\"dashboard_" + ClassicName + "_form\"/>\r\n";
            Dashboard += "\t\t</record>\r\n";

            Dashboard += "\r\n";

            Dashboard += "\t\t<menuitem name=\"" + Alias + "\" id=\"" + ClassicName + ".menu_" + ClassicName + "\" action=\"open_dashboard_" + ClassicName + "\"/>\r\n";

            Dashboard = Dashboard.Substring(2);

            Dictionary<string, string> __module_dashboard_Values__ = new Dictionary<string, string>();

            __module_dashboard_Values__["classview"] = Dashboard;

            Utils.MakeFileFromModel("openerp_class_view.xml.model", DirectoryInfo_Base.FullName + "/" + ClassicName + "_dashboard_view.xml", __module_dashboard_Values__);



            Dictionary<string, string> __module_Values__ = new Dictionary<string, string>();

            __module_Values__["classview"] = Menus;

            Utils.MakeFileFromModel("openerp_class_view.xml.model", DirectoryInfo_Base.FullName + "/" + ClassicName + "_view.xml", __module_Values__);



            string imports = "";
            string report_imports = "";
            string wizard_imports = "";
            string updateXml = "";
            string demoXml = "";
            string images = "";
            string test = "";
            string depends = "";

            foreach (ClassData classData in ListOfClass)
            {
                classData.GetLinkData();
            }

            string updateWorkflowXml = "";
            string updateReportXml = "";
            string updateWizardXml = "";
            string updateDataXml = "";
            string updateProcessXml = "";

            foreach (ClassData classData in ListOfClass)
            {
                if (!((classData.AssociationClass) && (classData.ListOfProperties.Count <= 1)))
                {
                    classData.Create();

                    imports += "import " + ClassicName + "_" + classData.Name + "\r\n";
                    
                    report_imports += "import report_" + ClassicName + "_" + classData.Name + "\r\n";
                    
                    wizard_imports += "import wizard_" + ClassicName + "_" + classData.Name + "\r\n";

                    updateXml += "'view/" + ClassicName + "_" + classData.Name + "_view.xml',\r\n\t";
                    updateReportXml += "'report/report_" + ClassicName + "_" + classData.Name + ".xml',\r\n\t";
                    updateWizardXml += "'wizard/wizard_" + ClassicName + "_" + classData.Name + ".xml',\r\n\t";

                    if (classData.StateWorkflow)
                    {
                        updateWorkflowXml += "'workflow/workflow_" + ClassicName + "_" + classData.Name + ".xml',\r\n\t";
                    }

                    if (classData.CodeProperty != null)
                    {
                        updateDataXml += "'data/data_" + ClassicName + "_" + classData.Name + ".xml',\r\n\t";
                    }
                    
                    if (classData.Process)
                    {
                        updateProcessXml += "'process/process_" + ClassicName + "_" + classData.Name + ".xml',\r\n\t";
                    }


                    Program.Form.SetTaskText(Program.Form.GetTaskText() + " \\ " + classData.Name);

                    Application.DoEvents();
                }
            }

            imports += "\r\n";
            imports += "import report\r\n";
            imports += "import wizard\r\n";
            

            updateXml += updateReportXml;
            updateXml += updateWizardXml;
            updateXml += updateWorkflowXml;
            updateXml += updateDataXml;
            updateXml += updateProcessXml;

            if (updateXml.Length > 3)
            {
                updateXml = updateXml.Substring(0, updateXml.Length - 3);
            }

            if (Depends.Count > 0)
            {
                foreach (string depend in Depends)
                {
                    depends += "\r\n\t'" + depend + "',";
                }

                depends += "\r\n\t";
            }


            this.Data["imports"] = imports;
            this.Data["report_imports"] = report_imports;
            this.Data["wizard_imports"] = wizard_imports;
            this.Data["update_xml"] = updateXml;
            this.Data["demo_xml"] = demoXml;
            this.Data["images"] = images;
            this.Data["category"] = Category;
            this.Data["depends"] = depends;
            this.Data["test"] = test;


            PyGenerator.CreateInit(this);
            PyGenerator.CreateReportInit(this);
            PyGenerator.CreateWizardInit(this);
            PyGenerator.CreateOpenERP(this);

            XmlGenerator.CreateView(this);

            foreach (ClassData classData in ListOfClass)
            {
                if (classData.Process)
                {
                    XmlGenerator.CreateProcess(classData);
                }
            }

            PoGenerator.CreateLanguage(this);

            Utils.CopyIcons(DirectoryInfo_Base.FullName + "/images/");
            Utils.CopyIcon(DirectoryInfo_Static.FullName);

            return (new DirectoryInfo(AddonPath + "/" + NameDirectory)).FullName;
        }

        public void AddDepend(string name)
        {
            List<string> toExclude = new List<string>();
            toExclude.AddRange(new string[] { "res" });

            if (toExclude.Contains(name)) return;

            if (!Depends.Contains(name))
            {
                Depends.Add(name);
            }
        }

        public ClassData GetClass(string name)
        {
            foreach (ClassData classData in ListOfClass)
            {
                if (classData.Name.Equals(name.Trim()))
                {
                    return classData;
                }
            }

            return null;
        }

        public ClassData GetClassByID(string Id)
        {
            foreach (ClassData classData in ListOfClass)
            {
                if (classData.ID.Equals(Id.Trim()))
                {
                    return classData;
                }
            }

            return null;
        }

        public List<ClassData> ListOfClass { get; set; }

        public string Name { get; set; }

        public string NameDirectory { get; set; }

        public string ID { get; set; }

        public DirectoryInfo DirectoryInfo_View { get; set; }

        public DirectoryInfo DirectoryInfo_Wizard { get; set; }

        public DirectoryInfo DirectoryInfo_Static { get; set; }

        public DirectoryInfo DirectoryInfo_Security { get; set; }

        public DirectoryInfo DirectoryInfo_Report { get; set; }

        public DirectoryInfo DirectoryInfo_Process { get; set; }

        public DirectoryInfo DirectoryInfo_I18n { get; set; }

        public DirectoryInfo DirectoryInfo_Base { get; set; }

        public Dictionary<string, object> Data { get; set; }

        public string Documentation { get; set; }

        public string ClassicName { get; set; }

        public ProjectData Parent { get; set; }

        public DirectoryInfo DirectoryInfo_Demo { get; set; }

        public DirectoryInfo DirectoryInfo_Workflow { get; set; }

        public List<string> Depends { get; set; }

        public string Category { get; set; }

        public string Alias { get; set; }

        public string Menus { get; set; }

        public Dictionary<string, string> ListOfMenu { get; set; }

        public bool Native { get; set; }

        public XmlNode XmlNode { get; set; }

        public int menu_count { get; set; }

        public string Dashboard { get; set; }

        public DirectoryInfo DirectoryInfo_Images { get; set; }

        public DirectoryInfo DirectoryInfo_Sequences { get; set; }

        public string ModuleName { get; set; }

        public string ModulePath { get; set; }
    }
}
