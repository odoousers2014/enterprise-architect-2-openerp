using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnterpriseArchitect2OpenERP
{
    public class SearchData : ViewData
    {
        public SearchData(ActData refView, ActionData refAction, string Name, ClassData classData, string sequence)
            : base(refView, refAction, Name, classData, sequence, "Search")
        {
            //
        }

        public SearchData(string Id, ActionData refAction, string Name, ClassData classData, string sequence)
            : base(Id, refAction, Name, classData, sequence, "Search")
        {
            //
        }
    }
}
