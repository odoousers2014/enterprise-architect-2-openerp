using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnterpriseArchitect2OpenERP
{
    public class KanbanData : ViewData
    {
        public KanbanData(ActData refView, ActionData refAction, string Name, ClassData classData, string sequence)
            : base(refView, refAction, Name, classData, sequence, "Kanban")
        {
            //
        }

        public KanbanData(string Id, ActionData refAction, string Name, ClassData classData, string sequence)
            : base(Id, refAction, Name, classData, sequence, "Kanban")
        {
            //
        }
    }
}
