using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnterpriseArchitect2OpenERP
{
    public class ParameterData
    {
        public ParameterData(string name, string type)
        {
            Name = name;
            Typage = type;
        }

        public string Typage { get; set; }

        public string Name { get; set; }
    }
}
