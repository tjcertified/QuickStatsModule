using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickStatsModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QuickStatsModule.Tests
{
    [TestClass()]
    public class QuickStatsModuleTests
    {
        private QuickStatsModule _qsmTestHarness;

        [TestInitialize]
        public void Setup()
        {
            _qsmTestHarness = new QuickStatsModule();
        }

        [TestMethod()]
        public void GenerateStatsMakesCorrectString()
        {
            string result = $@"<hr>
                   <p>Response Size: 1</p>
                   <p>Total Request Time: 2</p>
                   <p>Total HttpHandler Time: 3</p>
                   <p>Total Pipeline Requests: 4</p>
                   <p>Average response size: 5 bytes</p>
                   <p>Largest response size: 10 bytes</p>
                   <p>Smallest response size: 1 bytes</p>
                ";
            Assert.AreEqual(result, _qsmTestHarness.GenerateStats(1, 2, 3, 4, 5, 10, 1));
        }
    }
}