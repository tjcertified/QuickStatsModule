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
        public QuickStatsModule()
        {
            contentSizes = new List<long>();
        }

        /// <summary>
        /// From IHttpModule: Disposes of the resources (other than memory) used by the module that implements
        /// System.Web.IHttpModule.
        /// </summary>
        public void Dispose()
        {
            _watcher = null;
        }

        /// <summary>
        ///     From IHttpModule: Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="application">An System.Web.HttpApplication that provides access to the methods, properties,
        ///     and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication application)
        {
            application.BeginRequest += Application_BeginRequest;
            application.EndRequest += Application_EndRequest;
            application.PreRequestHandlerExecute += Application_PreRequestHandlerExecute;
            application.PostRequestHandlerExecute += Application_PostRequestHandlerExecute;
        }

        /// <summary>
        /// Generates a string to add to the response of the current HttpContext with details about size 
        /// and timing of the current response and historical responses where this module is deployed.
        /// </summary>
        /// <param name="responseSize">Current response's size in bytes</param>
        /// <param name="requestMilliseconds">How long this request took to process</param>
        /// <param name="handlerMilliseconds">How long the HttpHandler portion of this request took to process</param>
        /// <param name="responseCount">How many responses have been logged by this module</param>
        /// <param name="responseAverage">Average response size found by this module (bytes)</param>
        /// <param name="responseMax">Largest response size found by this module (bytes)</param>
        /// <param name="responseMin">Smallest reponse size found by this module (bytes)</param>
        /// <returns></returns>
        public string GenerateStats(long responseSize, double requestMilliseconds, double handlerMilliseconds, int responseCount, double responseAverage, double responseMax, double responseMin)
        {
            return string.Format(
                $@"<hr>
                   <p>Response Size: {responseSize} bytes</p>
                   <p>Total Request Time: {requestMilliseconds}ms</p>
                   <p>Total HttpHandler Time: {handlerMilliseconds}ms</p>
                   <p>Total Pipeline Requests: {responseCount}</p>
                   <p>Average response size: {responseAverage} bytes</p>
                   <p>Largest response size: {responseMax} bytes</p>
                   <p>Smallest response size: {responseMin} bytes</p>
                "
                );
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

        private const string APP_START_KEY = "appStartTime";
        private const string APP_END_KEY = "appEndTime";
        private const string HANDLER_START_KEY = "handlerStartTime";
        private const string HANDLER_END_KEY = "handlerEndTime";
        private StreamWatcher _watcher;
        private List<long> contentSizes;
    }
}
