using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace EnterpriseArchitect2OpenERP
{
    public class PathData
    {
        public static string Models
        {
            get
            {
                return (new DirectoryInfo(Directory.GetParent(Application.ExecutablePath).FullName + "/models/")).FullName;
            }
        }

        public static string Addons
        {
            get
            {
                string user = Application.UserAppDataPath.Substring(0, Application.UserAppDataPath.IndexOf("AppData"));

                return (new DirectoryInfo(user + "/documents/AddonsOpenERP/")).FullName;
            }
        }
    }
}
