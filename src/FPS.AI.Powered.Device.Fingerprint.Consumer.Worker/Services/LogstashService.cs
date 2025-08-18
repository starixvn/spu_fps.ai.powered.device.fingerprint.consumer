using Device.FingerPrint.Core;
using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Models;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Device.FingerPrint.QueueConsumer.Services
{
    public interface ILogstashService
    {
        Task PushLogAsync(IEnumerable<RawTransactionModel> transactions);
    }

    public class LogstashService : ILogstashService
    {
        private readonly IAppSettings appSettings;
        private readonly Fanex.Logging.ILogger logger;

        public LogstashService(IAppSettings appSettings, Fanex.Logging.ILogger logger)
        {
            this.appSettings = appSettings;
            this.logger = logger;
        }

        public async Task PushLogAsync(IEnumerable<RawTransactionModel> transactions)
        {
            using var client = new TcpClient(appSettings.LogstashTcpHostname, appSettings.LogstashTcpPort);
            using var stream = client.GetStream();

            foreach (var transaction in transactions)
            {
                if (appSettings.LogstashIgnoreSubscribers.Count != 0 && appSettings.LogstashIgnoreSubscribers.Contains(transaction.SubscriberName.ToLowerInvariant()))
                {
                    continue;
                }

                var logMessage = TransformLogMessage(transaction);

                string jsonLog = JsonConvert.SerializeObject(logMessage);

                byte[] data = Encoding.UTF8.GetBytes(jsonLog + "\n");
                await stream.WriteAsync(data);
            }
        }

        private TransactionLogstashModel TransformLogMessage(RawTransactionModel transaction)
        {
            var fingerprintLogstashModel = GetFingerprintAttributes(transaction.FingerprintAttribute?.FPFingerPrintAttribute);

            transaction.SubscriberName = transaction.SubscriberName.ToLowerInvariant();

            var logMessage = new TransactionLogstashModel
            {
                Message = transaction,
                Tags = "LoginTransaction",
                FingerprintAttributes = fingerprintLogstashModel?.FingerprintAttributes,
                UnhandledAttributes = fingerprintLogstashModel?.UnhandledAttributes,
            };

            return logMessage;
        }

        private FingerprintLogstashModel GetFingerprintAttributes<T>(T fingerprintAttribute)
        {
            var attributes = new List<string>();
            var unhandledAttributes = new List<string>();

            if (EqualityComparer<T>.Default.Equals(fingerprintAttribute, default(T)))
                return null;

            var unhandledValues = appSettings.UnhandledFingerprintAttributeValues;

            foreach (var property in fingerprintAttribute.GetType().GetProperties())
            {
                var propertyValue = property.GetValue(fingerprintAttribute);

                if (propertyValue == null)
                {
                    unhandledAttributes.Add(property.Name);
                    continue;
                }

                if (propertyValue.GetType() == typeof(string))
                {
                    var stringValue = propertyValue.ToString();
                    if (!string.IsNullOrEmpty(stringValue)
                        && !unhandledValues.Contains(stringValue.Trim('"').ToLowerInvariant()))
                    {
                        attributes.Add(property.Name);
                        continue;
                    }

                    unhandledAttributes.Add(property.Name);
                }
                else
                {
                    attributes.Add(property.Name);
                }
            }

            return new FingerprintLogstashModel
            {
                FingerprintAttributes = attributes,
                UnhandledAttributes = unhandledAttributes
            };
        }

        private void LogException(Exception ex)
        {
            var logMessage = new LogMessage(ex.Message)
                   .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                   .ToString();

            logger.Error(logMessage);
        }
    }
}
