using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnterpriseArchitect2OpenERP
{
    public class ViewData
    {
        public ViewData()
        {
            //
        }

        public ViewData(ActData refView, ActionData refAction, string Name, ClassData classData, string sequence, string kind)
        {
            Kind = kind;

            ID = refView.ID + "_" + Kind.ToLower();
            Parent = classData;

            RefView = refView;
            RefAction = refAction;

            Data = new Dictionary<string, object>();

            Data["name"] = Name;
            Data["sequence"] = sequence;
        }

        public ViewData(string Id, ActionData refAction, string Name, ClassData classData, string sequence, string kind)
        {
            Kind = kind;

            ID = Id;
            Parent = classData;

            RefAction = refAction;

            Data = new Dictionary<string, object>();

            Data["name"] = Name;
            Data["sequence"] = sequence;
        }

        public string ID { get; set; }

        public string Kind { get; set; }

        public ClassData Parent { get; set; }

        public ActData RefView { get; set; }

        public Dictionary<string, object> Data { get; set; }

        public ActionData RefAction { get; set; }
    }
}
