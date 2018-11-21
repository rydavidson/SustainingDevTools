using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public class Startup
    {

        [STAThread]
        public static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {

            }
            else
            {
                App app = new App();
                app.InitializeComponent();
                app.Run();
            }
        }
    }
}
