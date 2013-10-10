using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioFlasher
{
    class Program
    {
        [STAThread]
        public static void Main( string[] args )
        {
            using ( FlasherWindow flashWindow = new FlasherWindow() )
            {
                //Utilities.SetWindowTitle( flashWindow );
                flashWindow.Run( 30.0, 0.0 );
            }
        }
    }
}
