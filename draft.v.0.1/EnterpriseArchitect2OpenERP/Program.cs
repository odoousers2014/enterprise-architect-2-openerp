using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace EnterpriseArchitect2OpenERP
{
    static class Program
    {
        public static ProjectData ProjectData { get; set; }
        public static Form1 Form { get; set; }
        
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form = new Form1();

            Application.Run(Form);
        }
    }
}
