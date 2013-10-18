using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace EnterpriseArchitect2OpenERP
{
    public enum KindLink
    {
        Associate,
        Generalize,
        Compose,
        Aggregate,
        AssociationClass,
        Assembly,
        Realize,
        Nesting,
        PackageMerge,
        PackageImport
    }

    public class LinkData
    {
        public LinkData()
        {
            //
        }

        public LinkData(ClassData classData, XmlNode xmlNode)
        {
            XmlNode = xmlNode;
            AssociationClass = null;

            Parent = classData;
            ID = xmlNode.Attributes["xmi:id"].Value;

            MultiplicityStart = null;
            MultiplicityEnd = null;

            switch (xmlNode.Name)
            {
                case "Aggregation":
                {
                    Kind = KindLink.Aggregate;
                    ClassStartID = xmlNode.Attributes["start"].Value;
                    ClassEndID = xmlNode.Attributes["end"].Value;
                    break;
                }

                case "Association":
                {
                    Kind = KindLink.Associate;
                    ClassStartID = xmlNode.Attributes["start"].Value;
                    ClassEndID = xmlNode.Attributes["end"].Value;
                    break;
                }

                case "Generalization":
                {
                    Kind = KindLink.Generalize;
                    ClassStartID = xmlNode.Attributes["start"].Value;
                    ClassEndID = xmlNode.Attributes["end"].Value;
                    break;
                }
            }

            Direction = "";
        }

        public XmlNode GetLink(string Id)
        {
            foreach (XmlNode xmlNode in Program.ProjectData.XmlNode["xmi:Extension"]["connectors"].ChildNodes)
            {
                if (xmlNode.Attributes["xmi:idref"].Value.Equals(Id))
                {
                    //xmlNode.OwnerDocument.ChildNodes[1]
                    return xmlNode;
                }
            }

            return null;
        }

        public void GetAllClass()
        {
            XmlNode lnkXmlNode = GetLink(ID);

            if (lnkXmlNode != null)
            {
                if ((lnkXmlNode["extendedProperties"] != null) &&
                    (lnkXmlNode["extendedProperties"].Attributes["associationclass"] != null))
                {
                    AssociationClass = Program.ProjectData.GetClassByID(lnkXmlNode["extendedProperties"].Attributes["associationclass"].Value);
                    AssociationClass.AssociationClass = true;
                }

                if ((lnkXmlNode["properties"] != null) && (lnkXmlNode["properties"].Attributes["direction"] != null))
                {
                    Direction = lnkXmlNode["properties"].Attributes["direction"].Value.ToLower();
                }


                ClassStart = Program.ProjectData.GetClassByID(ClassStartID);

                if (lnkXmlNode["target"]["role"].Attributes["name"] != null)
                {
                    PropertyEndID = lnkXmlNode["target"]["role"].Attributes["name"].Value;
                    PropertyEnd = ClassStart.GetProperty(PropertyEndID);

                    if (Kind.Equals(KindLink.Aggregate))
                    {
                        if (lnkXmlNode["target"]["type"].Attributes["aggregation"].Value.Equals("composite"))
                        {
                            Kind = KindLink.Compose;
                        }
                    }

                    if (lnkXmlNode["target"]["type"].Attributes["multiplicity"] != null)
                    {
                        MultiplicityEnd = lnkXmlNode["target"]["type"].Attributes["multiplicity"].Value;
                    }
                }


                ClassEnd = Program.ProjectData.GetClassByID(ClassEndID);

                if (lnkXmlNode["source"]["role"].Attributes["name"] != null)
                {
                    PropertyStartID = lnkXmlNode["source"]["role"].Attributes["name"].Value;
                    PropertyStart = ClassEnd.GetProperty(PropertyStartID);

                    if (lnkXmlNode["source"]["type"].Attributes["multiplicity"] != null)
                    {
                        MultiplicityStart = lnkXmlNode["source"]["type"].Attributes["multiplicity"].Value;
                    }
                }

                if (!Kind.Equals(KindLink.Generalize))
                {
                    if ((PropertyEnd == null) && (PropertyStart == null))
                    {
                        MessageBox.Show("Erreur: Lien incorrect entre les classes " + ClassStart.Name + " et " + ClassEnd.Name);
                        Application.Exit();
                    }

                    if (PropertyStart != null) PropertyStart.LinkData = this;
                    if (PropertyEnd != null) PropertyEnd.LinkData = this;
                }
                else
                {
                    //
                }

                switch (Kind)
                {
                    case KindLink.Aggregate:
                    {
                        //PropertyEnd.LinkData = this;
                        //PropertyEnd.LinkData = this;
                        break;
                    }

                    case KindLink.Compose:
                    {
                        //PropertyEnd.LinkData = this;
                        break;
                    }

                    case KindLink.Generalize:
                    {
                        break;
                    }

                    case KindLink.Associate:
                    {
                        //PropertyStart.LinkData = this;

                        break;
                    }
                }
            }
        }

        public override string ToString()
        {
            return Kind.ToString();
        }

        public ClassData Parent { get; set; }

        public KindLink Kind { get; set; }

        public string ClassStartID { get; set; }

        public string ClassEndID { get; set; }

        public ClassData ClassStart { get; set; }

        public ClassData ClassEnd { get; set; }

        public string PropertyStartID { get; set; }

        public string PropertyEndID { get; set; }

        public PropertyData PropertyEnd { get; set; }

        public PropertyData PropertyStart { get; set; }

        public string ID { get; set; }

        public XmlNode XmlNode { get; set; }

        public string MultiplicityStart { get; set; }

        public string MultiplicityEnd { get; set; }

        public ClassData AssociationClass { get; set; }

        public string Direction { get; set; }
    }
}
