using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace EnterpriseArchitect2OpenERP
{
    public class ProjectData
    {
        public ProjectData(XmlNode xmlNode)
        {
            XmlNode = xmlNode;

            ListOfModuleData = new List<ModuleData>();

            ListOfTraduction = new Dictionary<string, string>();

            foreach (XmlNode moduleXmlNode in xmlNode["uml:Model"]["packagedElement"].ChildNodes)
            {
                if (moduleXmlNode.Attributes["xmi:type"].Value.Equals("uml:Package"))
                {
                    if (moduleXmlNode.Attributes["name"].Value.Equals("OpenERP"))
                    {
                        foreach (XmlNode nativeModuleXmlNode in xmlNode["xmi:Extension"]["elements"].ChildNodes)
                        {
                            if ((nativeModuleXmlNode.Attributes["xmi:type"] != null) &&
                                (nativeModuleXmlNode.Attributes["xmi:type"].Value.Equals("uml:Package")))
                            {
                                if ((nativeModuleXmlNode["model"] != null) &&
                                    (nativeModuleXmlNode["model"].Attributes["package"] != null) &&
                                    (nativeModuleXmlNode["model"].Attributes["package"].Value.Equals(moduleXmlNode.Attributes["xmi:id"].Value)))
                                {
                                    if (!nativeModuleXmlNode.Attributes["name"].Value.Equals("Enumerations"))
                                    {
                                        ModuleData moduleData = new ModuleData(this, nativeModuleXmlNode, xmlNode["xmi:Extension"]["elements"]);
                                        moduleData.Native = true;

                                        ListOfModuleData.Add(moduleData);
                                    }
                                    else
                                    {
                                        ListOfEnumerationData = new List<EnumerationData>();
                                        foreach (XmlNode enumerationNode in xmlNode["xmi:Extension"]["elements"].ChildNodes)
                                        {
                                            if ((enumerationNode["model"] != null) &&
                                                (enumerationNode["model"].Attributes["package"] != null) &&
                                                (enumerationNode["model"].Attributes["package"].Value.Equals(nativeModuleXmlNode.Attributes["xmi:idref"].Value)))
                                            {
                                                EnumerationData enumerationData = new EnumerationData(enumerationNode);
                                                ListOfEnumerationData.Add(enumerationData);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }
                }
            }


            foreach (XmlNode moduleXmlNode in xmlNode["uml:Model"]["packagedElement"].ChildNodes)
            {
                if (moduleXmlNode.Attributes["xmi:type"].Value.Equals("uml:Package"))
                {
                    if (!moduleXmlNode.Attributes["name"].Value.Equals("OpenERP"))
                    {
                        ModuleData moduleData = new ModuleData(this, moduleXmlNode, xmlNode["xmi:Extension"]["elements"]);

                        ListOfModuleData.Add(moduleData);
                    }
                }
            }

            foreach (ModuleData moduleData in ListOfModuleData)
            {
                foreach (ClassData classData in moduleData.ListOfClass)
                {
                    addTraduction(classData, classData.Alias);

                    foreach (PropertyData propertyData in classData.ListOfProperties)
                    {
                        addTraduction(classData, propertyData.Alias);
                    }
                }
            }
        }

        public void addTraduction(ClassData classData, string text)
        {
            if ((!ListOfTraduction.ContainsKey(text)) && (!text.Trim().Equals("")))
            {
                string traduction = "";

                traduction += "#. module: " + classData.Parent.ClassicName + "\r\n";
                traduction += "#: view:" + classData.Parent.ClassicName + "." + classData.Name + ":0\r\n";
                traduction += "msgid \"" + Utils.ReplaceSpecialChar(text) + "\"\r\n";
                traduction += "msgstr \"" + text + "\"\r\n";

                traduction += "\r\n";

                ListOfTraduction.Add(text, traduction);
            }
        }

        public void CreateAll()
        {
            foreach (ModuleData moduleData in ListOfModuleData)
            {
                if (moduleData.ListOfClass.Count > 0)
                {
                    if (!moduleData.Native)
                    {
                        Program.Form.SetTaskText("En cours : " + moduleData.Name);

                        moduleData.Create();
                    }
                }

                
                Application.DoEvents();
            }
        }

        public ClassData GetClassByID(string Id)
        {
            foreach (ModuleData moduleData in ListOfModuleData)
            {
                foreach (ClassData classData in moduleData.ListOfClass)
                {
                    if (classData.ID.Equals(Id))
                    {
                        return classData;
                    }
                }
            }

            return null;
        }

        public void Create(List<string> listOfModule)
        {
            foreach (ModuleData moduleData in ListOfModuleData)
            {
                if (listOfModule.IndexOf(moduleData.Name) >= 0)
                {
                    if (moduleData.ListOfClass.Count > 0)
                    {
                        if (!moduleData.Native)
                        {
                            Program.Form.SetTaskText("En cours : " + moduleData.Name);

                            moduleData.Create();
                        }
                    }


                    Application.DoEvents();
                }
            }
        }

        public string CheckSelection(string Typage)
        {
            string result = Typage;

            foreach (EnumerationData enumerationData in ListOfEnumerationData)
            {
                if (enumerationData.Name.ToLower().Equals(Typage))
                {
                    return "selection";
                }
            }

            return result;
        }

        public EnumerationData GetSelection(string name)
        {
            foreach (EnumerationData enumerationData in ListOfEnumerationData)
            {
                if (enumerationData.Name.ToLower().Equals(name.ToLower()))
                {
                    return enumerationData;
                }
            }

            return null;
        }

        public List<ModuleData> ListOfModuleData { get; set; }
        public List<EnumerationData> ListOfEnumerationData { get; set; }
        public XmlNode XmlNode { get; set; }

        public Dictionary<string, string> ListOfTraduction { get; set; }

        public string AddonsPath { get; set; }
    }
}
