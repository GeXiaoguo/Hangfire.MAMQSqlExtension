namespace Hangfire.MAMQSqlExtension
{
    using Hangfire;
    using Hangfire.SqlServer;
    using System;
    using System.Collections.Generic;

    public static class MAMQIGlobalConfigurationExtension
    {
        public static IGlobalConfiguration<MAMQSqlServerStorage> UseMAMQSqlServerStorage(
            this IGlobalConfiguration configuration,
            string nameOrConnectionString,
            SqlServerStorageOptions options,
           IEnumerable<string> queues)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (nameOrConnectionString == null) throw new ArgumentNullException(nameof(nameOrConnectionString));
            if (options == null) throw new ArgumentNullException(nameof(options));

            var storage = new MAMQSqlServerStorage(nameOrConnectionString, options, queues);
            return configuration.UseStorage(storage);
        }
    }
}
