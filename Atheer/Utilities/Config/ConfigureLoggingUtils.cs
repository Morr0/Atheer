using System;
using System.Linq;
using System.Threading;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using AWS.Logger;
using Microsoft.Extensions.Logging;

namespace Atheer.Utilities.Config
{
    public static class ConfigureLoggingUtils
    {
        public static void Handle(bool isProduction, ILoggingBuilder builder, string awsLogGroupName)
        {
            builder.ClearProviders();

            if (!isProduction)
            {
                builder.AddConsole();
            }

            if (string.IsNullOrEmpty(awsLogGroupName)) return;
            
            string logGroupName = $"Atheer-Server-App-{awsLogGroupName}";
            using var cloudwatchLogsClient = new AmazonCloudWatchLogsClient();
                
            bool logGroupExists = LogGroupExists(cloudwatchLogsClient, logGroupName);
            if (!logGroupExists)
            {
                int days = isProduction ? 31 : 1;
                CreateLogGroup(cloudwatchLogsClient, logGroupName, days);
            }

            var awsLoggerConfig = new AWSLoggerConfig
            {
                LogGroup = logGroupName,
                BatchPushInterval = isProduction ? TimeSpan.FromMinutes(1) : TimeSpan.FromSeconds(2),
                DisableLogGroupCreation = true,
                LogStreamNamePrefix = string.Empty
            };

            builder.AddAWSProvider(awsLoggerConfig, LogLevel.Information);
            Console.WriteLine($"Will write logs to: {logGroupName}");
        }

        private static bool LogGroupExists(IAmazonCloudWatchLogs client, string logGroupName)
        {
            var describeLogGroupRequest = new DescribeLogGroupsRequest
            {
                LogGroupNamePrefix = logGroupName,
                Limit = 1
            };
            var describeLogGroupResponse =
                (client.DescribeLogGroupsAsync(describeLogGroupRequest)).GetAwaiter()
                .GetResult();
            
            return describeLogGroupResponse.LogGroups.Any(x => x.LogGroupName == logGroupName);
        }

        private static void CreateLogGroup(IAmazonCloudWatchLogs client, string logGroupName, int retentionDays)
        {
            var createLogGroupRequest = new CreateLogGroupRequest
            {
                LogGroupName = logGroupName
            };
            (client.CreateLogGroupAsync(createLogGroupRequest)).GetAwaiter()
                .GetResult();
                    
            Thread.Sleep(1000);

                    
            var putRetentionPolicyRequest = new PutRetentionPolicyRequest
            {
                LogGroupName = logGroupName,
                RetentionInDays = retentionDays
            };
            (client.PutRetentionPolicyAsync(putRetentionPolicyRequest))
                .GetAwaiter().GetResult();
        }
    }
}