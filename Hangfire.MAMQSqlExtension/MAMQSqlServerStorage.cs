
namespace Hangfire.MAMQSqlExtension
{
    using Hangfire.Logging;
    using Hangfire.Server;
    using Hangfire.States;
    using Hangfire.Storage;
    using Hangfire.SqlServer;
    using System.Collections.Generic;

    public class MAMQSqlServerStorage : JobStorage
    {
        private readonly IEnumerable<string>? _queues;
        private readonly SqlServerStorage _inner;

        public MAMQSqlServerStorage(string nameOrConnectionString, SqlServerStorageOptions options, IEnumerable<string>? queues)
        {
            _inner = new SqlServerStorage(nameOrConnectionString, options);
            _queues = queues;
        }

        public override bool LinearizableReads => _inner.LinearizableReads;

        public override IStorageConnection GetConnection()
        {
            return new MAMQSqlServerConnection(_inner, _queues);
        }

        public override IMonitoringApi GetMonitoringApi()
        {
            return _inner.GetMonitoringApi();
        }

        public override IEnumerable<IServerComponent> GetComponents()
        {
            return _inner.GetComponents();
        }

        public override IEnumerable<IStateHandler> GetStateHandlers()
        {
            return _inner.GetStateHandlers();
        }

        public override void WriteOptionsToLog(ILog logger)
        {
            _inner.WriteOptionsToLog(logger);
        }
    }
}
