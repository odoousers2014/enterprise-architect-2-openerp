using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace EnterpriseArchitect2OpenERP
{
    public class ClassData
    {
        public ClassData(ModuleData moduleData, XmlNode xmlNode)
        {
            Parent = moduleData;
            AssociationClass = false;

            Inherit = null;

            ListOfGroups = new Dictionary<string, GroupData>();

            InDashBoard = false;
            StateWorkflow = false;
            Process = false;

            DashboardAction = new Dictionary<string, string>();

            if (xmlNode.Attributes["xmi:type"] != null)
            {
                if (xmlNode.Attributes["xmi:type"].Value.Equals("uml:Class"))
                {
                    Name = xmlNode.Attributes["name"].Value.ToLower();
                    
                    ID = xmlNode.Attributes["xmi:idref"].Value;

                    Alias = (xmlNode["properties"].Attributes["alias"] != null) ? xmlNode["properties"].Attributes["alias"].Value : Utils.firstCharUpper(Name.Replace("_", " "));

                    Documentation = (xmlNode["properties"].Attributes["documentation"] != null) ? xmlNode["properties"].Attributes["documentation"].Value : null;
                    if (Documentation == null)
                    {
                        Documentation = Alias;
                    }

                    ListOfProperties = new List<PropertyData>();

                    PropertyData Identifiant = new PropertyData();
                    Identifiant.Name = Name + "_id";
                    Identifiant.Typage = "integer";
                    Identifiant.Parent = this;

                    ListOfProperties.Add(Identifiant);

                    if (xmlNode["attributes"] != null)
                    {
                        foreach (XmlNode propXmlNode in xmlNode["attributes"].ChildNodes)
                        {
                            PropertyData propertyData = new PropertyData(this, propXmlNode);

                            ListOfProperties.Add(propertyData);
                        }
                    }

                    ListOfOperations = new Dictionary<string, OperationData>();
                    if (xmlNode["operations"] != null)
                    {
                        foreach (XmlNode opXmlNode in xmlNode["operations"].ChildNodes)
                        {
                            OperationData operationData = new OperationData(this, opXmlNode);

                            ListOfOperations.Add(operationData.Name, operationData);
                        }
                    }

                    ListOfLinks = new List<LinkData>();
                    if (xmlNode["links"] != null)
                    {
                        foreach (XmlNode lnkXmlNode in xmlNode["links"].ChildNodes)
                        {
                            LinkData linkData = new LinkData(this, lnkXmlNode);

                            ListOfLinks.Add(linkData);
                        }
                    }

                    ListOfConstraints = new Dictionary<string, string>();
                    if (xmlNode["constraints"] != null)
                    {
                        foreach (XmlNode constXmlNode in xmlNode["constraints"].ChildNodes)
                        {
                            ListOfConstraints.Add(constXmlNode.Attributes["name"].Value, constXmlNode.Attributes["description"].Value);
                        }
                    }

                    
                    Columns = Utils.getExtra(xmlNode, "col", "6")[0];
                    if (ListOfProperties.Count < 3)
                    {
                        Columns = "4";
                        if (ListOfProperties.Count < 2) Columns = "2";
                    }

                    DateStart = Utils.getExtra(xmlNode, "date_start", "")[0];
                    DateStop = Utils.getExtra(xmlNode, "date_stop", "")[0];

                    GraphX = Utils.getExtra(xmlNode, "graphx", "")[0];
                    GraphY = Utils.getExtra(xmlNode, "graphy", "");

                    Code = Utils.getExtra(xmlNode, "code", "")[0];
                    Target = Utils.getExtra(xmlNode, "target", "")[0];

                    FormId = Utils.getExtra(xmlNode, "formid", "")[0];
                    TreeId = Utils.getExtra(xmlNode, "treeid", "")[0];

                    ModuleName = Utils.getExtra(xmlNode, "module", moduleData.ModuleName)[0];

                    XpathForm = Utils.getExtra(xmlNode, "xpathform", "/form")[0];
                    FormPosition = Utils.getExtra(xmlNode, "formpos", "inside")[0];
                    XpathTree = Utils.getExtra(xmlNode, "xpathtree", "/tree")[0];
                    TreePosition = Utils.getExtra(xmlNode, "treepos", "inside")[0];

                    Accounting = Utils.getExtra(xmlNode, "accounting", "")[0];
                    AccountingFunction = "";
                    AccountingArgs = new List<string>();

                    if (!Accounting.Equals(""))
                    {
                        string[] func = Accounting.Split(new char[] { '(' });

                        AccountingFunction = func[0];

                        if (AccountingFunction.Equals("move"))
                        {
                            string[] args = func[1].Substring(0, func[1].Length - 1).Split(new char[] { ',' });
                            for (int i = 0; i < args.Length; i++)
                            {
                                AccountingArgs.Add(args[i].Trim());
                            }
                        }
                    }

                    Picture = "";

                    string mode = "";

                    if (!DateStart.Equals(""))
                    {
                        mode += ",calendar";
                    }

                    if (!GraphX.Equals(""))
                    {
                        mode += ",graph";
                    }



                    ListOfAction = new List<ActionData>();

                    ListOfAction.Add(new ActionData("action_" + Name, Alias, this, "view_" + Name + "_tree", "form,tree" + mode));

                    if (Target.ToLower().Trim().Equals("new"))
                    {
                        ListOfAction[0].Data["target"] = "new";
                    }

                    ListOfAction.Add(new ActionData(ListOfAction[0], this, "view_" + Name + "_tree", ViewKind.Tree, "1"));
                    ListOfAction.Add(new ActionData(ListOfAction[0], this, "view_" + Name + "_form", ViewKind.Form, "2"));

                    int seq = 3;

                    if (!DateStart.Equals(""))
                    {
                        ListOfAction.Add(new ActionData(ListOfAction[0], this, "view_" + Name + "_calendar", ViewKind.Calendar, seq.ToString()));
                        seq++;
                    }

                    if (!GraphX.Equals(""))
                    {
                        ListOfAction.Add(new ActionData(ListOfAction[0], this, "view_" + Name + "_graph", ViewKind.Graph, seq.ToString()));
                        seq++;
                    }

                    if (!Picture.Equals(""))
                    {
                        ListOfAction.Add(new ActionData(ListOfAction[0], this, "view_" + Name + "_kanban", ViewKind.Kanban, seq.ToString()));
                        seq++;
                    }


                    //ActionRapportData = new ActionData("action_rapport_" + Name, Alias, this, "", "form");
                    //ActionRapportData.Data["res_model"] = "rapport." + Parent.ClassicName + "." + Name;;
                    //ActionRapportData.Data["view_type"] = "form";
                    //ActionRapportData.Data["search_view_id"] = "view_rapport_" + Name + "_search";
                    //ActionRapportData.Data["context"] = "";

                    //ListOfAction.Add(ActionRapportData);
                    
                    /*ListOfAction.Add(new ActionData(ListOfAction[0], this, "view_" + Name + "_gantt", ViewKind.Gantt, "4"));*/


                    
                    ListOfView = new List<ViewData>();

                    ListOfView.Add(new ActData("view_" + Name, Alias, this, "form,tree"));

                    ListOfView.Add(new TreeData("view_" + Name + "_tree", ListOfAction[0], Alias, this, "1"));
                    ListOfView.Add(new FormData("view_" + Name + "_form", ListOfAction[0], Alias, this, "2"));

                    seq = 3;

                    if (!DateStart.Equals(""))
                    {
                        ListOfView.Add(new CalendarData("view_" + Name + "_calendar", ListOfAction[0], Alias, this, seq.ToString()));
                        seq++;
                    }

                    if (!GraphX.Equals(""))
                    {
                        ListOfView.Add(new GraphData("view_" + Name + "_graph", ListOfAction[0], Alias, this, seq.ToString()));
                        seq++;
                    }

                    if (!Picture.Equals(""))
                    {
                        ListOfView.Add(new KanbanData("view_" + Name + "_kanban", ListOfAction[0], Alias, this, seq.ToString()));
                        seq++;
                    }
                    
                    /*ListOfView.Add(new GanttData((ActData)ListOfView[0], ListOfAction[0], Alias, this, "4"));*/

                    if (Inherit == null)
                    {
                        ListOfView.Add(new SearchData("view_" + Name + "_search", ListOfAction[0], Alias, this, seq.ToString()));
                    }

                    //SearchData searchRapportData = new SearchData("view_rapport_" + Name + "_search", actionRapportData, Alias, this, seq.ToString());

                    //ListOfView.Add(searchRapportData);
                    //seq++;

                    string dashboard = Utils.getExtra(xmlNode, "dashboard", "")[0];

                    if (!dashboard.Equals(""))
                    {
                        string[] dashboards = dashboard.Split(new char[] { ':' });
                        dashboard = dashboards[0].Trim();

                        string dashboard_position = "1";
                        if (dashboard.Length > 1)
                        {
                            dashboard_position = dashboards[1].Trim();
                        }

                        InDashBoard = true;

                        DashboardAction["id"] = "action_dashboard_" + Name + "_" + dashboard;
                        DashboardAction["name"] = Alias;
                        DashboardAction["res_model"] = Parent.ClassicName + "." + Name;
                        DashboardAction["view_type"] = "form";
                        DashboardAction["view_mode"] = "tree,form" + mode;// dashboard;
                        //DashboardAction["context"] = "{}";
                        DashboardAction["view_id"] = "view_" + Name + "_" + dashboard;
                        DashboardAction["position"] = dashboard_position;
                        //DashboardAction["domain"] = "[]";
                    }


                    ListOfMenu = new List<MenuData>();
                    Menu = Utils.getExtra(xmlNode, "menu", "")[0];

                    if (!Menu.Equals("false"))
                    {
                        string menuParent = "default";
                        string menuRapportParent = "leftsubmenu_rapport_" + Parent.ClassicName;

                        if (!Menu.Equals(""))
                        {
                            string classicName = Utils.ClassicName(Menu);

                            if (!Parent.ListOfMenu.ContainsKey("leftmenu_" + classicName))
                            {
                                Parent.Menus += "\r\n\t\t" + "<menuitem name=\"" + Menu + "\" parent=\"menu_" + Parent.ClassicName + "\" id=\"leftmenu_" + classicName + "\" sequence=\"" + Parent.menu_count.ToString() + "\"/>";

                                Parent.ListOfMenu["leftmenu_" + classicName] = "leftmenu_" + classicName;

                                Parent.menu_count++;

                                // ------------------------------

                                Parent.Menus += "\r\n\t\t" + "<menuitem name=\"" + Menu + "\" parent=\"leftmenu_rapport_" + Parent.ClassicName + "\" id=\"leftsubmenu_rapport_" + classicName + "\" sequence=\"" + Parent.menu_count.ToString() + "\"/>";

                                Parent.ListOfMenu["leftsubmenu_rapport_" + classicName] = "leftsubmenu_rapport_" + classicName;

                                Parent.menu_count++;
                            }

                            menuParent = "leftmenu_" + classicName;
                            menuRapportParent = "leftsubmenu_rapport_" + classicName;
                        }


                        string menu_name = "leftmenu_" + Name;
                        string menu_rapport_name = "leftsubmenu_rapport_" + Name;

                        if (Parent.ListOfMenu.ContainsKey(menu_name))
                        {
                            Random rnd = new Random(DateTime.Now.Second);
                            menu_name += "_" + rnd.Next(1000, 9999).ToString();
                        }

                        if (Parent.ListOfMenu.ContainsKey(menu_rapport_name))
                        {
                            Random rnd = new Random(DateTime.Now.Second);
                            menu_rapport_name += "_" + rnd.Next(1000, 9999).ToString();
                        }

                        ListOfMenu.Add(new MenuData(menu_name, Alias, Parent.ListOfMenu[menuParent], "action_" + Name, this));
                        MenuRapport = "\t\t" + "<menuitem name=\"" + Alias + "\" parent=\"" + menuRapportParent + "\" id=\"" + menu_name + "_rapport\" action=\"action_rapport_" + Name + "\" sequence=\"" + Parent.menu_count.ToString() + "\"/>";

                        Parent.menu_count++;
                    }
                }
            }
        }

        public void GetLinkData()
        {
            List<LinkData> newListOfLinks = new List<LinkData>();

            foreach (LinkData linkData in ListOfLinks)
            {
                linkData.GetAllClass();

                if (linkData.Direction != null)
                {
                    if (linkData.Direction.Equals("destination -> source"))
                    {
                        if (linkData.ClassEnd.GetProperty(linkData.ClassStart.Name + "_ids") == null)
                        {
                            PropertyData propertyData = new PropertyData();

                            propertyData.Parent = linkData.ClassEnd;
                            propertyData.Name = linkData.ClassStart.Name + "_ids";
                            propertyData.Default = null;

                            linkData.ClassEnd.ListOfProperties.Add(propertyData);

                            LinkData newLinkData = new LinkData();
                            newLinkData.ClassStart = linkData.ClassStart;
                            newLinkData.ClassEnd = linkData.ClassEnd;

                            newLinkData.PropertyStart = propertyData;
                            propertyData.LinkData = newLinkData;
                            newLinkData.Kind = KindLink.Associate;

                            newLinkData.PropertyEnd = newLinkData.ClassEnd.GetProperty("id");

                            newLinkData.MultiplicityStart = "0..*";
                            newLinkData.MultiplicityEnd = "1";

                            newListOfLinks.Add(newLinkData);

                            if (!linkData.ClassStart.Equals(this))
                            {
                                linkData.ClassStart.ListOfLinks.Add(newLinkData);
                            }
                        }
                    }
                    else // Destination = source -> destination
                    {
                        switch (linkData.Kind)
                        {
                            case KindLink.Generalize:
                            {
                                Inherit = linkData.ClassEnd;
                                break;
                            }
                        }
                    }
                }
            }

            ListOfLinks.AddRange(newListOfLinks);
        }

        public void Create()
        {
            Name = Name.Replace(".", "_");

            List<PropertyData> listOfPropertyData_Code = new List<PropertyData>();
            List<PropertyData> listOfPropertyData_Name = new List<PropertyData>();
            List<PropertyData> listOfPropertyData_Others = new List<PropertyData>();

            foreach (PropertyData propertyData in ListOfProperties)
            {
                if (propertyData.Name.Equals("code"))
                {
                    listOfPropertyData_Code.Add(propertyData);
                }
                else if (propertyData.Name.Equals("name"))
                {
                    listOfPropertyData_Name.Add(propertyData);
                }
                else
                {
                    listOfPropertyData_Others.Add(propertyData);
                }
            }

            ListOfProperties = new List<PropertyData>();
            ListOfProperties.AddRange(listOfPropertyData_Code);
            ListOfProperties.AddRange(listOfPropertyData_Name);
            ListOfProperties.AddRange(listOfPropertyData_Others);


            ListOfGroups = new Dictionary<string, GroupData>();

            ListOfGroupsReport = new Dictionary<string, GroupData>();
            ListOfPropertiesReport = new List<PropertyData>();

            GroupData rootGroupData = new GroupData("default");
            rootGroupData.Root = true;
            rootGroupData.ClassData = this;
            rootGroupData.Report = false;
            ListOfGroups.Add("default", rootGroupData);

            GroupData rootReportGroupData = new GroupData("default");
            rootReportGroupData.Root = true;
            rootReportGroupData.ClassData = this;
            rootReportGroupData.Report = true;


            string sel = "";
            List<PropertyData> listOfDates = new List<PropertyData>();
            foreach (PropertyData propertyData in ListOfProperties)
            {
                if (propertyData.Typage.Equals("date") || propertyData.Typage.Equals("datetime") || propertyData.Typage.Equals("time"))
                {
                    listOfDates.Add(propertyData);
                    ListOfPropertiesReport.Add(propertyData);
                }
            }

            if (listOfDates.Count > 0) sel += "('date', 'Date'),";

            if (!sel.Equals(""))
            {
                PropertyData property = new PropertyData();
                property.Name = "filtrerpar";
                property.Alias = "Filtrer par";
                property.Typage = "selection";

                sel = sel.Substring(0, sel.Length - 1);
                property.Selection = "[" + sel + "]";


                PageData pageFilter = new PageData("Filtres");
                pageFilter.ListOfGroups["default"] = new GroupData("default");
                pageFilter.ListOfGroups["default"].ClassData = this;
                pageFilter.ListOfGroups["default"].ListOfProperties.Add(property);
                ListOfPropertiesReport.Add(property);

                if (listOfDates.Count > 0)
                {
                    pageFilter.ListOfGroups["default"].ListOfGroups.Add("dates", new GroupData("Dates", this));
                    pageFilter.ListOfGroups["default"].ListOfGroups["dates"].ListOfProperties = listOfDates;
                }

                rootReportGroupData.ListOfPages.Add("filter", pageFilter);
            }


            rootReportGroupData.ListOfPages.Add("avances", new PageData("Avancés"));
            rootReportGroupData.ListOfPages.Add("options", new PageData("Options"));

            ListOfGroupsReport.Add("default", rootReportGroupData);

            foreach (PropertyData propertyData in ListOfProperties)
            {
                bool dontthread = false;

                if (!propertyData.Name.Equals(Name + "_id"))
                {
                    propertyData.Update();


                    if ((propertyData.Name.Equals("state")) && (propertyData.Typage.Equals("selection")))
                    {
                        dontthread = true;
                        StateWorkflow = true;
                        StateProperty = propertyData;

                        propertyData.Readonly = "1";
                        propertyData.Select = "1";

                        ListOfWorkFlowButton = new Dictionary<string, ButtonData>();
                        ListOfWorkFlowFunction = new Dictionary<string, OperationData>();
                        foreach (KeyValuePair<string, string> KeyValue in propertyData.SelectionData.ListOfValues)
                        {
                            OperationData operationData = GetOperation(propertyData.TypageInitial, KeyValue.Key);

                            if (operationData != null)
                            {
                                ButtonData buttonData = new ButtonData();
                                buttonData.States = KeyValue.Key;
                                
                                if (operationData.CallBy.Equals("button"))
                                {
                                    buttonData.Text = KeyValue.Value;
                                    buttonData.Name = operationData.Name;
                                }
                                else if (operationData.CallBy.Equals(""))
                                {
                                    buttonData.RenderXML = false;
                                    ListOfWorkFlowFunction.Add(KeyValue.Key, operationData);

                                    operationData.ReturnType = "boolean";
                                }

                                ListOfWorkFlowButton.Add(KeyValue.Key, buttonData);
                            }
                        }
                    }
                    else if (propertyData.Name.Equals("name"))
                    {
                        //
                    }
                    else if (propertyData.Name.Equals("code"))
                    {
                        if (!Code.Equals(""))
                        {
                            CodeProperty = propertyData;
                            propertyData.Uniq = true;
                        }
                    }
                    else if (propertyData.Name.Equals("sequence"))
                    {
                        //
                    }
                    else if (propertyData.Name.Equals("parent_id"))
                    {
                        //
                    }



                    if (!dontthread)
                    {
                        if (propertyData.Typage.Equals("text"))
                        {
                            if (propertyData.Group.Equals("") && propertyData.Page.Equals(""))
                            {
                                propertyData.Page = propertyData.Alias;
                                propertyData.ColSpan = "6";
                                propertyData.ShowLabel = false;
                            }
                        }
                        else if (propertyData.Typage.Equals("selection") || (propertyData.LinkData != null))
                        {
                            ListOfGroupsReport["default"].ListOfProperties.Add(propertyData);
                            ListOfPropertiesReport.Add(propertyData);
                        }


                        if (propertyData.Group.Equals(string.Empty) && propertyData.Page.Equals(string.Empty))
                        {
                            ListOfGroups["default"].ListOfProperties.Add(propertyData);
                        }
                        else if (!propertyData.Group.Equals(string.Empty) && propertyData.Page.Equals(string.Empty))
                        {
                            if (!ListOfGroups["default"].ListOfGroups.ContainsKey(propertyData.Group)) ListOfGroups["default"].ListOfGroups.Add(propertyData.Group, new GroupData(propertyData.Group, this));

                            ListOfGroups["default"].ListOfGroups[propertyData.Group].ListOfProperties.Add(propertyData);
                        }
                        else if (propertyData.Group.Equals(string.Empty) && !propertyData.Page.Equals(string.Empty))
                        {
                            if (!ListOfGroups["default"].ListOfPages.ContainsKey(propertyData.Page))
                            {
                                ListOfGroups["default"].ListOfPages.Add(propertyData.Page, new PageData(propertyData.Page));
                                ListOfGroups["default"].ListOfPages[propertyData.Page].ListOfGroups.Add("default", new GroupData("default", this));
                            }

                            ListOfGroups["default"].ListOfPages[propertyData.Page].ListOfGroups["default"].ListOfProperties.Add(propertyData);
                        }
                        else if (!propertyData.Group.Equals(string.Empty) && !propertyData.Page.Equals(string.Empty))
                        {
                            if (!ListOfGroups["default"].ListOfPages.ContainsKey(propertyData.Page))
                            {
                                ListOfGroups["default"].ListOfPages.Add(propertyData.Page, new PageData(propertyData.Page));
                                ListOfGroups["default"].ListOfPages[propertyData.Page].ListOfGroups.Add("default", new GroupData("default", this));
                            }

                            if (!ListOfGroups["default"].ListOfPages[propertyData.Page].ListOfGroups.ContainsKey(propertyData.Group))
                            {
                                ListOfGroups["default"].ListOfPages[propertyData.Page].ListOfGroups.Add(propertyData.Group, new GroupData(propertyData.Group, this));
                            }

                            ListOfGroups["default"].ListOfPages[propertyData.Page].ListOfGroups[propertyData.Group].ListOfProperties.Add(propertyData);
                        }
                    }
                    else
                    {
                        if (propertyData.Typage.Equals("selection") || (propertyData.LinkData != null))
                        {
                            ListOfGroupsReport["default"].ListOfProperties.Add(propertyData);
                            ListOfPropertiesReport.Add(propertyData);
                        }
                    }
                }
            }

            foreach (PropertyData propertyData in ListOfProperties)
            {
                if (propertyData.Typage.Equals("one2many"))
                {
                    foreach (PropertyData property in ListOfProperties)
                    {
                        if (!propertyData.Equals(property))
                        {
                            if (!property.ShareTo.Equals(""))
                            {
                                string[] shareto = property.ShareTo.Split(new char[] { '.' });

                                string clssppt = Parent.ClassicName + "." + shareto[0];

                                if (clssppt.Equals(propertyData.LinkManyClass))
                                {
                                    ClassData clssData = Parent.GetClass(propertyData.LinkManyClass.Split(new char[] { '.' })[1]);

                                    PropertyData propData = clssData.GetProperty(shareto[1]);

                                    propData.MaxUse = Name + "." + propData.Name;
                                }
                            }
                        }
                    }
                }
            }

            Dictionary<string, PageData> ListOfPages = new Dictionary<string, PageData>();
            Dictionary<string, PageData> ListOfPages2 = new Dictionary<string, PageData>();
            foreach (KeyValuePair<string, PageData> pageData in ListOfGroups["default"].ListOfPages)
            {
                if ((pageData.Value.ListOfGroups != null) && (pageData.Value.ListOfGroups.Count > 0) &&
                    (pageData.Value.ListOfGroups.ContainsKey("default")) && (pageData.Value.ListOfGroups["default"].ListOfProperties != null) &&
                    (pageData.Value.ListOfGroups["default"].ListOfProperties.Count > 0) && (pageData.Value.ListOfGroups["default"].ListOfProperties[0].Typage.Equals("text")))
                {
                    ListOfPages2.Add(pageData.Value.Label, pageData.Value);
                }
                else
                {
                    ListOfPages.Add(pageData.Value.Label, pageData.Value);
                }
            }
            
            foreach(KeyValuePair<string, PageData> pageData in ListOfPages2)
            {
                ListOfPages.Add(pageData.Value.Label, pageData.Value);
            }

            ListOfGroups["default"].ListOfPages = ListOfPages;

            ListOfButton = new List<ButtonData>();
            foreach (KeyValuePair<string, OperationData> operationDataKV in ListOfOperations)
            {
                OperationData operationData = operationDataKV.Value;

                if ((operationData.CallBy.Equals("button")) && (operationData.State.Equals("")))
                {
                    ButtonData buttonData = new ButtonData(operationData);

                    ListOfButton.Add(buttonData);
                }
                else if ((operationData.CallBy.Equals("button")) && (!operationData.State.Equals("")))
                {
                    FunctionModel pythonCode = Utils.GetFunctionModel("function_caller_action_wkf", FunctionUsage.Classic);

                    string[] states = operationData.State.Split(new char[] { '.' });

                    string state = states[1].Trim(); //GetNextState(states[1].Trim());

                    if (!state.Equals(""))
                    {
                        /*Dictionary<string, string> pythonCode_values = new Dictionary<string, string>();
                        pythonCode_values["function_name"] = operationData.Name;
                        pythonCode_values["action_wkf"] = "action_" + state + "_wkf";

                        pythonCode.Content = Utils.replaceValues(pythonCode.Content, pythonCode_values) + "\r\n\t";*/

                        pythonCode.ReplaceValues(operationData.Name, "action_" + state + "_wkf");

                        operationData.PythonCode = pythonCode.Content;
                    }
                }
                else if (!operationData.CallBy.Equals("button") && !operationData.CallBy.Equals(""))
                {
                    PropertyData propertyData = GetProperty(operationData.CallBy);

                    string param = "";
                    foreach (ParameterData parameterData in operationData.ListOfParameter)
                    {
                        param += parameterData.Name + ", ";
                    }

                    if (!param.Equals("")) param = param.Substring(0, param.Length - 2);

                    propertyData.OnChange = operationData.Name + "(" + param + ")";
                }
            }

            ListOfGroups["default"].ListOfButtons = ListOfButton;

            PyGenerator.CreateClass(this);
            PyGenerator.CreateReportModel(this);
            PyGenerator.CreateReportWizardModel(this);

            XmlGenerator.CreateView(this);
            XmlGenerator.CreateReportView(this);
            XmlGenerator.CreateWizardReportView(this, MenuRapport);

            if (CodeProperty != null)
            {
                XmlGenerator.CreateDatas(this);
            }

            if (StateWorkflow)
            {
                XmlGenerator.CreateWorkFlow(this);
            }

            XslGenerator.CreateReport(this);
        }

        public string GetPostState(string state)
        {
            string prev = "";

            foreach (KeyValuePair<string, string> KeyValue in this.StateProperty.SelectionData.ListOfValues)
            {
                if (KeyValue.Key.Equals(state))
                {
                    break;
                }

                prev = KeyValue.Key;
            }

            return prev;
        }

        public string GetNextState(string state)
        {
            string prev = "";
            string rtrn = "";

            foreach (KeyValuePair<string, string> KeyValue in this.StateProperty.SelectionData.ListOfValues)
            {
                if (!prev.Equals(""))
                {
                    rtrn = KeyValue.Key;
                    break;
                }

                if (KeyValue.Key.Equals(state))
                {
                    prev = KeyValue.Key;
                }

                
            }

            return prev;
        }

        public OperationData GetOperation(string typage, string key)
        {
            foreach (KeyValuePair<string, OperationData> operationDataKV in ListOfOperations)
            {
                OperationData operationData = operationDataKV.Value;

                string[] states = operationData.State.Split(new char[]{ '.' });

                if (states.Length == 2)
                {
                    if ((states[0].Trim().ToLower().Equals(typage.ToLower())) && (states[1].Trim().ToLower().Equals(key.ToLower())))
                    {
                        return operationData;
                    }
                }
            }

            return null;
        }

        public PropertyData GetProperty(string name)
        {
            if (name.Equals("id"))
            {
                name = Name + "_id";
            }

            foreach (PropertyData propertyData in ListOfProperties)
            {
                if (propertyData.Name.Equals(name.Trim()))
                {
                    return propertyData;
                }
            }

            return null;
        }

        public MenuData GetMenu(string id)
        {
            foreach (MenuData menuData in ListOfMenu)
            {
                if (menuData.ID.Equals(id.Trim()))
                {
                    return menuData;
                }
            }

            return null;
        }

        public ActionData GetAction(string id)
        {
            foreach (ActionData actionData in ListOfAction)
            {
                if (actionData.ID.Equals(id.Trim()))
                {
                    return actionData;
                }
            }

            return null;
        }

        public ViewData GetView(string id)
        {
            foreach (ViewData viewData in ListOfView)
            {
                if (viewData.ID.Equals(id.Trim()))
                {
                    return viewData;
                }
            }

            return null;
        }

        public FormData GetForm(string id)
        {
            ViewData viewData = GetView(id);

            if ((viewData != null) && (viewData.Kind.Equals("Form")))
            {
                return (FormData)viewData;
            }

            return null;
        }

        public TreeData GetTree(string id)
        {
            ViewData viewData = GetView(id);

            if ((viewData != null) && (viewData.Kind.Equals("Tree")))
            {
                return (TreeData)viewData;
            }

            return null;
        }

        public GraphData GetGraph(string id)
        {
            ViewData viewData = GetView(id);

            if ((viewData != null) && (viewData.Kind.Equals("Graph")))
            {
                return (GraphData)viewData;
            }

            return null;
        }

        public CalendarData GetCalendar(string id)
        {
            ViewData viewData = GetView(id);

            if ((viewData != null) && (viewData.Kind.Equals("Calendar")))
            {
                return (CalendarData)viewData;
            }

            return null;
        }

        public SearchData GetSearch(string id)
        {
            ViewData viewData = GetView(id);

            if ((viewData != null) && (viewData.Kind.Equals("Search")))
            {
                return (SearchData)viewData;
            }

            return null;
        }

        public GanttData GetGantt(string id)
        {
            ViewData viewData = GetView(id);

            if ((viewData != null) && (viewData.Kind.Equals("Gantt")))
            {
                return (GanttData)viewData;
            }

            return null; 
        }

        public override string ToString()
        {
            return Name;
        }

        public List<PropertyData> ListOfProperties { get; set; }

        public List<ActionData> ListOfAction { get; set; }

        public List<ViewData> ListOfView { get; set; }

        public List<MenuData> ListOfMenu { get; set; }

        public string Documentation { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public string xmlView { get; set; }

        public string pyContent { get; set; }

        public ModuleData Parent { get; set; }

        public Dictionary<string, OperationData> ListOfOperations { get; set; }

        public string ID { get; set; }

        public List<LinkData> ListOfLinks { get; set; }

        public bool AssociationClass { get; set; }

        public Dictionary<string, GroupData> ListOfGroups { get; set; }

        public string Menu { get; set; }

        public List<ButtonData> ListOfButton { get; set; }

        public Dictionary<string, string> ListOfConstraints { get; set; }

        public bool InDashBoard { get; set; }

        public Dictionary<string, string> DashboardAction { get; set; }

        public string Columns { get; set; }

        public string DateStart { get; set; }

        public string DateStop { get; set; }

        public string GraphX { get; set; }

        public List<string> GraphY { get; set; }

        public string MenuRapport { get; set; }

        public string Picture { get; set; }

        public Dictionary<string, GroupData> ListOfGroupsReport { get; set; }

        public List<PropertyData> ListOfPropertiesReport { get; set; }

        public bool StateWorkflow { get; set; }

        public PropertyData StateProperty { get; set; }

        public Dictionary<string, ButtonData> ListOfWorkFlowButton { get; set; }

        public Dictionary<string, OperationData> ListOfWorkFlowFunction { get; set; }

        public string Code { get; set; }

        public PropertyData CodeProperty { get; set; }

        public bool Process { get; set; }

        public string ModuleName { get; set; }

        public string Target { get; set; }

        public ClassData Inherit { get; set; }

        public string FormId { get; set; }

        public string TreeId { get; set; }

        public string XpathForm { get; set; }

        public string XpathTree { get; set; }

        public string FormPosition { get; set; }

        public string TreePosition { get; set; }

        public string Accounting { get; set; }

        public string AccountingFunction { get; set; }

        public List<string> AccountingArgs { get; set; }
    }
}
