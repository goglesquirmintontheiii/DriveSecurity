using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveSecurity;

namespace DriveSecurityDev
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DriveWatcher dw = new DriveWatcher();
            dw.DriveChange += Dw_DriveChange;
            dw.Start();
            while (true)
            {

            }
        }

        private static void Dw_DriveChange(object sender, DriveChangeArgs e)
        {
            Console.WriteLine($"{e.drive.path} was {e.status}");
        }
    }
}
