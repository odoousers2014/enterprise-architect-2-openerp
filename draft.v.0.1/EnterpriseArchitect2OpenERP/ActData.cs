using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnterpriseArchitect2OpenERP
{
    public class ActData : ViewData
    {
        public ActData(string Id, string Name, ClassData classData, string viewMode)
        {
            Kind = "Act";

            ID = Id;
            Parent = classData;

            Data = new Dictionary<string, object>();

            Data["name"] = classData.Alias;
            Data["res_model"] = classData.Parent.ClassicName + "." + classData.Name;
            Data["src_model"] = "";
            Data["view_mode"] = viewMode;
            Data["context1"] = "{}";
            Data["domain"] = "[]";
            Data["context2"] = "{}";
        }
    }
}
