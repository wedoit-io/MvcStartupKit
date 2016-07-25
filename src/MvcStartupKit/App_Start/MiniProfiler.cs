
[assembly: WebActivatorEx.PreApplicationStartMethod(
    typeof(MvcStartupKit.MiniProfilerPackage), "PreStart")]

[assembly: WebActivatorEx.PostApplicationStartMethod(
    typeof(MvcStartupKit.MiniProfilerPackage), "PostStart")]

namespace MvcStartupKit
{
    using System.Web;
    using System.Web.Mvc;
    using StackExchange.Profiling;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using StackExchange.Profiling.Mvc;
    using StackExchange.Profiling.EntityFramework6;

    public static class MiniProfilerPackage
    {
        public static void PreStart()
        {

            // Be sure to restart you ASP.NET Developement server, this code will not run until you do that. 

            //TODO: See - _MINIPROFILER UPDATED Layout.cshtml
            //      For profiling to display in the UI you will have to include the line @StackExchange.Profiling.MiniProfiler.RenderIncludes() 
            //      in your master layout

            //TODO: Non SQL Server based installs can use other formatters like: new StackExchange.Profiling.SqlFormatters.InlineFormatter()
            MiniProfiler.Settings.SqlFormatter = new StackExchange.Profiling.SqlFormatters.SqlServerFormatter();
            MiniProfiler.Settings.MaxJsonResponseSize = 2147483647;

            //TODO: To profile a standard DbConnection: 
            // var profiled = new ProfiledDbConnection(cnn, MiniProfiler.Current);

            MiniProfilerEF6.Initialize();

            //Make sure the MiniProfiler handles BeginRequest and EndRequest
            DynamicModuleUtility.RegisterModule(typeof(MiniProfilerStartupModule));

            //Setup profiler for Controllers via a Global ActionFilter
            GlobalFilters.Filters.Add(new ProfilingActionFilter());

            // You can use this to check if a request is allowed to view results
            //MiniProfiler.Settings.Results_Authorize = (request) =>
            //{
            // you should implement this if you need to restrict visibility of profiling on a per request basis 
            //    return !DisableProfilingResults; 
            //};

            // the list of all sessions in the store is restricted by default, you must return true to alllow it
            //MiniProfiler.Settings.Results_List_Authorize = (request) =>
            //{
            // you may implement this if you need to restrict visibility of profiling lists on a per request basis 
            //return true; // all requests are kosher
            //};
        }

        public static void PostStart()
        {

        }
    }

    public class MiniProfilerStartupModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += (sender, e) =>
            {
                var request = ((HttpApplication)sender).Request;
                //TODO: By default only local requests are profiled, optionally you can set it up
                //  so authenticated users are always profiled
                MiniProfiler.Start();

               // if (request.IsLocal) { MiniProfiler.Start(); }


            };


            // TODO: You can control who sees the profiling information
            /*
            context.AuthenticateRequest += (sender, e) =>
            {
                if (!CurrentUserIsAllowedToSeeProfiler())
                {
                    StackExchange.Profiling.MiniProfiler.Stop(discardResults: true);
                }
            };
            */

            context.EndRequest += (sender, e) =>
            {
                MiniProfiler miniProfiler = MiniProfiler.Current;

                MiniProfiler.Stop();
            };
        }

        public void Dispose() { }
    }
}