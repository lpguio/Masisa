using DevExpress.LookAndFeel;
using EnroladorStandAlone.Helper;
using System;
using System.Windows.Forms;

namespace EnroladorStandAlone
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DevExpress.Skins.SkinManager.EnableFormSkins();
            DevExpress.UserSkins.BonusSkins.Register();
            UserLookAndFeel.Default.SetSkinStyle("DevExpress Style");

            SingleGlobalInstance.Run(5000, new Form1());
            //Application.Run(new Form1());
        }
    }
}