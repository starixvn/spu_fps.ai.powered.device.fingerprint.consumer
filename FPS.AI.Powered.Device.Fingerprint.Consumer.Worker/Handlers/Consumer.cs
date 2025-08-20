using FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Model;
using FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Services;
using MassTransit;
using MassTransit.JobService;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Handlers
{
    public class Consumer(
            IMobileRawTransactionService mobileRawTransactionService,
            IAppSettings appSettings
        ) : IConsumer<Batch<MessageQueue>>
    {
        private readonly IMobileRawTransactionService mobileRawTransactionService = mobileRawTransactionService;
        private readonly IAppSettings appSettings = appSettings;

        public Task Consume(ConsumeContext<Batch<MessageQueue>> context)
        {
            try
            {
                var rawTransModels = TransformRawTransactionModel(context.Message);

                if (rawTransModels != null && rawTransModels.Any())
                {
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

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _ = QueueConsumer.Utilities.LogErrorAsync(ex);
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
    }
}
