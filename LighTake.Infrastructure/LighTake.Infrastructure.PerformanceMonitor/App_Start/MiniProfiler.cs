using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using LighTake.Infrastructure.Common;
using LighTake.Infrastructure.PerformanceMonitor.App_Start;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using StackExchange.Profiling.EntityFramework6;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using StackExchange.Profiling.Mvc;
using System.Collections.Generic;
using System;
using StackExchange.Profiling.Storage;
using LighTake.Infrastructure.Common.Logging;
using System.Text;

//using System.Data;
//using System.Data.Entity;
//using System.Data.Entity.Infrastructure;
//using StackExchange.Profiling.Data.EntityFramework;
//using StackExchange.Profiling.Data.Linq2Sql;

[assembly: WebActivator.PreApplicationStartMethod(
    typeof(MiniProfilerPackage), "PreStart")]

[assembly: WebActivator.PostApplicationStartMethod(
    typeof(MiniProfilerPackage), "PostStart")]


namespace LighTake.Infrastructure.PerformanceMonitor.App_Start
{
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

            MiniProfiler.Settings.IgnoredPaths = new[] { "/css/", "/images/", "/Scripts/*", "/style/*", "/js/*", "/Content/*" }
                                                .Union(MiniProfiler.Settings.IgnoredPaths)
                                                .ToArray();

            //开启写入日志文件
            //MiniProfiler.Settings.Storage = new Log4NetStorage();

            //TODO: To profile a standard DbConnection: 
            // var profiled = new ProfiledDbConnection(cnn, MiniProfiler.Current);

            //TODO: If you are profiling EF code first try: 
            MiniProfilerEF6.Initialize();
            DbConfiguration.Loaded +=
                (sender, e) =>
                e.ReplaceService<System.Data.Entity.Core.Common.DbProviderServices>(
                    (services, o) => EFProfiledSqlClientDbProviderServices.Instance
                    );
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
            // Intercept ViewEngines to profile all partial views and regular views.
            // If you prefer to insert your profiling blocks manually you can comment this out
            var copy = ViewEngines.Engines.ToList();
            ViewEngines.Engines.Clear();
            foreach (var item in copy)
            {
                ViewEngines.Engines.Add(new ProfilingViewEngine(item));
            }
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
                //request.IsLocal && 
                if (Tools.GetAppSettings("MiniProfilerEnable") == "1") { MiniProfiler.Start(); }
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

            context.EndRequest += (sender, e) => MiniProfiler.Stop();
        }

        public void Dispose() { }
    }

    ///// <summary>
    /////     User Log4Net as storage.
    ///// </summary>
    //internal class Log4NetStorage : StackExchange.Profiling.Storage.IStorage
    //{


    //    /// <summary>
    //    ///     Initializes static members of the <see cref="Log4NetStorage" /> class.
    //    /// </summary>
    //    static Log4NetStorage()
    //    {

    //    }

    //    /// <summary>
    //    /// Returns a list of <see cref="P:StackExchange.Profiling.MiniProfiler.Id"/>s that haven't been seen by
    //    ///     <paramref name="user"/>.
    //    /// </summary>
    //    /// <param name="user">
    //    /// User identified by the current <c>MiniProfiler.Settings.UserProvider</c>
    //    /// </param>
    //    /// <returns>
    //    /// the list of key values.
    //    /// </returns>
    //    public List<Guid> GetUnviewedIds(string user)
    //    {
    //        return null;
    //        //throw new NotSupportedException("This method should never run");
    //    }


    //    /// <summary>
    //    /// list the result keys.
    //    /// </summary>
    //    /// <param name="maxResults">
    //    /// The max results.
    //    /// </param>
    //    /// <param name="start">
    //    /// The start.
    //    /// </param>
    //    /// <param name="finish">
    //    /// The finish.
    //    /// </param>
    //    /// <param name="orderBy">
    //    /// order by.
    //    /// </param>
    //    /// <returns>
    //    /// the list of keys in the result.
    //    /// </returns>
    //    public IEnumerable<Guid> List(int maxResults, DateTime? start = null, DateTime? finish = null,
    //        ListResultsOrder orderBy = ListResultsOrder.Descending)
    //    {
    //        throw new NotSupportedException("This method should never run");
    //    }


    //    /// <summary>
    //    /// Returns a <see cref="T:StackExchange.Profiling.MiniProfiler"/> from storage based on <paramref name="id"/>, which
    //    ///     should map to <see cref="P:StackExchange.Profiling.MiniProfiler.Id"/>.
    //    /// </summary>
    //    /// <param name="id">
    //    /// The id.
    //    /// </param>
    //    /// <remarks>
    //    /// Should also update that the resulting profiler has been marked as viewed by its profiling
    //    ///     <see cref="P:StackExchange.Profiling.MiniProfiler.User"/>.
    //    /// </remarks>
    //    /// <returns>
    //    /// The <see cref="T:StackExchange.Profiling.MiniProfiler"/>.
    //    /// </returns>
    //    public MiniProfiler Load(Guid id)
    //    {
    //        throw new NotSupportedException("This method should never run");
    //    }


    //    /// <summary>
    //    /// Stores <paramref name="profiler"/> under its <see cref="P:StackExchange.Profiling.MiniProfiler.Id"/>.
    //    /// </summary>
    //    /// <param name="profiler">
    //    /// The results of a profiling session.
    //    /// </param>
    //    /// <remarks>
    //    /// Should also ensure the profiler is stored as being un-viewed by its profiling
    //    ///     <see cref="P:StackExchange.Profiling.MiniProfiler.User"/>.
    //    /// </remarks>
    //    public void Save(MiniProfiler profiler)
    //    {
    //        if (profiler == null) return;

    //        try
    //        {
    //            var sqls = profiler.GetSqlTimings();
    //            StringBuilder sb = new StringBuilder();
    //            foreach (var s in sqls)
    //            {
    //                StringBuilder par = new StringBuilder();
    //                foreach (var p in s.Parameters)
    //                {
    //                    par.AppendFormat("{0},{1}", p.Name, p.Value);
    //                }
    //                sb.AppendFormat("<span style='color:red;'>{1}</span><br/>{0}<br/>{2}<br/><br/>",
    //                    s.CommandString,
    //                    s.DurationMilliseconds,
    //                    par.ToString()
    //                    );
    //            }

    //            Log.Info(string.Format("User<{0}>: {1},sql : {2}", profiler.User, profiler, sb.ToString()));
    //        }
    //        catch(Exception ex)
    //        {
    //            Log.Exception(ex);
    //        }           
    //    }


    //    /// <summary>
    //    /// Sets a particular profiler session so it is considered "un-viewed"
    //    /// </summary>
    //    /// <param name="user">
    //    /// The user.
    //    /// </param>
    //    /// <param name="id">
    //    /// The id.
    //    /// </param>
    //    public void SetUnviewed(string user, Guid id)
    //    {
    //        // do nothing
    //    }


    //    /// <summary>
    //    /// Sets a particular profiler session to "viewed"
    //    /// </summary>
    //    /// <param name="user">
    //    /// The user.
    //    /// </param>
    //    /// <param name="id">
    //    /// The id.
    //    /// </param>
    //    public void SetViewed(string user, Guid id)
    //    {
    //        throw new NotSupportedException("This method should never run");
    //    }
    //}
}

