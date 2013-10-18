using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EnterpriseArchitect2OpenERP
{
    public enum PropertyWhere
    {
        None,
        Start,
        End
    }

    public class PropertyData
    {
        public PropertyData()
        {
            Parent = null;
            LinkData = null;

            Name = "";
            Typage = "";
            TypageInitial = "";

            Alias = "";

            Documentation = "";

            Default = "";

            Size = "";
            Required = "";
            Memory = "";
            Selection = "";
            Ondelete = "";
            Translate = "";
            Readonly = "";

            LinkManyClass = "";
            LinkManyId1 = "";
            LinkManyId2 = "";
            LinkManyRel = "";

            OnChange = "";

            PropertyWhere = PropertyWhere.None;

            ShowInTree = true;

            ShowLabel = true;
            Page = "";
            Group = "";
            ColSpan = "";

            Select = "";
            StatusBar = "";
            States = "";
            Uniq = false;

            Function = "";

            TreeEditable = true;
            ClassLink = null;

            ShareTo = "";
            MaxUse = "";
        }

        public PropertyData(ClassData classData, XmlNode xmlNode)
        {
            Parent = classData;
            OnChange = "";
            LinkData = null;

            Name = xmlNode.Attributes["name"].Value.ToLower();

            TypageInitial = xmlNode["properties"].Attributes["type"].Value.ToLower();
            Typage = Utils.CorrectType(TypageInitial);

            Alias = (xmlNode["style"].Attributes["value"] != null) ? xmlNode["style"].Attributes["value"].Value : Utils.firstCharUpper(Name).Replace("_", " ");
            Alias = (Alias.Trim().Equals(string.Empty)) ? Name : Alias;

            Documentation = (xmlNode["documentation"].Attributes["value"] != null) ? xmlNode["documentation"].Attributes["value"].Value : "";

            Default = (xmlNode["initial"].Attributes["body"] != null) ? xmlNode["initial"].Attributes["body"].Value : null;

            
            Required = ((xmlNode["stereotype"].Attributes["stereotype"] != null) && (xmlNode["stereotype"].Attributes["stereotype"].Value.Equals("Required"))) ? "True" : "";
            Memory = ((xmlNode["stereotype"].Attributes["stereotype"] != null) && (xmlNode["stereotype"].Attributes["stereotype"].Value.Equals("Memory"))) ? "True" : "";

            Selection = "";
            Ondelete = "";
            Translate = Utils.getExtra(xmlNode, "translate", "")[0];
            Readonly = "";

            LinkManyClass = "";
            LinkManyId1 = "";
            LinkManyId2 = "";
            LinkManyRel = "";

            ClassLink = null;

            PropertyWhere = PropertyWhere.None;

            Size = Utils.getExtra(xmlNode, "size", "")[0];
            Size = ((Size.Equals(string.Empty)) && (Typage.Equals("char"))) ? "64" : string.Empty;

            ShowLabel = (Utils.getExtra(xmlNode, "label", "1")[0] == "1") ? true : false;
            Page = Utils.getExtra(xmlNode, "page", "")[0];
            Group = Utils.getExtra(xmlNode, "group", "")[0];
            ColSpan = Utils.getExtra(xmlNode, "colspan", "")[0];

            TreeEditable = (Utils.getExtra(xmlNode, "editable", "1")[0] == "1") ? true : false;

            ShowInTree = (Utils.getExtra(xmlNode, "showintree", "true")[0].ToLower().Equals("true")) ? true : false;
            ShowInView = (Utils.getExtra(xmlNode, "showinview", "true")[0].ToLower().Equals("true")) ? true : false;

            Select = Utils.getExtra(xmlNode, "select", "")[0];
            Criteria = (Utils.getExtra(xmlNode, "criteria", "false")[0].ToLower().Equals("false")) ? false : true;
            if ((Criteria) && (Select.Equals(""))) Select = "1";

            States = Utils.getExtra(xmlNode, "state", "")[0];
            Uniq = (Utils.getExtra(xmlNode, "uniq", "0")[0] == "1") ? true : false;

            OEtitle = int.Parse(Utils.getExtra(xmlNode, "oetitle", "0")[0]);

            Function = Utils.getExtra(xmlNode, "function", "")[0];
            Domain = Utils.getExtra(xmlNode, "domain", "")[0];

            ShareTo = Utils.getExtra(xmlNode, "shareto", "")[0];
            MaxUse = Utils.getExtra(xmlNode, "maxuse", "")[0];
        }

        public void Update()
        {
            string previousTypage = Typage;
            Typage = Utils.CorrectType(Typage);

            if (Typage.Equals("selection"))
            {
                if (Selection.Equals(""))
                {
                    Selection = "[";
                    StatusBar = "";

                    SelectionData = Program.ProjectData.GetSelection(previousTypage);
                    
                    foreach (KeyValuePair<string, string> element in SelectionData.ListOfValues)
                    {
                        Selection += "('" + element.Key + "', \"" + element.Value + "\"),";
                        StatusBar += element.Key + ",";
                    }

                    Selection = Selection.Substring(0, Selection.Length - 1) + "]";
                    StatusBar = StatusBar.Substring(0, StatusBar.Length - 1);
                }
            }

            if (LinkData != null)
            {
                if (LinkData.ClassStart != null)
                {
                    if (!Parent.Parent.ListOfClass.Contains(LinkData.ClassStart))
                    {
                        Parent.Parent.AddDepend(LinkData.ClassStart.Parent.ClassicName);
                    }
                }


                if (LinkData.ClassEnd != null)
                {
                    if (!Parent.Parent.ListOfClass.Contains(LinkData.ClassEnd))
                    {
                        Parent.Parent.AddDepend(LinkData.ClassEnd.Parent.ClassicName);
                    }
                }

                ClassData ClasseOpposee = LinkData.ClassStart;
                if (LinkData.ClassStart.Equals(Parent))
                {
                    ClasseOpposee = LinkData.ClassEnd;
                }

                string MultipliciteOpposee = LinkData.MultiplicityStart;
                PropertyData ProprieteOpposee = LinkData.PropertyStart;
                if (LinkData.PropertyStart.Equals(this))
                {
                    ProprieteOpposee = LinkData.PropertyEnd;
                    MultipliciteOpposee = LinkData.MultiplicityEnd;
                }

                if ((LinkData.Kind.Equals(KindLink.Aggregate)) || (LinkData.Kind.Equals(KindLink.Compose)))
                {
                    if (LinkData.PropertyEnd.Equals(this))
                    {
                        Typage = "many2one";
                    }
                    else if ((LinkData.PropertyStart.Equals(this)) && (LinkData.Kind.Equals(KindLink.Compose)))
                    {
                        Typage = "one2many";
                    }
                }

                switch (LinkData.Kind)
                {
                    case KindLink.Associate:
                    {
                        string Multiplicity = null;

                        if (LinkData.PropertyStart.Equals(this))
                        {
                            Multiplicity = LinkData.MultiplicityStart;
                        }
                        else
                        {
                            Multiplicity = LinkData.MultiplicityEnd;
                        }

                        if (Multiplicity != null)
                        {
                            string multiplicity = Multiplicity.Replace("..", ".");

                            string[] multi = multiplicity.Split(new char[] { '.' });

                            if (multi.Length == 1)
                            {
                                switch (multi[0])
                                {
                                    /*case "0":
                                    {
                                        break;
                                    }*/

                                    case "1":
                                    {
                                        Typage = "many2one";
                                        Required = "True";
                                        Ondelete = "cascade";

                                        break;
                                    }
                                }
                            }
                            else // (multi.Length > 1)
                            {
                                switch (Multiplicity)
                                {
                                    case "0..1":
                                    {
                                        Typage = "many2one";
                                        //Required = "True";
                                        Ondelete = "set null";
                                        
                                        break;
                                    }

                                    case "0..*":
                                    case "1..*":
                                    {
                                        if (Utils.GetMax(MultipliciteOpposee).Equals("*"))
                                        {
                                            Typage = "many2many";

                                            if (multi[0].Equals("1"))
                                            {
                                                Ondelete = "cascade";
                                            }
                                        }
                                        else
                                        {
                                            Typage = "one2many";
                                        }

                                        break;
                                    }

                                    default:
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        break;
                    }

                    case KindLink.Aggregate:
                    {
                        if (Typage.Equals("many2one"))
                        {
                            Ondelete = "set null";
                        }

                        break;
                    }

                    case KindLink.Compose:
                    {
                        if (Typage.Equals("one2many"))
                        {
                            Ondelete = "cascade";
                        }
                        else if (Typage.Equals("many2one"))
                        {
                            Required = "True";
                        }

                        break;
                    }
                }


                switch(Typage)
                {
                    case "many2one":
                    {
                        //

                        break;
                    }

                    case "one2many":
                    {
                        LinkManyId1 = ProprieteOpposee.Name;

                        break;
                    }

                    case "many2many":
                    {
                        LinkManyId1 = Parent.Name + "_id";
                        LinkManyId2 = ProprieteOpposee.Name;
                        LinkManyRel = LinkData.AssociationClass.Name;

                        break;
                    }
                }

                ClassLink = ClasseOpposee;

                if ((new string[] { "one2many", "many2one", "many2many" }).Contains<string>(Typage))
                {
                    if (ClasseOpposee.Inherit != null)
                    {
                        LinkManyClass = ClasseOpposee.Inherit.ModuleName + "." + ClasseOpposee.Inherit.Name;
                    }
                    else
                    {
                        LinkManyClass = ClasseOpposee.ModuleName + "." + ClasseOpposee.Name;
                    }
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public string Documentation { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public string Typage { get; set; }

        public string Size { get; set; }

        public string Required { get; set; }

        public string Translate { get; set; }

        public string Readonly { get; set; }

        public string Selection { get; set; }

        public string Ondelete { get; set; }

        public ClassData Parent { get; set; }

        public string LinkManyClass { get; set; }

        public string LinkManyId1 { get; set; }

        public string LinkManyId2 { get; set; }

        public string LinkManyRel { get; set; }

        public string Default { get; set; }

        public LinkData LinkData { get; set; }

        public PropertyWhere PropertyWhere { get; set; }

        public string TypageInitial { get; set; }

        public bool ShowLabel { get; set; }

        public string Page { get; set; }

        public string Group { get; set; }

        public string ColSpan { get; set; }

        public string OnChange { get; set; }

        public bool ShowInTree { get; set; }

        public bool ShowInView { get; set; }

        public string Select { get; set; }

        public bool Criteria { get; set; }

        public string StatusBar { get; set; }

        public EnumerationData SelectionData { get; set; }

        public string States { get; set; }

        public bool Uniq { get; set; }

        public string Memory { get; set; }

        public string Function { get; set; }

        public int OEtitle { get; set; }

        public bool TreeEditable { get; set; }

        public string Domain { get; set; }

        public ClassData ClassLink { get; set; }

        public string ShareTo { get; set; }

        public string MaxUse { get; set; }
    }
}
