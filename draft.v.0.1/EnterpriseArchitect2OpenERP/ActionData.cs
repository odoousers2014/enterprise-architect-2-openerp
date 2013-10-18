using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnterpriseArchitect2OpenERP
{
    public class ActionData
    {
        public ActionData(ActionData refAction, ClassData classData, string viewID, ViewKind viewKind, string sequence)
        {
            ID = refAction.ID + "_" + viewKind.ToString().ToLower();
            Parent = classData;

            Data = new Dictionary<string, object>();

            Data["model"] = "ir.actions.act_window.view";
            Data["name"] = "";
            Data["view_id"] = viewID;
            Data["domain"] = "";
            Data["context"] = "";
            Data["sequence"] = sequence;
            Data["view_id"] = "";
            Data["res_model"] = "";
            Data["view_type"] = "";
            Data["target"] = "";
            Data["view_mode"] = viewKind.ToString().ToLower();
            Data["search_view_id"] = "";
            Data["act_window_id"] = refAction.ID;
        }

        public ActionData(string Id, string Name, ClassData classData, string defaultViewID, string viewMode)
        {
            ID = Id;
            Parent = classData;

            Data = new Dictionary<string, object>();

            Data["model"] = "ir.actions.act_window";
            Data["name"] = Name;
            Data["view_id"] = defaultViewID;
            Data["domain"] = "";
            Data["context"] = "";
            Data["sequence"] = "";
            Data["res_model"] = classData.ModuleName + "." + classData.Name;
            Data["view_type"] = "";
            Data["target"] = "";
            Data["view_mode"] = viewMode;
            Data["search_view_id"] = "view_" + classData.Name + "_search";
            Data["act_window_id"] = "";
        }

        public string ID { get; set; }

        public ClassData Parent { get; set; }

        public Dictionary<string, object> Data { get; set; }
    }
}
