# Hangfire.MAMQSqlExtension
A **M**ulti-**A**pp, **M**ulti-**Q**ueue **SqlExtension** for running multiple servers for multiple applications against a single SQL Server database.

## The problem
An intuitive idea of running multiple Hangfire servers for multiple applications is to run different applications in different queues. Each queue can be watched by a dedicated server.

For example: 

`AppServer1` could run jobs defined in library `AppLib1` that are registered as recurring jobs in queue `AppQueue1`. And `AppServer2` would run jobs defined in library `AppLib2` that are registered as recurring jobs in queue `AppQueue2`. And the two server executables would only load assemblies belonging to their own applications and can be totally independent to each other.

Unfortunately this does not work. Hangfire job queue was initially designed only for job execution, not for job scheduling. In the set up above, schedulers in `AppServer1` will see jobs in `AppQueue2` and would not respect the queue setting. It tries to deserialize the jobs for the purpose of scheduling. This will immediately cause a dll not found exception. What's more, when job fails and being retried, retries will also not respect the queue setting. Jobs set for `AppQueue2` will be retired by `AppServer1` and will cause retry failure again.

This extension attempts to provide a workaround before the multiple application, multiple server, single database setup is officially supported.


### Usage
Usage examples are the provided in the [examples](https://github.com/GeXiaoguo/Hangfire.MAMQSqlExtension/tree/master/examples). The necessary steps are listed below.

1. Install nuget package [Hangfire.MAMQSqlExtension](https://www.nuget.org/packages/Hangfire.MAMQSqlExtension/1.0.1):  i.e. `Install-Package Hangfire.MAMQSqlExtension -Version 1.0.2`

2. Filter the jobs according to their queue settings before giving them to `RecurringJobScheduler` and `DelayedJobScheduler` for scheduling

```
GlobalConfiguration.Configuration
   .UseMAMQSqlServerStorage(connectionString, new SqlServerStorageOptions{...}, new[]{ "MyQueue" })
```

3. Force the job retries to respect the queue setting

```
[RetryInQueue("MyQueue")]
public void MethodForJob()
{
    ...
}
```

4. Specify queues for the `BackgroundJobServer`

```
    var serverOptions = new BackgroundJobServerOptions
    {
        Queues = new [] {"MyQueue"},
    }
    new BackgroundJobServer(serverOptions)
```

5. Specify a queue when registering the job

```
RecurringJob.AddOrUpdate(jobID, jobAction, cron, TimeZoneInfo.Local, "MyQueue");
```

6. In addition, since the Dashboard is database specific, it has to be shared between the two applications. The dashboard will need to be able to deserialize jobs for both applications. This means that the Dashboard executable needs to reference both all dlls that the jobs are defined in (e.g. `AppLib1` and `AppLib2`).
