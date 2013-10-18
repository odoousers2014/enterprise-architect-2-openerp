using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace EnterpriseArchitect2OpenERP
{
    public class PyGenerator
    {
        public static string CreateClass(ClassData classData)
        {
            Dictionary<string, string> openerp_classValues = new Dictionary<string, string>();

            string fields = "";
            string defaults = "";

            string constraints = "";
            string sql_constraints = "";
            string constraints_functions = "";

            string inherit = "# delete this line";

            string functions = "";
            string functions_before = "# delete this line";
            

            foreach (PropertyData propertyData in classData.ListOfProperties)
            {
                if (!propertyData.Typage.Equals("many2many") && !propertyData.Typage.Equals("one2many"))
                {
                    RenderPropertyPy(classData, propertyData, ref fields, ref defaults, ViewKind.Form);

                    if (propertyData.Uniq)
                    {
                        sql_constraints += ((sql_constraints.Equals(""))?"":"\r\n\t\t") + "('" + propertyData.Name + "_" + classData.Name + "_uniq', 'unique(" + propertyData.Name + ")', 'La valeur du champ [" + propertyData.Alias + "] doit être unique !'),";
                    }

                    if (!propertyData.Function.Equals(string.Empty))
                    {
                        functions_before += (functions_before.Equals(string.Empty)) ? "" : "\r\n\r\n\t";

                        string fieldFunctionType = Utils.GetFunctionName(propertyData.Function);

                        FunctionModel pythonCode = Utils.GetFunctionModel("function_" + fieldFunctionType, FunctionUsage.Field);

                        string result = "";
                        if (pythonCode.Default)
                        {
                            if ((new string[] { "int" }).Contains<string>(propertyData.Typage))
                            {
                                result = "0";
                            }

                            if ((new string[] { "float" }).Contains<string>(propertyData.Typage))
                            {
                                result = "0.0";
                            }

                            if ((new string[] { "char", "string", "text" }).Contains<string>(propertyData.Typage))
                            {
                                result = "''";
                            }
                        }

                        List<string> pythonCode_values = new List<string>();
                        pythonCode_values.Add(propertyData.Name + "_" + fieldFunctionType);
                        
                        switch (fieldFunctionType)
                        {
                            case "sum":
                            {
                                string property_zero = "0";
                                if (propertyData.Typage.Equals("float"))
                                {
                                    property_zero = "0.0";
                                }

                                pythonCode_values.Add(property_zero);

                                pythonCode_values.Add(propertyData.Name);

                                List<string> arguments = Utils.GetFunctionArguments(propertyData.Function);

                                pythonCode_values.Add(arguments[1]);
                                pythonCode_values.Add(arguments[0]);

                                break;
                            }

                            default:
                            {
                                pythonCode_values.Add(result);

                                break;
                            }
                        }

                        /*Dictionary<string, string> pythonCode_values = new Dictionary<string, string>();
                        pythonCode_values["function_name"] = propertyData.Name + "_" + fieldFunctionType;
                        pythonCode_values["property_name"] = propertyData.Name;
                        pythonCode_values["property_to_sum"] = "";
                        pythonCode_values["children"] = "";

                        pythonCode.Content = Utils.replaceValues(pythonCode.Content, pythonCode_values) + "\r\n\t";*/

                        pythonCode.ReplaceValues(pythonCode_values);
                        
                        functions_before += pythonCode;
                    }
                }
            }

            foreach (PropertyData propertyData in classData.ListOfProperties)
            {
                if (!propertyData.MaxUse.Equals(""))
                {
                    functions_before += (functions_before.Equals(string.Empty)) ? "" : "\r\n\r\n\t";

                    FunctionModel pythonCode = Utils.GetFunctionModel("function_label", FunctionUsage.Classic);

                    Dictionary<string, string> pythonCode_values = new Dictionary<string, string>();

                    pythonCode_values.Add("module", classData.Parent.ClassicName);

                    pythonCode_values.Add("classname", classData.Name);
                    pythonCode_values.Add("property_name", propertyData.Name);

                    string[] maxuse = propertyData.MaxUse.Split(new char[] { '.' });

                    pythonCode_values.Add("classlink", maxuse[0]);
                    pythonCode_values.Add("property_link", maxuse[1]);

                    pythonCode.ReplaceValues(pythonCode_values);

                    functions_before += pythonCode;



                    functions += (functions.Equals(string.Empty)) ? "" : "\r\n\r\n\t";

                    FunctionModel pythonCode2 = Utils.GetFunctionModel("function_onchange_maxuse", FunctionUsage.Classic);

                    Dictionary<string, string> pythonCode2_values = new Dictionary<string, string>();

                    pythonCode2_values.Add("property_name", propertyData.Name);
                    pythonCode2_values.Add("classlink", maxuse[0]);

                    pythonCode2.ReplaceValues(pythonCode2_values);

                    functions += pythonCode2;
                }
            }

            foreach (PropertyData propertyData in classData.ListOfProperties)
            {
                if (propertyData.Typage.Equals("many2many"))
                {
                    RenderPropertyPy(classData, propertyData, ref fields, ref defaults, ViewKind.Form);
                }
            }

            foreach (PropertyData propertyData in classData.ListOfProperties)
            {
                if (propertyData.Typage.Equals("one2many"))
                {
                    RenderPropertyPy(classData, propertyData, ref fields, ref defaults, ViewKind.Form);
                }
            }


            Dictionary<string, OperationData> listOfOperationsData = classData.ListOfOperations;
            
            if (classData.StateProperty != null)
            {
                int i = 1;

                foreach (KeyValuePair<string, string> KeyValue in classData.StateProperty.SelectionData.ListOfValues)
                {
                    string key = KeyValue.Key;

                    OperationData operationData = new OperationData();
                    operationData.Parent = classData;
                    operationData.ListOfParameter = new List<ParameterData>();

                    FunctionModel pythonCode = new FunctionModel(FunctionUsage.Classic);

                    if (i == 1)
                    {
                        pythonCode = Utils.GetFunctionModel("function_first_action_wkf", FunctionUsage.Classic);
                    }
                    else
                    {
                        pythonCode = Utils.GetFunctionModel("function_action_wkf", FunctionUsage.Classic);
                    }

                    /*Dictionary<string, string> pythonCode_values = new Dictionary<string, string>();
                    pythonCode_values["function_name"] = "action_" + key + "_wkf";
                    pythonCode_values["state"] = key;

                    pythonCode.Content = Utils.replaceValues(pythonCode.Content, pythonCode_values) + "\r\n\t";*/

                    pythonCode.ReplaceValues("action_" + key + "_wkf", key);

                    operationData.Name = "action_" + key + "_wkf";
                    operationData.PythonCode = pythonCode.Content;

                    listOfOperationsData.Add(operationData.Name, operationData);

                    i++;
                }
            }

            Dictionary<string, string> accountArgs = Utils.FunctionArgs(classData.AccountingArgs);

            //string state = Utils.GetArg(accountArgs, "state", "");
            //string button = Utils.GetArg(accountArgs, "button", "");


            foreach (KeyValuePair<string, OperationData> operationDataKV in listOfOperationsData)
            {
                OperationData operationData = operationDataKV.Value;

                functions += (functions.Equals(string.Empty)) ? "" : "\r\n\r\n\t";

                string parameters = "";
                int i = 0;
                foreach (ParameterData param in operationData.ListOfParameter)
                {
                    if (!param.Name.Equals(""))
                    {
                        parameters += ", " + param.Name;
                    }
                    else
                    {
                        parameters += ", arg" + i.ToString();

                        i++;
                    }
                }

                if (operationData.PythonCode.Equals(""))
                {
                    functions += "def " + operationData.Name + "(self, cr, uid, ids" + parameters + ", context=None):\r\n";
                    functions += "\t\t# contenu généré, à modifier si besoin\r\n";
                    functions += "\t\treturn " + Utils.CorrectReturn(operationData.ReturnType);
                }
                else
                {
                    functions += operationData.PythonCode;
                }
            }


            //string add_after = "";
            foreach (KeyValuePair<string, string> constraint in classData.ListOfConstraints)
            {
                if (constraints_functions.Equals(""))
                {
                    constraints_functions = "\r\n\t";
                    //add_after += "\r\n";
                }

                string[] contrainte = constraint.Value.Split(new char[] { '\n' });

                string[] contrainte_property = contrainte[0].Trim().Split(new char[] { ':' });
                string[] contrainte_constraint = contrainte[1].Trim().Split(new char[] { ':' });
                string[] contrainte_message = contrainte[2].Trim().Split(new char[] { ':' });


                string[] con_properties = contrainte_property[1].Split(new char[] { ',' });
                string contrainte_properties = "";
                string virgule = "";
                foreach (string con_property in con_properties)
                {
                    contrainte_properties += virgule + "'" + con_property.Trim() + "'";
                    virgule = ",";
                }

                string[] operandes = contrainte_constraint[1].Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                string operande1 = operandes[0];
                string oprator = operandes[1];
                string operande2 = operandes[2];

                FunctionModel function_content = Utils.GetFunctionModel("function_constraint_operator", FunctionUsage.Classic);

                /*Dictionary<string, string> constraint_content_values = new Dictionary<string, string>();
                constraint_content_values["constraint_name"] = constraint.Key;
                constraint_content_values["operande1"] = operande1;
                constraint_content_values["operator"] = Utils.TrueString(oprator);
                constraint_content_values["operande2"] = operande2;

                function_content.Content = Utils.replaceValues(function_content.Content, constraint_content_values);*/

                function_content.ReplaceValues(constraint.Key, operande1, Utils.TrueString(oprator), operande2);

                constraints_functions += function_content + "\r\n\t";

                constraints += "(_" + constraint.Key + ", \"" + contrainte_message[1].Trim() + "\", [" + contrainte_properties + "]),\r\n\t\t";
            }

            if (!constraints.Equals("")) constraints = constraints.Substring(0, constraints.Length - 4);

            //constraints_functions += add_after;

            openerp_classValues["name"] = classData.ModuleName + "." + classData.Name;
            if (classData.Inherit != null)
            {
                openerp_classValues["name"] = "# delete this line";
                inherit = "_inherit = \"" + classData.Inherit.ModuleName + "." + classData.Inherit.Name + "\"";
            }

            openerp_classValues["classname"] = classData.Parent.ClassicName +"_" + classData.Name;
            openerp_classValues["description"] = classData.Documentation;
            openerp_classValues["inherit"] = inherit;
            openerp_classValues["fields"] = fields;
            openerp_classValues["defaults"] = defaults;
            openerp_classValues["constraints_functions"] = constraints_functions;
            openerp_classValues["constraints"] = constraints;
            openerp_classValues["sql_constraints"] = sql_constraints;
            openerp_classValues["functions"] = functions;
            openerp_classValues["functions_before"] = functions_before;



            string filePy = classData.Parent.DirectoryInfo_Base.FullName + "/" + classData.Parent.ClassicName + "_" + classData.Name + ".py";
            
            Utils.MakeFileFromModel("openerp_class.py.model", filePy, openerp_classValues);

            return filePy;
        }

        public static string CreateReportModel(ClassData classData)
        {
            Dictionary<string, string> openerp_classValues = new Dictionary<string, string>();

            openerp_classValues["classname"] = "report_" + classData.Parent.ClassicName + "_" + classData.Name;
            openerp_classValues["name"] = "report." + classData.Parent.ClassicName + "." + classData.Name + ".rml";
            openerp_classValues["table"] = classData.Parent.ClassicName + "." + classData.Name;
            openerp_classValues["tablename"] = Utils.ClassicName(classData.Name);
            openerp_classValues["xslfile"] = "addons/" + classData.Parent.ClassicName + "/report/report_" + classData.Parent.ClassicName + "_" + classData.Name + ".xsl";

            string fields = "";

            foreach (PropertyData propertyData in classData.ListOfProperties)
            {
                if (!propertyData.Name.Equals(classData.Name + "_id"))
                {
                    fields += "fields += \"<" + propertyData.Name + ">\" + " + openerp_classValues["tablename"] + "." + propertyData.Name + " + \"</" + propertyData.Name + ">\"\r\n\t\t\t";
                }
            }

            openerp_classValues["fields"] = fields;

            string filePy = classData.Parent.DirectoryInfo_Base.FullName + "/report/report_" + classData.Parent.ClassicName + "_" + classData.Name + ".py";

            Utils.MakeFileFromModel("openerp_report.py.model", filePy, openerp_classValues);

            return filePy;
        }

        public static string CreateReportWizardModel(ClassData classData)
        {
            Dictionary<string, string> openerp_classValues = new Dictionary<string, string>();

            openerp_classValues["classname"] = "wizard_" + classData.Parent.ClassicName + "_" + classData.Name;
            openerp_classValues["name"] = classData.Parent.ClassicName + ".wizard." + classData.Name;
            openerp_classValues["description"] = classData.Documentation;

            string fields = "";
            string fields_coma = "";
            string defaults = "";

            PropertyData property = new PropertyData();
            property.Name = "typerapport";
            property.Alias = "Type de rapport";
            property.Typage = "selection";
            property.Selection = "[('listcomplete', \"Liste complète\")]";

            RenderPropertyPy(classData, property, ref fields, ref defaults, ViewKind.ReportFilter);

            fields_coma = "'" + property.Name + "', ";

            foreach (PropertyData propertyData in classData.ListOfPropertiesReport)
            {
                RenderPropertyPy(classData, propertyData, ref fields, ref defaults, ViewKind.ReportFilter);

                fields_coma += "'" + propertyData.Name + "', ";
            }

            fields_coma = fields_coma.Substring(0, fields_coma.Length - 2);
            
            openerp_classValues["fields"] = fields;
            openerp_classValues["fields_coma"] = fields_coma;
            openerp_classValues["defaults"] = defaults;
            openerp_classValues["reportname"] = classData.Parent.ClassicName + "." + classData.Name + ".rml";

            string filePy = classData.Parent.DirectoryInfo_Base.FullName + "/wizard/wizard_" + classData.Parent.ClassicName + "_" + classData.Name + ".py";

            Utils.MakeFileFromModel("openerp_wizard.py.model", filePy, openerp_classValues);

            return filePy;
        }

        public static void RenderPropertyPy(ClassData classData, PropertyData propertyData, ref string fields, ref string defaults, ViewKind viewKind)
        {
            if (!propertyData.Name.Equals(classData.Name + "_id"))
            {
                fields += (fields.Equals(string.Empty)) ? "" : "\r\n\t\t";

                string fieldName = propertyData.Name;
                string fieldTypage = propertyData.Typage;
                string fieldSelection = (!propertyData.Selection.Equals(string.Empty)) ? propertyData.Selection + ", " : "";
                string fieldAlias = "\"" + propertyData.Alias + "\"";

                string fieldSize = (!propertyData.Size.Equals(string.Empty)) ? ", size=" + propertyData.Size : "";
                fieldSize = ((fieldSize.Equals(string.Empty)) && (fieldTypage.Equals("char"))) ? ", size=64" : fieldSize;
                fieldSize = (!(fieldSize.Equals(string.Empty)) && (fieldTypage.Equals("reference"))) ? ", " + propertyData.Size : fieldSize;

                string fieldRequired = "";
                string fieldOndelete = "";
                string fieldTranslate = "";
                string fieldReadonly = "";
                string fieldMemory = "";
                string fieldDomain = "";

                string fieldFunction = "";

                if (!viewKind.Equals(ViewKind.ReportFilter))
                {
                    if (classData.Inherit == null)
                    {
                        fieldRequired = (!propertyData.Required.Equals(string.Empty)) ? ", required=" + propertyData.Required : "";
                    }

                    fieldOndelete = (!propertyData.Ondelete.Equals(string.Empty)) ? ", ondelete='" + propertyData.Ondelete + "'" : "";
                    fieldTranslate = (!propertyData.Translate.Equals(string.Empty)) ? ", translate=" + propertyData.Required : "";
                    fieldReadonly = (!propertyData.Readonly.Equals(string.Empty)) ? ", readonly=" + propertyData.Readonly : "";
                    fieldMemory = (!propertyData.Memory.Equals(string.Empty)) ? ", store=" + ((propertyData.Memory.Equals("True")) ? "False" : "True") : "";

                    if (!propertyData.Domain.Equals(string.Empty))
                    {
                        string[] domains = propertyData.Domain.Split(new char[]{','});

                        string domain = "[";

                        for (int i = 0; i < domains.Length; i++)
                        {
                            string[] parts = Regex.Split(domains[i], "(=|!=)");

                            domain += "(";
                            domain += "'" + parts[0] + "',";
                            domain += "'" + parts[1] + "',";
                            domain += "'" + parts[2] + "'";
                            domain += ")";

                            if (i < domains.Length - 1)
                            {
                                domain += ",";
                            }
                        }

                        domain += "]";

                        fieldDomain = ", domain=" + domain;
                    }

                    if (!propertyData.Function.Equals(string.Empty))
                    {
                        string fieldFunctionType = Utils.GetFunctionName(propertyData.Function);
                        fieldFunction = "fields.function(_" + fieldName + "_" + fieldFunctionType + ", type='" + fieldTypage + "', string=" + Utils.ReplaceSpecialChar(fieldAlias) + ", method=True)";
                    }
                }

                string fieldDocumentation = (!propertyData.Documentation.Equals(string.Empty)) ? ", help=\"" + propertyData.Documentation.Trim().Replace("\n", "").Replace("\r", "") + "\"" : "";

                string fieldLink = "";
                if ((new List<string>(new string[] { "many2one", "one2many", "many2many" })).IndexOf(propertyData.Typage) >= 0)
                {
                    switch (propertyData.Typage)
                    {
                        case "many2one":
                        {
                            fieldLink = "'" + propertyData.LinkManyClass + "', ";
                            break;
                        }

                        case "one2many":
                        {
                            fieldLink = "'" + propertyData.LinkManyClass + "', '" + propertyData.LinkManyId1 + "', ";
                            break;
                        }

                        case "many2many":
                        {
                            fieldLink = "'" + propertyData.LinkManyClass + "', '" + propertyData.LinkManyRel + "', '" + propertyData.LinkManyId1 + "', '" + propertyData.LinkManyId2 + "', ";
                            break;
                        }
                    }
                }

                if (fieldFunction.Equals(string.Empty))
                {
                    fields += "'" + fieldName + "' : fields." + fieldTypage + "(" + fieldLink + fieldSelection + fieldAlias + fieldSize + fieldRequired + fieldTranslate + fieldReadonly + fieldOndelete + fieldDomain + fieldMemory + fieldDocumentation + "),";
                }
                else
                {
                    fields += "'" + fieldName + "' : " + fieldFunction + ",";
                }

                if ((fieldName.Equals("code")) && (classData.CodeProperty != null))
                {
                    defaults += (defaults.Equals(string.Empty)) ? "" : "\r\n\t\t";
                    defaults += "'" + fieldName + "': lambda self, cr, uid, context = {} : self.pool.get('ir.sequence').get(cr, uid, 'code." + classData.Name + "'),";
                }
                else if (propertyData.Default != null) 
                {
                    defaults += (defaults.Equals(string.Empty)) ? "" : "\r\n\t\t";
                    defaults += "'" + fieldName + "': " + CorrectTypeFormat(propertyData.Default, propertyData.Typage) + ",";
                }

                if (!propertyData.MaxUse.Equals(""))
                {
                    fields += "\r\n\t\t" + "'" + fieldName + "_label': fields.function(_" + fieldName + "_label, method=True, type='text', store=False),";

                    defaults += (defaults.Equals(string.Empty)) ? "" : "\r\n\t\t";
                    defaults += "'" + fieldName + "_label': _" + fieldName + "_label_init,";
                }
            }
        }

        public static string CorrectTypeFormat(string value, string type)
        {
            string result = value;

            if ((new List<string>(new string[] { "string", "char", "text", "selection" })).IndexOf(type) >= 0)
            {
                result = "'" + value + "'";
            }

            return result;
        }


        public static string CreateInit(ModuleData moduleData)
        {
            Dictionary<string, string> __init__Values = new Dictionary<string, string>();

            __init__Values["imports"] = (string)moduleData.Data["imports"];
            
            Utils.MakeFileFromModel("__init__.py.model", moduleData.DirectoryInfo_Base.FullName + "/__init__.py", __init__Values);

            return moduleData.DirectoryInfo_Base.FullName + "/__init__.py";
        }

        public static string CreateReportInit(ModuleData moduleData)
        {
            Dictionary<string, string> __init__Values = new Dictionary<string, string>();

            __init__Values["imports"] = (string)moduleData.Data["report_imports"];

            Utils.MakeFileFromModel("__init__.py.model", moduleData.DirectoryInfo_Base.FullName + "/report/__init__.py", __init__Values);

            return moduleData.DirectoryInfo_Base.FullName + "/report/__init__.py";
        }

        public static string CreateWizardInit(ModuleData moduleData)
        {
            Dictionary<string, string> __init__Values = new Dictionary<string, string>();

            __init__Values["imports"] = (string)moduleData.Data["wizard_imports"];

            Utils.MakeFileFromModel("__init__.py.model", moduleData.DirectoryInfo_Base.FullName + "/wizard/__init__.py", __init__Values);

            return moduleData.DirectoryInfo_Base.FullName + "/wizard/__init__.py";
        }

        public static string CreateOpenERP(ModuleData moduleData)
        {
            Dictionary<string, string> __openerp__Values = new Dictionary<string, string>();

            __openerp__Values["name"] = moduleData.Name;
            __openerp__Values["description"] = moduleData.Documentation;
            __openerp__Values["update_xml"] = (string)moduleData.Data["update_xml"];
            __openerp__Values["demo_xml"] = (string)moduleData.Data["demo_xml"];
            __openerp__Values["images"] = (string)moduleData.Data["images"];
            __openerp__Values["category"] = (string)moduleData.Data["category"];
            __openerp__Values["depends"] = (string)moduleData.Data["depends"];
            __openerp__Values["test"] = (string)moduleData.Data["test"];
            __openerp__Values["classicname"] = moduleData.ClassicName;

            Utils.MakeFileFromModel("__openerp__.py.model", moduleData.DirectoryInfo_Base.FullName + "/__openerp__.py", __openerp__Values);

            return moduleData.DirectoryInfo_Base.FullName + "/__openerp__.py";
        }
    }
}
