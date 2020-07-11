using ClassLibraryApp1;
using Hangfire;
using Hangfire.MAMQSqlExtension;
using Hangfire.SqlServer;
using System;

namespace ConsoleApp1
{
    class Program
    {
        public static void Main()
        {
            GlobalConfiguration.Configuration
                .UseColouredConsoleLogProvider()
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseResultsInContinuations()
                .UseMAMQSqlServerStorage(@"Server=.\SQLEXPRESS;Database=hangfire_test;Trusted_Connection=True;", new SqlServerStorageOptions
                {
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true,
                }, new[] {"app1_queue" });

            RecurringJob.AddOrUpdate("app1_job", () => App1_Tasks.Do_App1_Task(), Cron.Minutely, TimeZoneInfo.Local, "app1_queue");

            var serverOptions = new BackgroundJobServerOptions
            {
                Queues = new[] { "app1_queue", "default" },
            };

            using (var server = new BackgroundJobServer(serverOptions))
            {
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }
        }
    }
}
