using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnterpriseArchitect2OpenERP
{
    public class GroupData
    {
        public GroupData(string label)
        {
            Label = label;
            Root = false;
            Report = false;

            ClassData = null;

            ListOfProperties = new List<PropertyData>();
            ListOfPages = new Dictionary<string, PageData>();
            ListOfButtons = new List<ButtonData>();
            ListOfGroups = new Dictionary<string, GroupData>();
        }

        public GroupData(string label, ClassData classData) : this(label)
        {
            ClassData = classData;
        }

        public string Label { get; set; }

        public List<PropertyData> ListOfProperties { get; set; }

        public Dictionary<string, PageData> ListOfPages { get; set; }

        public List<ButtonData> ListOfButtons { get; set; }

        public Dictionary<string, GroupData> ListOfGroups { get; set; }

        public bool Root { get; set; }

        public ClassData ClassData { get; set; }

        public bool Report { get; set; }
    }
}
