using Hangfire.MAMQSqlExtension;
using System;

namespace ClassLibraryApp1
{
    public class App1_Tasks
    {
        [RetryInQueue("app1_queue")]
        public static void Do_App1_Task() 
        {
            Console.WriteLine("Processing App1_Task");
            if (DateTime.Now.Millisecond % 10 < 2)
                throw new InvalidOperationException("Random fail");
        }
    }
}
