using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnterpriseArchitect2OpenERP
{
    public class TreeData : ViewData
    {
        public TreeData(ActData refView, ActionData refAction, string Name, ClassData classData, string sequence)
            : base(refView, refAction, Name, classData, sequence, "Tree")
        {
            //
        }

        public TreeData(string Id, ActionData refAction, string Name, ClassData classData, string sequence)
            : base(Id, refAction, Name, classData, sequence, "Tree")
        {
            //
        }
    }
}
