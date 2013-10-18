using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EnterpriseArchitect2OpenERP
{
    public class EnumerationData
    {
        public EnumerationData(XmlNode xmlNode)
        {
            ListOfValues = new Dictionary<string, string>();

            if (xmlNode["attributes"] != null)
            {
                Name = xmlNode.Attributes["name"].Value;

                foreach (XmlNode attNode in xmlNode["attributes"].ChildNodes)
                {
                    string key = attNode.Attributes["name"].Value;
                    string value = attNode["style"].Attributes["value"].Value;

                    ListOfValues.Add(key, value);
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public string Name { get; set; }
        public Dictionary<string, string> ListOfValues { get; set; }
    }
}
