using System;
using Microsoft.Web.Administration;
using System.EnterpriseServices.Internal;
using QuickStatsModule;

namespace QSMInstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            // First, push the Assembly to the GAC
            Console.WriteLine("Attempting to install 'QuickStatsModule' to the Global Assembly Cache.");
            Publish dllGacPublisher = new Publish();
            dllGacPublisher.GacInstall("QuickStatsModule.dll");
            
            // Next, install it in the ApplicationHost.Config
            using (ServerManager serverManager = new ServerManager())
            {                
                Console.WriteLine("Installing 'QuickStatsModule");
                Configuration config = serverManager.GetApplicationHostConfiguration();
                var section = config.GetSection("system.webServer/modules", "");
                var collection = section.GetCollection();
                var element = collection.CreateElement();
                element.Attributes["name"].Value = "QuickStatsModule";
                element.Attributes["type"].Value =
                    $"{nameof(QuickStatsModule)}.{nameof(QuickStatsModule.QuickStatsModule)}, {nameof(QuickStatsModule)}, Version=1.0.0.0, Culture=neutral, PublicKeyToken=0d1283041f310768";
                Console.WriteLine($"Module being installed is: {element.Attributes["name"].Value}");
                Console.WriteLine($"Module install details: {element.Attributes["type"].Value}");
                collection.Add(element);
                serverManager.CommitChanges();
                Console.WriteLine("'QuickStatsModule' successfully installed to the GAC and IIS.");
            }
        }
    }
}
