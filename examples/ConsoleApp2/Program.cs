using System;
using ClassLibraryApp2;
using Hangfire;
using Hangfire.MAMQSqlExtension;
using Hangfire.SqlServer;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            GlobalConfiguration.Configuration
                .UseMAMQSqlServerStorage(@"Server=.\SQLEXPRESS;Database=hangfire_test;Trusted_Connection=True;", new SqlServerStorageOptions
                {
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true,
                }, new[] { "app2_queue" })
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings();

            RecurringJob.AddOrUpdate("app2_job", () => App2_Tasks.Do_App2_Task(), Cron.Minutely, TimeZoneInfo.Local, "app2_queue");

            var optoins = new BackgroundJobServerOptions
            {
                Queues = new[] { "app2_queue" }
            };
            using (var server = new BackgroundJobServer(optoins))
            {
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }
        }
    }
}
