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
            long responseSize = 10;
            int requestTime = 5;
            int handlerTime = 10;
            int responses = 30;
            int avgSize = 10;
            int maxSize = 20;
            int minSize = 5;
            string result = $@"<hr>
                   <p>Response Size: {responseSize} bytes</p>
                   <p>Total Request Time: {requestTime}ms</p>
                   <p>Total HttpHandler Time: {handlerTime}ms</p>
                   <p>Total Pipeline Requests: {responses}</p>
                   <p>Average response size: {avgSize} bytes</p>
                   <p>Largest response size: {maxSize} bytes</p>
                   <p>Smallest response size: {minSize} bytes</p>
                ";
            Assert.AreEqual(result, _qsmTestHarness.GenerateStats(responseSize, requestTime, handlerTime, responses, avgSize, maxSize, minSize));
        }
    }
}