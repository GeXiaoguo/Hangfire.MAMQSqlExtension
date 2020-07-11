using Hangfire.MAMQSqlExtension;
using System;

namespace ClassLibraryApp2
{
    public class App2_Tasks
    {
        [RetryInQueue("app2_queue")]
        public static void Do_App2_Task()
        {
            Console.WriteLine("Processing App2_Task");
            if (DateTime.Now.Millisecond % 10 < 2)
                throw new InvalidOperationException("Random fail");
        }
    }
}
