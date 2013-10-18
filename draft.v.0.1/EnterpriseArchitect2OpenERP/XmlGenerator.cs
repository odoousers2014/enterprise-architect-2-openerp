using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EnterpriseArchitect2OpenERP
{
    public class XmlGenerator
    {
        public static string CreateView(ModuleData moduleData)
        {
            return "";
        }

        public static string CreateView(ClassData classData)
        {
            Dictionary<string, string> openerp_class_viewValues = new Dictionary<string, string>();

            string classview = "";



            if (classData.ListOfView.Count > 0)
            {
                classview += "<!-- [ VIEWS SECTION ] -->\r\n\r\n\t\t";
            }

            foreach (ViewData viewData in classData.ListOfView)
            {
                switch (viewData.Kind)
                {
                    case "Act":
                    {
                        classview += addAct((ActData)viewData);
                        break;
                    }

                    case "Form":
                    {
                        classview += addForm((FormData)viewData);
                        break;
                    }

                    case "Tree":
                    {
                        classview += addTree((TreeData)viewData);
                        break;
                    }

                    case "Graph":
                    {
                        classview += addGraph((GraphData)viewData);
                        break;
                    }

                    case "Calendar":
                    {
                        classview += addCalendar((CalendarData)viewData);
                        break;
                    }

                    case "Search":
                    {
                        classview += addSearch((SearchData)viewData);
                        break;
                    }

                    case "Kanban":
                    {
                        classview += addKanban((KanbanData)viewData);
                        break;
                    }

                    case "Gantt":
                    {
                        //classview += addGantt((GanttData)viewData);
                        break;
                    }
                }
            }

            if (classData.ListOfAction.Count > 0)
            {
                classview += "<!-- [ ACTIONS SECTION ] -->\r\n\r\n\t\t";
            }

            foreach (ActionData actionData in classData.ListOfAction)
            {
                classview += addAction(actionData);
            }

            if (classData.ListOfMenu.Count > 0)
            {
                classview += "<!-- [ MENUS SECTION ] -->\r\n\r\n\t\t";
            }

            foreach (MenuData menuData in classData.ListOfMenu)
            {
                classview += addMenu(menuData);
            }

            openerp_class_viewValues["classview"] = classview;

            string fileXml = classData.Parent.DirectoryInfo_Base.FullName + "/view/" + classData.Parent.ClassicName + "_" + classData.Name + "_view.xml";

            Utils.MakeFileFromModel("openerp_class_view.xml.model", fileXml, openerp_class_viewValues);

            return fileXml;
        }

        public static string CreateReportView(ClassData classData)
        {
            Dictionary<string, string> openerp_class_viewValues = new Dictionary<string, string>();

            string classview = "";

            classview += "<report string=\"" +  Utils.ReplaceSpecialChar(classData.Alias) + "\"\r\n";

            if (classData.Inherit != null)
            {
                classview += "\t\t\tmodel=\"" + classData.Inherit.ModuleName + "." + classData.Inherit.Name + "\"\r\n";
            }
            else
            {
                classview += "\t\t\tmodel=\"" + classData.Parent.ClassicName + "." + classData.Name + "\"\r\n";
            }

            classview += "\t\t\tname=\"" + classData.Parent.ClassicName + "." + classData.Name + ".rml\"\r\n";
            classview += "\t\t\txsl=\"" + classData.Parent.ClassicName + "/report/report_" + classData.Parent.ClassicName + "_" + classData.Name + ".xsl\"\r\n";
            classview += "\t\t\tauto=\"False\"\r\n";
            classview += "\t\t\tid=\"" + classData.Parent.ClassicName + "_" + classData.Name + "_id\"\r\n";
            classview += "\t\t\tmulti=\"1\"\r\n";
            classview += "\t\t/>\r\n";

            openerp_class_viewValues["classview"] = classview;

            string fileXml = classData.Parent.DirectoryInfo_Base.FullName + "/report/report_" + classData.Parent.ClassicName + "_" + classData.Name + ".xml";

            Utils.MakeFileFromModel("openerp_class_view.xml.model", fileXml, openerp_class_viewValues);

            return fileXml;
        }

        public static string CreateProcess(ClassData classData)
        {
            Dictionary<string, string> openerp_module_processValues = new Dictionary<string, string>();

            string moduleprocess = "<record id=\"process_process_" + classData.Parent.ClassicName + "_" + classData.Name + "\" model=\"process.process\">\r\n";
            moduleprocess += "\t\t\t<field eval=\"1\" name=\"active\"/>\r\n";
            moduleprocess += "\t\t\t<field name=\"model_id\" ref=\"" + classData.Parent.ClassicName + ".model_" + classData.Parent.ClassicName + "_" + classData.Name + "\"/>\r\n";
            moduleprocess += "\t\t\t<field eval=\"&quot;&quot;&quot;" + Utils.ReplaceSpecialChar(classData.Alias) + "&quot;&quot;&quot;\" name=\"name\"/>\r\n";
            moduleprocess += "\t\t</record>\r\n";


            openerp_module_processValues["classview"] = moduleprocess;

            string fileXml = classData.Parent.DirectoryInfo_Base.FullName + "/process/process_" + classData.Parent.ClassicName + "_" + classData.Name + ".xml";

            Utils.MakeFileFromModel("openerp_class_view.xml.model", fileXml, openerp_module_processValues);

            return fileXml;
        }

        public static string CreateWizardReportView(ClassData classData, string MenuRapport)
        {
            Dictionary<string, string> openerp_class_viewValues = new Dictionary<string, string>();

            string classview = "";

            Program.ProjectData.addTraduction(classData, "Rapport: " + classData.Alias);
            Program.ProjectData.addTraduction(classData, "Imprimer");
            Program.ProjectData.addTraduction(classData, "Annuler");

            classview += "<record id=\"view_" + classData.Parent.ClassicName + "_" + classData.Name + "_wizard\" model=\"ir.ui.view\">\r\n";

            classview += "\t\t\t<field name=\"name\">Rapport : " + Utils.ReplaceSpecialChar(classData.Alias) + "</field>\r\n";
            classview += "\t\t\t<field name=\"model\">" + classData.Parent.ClassicName + ".wizard." + classData.Name + "</field>\r\n";
            classview += "\t\t\t<field name=\"type\">form</field>\r\n";
            classview += "\t\t\t<field name=\"arch\" type=\"xml\">\r\n";
            classview += "\t\t\t\t<form string=\"" + Utils.ReplaceSpecialChar(classData.Alias) + "\" col=\"" + classData.Columns + "\" version=\"7.0\">\r\n";

            classview += "\t\t\t\t\t<group col=\"" + classData.Columns + "\" colspan=\"" + classData.Columns + "\">\r\n";
            classview += "\t\t\t\t\t\t<separator colspan=\"" + classData.Columns + "\" string=\"" + Utils.ReplaceSpecialChar(classData.Alias) + "\"/>\r\n";
            classview += "\t\t\t\t\t\t<label colspan=\"" + classData.Columns + "\" string=\"Ce rapport permet d'imprimer ou de générer un pdf.\"/>\r\n";

            classview += "\t\t\t\t\t\t<field name=\"typerapport\" colspan=\"2\"/>\r\n";

            classview += "\t\t\t\t\t\t<separator colspan=\"" + classData.Columns + "\"/>\r\n";

            classview += addFormGroup(classData.ListOfGroupsReport["default"], "\t", true, ViewKind.ReportFilter);

            classview += "\t\t\t\t\t</group>\r\n";

            classview += "\t\t\t\t\t<footer>\r\n";
            classview += "\t\t\t\t\t\t<button name=\"print_report\" string=\"Imprimer\" type=\"object\" icon=\"gtk-print\" colspan=\"2\"/>\r\n";
            classview += "\t\t\t\t\t\tou\r\n";
            classview += "\t\t\t\t\t\t<button icon=\"gtk-cancel\" special=\"cancel\" string=\"Annuler\" colspan=\"2\"/>\r\n";
            classview += "\t\t\t\t\t</footer>\r\n";

            classview += "\t\t\t\t</form>\r\n";
            classview += "\t\t\t</field>\r\n";

            classview += "\t\t</record>\r\n";

            classview += "\r\n";

            Program.ProjectData.addTraduction(classData, "Rapport: " + classData.Alias);
            
            classview += "\t\t<record model=\"ir.actions.act_window\" id=\"action_rapport_" + classData.Name + "\">\r\n";
            classview += "\t\t\t<field name=\"name\">Rapport : " + Utils.ReplaceSpecialChar(classData.Alias) + "</field>\r\n";
            classview += "\t\t\t<field name=\"type\">ir.actions.act_window</field>\r\n";
            classview += "\t\t\t<field name=\"res_model\">" + classData.Parent.ClassicName + ".wizard." + classData.Name + "</field>\r\n";
            classview += "\t\t\t<field name=\"view_type\">form</field>\r\n";
            classview += "\t\t\t<field name=\"view_mode\">form</field>\r\n";
            //classview += "\t\t\t<field name=\"view_id\" ref=\"view_" + classData.Parent.ClassicName + "_" + classData.Name + "_wizard\"/>\r\n";
            classview += "\t\t\t<field name=\"target\">new</field>\r\n";
            classview += "\t\t</record>\r\n";

            classview += "\r\n";
            classview += MenuRapport;
            
            openerp_class_viewValues["classview"] = classview;

            string fileXml = classData.Parent.DirectoryInfo_Base.FullName + "/wizard/wizard_" + classData.Parent.ClassicName + "_" + classData.Name + ".xml";

            Utils.MakeFileFromModel("openerp_wizard.xml.model", fileXml, openerp_class_viewValues);

            return fileXml;
        }

        public static string addAct(ActData actData)
        {
            string render = "<act_window\r\n";

            render += "\t\t\tid=\"" + actData.ID + "\"\r\n";
            render += "\t\t\tname=\"" + (string)actData.Data["name"] + "\"\r\n";
            render += "\t\t\tres_model=\"" + (string)actData.Data["res_model"] + "\"\r\n";
            render += "\t\t\tsrc_model=\"" + (string)actData.Data["src_model"] + "\"\r\n";
            render += "\t\t\tview_mode=\"" + (string)actData.Data["view_mode"] + "\"\r\n";
            render += "\t\t\tcontext=\"" + (string)actData.Data["context1"] + "\"\r\n";
            render += "\t\t\tdomain=\"" + (string)actData.Data["domain"] + "\"\r\n";
            render += "\t\t/>\r\n\r\n";

            render += "\t\t<record model=\"ir.actions.act_window\" id=\"" + actData.ID + "\">\r\n";
            render += "\t\t\t<field name=\"name\">" + (string)actData.Data["name"] + "</field>\r\n";
            render += "\t\t\t<field name=\"context\">" + (string)actData.Data["context2"] + "</field>\r\n";
            render += "\t\t</record>\r\n\r\n\r\n\t\t";

            render = "";

            return render;
        }

        public static string addForm(FormData formData)
        {
            string render = "<record model=\"ir.ui.view\" id=\"" + formData.ID + "\">\r\n";

            Program.ProjectData.addTraduction(formData.Parent, (string)formData.Data["name"]);

            render += "\t\t\t<field name=\"name\">" + Utils.ReplaceSpecialChar((string)formData.Data["name"]) + "</field>\r\n";

            if (formData.Parent.Inherit != null)
            {
                render += "\t\t\t<field name=\"model\">" + formData.Parent.Inherit.ModuleName + "." + formData.Parent.Inherit.Name + "</field>\r\n";
            }
            else
            {
                render += "\t\t\t<field name=\"model\">" + (string)formData.RefAction.Data["res_model"] + "</field>\r\n";
            }


            render += "\t\t\t<field name=\"type\">form</field>\r\n";

            if (formData.Parent.Inherit != null)
            {
                string inherit_form = "";

                inherit_form = formData.Parent.Inherit.Parent.ModulePath + ".view_" + formData.Parent.Inherit.Name + "_form";
                if (!formData.Parent.Inherit.FormId.Equals(string.Empty))
                {
                    inherit_form = formData.Parent.Inherit.Parent.ModulePath + "." + formData.Parent.Inherit.FormId;
                }

                if (!formData.Parent.FormId.Equals(string.Empty))
                {
                    inherit_form = formData.Parent.Inherit.Parent.ModulePath + "." + formData.Parent.FormId;
                }

                render += "\t\t\t<field name=\"inherit_id\" ref=\"" + inherit_form + "\"/>\r\n";
            }

            render += "\t\t\t<field name=\"arch\" type=\"xml\">\r\n";

            if (formData.Parent.Inherit != null)
            {
                render += "\t\t\t\t<data>\r\n";
                render += "\t\t\t\t<xpath expr=\"" + formData.Parent.XpathForm + "\" position=\"" + formData.Parent.FormPosition + "\">\r\n";

                render += addFormGroup(formData.Parent, formData.Parent.ListOfGroups["default"], "\t", true, ViewKind.Form);
            }
            else
            {
                render += "\t\t\t\t<form string=\"" + Utils.ReplaceSpecialChar(formData.Parent.Alias) + "\" col=\"" + formData.Parent.Columns + "\" version=\"7.0\">\r\n";

                render += addHeaderForm(formData);

                render += "\t\t\t\t\t<sheet string=\"" + Utils.ReplaceSpecialChar(formData.Parent.Alias) + "\">\r\n";
                render += addFormGroup(formData.Parent, formData.Parent.ListOfGroups["default"], "\t\t", true, ViewKind.Form);
                render += "\t\t\t\t\t</sheet>\r\n";
            }

            render += addFooterForm(formData);

            if (formData.Parent.Inherit != null)
            {
                render += "\t\t\t\t</xpath>\r\n";
                render += "\t\t\t\t</data>\r\n";
            }
            else
            {
                render += "\t\t\t\t</form>\r\n";
            }

            render += "\t\t\t</field>\r\n";
            render += "\t\t</record>\r\n\r\n\t\t";

            return render;
        }

        public static string addFormGroup(GroupData groupData, string decalage, bool first, ViewKind viewKind)
        {
            return addFormGroup(null, groupData, decalage, first, viewKind);
        }

        public static string addFormGroup(ClassData classData, GroupData groupData, string decalage, bool first, ViewKind viewKind)
        {
            string render = "";

            if (classData != null)
            {
                if ((first) && (classData.StateWorkflow))
                {
                    render = "\t\t\t\t" + decalage + "<h1 colspan=\"" + classData.Columns + "\" col=\"" + classData.Columns + "\">\r\n";

                    foreach (KeyValuePair<string, string> state in classData.StateProperty.SelectionData.ListOfValues)
                    {
                        Program.ProjectData.addTraduction(classData, state.Value);
                        Program.ProjectData.addTraduction(classData, classData.Alias + " : " + state.Value);

                        render += "\t\t\t\t\t" + decalage + "<label string=\"" + Utils.ReplaceSpecialChar(classData.Alias + " : " + state.Value) + "\" attrs=\"{'invisible':[('state', 'not in', ['" + state.Key + "'])]}\"/>\r\n";
                    }

                    render += "\t\t\t\t" + decalage + "</h1>\r\n";
                }
            }

            render += "\t\t\t\t" + decalage + "<group colspan=\"" + groupData.ClassData.Columns + "\" col=\"" + groupData.ClassData.Columns + "\">\r\n";

            if (!groupData.Label.Equals("default"))
            {
                if (!first) render += "\t\t\t\t" + decalage + "<newline/>\r\n";
                render += "\t\t\t\t" + decalage + "<separator string=\"" + groupData.Label + "\" colspan=\"" + groupData.ClassData.Columns + "\"/>\r\n";
            }

            render += addGroupProperties(groupData, decalage + "\t", viewKind);

            int columns = int.Parse(groupData.ClassData.Columns);
            if ((groupData.ListOfProperties.Count < 3) && (columns >= 4))
            {
                int n = columns - groupData.ListOfProperties.Count * 2;
                render += "\t\t\t\t" + decalage + "<label colspan=\"" + n.ToString() + "\" string=\"\"/>\r\n";
            }

            render += "\t\t\t\t" + decalage + "</group>\r\n";

            render += addGroupPages(groupData.ListOfPages, decalage, viewKind);

            /*if ((!groupData.Report) && (groupData.Root))
            {
                render += addWidgetFormGroup(classData, groupData, decalage + "\t");
            }*/

            return render;
        }

        private static string addFooterForm(FormData formData)
        {
            string render = "";
            GroupData groupData = formData.Parent.ListOfGroups["default"];

            ClassData classData = formData.Parent;

            if (classData.StateWorkflow)
            {
                render += "\t\t\t\t\t" + "<footer>\r\n";

                if (groupData.ListOfButtons.Count > 0)
                {
                    render += addGroupButtons(groupData.ListOfButtons, "\t");
                }

                render += "\t\t\t\t\t" + "</footer>\r\n";
            }

            return render;
        }

        private static string addGroupPages(Dictionary<string, PageData> pages, string decalage, ViewKind viewKind)
        {
            string render = "";

            if (pages.Count > 0)
            {
                string Columns = "6";

                ClassData classData = null;
                foreach (KeyValuePair<string, PageData> page in pages)
                {
                    foreach (KeyValuePair<string, GroupData> group in page.Value.ListOfGroups)
                    {
                        classData = group.Value.ClassData;

                        if (classData != null) break;
                    }

                    if (classData != null) break;
                }

                if (classData != null) Columns = classData.Columns;

                render += "\t\t\t\t" + decalage + "<notebook colspan=\"" + Columns + "\">\r\n";

                foreach (KeyValuePair<string, PageData> page in pages)
                {
                    render += "\t\t\t\t" + decalage + "\t" + "<page string=\"" + Utils.ReplaceSpecialChar(page.Value.Label) + "\">\r\n";

                    bool first = true;
                    foreach (KeyValuePair<string, GroupData> groupData in page.Value.ListOfGroups)
                    {
                        render += addFormGroup(groupData.Value, decalage + "\t\t", first, viewKind);
                        first = false;
                    }

                    render += "\t\t\t\t" + decalage + "\t" + "</page>\r\n";
                }

                render += "\t\t\t\t" + decalage + "</notebook>\r\n";
            }

            return render;
        }

        private static string addHeaderForm(FormData formData)
        {
            string render = "";

            ClassData classData = formData.Parent;

            if (classData.StateWorkflow)
            {
                render += "\t\t\t\t\t" + "<header>\r\n";

                string states = "";

                Dictionary<string, ButtonData> ListOfWorkFlowButton = new Dictionary<string, ButtonData>();
                foreach (KeyValuePair<string, string> state in classData.StateProperty.SelectionData.ListOfValues)
                {
                    foreach (KeyValuePair<string, ButtonData> buttonData in classData.ListOfWorkFlowButton)
                    {
                        if (buttonData.Value.States.Equals(state.Key))
                        {
                            ListOfWorkFlowButton.Add(buttonData.Key, buttonData.Value);
                            break;
                        }
                    }

                    states += ((states.Length > 0) ? "," : "") + state.Key;
                }

                if (!classData.AccountingFunction.Equals(""))
                {
                    if (classData.AccountingArgs.Count > 0)
                    {
                        Dictionary<string, string> accountArgs = Utils.FunctionArgs(classData.AccountingArgs);

                        string state = Utils.GetArg(accountArgs, "state", "");
                        string button = Utils.GetArg(accountArgs, "button", "");

                        if (!state.Equals(""))
                        {
                            state = " states=\"" + state + "\"";
                        }

                        if (!button.Equals(""))
                        {
                            render += "\t\t\t\t\t\t" + "<button name=\"button_cancel\" string=\"" + button + "\"" + state + "/>\r\n";
                            render += "\t\t\t\t\t\t" + "<!-- -->\r\n";
                        }
                    }
                }

                ButtonData prevButtonData = null;
                int i = 1;
                foreach (KeyValuePair<string, ButtonData> buttonData in ListOfWorkFlowButton)
                {
                    if ((i > 1) && (buttonData.Value.RenderXML))
                    {
                        render += "\t\t\t\t\t\t" + "<button name=\"" + buttonData.Value.Name + "\" string=\"" + Utils.ReplaceSpecialChar(buttonData.Value.Text) + "\" icon=\"gtk-apply\" type=\"object\" states=\"" + prevButtonData.States + "\"/>\r\n";
                    }

                    prevButtonData = buttonData.Value;
                    i++;
                }

                Program.ProjectData.addTraduction(classData, "Annuler");

                render += "\t\t\t\t\t\t" + "<button name=\"button_cancel\" string=\"Annuler\" icon=\"gtk-cancel\" states=\"" + states + "\"/>\r\n";
                render += "\t\t\t\t\t\t" + "<!-- -->\r\n";
                render += "\t\t\t\t\t\t" + "<field name=\"state\" nolabel=\"1\" widget=\"statusbar\" statusbar_visible=\"" + classData.StateProperty.StatusBar + "\" statusbar_colors='{\"auto\":\"blue\"}'/>\r\n";

                render += "\t\t\t\t\t" + "</header>\r\n";
            }

            return render;
        }

        /*public static string addWidgetFormGroup(ClassData classData, GroupData groupData, string decalage)
        {
            string render = "";

            if (groupData.ListOfButtons.Count > 0)
            {
                render += "\t\t\t\t" + decalage + "<newline/>\r\n";

                render += "\t\t\t\t" + decalage + "<group colspan=\"" + classData.Columns + "\" col=\"" + classData.Columns + "\">\r\n";

                if (groupData.ListOfButtons.Count > 0)
                {
                    render += addGroupButtons(groupData.ListOfButtons, decalage);
                }

                render += "\t\t\t\t" + decalage + "</group>\r\n";
            }

            return render;
        }*/

        private static string addGroupButtons(List<ButtonData> buttons, string decalage)
        {
            string render = "";

            if (buttons.Count > 0)
            {
                Program.ProjectData.addTraduction(buttons[0].Parent.Parent, "Opérations");
            }

            render += "\t\t\t\t\t" + decalage + "<group colspan=\"3\" col=\"3\">\r\n";
            render += "\t\t\t\t\t\t" + decalage + "<separator string=\"Operations\" colspan=\"3\"/>\r\n";

            foreach (ButtonData buttonData in buttons)
            {
                string type = " type=\"" + buttonData.Typage + "\"";
                string states = "";
                string confirm = "";
                string groups = "";

                Program.ProjectData.addTraduction(buttonData.Parent.Parent, buttonData.Label);

                render += "\t\t\t\t\t\t" + decalage + "<button name=\"" + buttonData.OnClick + "\" string=\"" + Utils.ReplaceSpecialChar(buttonData.Label) + "\"" + type + states + confirm + groups + "/>\r\n";
            }

            render += "\t\t\t\t\t" + decalage + "</group>\r\n";

            return render;
        }

        private static string addGroupProperties(GroupData groupData, string decalage, ViewKind viewKind)
        {
            string render = "";
            List<PropertyData> properties = groupData.ListOfProperties;
            List<PropertyData> oe_properties = new List<PropertyData>();

            if ((groupData.Root) && (!groupData.Report))
            {
                List<PropertyData> no_oe_properties = new List<PropertyData>();
                foreach (PropertyData propertyData in properties)
                {
                    if (!propertyData.ShowInView)
                    {
                        continue;
                    }

                    if (propertyData.OEtitle > 0)
                    {
                        oe_properties.Add(propertyData);
                    }
                    else
                    {
                        no_oe_properties.Add(propertyData);
                    }
                }

                if (oe_properties.Count > 0)
                {
                    List<PropertyData> oe = new List<PropertyData>(oe_properties.Count);
                    foreach (PropertyData propertyData in oe_properties)
                    {
                        oe.Add(null);
                    }

                    foreach (PropertyData propertyData in oe_properties)
                    {
                        oe[propertyData.OEtitle - 1] = propertyData;
                    }
                    oe_properties = oe;

                    render += "\t\t\t\t" + decalage + "<group colspan=\"" + groupData.ClassData.Columns + "\" col=\"1\">\r\n";
                    render += "\t\t\t\t\t" + decalage + "<div class=\"oe_title\">\r\n";
                    render += "\t\t\t\t\t\t" + decalage + "<div class=\"oe_edit_only\">\r\n";

                    render += "\t\t\t\t\t\t\t" + decalage;

                    string suite = ", ";
                    for (int i = 0; i < oe_properties.Count; i++)
                    {
                        if (i == oe_properties.Count - 2)
                        {
                            suite = " et ";
                        }

                        if (i == oe_properties.Count - 1)
                        {
                            suite = "";
                        }

                        //render += "\t\t\t\t\t\t\t" + decalage + "<label for=\"" + oe_properties[i].Name + "\"/>" + suite + "\r\n";
                        render += "<b>" + oe_properties[i].Alias + "</b>" + suite;
                    }

                    render += "\r\n";

                    render += "\t\t\t\t\t\t" + decalage + "</div>\r\n";
                    render += "\t\t\t\t\t\t" + decalage + "<h1>\r\n";
                    render += "\t\t\t\t\t\t\t" + decalage + "<field name=\"" + oe_properties[0].Name + "\" default_focus=\"1\" placeholder=\"" +  Utils.ReplaceSpecialChar(oe_properties[0].Alias) + "\"/>\r\n";
                    render += "\t\t\t\t\t\t" + decalage + "</h1>\r\n";

                    for (int i = 1; i < oe_properties.Count; i++)
                    {
                        PropertyData propertyData = oe_properties[i];

                        if (!propertyData.Typage.Equals("many2many") && !propertyData.Typage.Equals("one2many"))
                        {
                            string nolabel = /*(propertyData.ShowLabel) ? "" :*/ " nolabel=\"1\"";

                            string colspan = "";
                            string unwritable = "";
                            string onchange = "";
                            string states = "";

                            if (!viewKind.Equals(ViewKind.ReportFilter))
                            {
                                //colspan = (propertyData.ColSpan.Equals("")) ? " colspan=\"2\"" : " colspan=\"" + propertyData.ColSpan + "\"";
                                unwritable = (propertyData.Readonly.Equals("")) ? "" : " readonly=\"" + propertyData.Readonly + "\"";
                                onchange = (propertyData.OnChange.Equals("")) ? "" : " on_change=\"" + propertyData.OnChange + "\"";
                                states = (propertyData.States.Equals("")) ? "" : " states=\"" + propertyData.States + "\"";
                            }

                            string select = (propertyData.Select.Equals("")) ? "" : " select=\"" + propertyData.Select + "\"";

                            string default_focus = "";
                            if (propertyData.Name.Equals("name"))
                            {
                                default_focus = " default_focus=\"1\"";
                            }

                            //render += "\t\t\t\t\t" + decalage + "<label for=\"" + propertyData.Name + "\"/>\r\n";
                            render += "\t\t\t\t\t" + decalage + "<field name=\"" + propertyData.Name + "\" placeholder=\"" + Utils.ReplaceSpecialChar(propertyData.Alias) + "\" " + nolabel + select + colspan + unwritable + onchange + states + default_focus + "/>\r\n";

                            if (propertyData.Name.Equals("code"))
                            {
                                //render += "\t\t\t\t" + decalage + "<label colspan=\"4\"/>\r\n";
                                //render += "\t\t\t\t" + decalage + "<separator colspan=\"" + classData.Columns + "\"/>\r\n";
                                //render += "\t\t\t\t" + decalage + "<newline/>\r\n";
                            }
                        }
                    }

                    render += "\t\t\t\t\t" + decalage + "</div>\r\n";
                    render += "\t\t\t\t\t" + decalage + "<div style='border:1'></div>\r\n";
                    render += "\t\t\t\t" + decalage + "</group>\r\n";
                    render += "\t\t\t\t" + decalage + "<separator colspan=\"" + groupData.ClassData.Columns + "\" style=\"border-top: 1px solid gainsboro;\"/>\r\n";

                    properties = no_oe_properties;
                }
            }

            List<PropertyData> maxuse_properties = new List<PropertyData>();
            foreach (PropertyData propertyData in properties)
            {
                if (!propertyData.MaxUse.Equals(""))
                {
                    maxuse_properties.Add(propertyData);
                }
            }

            foreach (PropertyData propertyData in properties)
            {
                if (!propertyData.Typage.Equals("many2many") && !propertyData.Typage.Equals("one2many"))
                {
                    string nolabel = (propertyData.ShowLabel) ? "" : " nolabel=\"1\"";

                    string colspan = "";
                    string unwritable = "";
                    string onchange = "";
                    string states = "";

                    if (!viewKind.Equals(ViewKind.ReportFilter))
                    {
                        colspan = (propertyData.ColSpan.Equals("")) ? " colspan=\"2\"" : " colspan=\"" + propertyData.ColSpan + "\"";
                        unwritable = (propertyData.Readonly.Equals("")) ? "" : " readonly=\"" + propertyData.Readonly + "\"";
                        onchange = (propertyData.OnChange.Equals("")) ? "" : " on_change=\"" + propertyData.OnChange + "\"";
                        states = (propertyData.States.Equals("")) ? "" : " states=\"" + propertyData.States + "\"";
                    }

                    string select = (propertyData.Select.Equals("")) ? "" : " select=\"" + propertyData.Select + "\"";

                    string default_focus = "";
                    if (propertyData.Name.Equals("name"))
                    {
                        default_focus = " default_focus=\"1\"";
                    }

                    if (!propertyData.MaxUse.Equals(""))
                    {
                        string[] maxuse = propertyData.MaxUse.Split(new char[] { '.' });

                        onchange = " on_change=\"onchange_" + propertyData.Name + "(" + maxuse[0] + "_id, " + propertyData.Name + ")\"";

                        render += "\t\t\t\t" + decalage + "<label for=\"" + propertyData.Name + "\"/>\r\n";
                        render += "\t\t\t\t" + decalage + "<div>\r\n";
                        render += "\t\t\t\t\t" + decalage + "<field name=\"" + propertyData.Name + "\"" + nolabel + select + unwritable + onchange + states + default_focus + "/>\r\n";
                        render += "\t\t\t\t\t" + decalage + "<field name=\"" + propertyData.Name + "_label\"/>\r\n"; //string=\"Valeur maximum autorisee : \"/>\r\n";
                        render += "\t\t\t\t" + decalage + "</div>\r\n";
                    }
                    else
                    {
                        if (propertyData.Typage.Equals("many2one"))
                        {
                            foreach (PropertyData propData in maxuse_properties)
                            {
                                string[] maxuse = propData.MaxUse.Split(new char[] { '.' });
                                string[] linkmanyclass = propertyData.LinkManyClass.Split(new char[] { '.' });

                                if (maxuse[0].Equals(linkmanyclass[1]))
                                {
                                    onchange = " on_change=\"onchange_" + propertyData.Name + "(" + propertyData.Name + ", " + propData.Name + ")\"";
                                }
                            }
                        }

                        render += "\t\t\t\t" + decalage + "<field name=\"" + propertyData.Name + "\"" + nolabel + select + colspan + unwritable + onchange + states + default_focus + "/>\r\n";
                    }
                    
                    if (propertyData.Name.Equals("code"))
                    {
                        //render += "\t\t\t\t" + decalage + "<label colspan=\"4\"/>\r\n";
                        //render += "\t\t\t\t" + decalage + "<separator colspan=\"" + classData.Columns + "\"/>\r\n";
                        //render += "\t\t\t\t" + decalage + "<newline/>\r\n";
                    }
                }
            }

            foreach (KeyValuePair<string, GroupData> group in groupData.ListOfGroups)
            {
                render += addFormGroup(group.Value, "\t\t", false, viewKind);
            }

            foreach (PropertyData propertyData in properties)
            {
                if (propertyData.Typage.Equals("many2many"))
                {
                    string colspan = (propertyData.ColSpan.Equals("")) ? " colspan=\"" + groupData.ClassData.Columns + "\"" : " colspan=\"" + propertyData.ColSpan + "\"";
                    string unwritable = (propertyData.Readonly.Equals("")) ? "" : " readonly=\"" + propertyData.Readonly + "\"";
                    string onchange = (propertyData.OnChange.Equals("")) ? "" : " on_change=\"" + propertyData.OnChange + "\"";
                    string select = (propertyData.Select.Equals("")) ? "" : " select=\"" + propertyData.Select + "\"";
                    string states = (propertyData.States.Equals("")) ? "" : " states=\"" + propertyData.States + "\"";

                    if (propertyData.ShowLabel)
                    {
                        render += "\t\t\t\t" + decalage + "<newline/>\r\n";
                        render += "\t\t\t\t" + decalage + "<separator string=\"" + Utils.ReplaceSpecialChar(propertyData.Alias) + "\"" + colspan + "/>\r\n";
                    }

                    //render += "\t\t\t\t" + decalage + "<field name=\"" + propertyData.Name + "\"" + colspan + unwritable + onchange + " nolabel=\"1\"/>\r\n";

                    render += "\t\t\t\t" + decalage + "<field" + colspan + " name=\"" + propertyData.Name + "\" nolabel=\"1\" widget=\"one2many_list\"" + select + states + unwritable + onchange +">\r\n";

                    if (propertyData.ClassLink != null)
                    {
                        if (!propertyData.ClassLink.Parent.Native)
                        {
                            render += "\t\t\t\t\t" + decalage + "<form string=\"" + Utils.ReplaceSpecialChar(propertyData.Alias) + "\">\r\n";

                            foreach (PropertyData prop in propertyData.ClassLink.ListOfProperties)
                            {
                                if (!prop.Name.Equals(propertyData.ClassLink.Name + "_id"))
                                {
                                    if (prop.ShowInTree)
                                    {
                                        render += "\t\t\t\t\t\t" + decalage + "<field name=\"" + prop.Name + "\"/>\r\n";
                                    }
                                }
                            }

                            render += "\t\t\t\t\t" + decalage + "</form>\r\n";
                        }
                    }

                    render += "\t\t\t\t" + decalage + "</field>\r\n";
                }
            }

            foreach (PropertyData propertyData in properties)
            {
                if (propertyData.Typage.Equals("one2many"))
                {
                    string colspan = (propertyData.ColSpan.Equals("")) ? " colspan=\"" + groupData.ClassData.Columns + "\"" : " colspan=\"" + propertyData.ColSpan + "\"";
                    string unwritable = (propertyData.Readonly.Equals("")) ? "" : " readonly=\"" + propertyData.Readonly + "\"";
                    string onchange = (propertyData.OnChange.Equals("")) ? "" : " on_change=\"" + propertyData.OnChange + "\"";
                    string select = (propertyData.Select.Equals("")) ? "" : " select=\"" + propertyData.Select + "\"";
                    string states = (propertyData.States.Equals("")) ? "" : " states=\"" + propertyData.States + "\"";

                    if (propertyData.ShowLabel)
                    {
                        render += "\t\t\t\t" + decalage + "<newline/>\r\n";
                        render += "\t\t\t\t" + decalage + "<separator string=\"" + Utils.ReplaceSpecialChar(propertyData.Alias) + "\"" + colspan + "/>\r\n";
                    }

                    //render += "\t\t\t\t" + decalage + "<field name=\"" + propertyData.Name + "\"" + colspan + " nolabel=\"1\"/>\r\n";

                    if ((propertyData.ClassLink != null) && (!propertyData.ClassLink.Parent.Native))
                    {
                        if (!states.Equals(""))
                        {
                            states = "";

                            if (!propertyData.Readonly.Equals("1"))
                            {
                                string[] sts = propertyData.States.Split(new char[] { ',' });
                                string st = "";
                                for (int i = 0; i < sts.Length; i++)
                                {
                                    st = "'" + sts[i].Trim() + "'" + ((i == sts.Length - 1)? "":",");
                                }

                                unwritable = " attrs=\"{'readonly':[('state', 'not in', (" + st + "))]}\"";
                            }
                        }
                    }

                    render += "\t\t\t\t" + decalage + "<field" + colspan + " name=\"" + propertyData.Name + "\" nolabel=\"1\" widget=\"one2many_list\"" + select + states + unwritable + onchange + ">\r\n";

                    if (propertyData.ClassLink != null)
                    {
                        if (!propertyData.ClassLink.Parent.Native)
                        {
                            string editable = "";

                            if (propertyData.TreeEditable)
                            {
                                editable = " editable=\"top\"";
                            }

                            render += "\t\t\t\t\t" + decalage + "<tree string=\"" + Utils.ReplaceSpecialChar(propertyData.Alias) + "\"" + editable + ">\r\n";

                            foreach (PropertyData prop in propertyData.ClassLink.ListOfProperties)
                            {
                                if ((!prop.Name.Equals(propertyData.ClassLink.Name + "_id")) && (!prop.Name.Equals(propertyData.Parent.Name + "_id")))
                                {
                                    if (prop.ShowInTree)
                                    {
                                        render += "\t\t\t\t\t\t" + decalage + "<field name=\"" + prop.Name + "\"/>\r\n";
                                    }
                                }
                            }

                            render += "\t\t\t\t\t" + decalage + "</tree>\r\n";
                        }
                    }

                    render += "\t\t\t\t" + decalage + "</field>\r\n";
                }
            }

            return render;
        }

        public static string addCalendar(CalendarData calendarData)
        {
            string render = "<record model=\"ir.ui.view\" id=\"" + calendarData.ID + "\">\r\n";

            string date_stop = (calendarData.Parent.DateStop.Equals("")) ? "" : " date_stop=\"" + calendarData.Parent.DateStop + "\"";

            render += "\t\t\t<field name=\"name\">" + (string)calendarData.Data["name"] + "</field>\r\n";
            render += "\t\t\t<field name=\"model\">" + (string)calendarData.RefAction.Data["res_model"] + "</field>\r\n";
            render += "\t\t\t<field name=\"type\">calendar</field>\r\n";
            render += "\t\t\t<field name=\"arch\" type=\"xml\">\r\n";
            render += "\t\t\t\t<calendar string=\"" +  Utils.ReplaceSpecialChar(calendarData.Parent.Alias) + "\" date_start=\"" + calendarData.Parent.DateStart + "\"" + date_stop + ">\r\n";
            render += "\t\t\t\t\t<field name=\"name\"/>\r\n";
            render += "\t\t\t\t</calendar>\r\n";
            render += "\t\t\t</field>\r\n";
            render += "\t\t</record>\r\n\r\n\t\t";

            return render;
        }

        public static string addTree(TreeData treeData)
        {
            string render = "<record model=\"ir.ui.view\" id=\"" + treeData.ID + "\">\r\n";

            Program.ProjectData.addTraduction(treeData.Parent, (string)treeData.Data["name"]);

            render += "\t\t\t<field name=\"name\">" + Utils.ReplaceSpecialChar((string)treeData.Data["name"]) + "</field>\r\n";

            if (treeData.Parent.Inherit != null)
            {
                render += "\t\t\t<field name=\"model\">" + treeData.Parent.Inherit.ModuleName + "." + treeData.Parent.Inherit.Name + "</field>\r\n";
            }
            else
            {
                render += "\t\t\t<field name=\"model\">" + (string)treeData.RefAction.Data["res_model"] + "</field>\r\n";
            }

            render += "\t\t\t<field name=\"type\">tree</field>\r\n";

            if (treeData.Parent.Inherit != null)
            {
                string inherit_tree = "";

                inherit_tree = treeData.Parent.Inherit.Parent.ModulePath + ".view_" + treeData.Parent.Inherit.Name + "_tree";
                if (!treeData.Parent.Inherit.TreeId.Equals(string.Empty))
                {
                    inherit_tree = treeData.Parent.Inherit.Parent.ModulePath + "." + treeData.Parent.Inherit.TreeId;
                }

                if (!treeData.Parent.TreeId.Equals(string.Empty))
                {
                    inherit_tree = treeData.Parent.Inherit.Parent.ModulePath + "." + treeData.Parent.TreeId;
                }

                render += "\t\t\t<field name=\"inherit_id\" ref=\"" + inherit_tree + "\"/>\r\n";
            }

            render += "\t\t\t<field name=\"arch\" type=\"xml\">\r\n";

            if (treeData.Parent.Inherit != null)
            {
                render += "\t\t\t\t<data>\r\n";
                render += "\t\t\t\t<xpath expr=\"" + treeData.Parent.XpathTree + "\" position=\"" + treeData.Parent.TreePosition + "\">\r\n";
            }
            else
            {
                render += "\t\t\t\t<tree string=\"" +  Utils.ReplaceSpecialChar(treeData.Parent.Alias) + "\">\r\n";
            }

            foreach (PropertyData propertyData in treeData.Parent.ListOfProperties)
            {
                if (!propertyData.Name.Equals(treeData.Parent.Name + "_id"))
                {
                    if (propertyData.ShowInTree)
                    {
                        render += "\t\t\t\t\t<field name=\"" + propertyData.Name + "\"/>\r\n";
                    }
                }
            }

            if (treeData.Parent.Inherit != null)
            {
                render += "\t\t\t\t</xpath>\r\n";
                render += "\t\t\t\t</data>\r\n";
            }
            else
            {
                render += "\t\t\t\t</tree>\r\n";
            }

            render += "\t\t\t</field>\r\n";
            render += "\t\t</record>\r\n\r\n\t\t";

            return render;
        }

        public static string addGraph(GraphData graphData)
        {
            string render = "<record model=\"ir.ui.view\" id=\"" + graphData.ID + "\">\r\n";

            Program.ProjectData.addTraduction(graphData.Parent, (string)graphData.Data["name"]);

            render += "\t\t\t<field name=\"name\">" + Utils.ReplaceSpecialChar((string)graphData.Data["name"]) + "</field>\r\n";

            if (graphData.Parent.Inherit != null)
            {
                render += "\t\t\t<field name=\"model\">" + graphData.Parent.Inherit.ModuleName + "." + graphData.Parent.Inherit.Name + "</field>\r\n";
            }
            else
            {
                render += "\t\t\t<field name=\"model\">" + (string)graphData.RefAction.Data["res_model"] + "</field>\r\n";
            }

            render += "\t\t\t<field name=\"type\">graph</field>\r\n";
            render += "\t\t\t<field name=\"arch\" type=\"xml\">\r\n";
            render += "\t\t\t\t<graph string=\"" +  Utils.ReplaceSpecialChar(graphData.Parent.Alias) + "\" type=\"bar\">\r\n";

            render += "\t\t\t\t\t<field name=\"" + graphData.Parent.GraphX + "\"/>\r\n";

            foreach (string graphy in graphData.Parent.GraphY)
            {
                render += "\t\t\t\t\t<field name=\"" + graphy + "\" operator=\"+\"/>\r\n";
            }

            render += "\t\t\t\t</graph>\r\n";
            render += "\t\t\t</field>\r\n";
            render += "\t\t</record>\r\n\r\n\t\t";

            return render;
        }

        public static string addSearch(SearchData searchData)
        {
            string render = "<record model=\"ir.ui.view\" id=\"" + searchData.ID + "\">\r\n";

            Program.ProjectData.addTraduction(searchData.Parent, (string)searchData.Data["name"]);

            render += "\t\t\t<field name=\"name\">" + Utils.ReplaceSpecialChar((string)searchData.Data["name"]) + "</field>\r\n";
            
            if (searchData.Parent.Inherit != null)
            {
                render += "\t\t\t<field name=\"model\">" + searchData.Parent.Inherit.ModuleName + "." + searchData.Parent.Inherit.Name + "</field>\r\n";
            }
            else
            {
                render += "\t\t\t<field name=\"model\">" + (string)searchData.RefAction.Data["res_model"] + "</field>\r\n";
            }

            render += "\t\t\t<field name=\"type\">search</field>\r\n";
            render += "\t\t\t<field name=\"arch\" type=\"xml\">\r\n";
            render += "\t\t\t\t<search string=\"" +  Utils.ReplaceSpecialChar(searchData.Parent.Alias) + "\">\r\n";

            string selection = "";
            foreach (PropertyData propertyData in searchData.Parent.ListOfProperties)
            {
                if (!propertyData.Name.Equals(searchData.Parent.Name + "_id"))
                {
                    if (propertyData.Criteria)
                    {
                        render += "\t\t\t\t\t<field name=\"" + propertyData.Name + "\" select=\"1\"/>\r\n";
                    }
                    else if (propertyData.Typage.Equals("selection"))
                    {
                        selection += "\t\t\t\t\t<field name=\"" + propertyData.Name + "\" select=\"1\"/>\r\n";
                    }
                }
            }

            render += (selection.Equals(""))? "" : "\t\t\t\t\t<newline/>\r\n" + selection;

            List<PropertyData> listOfPropertyID = new List<PropertyData>();
            List<PropertyData> listOfPropertySel = new List<PropertyData>();
            
            foreach (PropertyData propertyData in searchData.Parent.ListOfProperties)
            {
                if (!propertyData.Name.Equals(searchData.Parent.Name + "_id"))
                {
                    if (propertyData.Name.Substring(propertyData.Name.Length - 3).Equals("_id"))
                    {
                        listOfPropertyID.Add(propertyData);
                    }
                    else if (propertyData.Typage.Equals("selection"))
                    {
                        listOfPropertySel.Add(propertyData);
                    }
                }
            }

            string groupby = "";
            foreach (PropertyData propertyData in listOfPropertyID)
            {
                if (groupby.Equals(""))
                {
                    groupby += "\t\t\t\t\t<newline/>\r\n";
                    groupby += "\t\t\t\t\t<group expand=\"0\" string=\"Regrouper par ...\">\r\n";
                }

                groupby += "\t\t\t\t\t\t<filter string=\"" +  Utils.ReplaceSpecialChar(propertyData.Alias) + "\" domain=\"[]\" context=\"{'group_by':'" + propertyData.Name + "'}\"/>\r\n";
            }

            if ((listOfPropertySel.Count > 0) && (!groupby.Equals("")))
            {
                groupby += "\t\t\t\t\t\t<separator orientation=\"vertical\"/>\r\n";
            }
            
            foreach (PropertyData propertyData in listOfPropertySel)
            {
                if (groupby.Equals(""))
                {
                    groupby += "\t\t\t\t\t<newline/>\r\n";
                    groupby += "\t\t\t\t\t<group expand=\"0\" string=\"Regrouper par ...\">\r\n";
                }

                groupby += "\t\t\t\t\t\t<filter string=\"" +  Utils.ReplaceSpecialChar(propertyData.Alias) + "\" domain=\"[]\" context=\"{'group_by':'" + propertyData.Name + "'}\"/>\r\n";
            }

            if (!groupby.Equals(""))
            {
                groupby += "\t\t\t\t\t</group>\r\n";
            }

            render += groupby;

            render += "\t\t\t\t</search>\r\n";
            render += "\t\t\t</field>\r\n";
            render += "\t\t</record>\r\n\r\n\t\t";

            return render;
        }

        public static string addGantt(GanttData ganttData)
        {
            string render = "<record model=\"ir.ui.view\" id=\"" + ganttData.ID + "\">\r\n";

            Program.ProjectData.addTraduction(ganttData.Parent, (string)ganttData.Data["name"]);

            render += "\t\t\t<field name=\"name\">" + Utils.ReplaceSpecialChar((string)ganttData.Data["name"]) + "</field>\r\n";

            if (ganttData.Parent.Inherit != null)
            {
                render += "\t\t\t<field name=\"model\">" + ganttData.Parent.Inherit.ModuleName + "." + ganttData.Parent.Inherit.Name + "</field>\r\n";
            }
            else
            {
                render += "\t\t\t<field name=\"model\">" + (string)ganttData.RefAction.Data["res_model"] + "</field>\r\n";
            }

            render += "\t\t\t<field name=\"type\">gantt</field>\r\n";
            render += "\t\t\t<field name=\"arch\" type=\"xml\">\r\n";
            render += "\t\t\t\t<gantt string=\"" +  Utils.ReplaceSpecialChar(ganttData.Parent.Alias) + "\">\r\n";
            render += "\t\t\t\t\t<!-- -->\r\n";
            render += "\t\t\t\t</gantt>\r\n";
            render += "\t\t\t</field>\r\n";
            render += "\t\t</record>\r\n\r\n\t\t";

            return render;
        }

        public static string addKanban(KanbanData kanbanData)
        {
            string render = "";

            return render;
        }

        public static string addAction(ActionData actionData)
        {
            string render = "<record model=\"" + (string)actionData.Data["model"] + "\" id=\"" + actionData.ID + "\">\r\n";

            Program.ProjectData.addTraduction(actionData.Parent, (string)actionData.Data["name"]);

            if (((string)actionData.Data["name"]).Length > 0) render += "\t\t\t<field name=\"name\">" + Utils.ReplaceSpecialChar((string)actionData.Data["name"]) + "</field>\r\n";
            if (((string)actionData.Data["view_id"]).Length > 0) render += "\t\t\t<field name=\"view_id\" ref=\"" + (string)actionData.Data["view_id"] + "\"/>\r\n";
            if (((string)actionData.Data["domain"]).Length > 0) render += "\t\t\t<field name=\"domain\">" + (string)actionData.Data["domain"] + "</field>\r\n";
            if (((string)actionData.Data["sequence"]).Length > 0) render += "\t\t\t<field name=\"sequence\">" + (string)actionData.Data["sequence"] + "</field>\r\n";
            if (((string)actionData.Data["context"]).Length > 0) render += "\t\t\t<field name=\"context\">" + (string)actionData.Data["context"] + "</field>\r\n";

            if (actionData.Parent.Inherit != null)
            {
                if (((string)actionData.Data["res_model"]).Length > 0) render += "\t\t\t<field name=\"res_model\">" + actionData.Parent.Inherit.ModuleName + "." + actionData.Parent.Inherit.Name + "</field>\r\n";
            }
            else
            {
                if (((string)actionData.Data["res_model"]).Length > 0) render += "\t\t\t<field name=\"res_model\">" + (string)actionData.Data["res_model"] + "</field>\r\n";
            }
            
            if (((string)actionData.Data["view_type"]).Length > 0) render += "\t\t\t<field name=\"view_type\">" + (string)actionData.Data["view_type"] + "</field>\r\n";
            if (((string)actionData.Data["view_mode"]).Length > 0) render += "\t\t\t<field name=\"view_mode\">" + (string)actionData.Data["view_mode"] + "</field>\r\n";
            if (((string)actionData.Data["target"]).Length > 0) render += "\t\t\t<!--<field name=\"target\">" + (string)actionData.Data["target"] + "</field>-->\r\n";

            if (actionData.Parent.Inherit == null)
            {
                if (((string)actionData.Data["search_view_id"]).Length > 0) render += "\t\t\t<field name=\"search_view_id\" ref=\"" + (string)actionData.Data["search_view_id"] + "\"/>\r\n";
            }

            if (((string)actionData.Data["act_window_id"]).Length > 0) render += "\t\t\t<field name=\"act_window_id\" ref=\"" + (string)actionData.Data["act_window_id"] + "\"/>\r\n";

            render += "\t\t</record>\r\n\r\n\t\t";

            return render;
        }

        public static string addMenu(MenuData menuData)
        {
            string icon = (menuData.Icon.Equals(""))? "" : " icon=\"" + menuData.Icon + "\"";
            string groups = (menuData.Groups.Equals("")) ? "" : " groups=\"" + menuData.Groups + "\"";
            string sequence = (menuData.Sequence.Equals(""))? "" : " sequence=\"" + menuData.Sequence + "\"";

            Program.ProjectData.addTraduction(menuData.Parent, (string)menuData.Name);

            string render = "\r\n\t\t<menuitem id=\"" + menuData.ID + "\" parent=\"" + menuData.MenuParent + "\" name=\"" + Utils.ReplaceSpecialChar((string)menuData.Name) + "\" action=\"" + menuData.Action + "\"" + icon + groups + sequence + "/>";

            return render;
        }

        public static string CreateWorkFlow(ClassData classData)
        {
            Dictionary<string, string> openerp_class_viewValues = new Dictionary<string, string>();

            string classview = "<record id=\"wkf_" + classData.Name + "\" model=\"workflow\">\r\n";
            classview += "\t\t\t<field name=\"name\">" + classData.Parent.ClassicName + "." + classData.Name + ".basic</field>\r\n";
            classview += "\t\t\t<field name=\"osv\">" + classData.Parent.ClassicName + "." + classData.Name + "</field>\r\n";
            classview += "\t\t\t<field name=\"on_create\">True</field>\r\n";
            classview += "\t\t</record>\r\n";

            string activities = "";
            string transitions = "";

            int i = 1;

            KeyValuePair<string, string> preKeyValue = new KeyValuePair<string,string>();
            foreach (KeyValuePair<string, string> KeyValue in classData.StateProperty.SelectionData.ListOfValues)
            {
                string key = KeyValue.Key;

                if (i == 1)
                {
                    activities += "\r\n";
                    activities += "\t\t<record id=\"act_" + key + "\" model=\"workflow.activity\">\r\n";
                    activities += "\t\t\t<field name=\"wkf_id\" ref=\"wkf_" + classData.Name + "\"/>\r\n";
                    activities += "\t\t\t<field name=\"flow_start\">True</field>\r\n";
                    activities += "\t\t\t<field name=\"name\">" + key + "</field>\r\n";
                    activities += "\t\t</record>\r\n";
                }
                else if (i == classData.StateProperty.SelectionData.ListOfValues.Count)
                {
                    activities += "\r\n";
                    activities += "\t\t<record id=\"act_" + key + "\" model=\"workflow.activity\">\r\n";
                    activities += "\t\t\t<field name=\"wkf_id\" ref=\"wkf_" + classData.Name + "\"/>\r\n";
                    activities += "\t\t\t<field name=\"kind\">function</field>\r\n";
                    activities += "\t\t\t<field name=\"name\">" + key + "</field>\r\n";
                    activities += "\t\t\t<field name=\"action\">action_" + key + "_wkf()</field>\r\n";
                    activities += "\t\t\t<field name=\"flow_stop\">True</field>\r\n";
                    activities += "\t\t</record>\r\n";

                    activities += "\r\n";
                    activities += "\t\t<record id=\"act_cancel\" model=\"workflow.activity\">\r\n";
                    activities += "\t\t\t<field name=\"wkf_id\" ref=\"wkf_" + classData.Name + "\"/>\r\n";
                    activities += "\t\t\t<field name=\"kind\">function</field>\r\n";
                    activities += "\t\t\t<field name=\"name\">cancel</field>\r\n";
                    activities += "\t\t\t<field name=\"action\">action_cancel_wkf()</field>\r\n";
                    activities += "\t\t\t<field name=\"flow_stop\">True</field>\r\n";
                    activities += "\t\t</record>\r\n";
                }
                else
                {
                    activities += "\r\n";
                    activities += "\t\t<record id=\"act_" + key + "\" model=\"workflow.activity\">\r\n";
                    activities += "\t\t\t<field name=\"wkf_id\" ref=\"wkf_" + classData.Name + "\"/>\r\n";
                    activities += "\t\t\t<field name=\"kind\">function</field>\r\n";
                    activities += "\t\t\t<field name=\"name\">" + key + "</field>\r\n";
                    activities += "\t\t\t<field name=\"action\">action_" + key + "_wkf()</field>\r\n";
                    activities += "\t\t</record>\r\n";
                }

                if (i != 1)
                {
                    // button
                    if (classData.ListOfWorkFlowButton.ContainsKey(key) && classData.ListOfWorkFlowButton[key].RenderXML)
                    {
                        transitions += "\r\n";
                        transitions += "\t\t<record id=\"trans_" + preKeyValue.Key + "_" + key + "\" model=\"workflow.transition\">\r\n";
                        transitions += "\t\t\t<field name=\"act_from\" ref=\"act_" + preKeyValue.Key + "\"/>\r\n";
                        transitions += "\t\t\t<field name=\"act_to\" ref=\"act_" + key + "\"/>\r\n";
                        transitions += "\t\t\t<field name=\"signal\">button_" + key + "</field>\r\n";
                        transitions += "\t\t</record>\r\n";

                        /*transitions += "\r\n";
                        transitions += "\t\t<record id=\"trans_" + key + "_cancel\" model=\"workflow.transition\">\r\n";
                        transitions += "\t\t\t<field name=\"act_from\" ref=\"act_" + key + "\"/>\r\n";
                        transitions += "\t\t\t<field name=\"act_to\" ref=\"act_cancel\"/>\r\n";
                        transitions += "\t\t\t<field name=\"signal\">button_cancel</field>\r\n";
                        transitions += "\t\t</record>\r\n";*/
                    }

                    // function
                    else if (classData.ListOfWorkFlowFunction.ContainsKey(key))
                    {
                        transitions += "\r\n";
                        transitions += "\t\t<record id=\"trans_" + preKeyValue.Key + "_" + key + "\" model=\"workflow.transition\">\r\n";
                        transitions += "\t\t\t<field name=\"act_from\" ref=\"act_" + preKeyValue.Key + "\"/>\r\n";
                        transitions += "\t\t\t<field name=\"act_to\" ref=\"act_" + key + "\"/>\r\n";
                        transitions += "\t\t\t<field name=\"condition\">" + classData.ListOfWorkFlowFunction[key].Name + "()</field>\r\n";
                        transitions += "\t\t</record>\r\n";

                        transitions += "\r\n";
                        transitions += "\t\t<record id=\"trans_" + key + "_" + preKeyValue.Key + "\" model=\"workflow.transition\">\r\n";
                        transitions += "\t\t\t<field name=\"act_from\" ref=\"act_" + key + "\"/>\r\n";
                        transitions += "\t\t\t<field name=\"act_to\" ref=\"act_" + preKeyValue.Key + "\"/>\r\n";
                        transitions += "\t\t\t<field name=\"condition\">not " + classData.ListOfWorkFlowFunction[key].Name + "()</field>\r\n";
                        transitions += "\t\t</record>\r\n";

                        /*transitions += "\r\n";
                        transitions += "\t\t<record id=\"trans_" + key + "_cancel_auto\" model=\"workflow.transition\">\r\n";
                        transitions += "\t\t\t<field name=\"act_from\" ref=\"act_" + key + "\"/>\r\n";
                        transitions += "\t\t\t<field name=\"act_to\" ref=\"act_cancel\"/>\r\n";
                        transitions += "\t\t\t<field name=\"condition\">test_cancel()</field>\r\n";
                        transitions += "\t\t</record>\r\n";*/
                    }
                }

                preKeyValue = KeyValue;

                i++;
            }

            classview += activities + transitions;

            openerp_class_viewValues["classview"] = classview;

            string fileXml = classData.Parent.DirectoryInfo_Base.FullName + "/workflow/workflow_" + classData.Parent.ClassicName + "_" + classData.Name + ".xml";

            Utils.MakeFileFromModel("openerp_workflow.xml.model", fileXml, openerp_class_viewValues);

            return fileXml;
        }

        public static string CreateDatas(ClassData classData)
        {
            Dictionary<string, string> openerp_class_viewValues = new Dictionary<string, string>();

            string classview = "<record forcecreate=\"1\" id=\"sequence_" + classData.Name + "_type\" model=\"ir.sequence.type\">\r\n";
            classview += "\t\t\t<field name=\"name\">" + classData.Name + " code</field>\r\n";
            classview += "\t\t\t<field name=\"code\">code." + classData.Name + "</field>\r\n";
            classview += "\t\t</record>\r\n";
            classview += "\r\n";
            classview += "\t\t<record forcecreate=\"1\" id=\"sequence_" + classData.Name + "\" model=\"ir.sequence\">\r\n";
            classview += "\t\t\t<field name=\"name\">" + classData.Name + " code</field>\r\n";
            classview += "\t\t\t<field name=\"code\">code." + classData.Name + "</field>\r\n";
            classview += "\t\t\t<field name=\"padding\">6</field>\r\n";
            classview += "\t\t\t<field name=\"prefix\">" + classData.Code + " %(y)s%(month)s%(day)s </field>\r\n";
            classview += "\t\t</record>\r\n";
            
            
            openerp_class_viewValues["classview"] = classview;

            string fileXml = classData.Parent.DirectoryInfo_Base.FullName + "/data/data_" + classData.Parent.ClassicName + "_" + classData.Name + ".xml";

            Utils.MakeFileFromModel("openerp_workflow.xml.model", fileXml, openerp_class_viewValues);

            return fileXml;
        }
    }
}
