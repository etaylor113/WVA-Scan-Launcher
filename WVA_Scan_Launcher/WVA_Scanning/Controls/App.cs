using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WVA_Scan_Launcher
{
    class App
    {
        public static void Install()
        {
            // Installs newest version of scanner app that has been placed in public/docs/WVA_Scan/temp
            Process p = new Process();
            p.StartInfo.FileName = "msiexec.exe";
            p.StartInfo.Arguments = "/i \"C:\\Users\\Public\\Documents\\WVA_Scan\\Temp\\WVA_Scan_App.msi\" /passive";
            p.Start();
            p.WaitForExit();
        }

        public static void Uninstall()
        {
            // Delete older version of scanner if it exists(2.3.3 and lower)
            Process p1 = new Process();
            p1.StartInfo.FileName = "msiexec.exe";
            p1.StartInfo.Arguments = "/x {98880FEB-1DAD-4799-8DA9-E7EAE9BD7A13} /passive";
            p1.Start();
            p1.WaitForExit();

            // Delete new version of scanner (2.3.4 and up)
            Process p2 = new Process();
            p2.StartInfo.FileName = "msiexec.exe";
            p2.StartInfo.Arguments = "/x {865F2CD1-0598-4A40-9819-D7D51B10A78E} /passive";
            p2.Start();
            p2.WaitForExit();
        }

        public static bool DoesExist()
        {
            if (File.Exists(Path.programx86 + Path.app))
                return true;
            else
                return false;
        }

        public static void LaunchWVA_Scan()
        {
            try
            {
                Process.Start(Path.programx86 + Path.app);
            }
            catch (Exception ex)
            {
                Error.Report(ex);
            }
        }

        public static void Close()
        {
            Environment.Exit(0);
        }
    }
}
