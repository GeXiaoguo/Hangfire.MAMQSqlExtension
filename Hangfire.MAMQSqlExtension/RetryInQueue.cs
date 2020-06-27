namespace Hangfire.MAMQSqlExtension
{
    using Hangfire.Common;
    using Hangfire.States;
    using Hangfire.Storage;

    public class RetryInQueueAttribute : JobFilterAttribute, IApplyStateFilter
    {
        private readonly string _queue;

        public RetryInQueueAttribute(string queue)
        {
            _queue = queue;
        }

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            var newState = context.NewState as EnqueuedState;
            if (!string.IsNullOrWhiteSpace(_queue) && newState != null && newState.Queue != _queue)
            {
                newState.Queue = _queue;
            }
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
        }
    }
}