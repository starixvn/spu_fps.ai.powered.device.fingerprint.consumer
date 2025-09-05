namespace Device.FingerPrint.QueueConsumer.Handlers
{
    using Device.FingerPrint.Core;
    using Device.FingerPrint.Features.Transaction.Services;
    using Device.FingerPrint.Model;
    using Device.FingerPrint.QueueConsumer.Services;
    using Device.FingerPrint.TransactionAPI.Model;
    using Fanex.Logging;
    using MassTransit;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class Consumer(
            IRawTransactionService rawTransactionService,
            IMobileRawTransactionService mobileRawTransactionService,
            ITransactionService transactionService,
            IAppSettings appSettings
        ) : IConsumer<Batch<MessageQueue>>
    {
        private readonly IRawTransactionService rawTransactionService = rawTransactionService;
        private readonly IMobileRawTransactionService mobileRawTransactionService = mobileRawTransactionService;
        private readonly ITransactionService transactionService = transactionService;
        private readonly IAppSettings appSettings = appSettings;

        public Task Consume(ConsumeContext<Batch<MessageQueue>> context)
        {
            try
            {
                if (ShouldSkipProcessing(context))
                {
                    return Task.CompletedTask;   
                }

                var rawTransModels = TransformMessages(context.Message);
                if (rawTransModels.Any())
                {
                    ProcessTransactions(rawTransModels);
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _ = QueueConsumer.Utilities.LogErrorAsync(ex);
                throw;
            }
        }

        private bool ShouldSkipProcessing(ConsumeContext<Batch<MessageQueue>> context)
        {
            return !appSettings.GetValue<bool>("Consumer:IsConsume") || context.Message.Length <= 0;
        }

        private void ProcessTransactions(IEnumerable<RawTransactionModel> rawTransModels)
        {
            transactionService.InsertRawFingerprint(rawTransModels);
            transactionService.InsertRawTransaction(rawTransModels);
        }

        private IEnumerable<RawTransactionModel> TransformMessages(Batch<MessageQueue> messages)
        {
            var validMessages = RemoveDuplicates(messages);
            var transformedData = new ConcurrentBag<RawTransactionModel>();
            var errorList = new ConcurrentBag<MessageQueue>();

            Parallel.ForEach(validMessages, new ParallelOptions { MaxDegreeOfParallelism = 10 }, message =>
            {
                ProcessSingleMessage(message, transformedData, errorList);
            });

            LogErrors([.. errorList]);
            return transformedData;
        }

        private void ProcessSingleMessage(
            ConsumeContext<MessageQueue> message,
            ConcurrentBag<RawTransactionModel> transformedData,
            ConcurrentBag<MessageQueue> errorList)
        {
            if (!IsValidMessage(message))
            {
                errorList.Add(message.Message);
                return;
            }

            try
            {
                var transformed = TransformMessage(message.Message);
                if (transformed != null)
                {
                    transformedData.Add(transformed);   
                }
            }
            catch (Exception ex)
            {
                _ = QueueConsumer.Utilities.LogErrorAsync(ex);
            }
        }

        private RawTransactionModel? TransformMessage(MessageQueue message)
        {
            return message.Mobile?.IsMobileAppTrans == true
                ? mobileRawTransactionService.TransformRawTransaction(message)
                : rawTransactionService.TransformRawTransaction(message);
        }

        private static IEnumerable<ConsumeContext<MessageQueue>> RemoveDuplicates(Batch<MessageQueue> messages)
        {
            var duplicates = messages
                .GroupBy(x => new { x.Message.Account, x.Message.TransDate })
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Skip(1))
                .ToList();

            if (duplicates.Count > 0)
            {
                var duplicateContent = JsonConvert.SerializeObject(duplicates.Select(x => x.Message));
                LogService.Error($"DuplicateMessages:\n {duplicateContent}");
            }

            return messages.Except(duplicates);
        }

        private static bool IsValidMessage(ConsumeContext<MessageQueue> message)
        {
            return message?.Message != null &&
                   !string.IsNullOrEmpty(message.Message.Account) &&
                   !Utilities.ContainsUnicodeCharacter(message.Message.Account) &&
                   message.Message.Account.Length <= 100;
        }

        private static void LogErrors(List<MessageQueue> errorList)
        {
            if (errorList.Count > 0)
            {
                var serializedErrors = JsonConvert.SerializeObject(errorList);
                LogService.Error($"Account ErrorList:\n {serializedErrors}");
            }
        }
    }
}