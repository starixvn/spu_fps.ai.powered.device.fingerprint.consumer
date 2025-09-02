namespace Device.FingerPrint.QueueConsumer.Handlers
{
    using Device.FingerPrint.Core;
    using Device.FingerPrint.Core.Encoders;
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

    public class Consumer : IConsumer<Batch<MessageQueue>>
    {
        private readonly IRawTransactionService rawTransactionService;
        private readonly IRawTransactionAttributeService rawTransactionAttributeService;
        private readonly IMobileRawTransactionService mobileRawTransactionService;
        private readonly ITransactionService transactionService;
        private readonly IAppSettings appSettings; 

        private const string hashKeyEverTagging = "EverTagging";

        public Consumer(
                IRawTransactionService rawTransactionService,
                IRawTransactionAttributeService rawTransactionAttributeService,
                IMobileRawTransactionService mobileRawTransactionService,
                ITransactionService transactionService,
                IAppSettings appSettings
        )
        {
            this.rawTransactionService = rawTransactionService;
            this.rawTransactionAttributeService = rawTransactionAttributeService;
            this.mobileRawTransactionService = mobileRawTransactionService;
            this.transactionService = transactionService;
            this.appSettings = appSettings;
        }

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

            ProcessValidTransactions(rawTransModels);
        }

        private bool ShouldProcessMessages()
        {
            return appSettings.GetValue<bool>("Consumer:IsConsume");
        }

        private static bool HasValidTransactions(IEnumerable<RawTransactionModel> rawTransModels)
        {
            return rawTransModels?.Any() == true;
        }

        private void ProcessValidTransactions(IEnumerable<RawTransactionModel> rawTransModels)
        {
            rawTransactionService.InsertRawTransactionDetail(rawTransModels);
            InsertRawTransactionWithLogging(rawTransModels);
            _ = AdditionalProcessingNotAwait(rawTransModels);
        }

        private void InsertRawTransactionWithLogging(IEnumerable<RawTransactionModel> rawTransModels)
        {
            try
            {
                transactionService.InsertRawTransaction(rawTransModels);
            }
            catch
            {
                rawTransactionService.LogTransactions(rawTransModels);
                throw;
            }
        }

        private IEnumerable<RawTransactionModel> TransformRawTransactionModel(Batch<MessageQueue> messages)
        {
            if (IsEmptyMessageBatch(messages))
            {
                return new List<RawTransactionModel>();
            }

            var transformationContext = CreateTransformationContext();
            var validatedMessages = ValidDuplicateTrans(messages);

            ProcessMessagesInParallel(validatedMessages, transformationContext);
            LogErrorsIfAny(transformationContext.ErrorList);

            return transformationContext.TransformedDataBag.ToList();
        }

        private static bool IsEmptyMessageBatch(Batch<MessageQueue> messages)
        {
            return messages.Length <= 0;
        }

        private static TransformationContext CreateTransformationContext()
        {
            return new TransformationContext
            {
                TransformedDataBag = new ConcurrentBag<RawTransactionModel>(),
                ErrorList = new List<MessageQueue>()
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

            return TransformStandardMessage(message);
        }

        private static bool IsMobileAppTransaction(ConsumeContext<MessageQueue> message)
        {
            return message.Message.Mobile?.IsMobileAppTrans == true;
        }

        private RawTransactionModel TransformStandardMessage(ConsumeContext<MessageQueue> message)
        {
            var rawTransModel = rawTransactionService.TransformRawTransaction(message.Message);
            return rawTransactionAttributeService.GetTransactionAttribute(rawTransModel);
        }

        private static void LogErrorsIfAny(List<MessageQueue> errorList)
        {
            if (errorList.Any())
            {
                var serializedErrors = JsonConvert.SerializeObject(errorList);
                LogService.Error($"Account ErrorList:\n {serializedErrors}");
            }
        }

        private async Task AdditionalProcessingNotAwait(IEnumerable<RawTransactionModel> transactionModels)
        {
            try
            {
                ProcessEverTagging(transactionModels);
            }
            catch (Exception ex)
            {
                _ = QueueConsumer.Utilities.LogErrorAsync(ex);
            }
        }

        private void ProcessEverTagging(IEnumerable<RawTransactionModel> transactionModels)
        {
            var everTagging = GetEncryptedTaggings(transactionModels);

            if (everTagging.Any())
            {
                InsertEverTagging(everTagging);
            }
        }

        private static IEnumerable<string> GetEncryptedTaggings(IEnumerable<RawTransactionModel> transactionModels)
        {
            return transactionModels
                .Where(x => !string.IsNullOrEmpty(x.FPTagging))
                .Select(x => DeviceEncoder.Encrypt(x.FPTagging));
        }

        private void InsertEverTagging(IEnumerable<string> everTagging)
        {
            rawTransactionService.InsertOrUpdateEverTagging(
                hashKeyEverTagging, 
                everTagging, 
                appSettings.EverTaggingExpireSecond);
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
            return groupedMessages
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Skip(1))
                .ToList();
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
