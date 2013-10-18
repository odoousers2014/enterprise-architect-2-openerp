using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnterpriseArchitect2OpenERP
{
    public class MenuData
    {
        public MenuData(string Id, string Name, string MenuParent, string Action, ClassData classData)
        {
            ID = Id;
            Parent = classData;
            this.Name = Name;

            this.MenuParent = MenuParent;
            this.Action = Action;

            Sequence = "";
            Groups = "";
            Icon = "";
        }

        public string ID { get; set; }

        public ClassData Parent { get; set; }

        public Dictionary<string, object> Data { get; set; }

        public object Sequence { get; set; }

        public object Groups { get; set; }

        public object Icon { get; set; }

        public object Name { get; set; }

        public string MenuParent { get; set; }

        public string Action { get; set; }
    }
}
