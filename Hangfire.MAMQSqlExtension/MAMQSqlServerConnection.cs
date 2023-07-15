

namespace Hangfire.MAMQSqlExtension
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Hangfire.Annotations;
    using Hangfire.Common;
    using Hangfire.Server;
    using Hangfire.SqlServer;
    using Hangfire.Storage;

    internal class MAMQSqlServerConnection : JobStorageConnection
    {
        private readonly JobStorageConnection _sqlServerConnection;
        private readonly IEnumerable<string>? _queues;

        public MAMQSqlServerConnection(SqlServerStorage storage, IEnumerable<string>? queues)
        {
            const string sqlServerConnectionTypeName = "Hangfire.SqlServer.SqlServerConnection";
            Type? type = typeof(SqlServerStorage)
                .Assembly
                .GetType(sqlServerConnectionTypeName);

            if (type == null)
            {
                throw new InvalidOperationException($"{sqlServerConnectionTypeName} has not been loaded into the process.");
            }

            var connection = type.Assembly.CreateInstance(type.FullName, false, BindingFlags.Instance | BindingFlags.Public, null, new[] { storage }, null, null) as JobStorageConnection;
            _sqlServerConnection = connection ?? throw new InvalidOperationException($"Could not create an instance for {sqlServerConnectionTypeName}");
            _queues = queues;
        }

        public override void Dispose()
        {
            _sqlServerConnection.Dispose();
        }

        public override IWriteOnlyTransaction CreateWriteTransaction()
        {
            return _sqlServerConnection.CreateWriteTransaction();
        }

        public override IDisposable AcquireDistributedLock([NotNull] string resource, TimeSpan timeout)
        {
            return _sqlServerConnection.AcquireDistributedLock(resource, timeout);
        }

        public override IFetchedJob FetchNextJob(string[] queues, CancellationToken cancellationToken)
        {
            return _sqlServerConnection.FetchNextJob(queues, cancellationToken);
        }

        public override string CreateExpiredJob(
            Job job,
            IDictionary<string, string> parameters,
            DateTime createdAt,
            TimeSpan expireIn)
        {
            return _sqlServerConnection.CreateExpiredJob(job, parameters, createdAt, expireIn);
        }

        public override JobData GetJobData(string id)
        {
            return _sqlServerConnection.GetJobData(id);
        }

        public override StateData GetStateData(string jobId)
        {
            return _sqlServerConnection.GetStateData(jobId);
        }

        public override void SetJobParameter(string id, string name, string value)
        {
            _sqlServerConnection.SetJobParameter(id, name, value);
        }

        public override string GetJobParameter(string id, string name)
        {
            return _sqlServerConnection.GetJobParameter(id, name);
        }

        public override HashSet<string> GetAllItemsFromSet(string key)
        {
            return _sqlServerConnection.GetAllItemsFromSet(key);
        }

        public override string GetFirstByLowestScoreFromSet(string key, double fromScore, double toScore)
        {
            return _sqlServerConnection.GetFirstByLowestScoreFromSet(key, fromScore, toScore);
        }

        public override List<string> GetFirstByLowestScoreFromSet(string key, double fromScore, double toScore, int count)
        {
            if (!_queues.Any() || string.IsNullOrEmpty(key))
                return new List<string>();

            var ids = _sqlServerConnection.GetFirstByLowestScoreFromSet(key, fromScore, toScore, count);

            if (_queues == null)
                return ids;

            return ids.Where(id =>
            {
                string? recurringJobId = key switch
                {
                    "recurring-jobs" => id,
                    "schedule" => _sqlServerConnection.GetJobParameter(id, "RecurringJobId")?.Replace("\"", ""),
                    _ => throw new InvalidOperationException($"{key} is not a recognized job type")
                };

                string queue = recurringJobId switch
                {
                    null => _sqlServerConnection.GetJobParameter(id, "Queue")?.Replace("\"", "") ?? "default",
                    _ => _sqlServerConnection.GetValueFromHash($"recurring-job:{recurringJobId}", "Queue") ?? "default"
                };

                return _queues.Contains(queue);
            })
            .ToList();
        }

        public override void SetRangeInHash(string key, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            _sqlServerConnection.SetRangeInHash(key, keyValuePairs);
        }

        public override Dictionary<string, string> GetAllEntriesFromHash(string key)
        {
            return _sqlServerConnection.GetAllEntriesFromHash(key);
        }

        public override void AnnounceServer(string serverId, ServerContext context)
        {
            _sqlServerConnection.AnnounceServer(serverId, context);
        }

        public override void RemoveServer(string serverId)
        {
            _sqlServerConnection.RemoveServer(serverId);
        }

        public override void Heartbeat(string serverId)
        {
            _sqlServerConnection.Heartbeat(serverId);
        }

        public override int RemoveTimedOutServers(TimeSpan timeOut)
        {
            return _sqlServerConnection.RemoveTimedOutServers(timeOut);
        }

        public override long GetSetCount(string key)
        {
            return _sqlServerConnection.GetSetCount(key);
        }

        public override List<string> GetRangeFromSet(string key, int startingFrom, int endingAt)
        {
            return _sqlServerConnection.GetRangeFromSet(key, startingFrom, endingAt);
        }

        public override TimeSpan GetSetTtl(string key)
        {
            return _sqlServerConnection.GetSetTtl(key);
        }

        public override long GetCounter(string key)
        {
            return _sqlServerConnection.GetCounter(key);
        }

        public override long GetHashCount(string key)
        {
            return _sqlServerConnection.GetHashCount(key);
        }

        public override TimeSpan GetHashTtl(string key)
        {
            return _sqlServerConnection.GetHashTtl(key);
        }

        public override string GetValueFromHash(string key, string name)
        {
            return _sqlServerConnection.GetValueFromHash(key, name);
        }

        public override long GetListCount(string key)
        {
            return _sqlServerConnection.GetListCount(key);
        }

        public override TimeSpan GetListTtl(string key)
        {
            return _sqlServerConnection.GetListTtl(key);
        }

        public override List<string> GetRangeFromList(string key, int startingFrom, int endingAt)
        {
            return _sqlServerConnection.GetRangeFromList(key, startingFrom, endingAt);
        }

        public override List<string> GetAllItemsFromList(string key)
        {
            return _sqlServerConnection.GetAllItemsFromList(key);
        }
    }
}
