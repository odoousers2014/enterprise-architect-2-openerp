using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnterpriseArchitect2OpenERP
{
    public class PoGenerator
    {
        public static string CreateLanguage(ModuleData moduleData)
        {
            Dictionary<string, string> openerp_class_poValues = new Dictionary<string, string>();

            string traductions = "";

            foreach (KeyValuePair<string, string> traduction in Program.ProjectData.ListOfTraduction)
            {
                traductions += traduction.Value;
            }

            openerp_class_poValues["name"] = moduleData.Name;
            openerp_class_poValues["traductions"] = traductions;

            string filePo_FR = moduleData.DirectoryInfo_Base.FullName + "/i18n/fr.po";
            Utils.MakeFileFromModel("language.po.model", filePo_FR, openerp_class_poValues);

            string filePo_EN = moduleData.DirectoryInfo_Base.FullName + "/i18n/en_GB.po";
            Utils.MakeFileFromModel("language.po.model", filePo_EN, openerp_class_poValues);

            return filePo_FR;
        }
    }
}
