using FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Encoders;
using FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Model;
using FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Services;
using FPS.Device.Fingerprint.Service;
using MassTransit;
using System.Collections.Concurrent;

namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Handlers
{
    public class Consumer(
            ITransactionService transactionService,
            IMobileRawTransactionService mobileRawTransactionService,
            IAppSettings appSettings
        ) : IConsumer<Batch<MessageQueue>>
    {
        private readonly IMobileRawTransactionService mobileRawTransactionService = mobileRawTransactionService;
        private readonly IAppSettings appSettings = appSettings;

        private const string hashKeyEverTagging = "EverTagging";

        public Task Consume(ConsumeContext<Batch<MessageQueue>> context)
        {
            try
            {
                var rawTransModels = TransformRawTransactionModel(context.Message);

                if (rawTransModels != null && rawTransModels.Any())
                {
                    try
                    {
                        transactionService.InsertRawTransaction(rawTransModels);
                    }
                    catch
                    {
                        throw;
                    }

                    _ = AdditionalProcessingNotAwait(rawTransModels);
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                //_ = QueueConsumer.Utilities.LogErrorAsync(ex);
                throw;
            }
        }

        private IEnumerable<RawTransactionModel> TransformRawTransactionModel(Batch<MessageQueue> messages)
        {
            if (messages.Length <= 0) return new List<RawTransactionModel>();

            var transformedDataBag = new ConcurrentBag<RawTransactionModel>();
            var errorList = new List<MessageQueue>();

            var validedMessages = ValidDuplicateTrans(messages);

            Parallel.ForEach(validedMessages, new ParallelOptions { MaxDegreeOfParallelism = 10 }, message =>
            {
                if (message?.Message != null
                    && !string.IsNullOrEmpty(message.Message.Account)
                    && !Utilities.ContainsUnicodeCharacter(message.Message.Account)
                    && message.Message.Account.Length <= 100)
                {
                    try
                    {
                        if (message.Message.Mobile != null && message.Message.Mobile.IsMobileAppTrans)
                        {
                            var rawTransFinalModel = mobileRawTransactionService.TransformRawTransaction(message.Message);

                            if (rawTransFinalModel != null)
                            {
                                transformedDataBag.Add(rawTransFinalModel);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //_ = QueueConsumer.Utilities.LogErrorAsync(ex);
                    }
                }
                else
                {
                    errorList.Add(message.Message);
                }
            });

            if (errorList.Any())
            {
                // LogService.Error($"Account ErrorList:\n {JsonConvert.SerializeObject(errorList)}");
            }

            return transformedDataBag.ToList();
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
                //LogService.Error($"DuplicateMessages:\n {JsonConvert.SerializeObject(duplicateMessagesContent)}");
            }

            var validedMessages = listMessage.Except(dupMessages);
            return validedMessages;
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
                    mobileRawTransactionService.InsertOrUpdateEverTagging(hashKeyEverTagging, everTagging, appSettings.EverTaggingExpireSecond);
                }
            }
            catch (Exception ex)
            {
                //_ = QueueConsumer.Utilities.LogErrorAsync(ex);
            }
        }
    }
}
