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
                if (!appSettings.GetValue<bool>("Consumer:IsConsume"))
                {
                    return Task.CompletedTask;
                }

                var rawTransModels = TransformRawTransactionModel(context.Message);

                if (rawTransModels != null && rawTransModels.Any())
                {
                    ProcessRawTransactions(rawTransModels);
                }

                return Task.CompletedTask;
            }           
            catch (Exception ex)
            {
                _ = QueueConsumer.Utilities.LogErrorAsync(ex);
                throw;
            }
        }

        private void ProcessRawTransactions(IEnumerable<RawTransactionModel> rawTransModels)
        {
            rawTransactionService.InsertRawTransactionDetail(rawTransModels, appSettings.IsExtraSub);

            try
            {
                transactionService.InsertRawTransaction(rawTransModels, appSettings.IsExtraSub);
            }
            catch
            {
                rawTransactionService.LogTransactions(rawTransModels);
                throw;
            }

            _ = AdditionalProcessingNotAwait(rawTransModels);
        }

        private IEnumerable<RawTransactionModel> TransformRawTransactionModel(Batch<MessageQueue> messages)
        {
            if (messages.Length <= 0)
            {
                return new List<RawTransactionModel>();
            }

            var transformedDataBag = new ConcurrentBag<RawTransactionModel>();
            var errorList = new List<MessageQueue>();

            var validedMessages = ValidDuplicateTrans(messages);

            Parallel.ForEach(validedMessages, new ParallelOptions { MaxDegreeOfParallelism = 10 }, message =>
            {
                if (IsValidMessage(message))
                {
                    ProcessMessage(message, transformedDataBag);
                }
                else
                {
                    errorList.Add(message.Message);
                }
            });

            if (errorList.Any())
            {
                // Error logging commented out - not needed for production
            }

            return transformedDataBag.ToList();
        }

        private static bool IsValidMessage(ConsumeContext<MessageQueue> message)
        {
            return message?.Message != null
                && !string.IsNullOrEmpty(message.Message.Account)
                && !Utilities.ContainsUnicodeCharacter(message.Message.Account)
                && message.Message.Account.Length <= 100;
        }

        private void ProcessMessage(ConsumeContext<MessageQueue> message, ConcurrentBag<RawTransactionModel> transformedDataBag)
        {
            try
            {
                var rawTransFinalModel = TransformMessageToRawTransaction(message.Message);

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

        private RawTransactionModel TransformMessageToRawTransaction(MessageQueue message)
        {
            if (message.Mobile != null && message.Mobile.IsMobileAppTrans)
            {
                return mobileRawTransactionService.TransformRawTransaction(message);
            }
            else
            {
                var rawTransModel = rawTransactionService.TransformRawTransaction(message);
                return rawTransactionAttributeService.GetTransactionAttribute(rawTransModel);
            }
        }

        private async Task AdditionalProcessingNotAwait(IEnumerable<RawTransactionModel> transactionModels)
        {
            try
            {
                var everTagging = transactionModels
                                .Where(x => !string.IsNullOrEmpty(x.FPTagging))
                                .Select(x => DeviceEncoder.Encrypt(x.FPTagging));

                if (everTagging.Any())
                {
                    rawTransactionService.InsertOrUpdateEverTagging(hashKeyEverTagging, everTagging, appSettings.EverTaggingExpireSecond);
                }
            }
            catch (Exception ex)
            {
                _ = QueueConsumer.Utilities.LogErrorAsync(ex);
            }
        }

        private static IEnumerable<ConsumeContext<MessageQueue>> ValidDuplicateTrans(Batch<MessageQueue> listMessage)
        {
            var groupedMessages = listMessage.GroupBy(x => new { x.Message.Account, x.Message.TransDate });

            var dupMessages = groupedMessages
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Skip(1))
                .ToList();

            if (dupMessages.Count > 0)
            {
                var duplicateMessagesContent = dupMessages.Select(x => x.Message);
                LogService.Error($"DuplicateMessages:\n {JsonConvert.SerializeObject(duplicateMessagesContent)}");
            }

            var validedMessages = listMessage.Except(dupMessages);
            return validedMessages;
        }
    }
}
