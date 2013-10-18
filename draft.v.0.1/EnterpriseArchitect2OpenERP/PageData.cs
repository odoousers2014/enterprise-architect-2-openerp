using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnterpriseArchitect2OpenERP
{
    public class PageData
    {
        public PageData(string label)
        {
            Label = label;

            ListOfGroups = new Dictionary<string, GroupData>();
        }

        public string Label { get; set; }

        public Dictionary<string, GroupData> ListOfGroups { get; set; }
    }
}
