using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QuickStatsModule
{
    public class QuickStatsModule : IHttpModule
    {

        private List<long> contentSizes;

        public QuickStatsModule()
        {
            contentSizes = new List<long>();
        }

        public String ModuleName
        {
            get { return "QuickStatsModule"; }
        }

        public void Dispose()
        {
        }

        public void Init(HttpApplication application)
        {
            application.BeginRequest += Application_BeginRequest;
            application.EndRequest += Application_EndRequest;
            application.PreRequestHandlerExecute += Application_PreRequestHandlerExecute;
            application.PostRequestHandlerExecute += Application_PostRequestHandlerExecute;
        }

        private void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpContext.Current.Items[HANDLER_START_KEY] = DateTime.Now;
        }

        private void Application_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpContext.Current.Items[HANDLER_END_KEY] = DateTime.Now;
        }

        private void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Items[APP_START_KEY] = DateTime.Now;
            _watcher = new StreamWatcher(HttpContext.Current.Response.Filter);
            HttpContext.Current.Response.Filter = _watcher;
        }

        private void Application_EndRequest(object sender, EventArgs e)
        {
            // This means that the request did not go completely through the pipeline for some reason
            // (css file, redirect, image, etc.), so it is not necessarily going to have all of the
            // information that we need to process it.
            if (HttpContext.Current.Items[HANDLER_END_KEY] == null)
            {
                return;
            }
            var appEnd = DateTime.Now;
            DateTime appStart = (DateTime)HttpContext.Current.Items[APP_START_KEY];
            DateTime handlerStart = (DateTime)HttpContext.Current.Items[HANDLER_START_KEY];
            DateTime handlerEnd = (DateTime)HttpContext.Current.Items[HANDLER_END_KEY];
            contentSizes.Add(_watcher.Length);
            HttpContext.Current.Response.Write(
                GenerateStats(_watcher.Length,
                              appEnd.Subtract(appStart).TotalMilliseconds,
                              handlerEnd.Subtract(handlerStart).TotalMilliseconds,
                              contentSizes.Count,
                              contentSizes.Average(),
                              contentSizes.Max(),
                              contentSizes.Min())
                              );

        }

        public string GenerateStats(long responseLength, double requestMilliseconds, double handlerMilliseconds, int responseCount, double responseAverage, double responseMax, double responseMin)
        {
            return string.Format(
                $@"<hr>
                   <p>Response Size: {responseLength}</p>
                   <p>Total Request Time: {requestMilliseconds}</p>
                   <p>Total HttpHandler Time: {handlerMilliseconds}</p>
                   <p>Total Pipeline Requests: {responseCount}</p>
                   <p>Average response size: {responseAverage} bytes</p>
                   <p>Largest response size: {responseMax} bytes</p>
                   <p>Smallest response size: {responseMin} bytes</p>
                "
                );
        }

        private const string APP_START_KEY = "appStartTime";
        private const string APP_END_KEY = "appEndTime";
        private const string HANDLER_START_KEY = "handlerStartTime";
        private const string HANDLER_END_KEY = "handlerEndTime";
        private StreamWatcher _watcher;

    }
}
