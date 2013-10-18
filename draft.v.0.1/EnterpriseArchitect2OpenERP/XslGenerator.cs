using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnterpriseArchitect2OpenERP
{
    public class XslGenerator
    {
        public static string CreateReport(ClassData classData)
        {
            Dictionary<string, string> openerp_class_viewValues = new Dictionary<string, string>();

            openerp_class_viewValues["tablename"] = Utils.ClassicName(classData.Name);
            openerp_class_viewValues["alias"] = classData.Alias;

            string fields = "";
            string fields_titles = "";

            float colwidth = (17f / (float)(classData.ListOfProperties.Count - 1));
            string colwidths = "";

            foreach (PropertyData propertyData in classData.ListOfProperties)
            {
                if (!propertyData.Name.Equals(classData.Name + "_id"))
                {
                    colwidths += ((colwidths.Length > 0)? ",":"") + colwidth.ToString("#.0").Replace(",", ".") + "cm";

                    fields_titles += "<td><para style=\"rowTitle\">" + propertyData.Alias.ToUpper() + "</para></td>\r\n\t\t\t\t";
                    fields += "<td><para style=\"rowContent\"><xsl:value-of select=\"" + propertyData.Name + "\"/></para></td>\r\n\t\t\t\t\t";
                }
            }

            if (fields.Length > 0)
            {
                fields_titles = fields_titles.Substring(0, fields_titles.Length - 6);
                fields = fields.Substring(0, fields.Length - 7);
            }

            openerp_class_viewValues["fields"] = fields;
            openerp_class_viewValues["fields_titles"] = fields_titles;
            openerp_class_viewValues["colwidths"] = colwidths;

            string fileXsl = classData.Parent.DirectoryInfo_Base.FullName + "/report/report_" + classData.Parent.ClassicName + "_" + classData.Name + ".xsl";

            Utils.MakeFileFromModel("openerp_report.xsl.model", fileXsl, openerp_class_viewValues);

            return fileXsl;
        }
    }
}
