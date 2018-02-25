using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Web.Administration;

namespace QuickStatsModule
{
    /// <summary>
    /// From http://blog.davidebbo.com/2011/02/register-your-http-modules-at-runtime.html,
    /// This allows you to drop in this dll to an IIS /bin folder and have it "just work"
    /// for all sites, as long as you add the assembly attribute noted there.
    /// </summary>
    public class Initializer
    {
        public static void Start()
        {
            //Register the QuickStatsModule
            HttpApplication.RegisterModule(typeof(QuickStatsModule));
        }
    }
}
