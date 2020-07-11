using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Owin.Hosting;
using Owin;
using System;

namespace DashboardApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseHangfireDashboard(String.Empty);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            GlobalConfiguration.Configuration
                .UseColouredConsoleLogProvider()
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseResultsInContinuations()
                .UseSqlServerStorage(@"Server=.\SQLEXPRESS;Database=hangfire_test;Trusted_Connection=True;", new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(1),
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true,
                    EnableHeavyMigrations = true
                });

            using (WebApp.Start<Startup>("http://localhost:12345"))
            {
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }
        }
    }
}
