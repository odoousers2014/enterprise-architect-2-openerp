using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EnterpriseArchitect2OpenERP
{
    public class OperationData
    {
        public OperationData()
        {
            //
        }

        public OperationData(ClassData classData, XmlNode xmlNode)
        {
            Parent = classData;

            Name = xmlNode.Attributes["name"].Value.ToLower();
            ReturnType = Utils.CorrectType(xmlNode["type"].Attributes["type"].Value.ToLower());

            Alias = (xmlNode["style"].Attributes["value"] != null) ? xmlNode["style"].Attributes["value"].Value : Utils.firstCharUpper(Name).Replace("_", " ");
            Alias = (Alias.Trim().Equals(string.Empty)) ? Name : Alias;

            ListOfParameter = new List<ParameterData>();
            foreach (XmlNode parameterNode in xmlNode["parameters"].ChildNodes)
            {
                string typ = Utils.CorrectType(parameterNode["properties"].Attributes["type"].Value.ToLower());
                string nam = "";

                if (!parameterNode.Attributes["xmi:idref"].Value.Contains("EAID_RETURNID"))
                {
                    List<XmlNode> xmlNames = XmlUtility.GetXmlNode(Parent.Parent.XmlNode.ParentNode.ParentNode, parameterNode.Attributes["xmi:idref"].Value);
                    if (xmlNames.Count > 0)
                    {
                        nam = xmlNames[0].Attributes["name"].Value;
                    }

                    ParameterData parameterData = new ParameterData(nam, typ);

                    ListOfParameter.Add(parameterData);
                }
            }

            CallBy = Utils.getExtra(xmlNode, "callby", "")[0];
            State = Utils.getExtra(xmlNode, "state", "")[0];
            PythonCode = "";
        }

        public ClassData Parent { get; set; }

        public string Name { get; set; }

        public string ReturnType { get; set; }

        public List<ParameterData> ListOfParameter { get; set; }

        public string CallBy { get; set; }

        public string Alias { get; set; }

        public string State { get; set; }

        public string PythonCode { get; set; }
    }
}
