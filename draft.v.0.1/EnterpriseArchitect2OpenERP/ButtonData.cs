using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnterpriseArchitect2OpenERP
{
    public class ButtonData
    {
        public ButtonData()
        {
            RenderXML = true;
        }

        public ButtonData(OperationData operationData)
        {
            Parent = operationData;

            OnClick = operationData.Name;
            Typage = "object";

            Label = operationData.Alias;
            States = "";
            Name = "";
            Text = "";

            RenderXML = true;
        }

        public string Label { get; set; }

        public OperationData Parent { get; set; }

        public string OnClick { get; set; }

        public string Typage { get; set; }

        public string States { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public bool RenderXML { get; set; }
    }
}
