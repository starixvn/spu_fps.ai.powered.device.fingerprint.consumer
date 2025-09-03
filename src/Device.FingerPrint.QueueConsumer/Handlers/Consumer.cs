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
                ProcessMessageBatch(context.Message);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _ = QueueConsumer.Utilities.LogErrorAsync(ex);
                throw;
            }
        }

        private void ProcessMessageBatch(Batch<MessageQueue> messageBatch)
        {
            if (!ShouldProcessMessages())
            {
                return;
            }

            var rawTransModels = TransformRawTransactionModel(messageBatch);

            if (!HasValidTransactions(rawTransModels))
            {
                return;
            }

            transactionService.InsertRawTransaction(rawTransModels);
        }

        private bool ShouldProcessMessages()
        {
            return appSettings.GetValue<bool>("Consumer:IsConsume");
        }

        private static bool HasValidTransactions(IEnumerable<RawTransactionModel> rawTransModels)
        {
            return rawTransModels?.Any() == true;
        }

        private IEnumerable<RawTransactionModel> TransformRawTransactionModel(Batch<MessageQueue> messages)
        {
            if (IsEmptyMessageBatch(messages))
            {
                return [];
            }

            var transformationContext = CreateTransformationContext();
            var validatedMessages = ValidDuplicateTrans(messages);

            ProcessMessagesInParallel(validatedMessages, transformationContext);
            LogErrorsIfAny(transformationContext.ErrorList);

            return [.. transformationContext.TransformedDataBag];
        }

        private static bool IsEmptyMessageBatch(Batch<MessageQueue> messages)
        {
            return messages.Length <= 0;
        }

        private static TransformationContext CreateTransformationContext()
        {
            return new TransformationContext
            {
                TransformedDataBag = [],
                ErrorList = []
            };
        }

        private void ProcessMessagesInParallel(IEnumerable<ConsumeContext<MessageQueue>> validatedMessages, TransformationContext context)
        {
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 10 };

            Parallel.ForEach(validatedMessages, parallelOptions, message =>
            {
                ProcessSingleMessage(message, context);
            });
        }

        private void ProcessSingleMessage(ConsumeContext<MessageQueue> message, TransformationContext context)
        {
            if (!IsValidMessage(message))
            {
                AddToErrorList(message, context.ErrorList);
                return;
            }

            TryTransformMessage(message, context.TransformedDataBag);
        }

        private static void AddToErrorList(ConsumeContext<MessageQueue> message, List<MessageQueue> errorList)
        {
            errorList.Add(message.Message);
        }

        private void TryTransformMessage(ConsumeContext<MessageQueue> message, ConcurrentBag<RawTransactionModel> transformedDataBag)
        {
            try
            {
                var rawTransFinalModel = TransformMessage(message);

                if (rawTransFinalModel != null)
                {
                    transformedDataBag.Add(rawTransFinalModel);
                }
            }
            catch (Exception ex)
            {
                _ = QueueConsumer.Utilities.LogErrorAsync(ex);
            }
        }

        private static bool IsValidMessage(ConsumeContext<MessageQueue> message)
        {
            return message?.Message != null &&
                   !string.IsNullOrEmpty(message.Message.Account) &&
                   !Utilities.ContainsUnicodeCharacter(message.Message.Account) &&
                   message.Message.Account.Length <= 100;
        }

        private RawTransactionModel TransformMessage(ConsumeContext<MessageQueue> message)
        {
            if (IsMobileAppTransaction(message))
            {
                return mobileRawTransactionService.TransformRawTransaction(message.Message);
            }

            return rawTransactionService.TransformRawTransaction(message.Message);
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

        private sealed class TransformationContext
        {
            public ConcurrentBag<RawTransactionModel> TransformedDataBag { get; set; }
            public List<MessageQueue> ErrorList { get; set; }
        }
    }
}
