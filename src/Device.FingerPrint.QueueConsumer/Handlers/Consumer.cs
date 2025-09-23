namespace Device.FingerPrint.QueueConsumer.Handlers
{
    using Device.FingerPrint.Core;
    using Device.FingerPrint.Features.Transaction.Services;
    using Device.FingerPrint.Model;
    using Device.FingerPrint.QueueConsumer.Services;
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
        private readonly object _errorListLock = new();

        public async Task Consume(ConsumeContext<Batch<MessageQueue>> context)
        {
            try
            {
                await ProcessMessageBatch(context.Message);
            }
            catch (Exception ex)
            {
                _ = QueueConsumer.Utilities.LogErrorAsync(ex);
                throw;
            }
        }

        private async Task ProcessMessageBatch(Batch<MessageQueue> messageBatch)
        {
            if (!appSettings.GetValue<bool>("Consumer:IsConsume"))
            {
                return;
            }

            var rawTransModels = TransformRawTransactionModel(messageBatch);
            
            if (rawTransModels?.Any() != true)
            {
                return;
            }

            await ProcessTransactionsByFlow(rawTransModels);
        }

        private async Task ProcessTransactionsByFlow(IEnumerable<RawTransactionModel> rawTransModels)
        {
            var newFlowMessages = rawTransModels.Where(m => m.TransFlow == 1);
            var regularFlowMessages = rawTransModels.Where(m => m.TransFlow == 0);

            if (newFlowMessages.Any())
            {
                await ProcessTransactionsNewFlow(newFlowMessages);
            }
            
            if (regularFlowMessages.Any())
            {
                transactionService.InsertRawTransaction(regularFlowMessages);
            }
        }

        private async Task ProcessTransactionsNewFlow(IEnumerable<RawTransactionModel> rawTransModels)
        {
            var task1 = Task.Run(() => transactionService.InsertRawFingerprint(rawTransModels));
            var task2 = Task.Run(() => transactionService.InsertRawTransaction(rawTransModels));
            var task3 = Task.Run(() => transactionService.InsertWebRTCIP(rawTransModels));

            await Task.WhenAll(task1, task2, task3);
        }

        private IEnumerable<RawTransactionModel> TransformRawTransactionModel(Batch<MessageQueue> messages)
        {
            if (messages.Length <= 0)
            {
                return [];
            }

            var validatedMessages = ValidDuplicateTrans(messages);
            var transformedData = TransformMessages(validatedMessages);
            
            return transformedData.transformedModels;
        }

        private (IEnumerable<RawTransactionModel> transformedModels, List<MessageQueue> errors) TransformMessages(IEnumerable<ConsumeContext<MessageQueue>> validatedMessages)
        {
            var transformedDataBag = new ConcurrentBag<RawTransactionModel>();
            var errorList = new List<MessageQueue>();
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 10 };

            Parallel.ForEach(validatedMessages, parallelOptions, message =>
            {
                if (!IsValidMessage(message))
                {
                    lock (_errorListLock)
                    {
                        errorList.Add(message.Message);
                    }
                    return;
                }

                var transformedModel = TryTransformMessage(message);
                if (transformedModel != null)
                {
                    transformedDataBag.Add(transformedModel);
                }
            });

            LogErrorsIfAny(errorList);
            return (transformedDataBag.ToArray(), errorList);
        }

        private static bool IsValidMessage(ConsumeContext<MessageQueue> message)
        {
            return message?.Message != null &&
                   !string.IsNullOrEmpty(message.Message.Account) &&
                   !Utilities.ContainsUnicodeCharacter(message.Message.Account) &&
                   message.Message.Account.Length <= 100;
        }

        private RawTransactionModel? TryTransformMessage(ConsumeContext<MessageQueue> message)
        {
            try
            {
                return IsMobileAppTransaction(message)
                    ? mobileRawTransactionService.TransformRawTransaction(message.Message)
                    : rawTransactionService.TransformRawTransaction(message.Message);
            }
            catch (Exception ex)
            {
                _ = QueueConsumer.Utilities.LogErrorAsync(ex);
                return null;
            }
        }

        private static bool IsMobileAppTransaction(ConsumeContext<MessageQueue> message)
        {
            return message.Message.Mobile?.IsMobileAppTrans == true;
        }

        private static void LogErrorsIfAny(List<MessageQueue> errorList)
        {
            if (errorList.Count != 0)
            {
                var serializedErrors = JsonConvert.SerializeObject(errorList);
                LogService.Error($"Account ErrorList:\n {serializedErrors}");
            }
        }

        private static IEnumerable<ConsumeContext<MessageQueue>> ValidDuplicateTrans(Batch<MessageQueue> listMessage)
        {
            var groupedMessages = GroupMessagesByAccountAndDate(listMessage);
            var duplicateMessages = GetDuplicateMessages(groupedMessages);

            LogDuplicateMessages(duplicateMessages);

            return GetValidMessages(listMessage, duplicateMessages);
        }

        private static IEnumerable<IGrouping<object, ConsumeContext<MessageQueue>>> GroupMessagesByAccountAndDate(Batch<MessageQueue> listMessage)
        {
            return listMessage.GroupBy(x => new { x.Message.Account, x.Message.TransDate });
        }

        private static List<ConsumeContext<MessageQueue>> GetDuplicateMessages(IEnumerable<IGrouping<object, ConsumeContext<MessageQueue>>> groupedMessages)
        {
            return [.. groupedMessages
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Skip(1))];
        }

        private static void LogDuplicateMessages(List<ConsumeContext<MessageQueue>> duplicateMessages)
        {
            if (duplicateMessages.Count > 0)
            {
                var duplicateMessagesContent = duplicateMessages.Select(x => x.Message);
                var serializedDuplicates = JsonConvert.SerializeObject(duplicateMessagesContent);
                LogService.Error($"DuplicateMessages:\n {serializedDuplicates}");
            }
        }

        private static IEnumerable<ConsumeContext<MessageQueue>> GetValidMessages(Batch<MessageQueue> listMessage, List<ConsumeContext<MessageQueue>> duplicateMessages)
        {
            return listMessage.Except(duplicateMessages);
        }

    }
}
