namespace Device.FingerPrint.QueueConsumer.Services
{
    using Device.FingerPrint.Core.Encoders;
    using Device.FingerPrint.Model;
    using Device.FingerPrint.TransactionAPI.Model;
    using Fanex.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public interface IBotDetectionService
    {
        string GetFlaggedCode(BotDModel botD, string deviceNo, string userAgent);

        BotDModel TransformBotdModel(MessageQueue message);

        string GetBotDetectionTrue(BotDModel botDModel);
    }

    public class BotDetectionService : IBotDetectionService
    {
        public string GetFlaggedCode(BotDModel botD, string deviceNo, string userAgent)
        {
            var flaggedCode = botD?.Result?.Bot == true ? botD.Result.BotKind : string.Empty;

            if (string.IsNullOrEmpty(flaggedCode))
            {
                var browserless = Browserless(userAgent, string.IsNullOrWhiteSpace(deviceNo));
                flaggedCode = browserless.ToString();
            }

            return flaggedCode;
        }

        public BotDModel TransformBotdModel(MessageQueue message)
        {
            if (!string.IsNullOrEmpty(message.BotD))
            {
                try
                {
                    var botD = Base64.Decode(message.BotD);

                    if (!string.IsNullOrEmpty(botD) && Utilities.IsValidJson(botD))
                    {
                        return JsonConvert.DeserializeObject<BotDModel>(botD);
                    }
                }
                catch (Exception ex)
                {
                    LogService.Info("Error TransformBotdModel: " + ex.Message + "\n" + JsonConvert.SerializeObject(message));
                }
            }

            return new BotDModel();
        }

        public string GetBotDetectionTrue(BotDModel botDModel)
        {
            try
            {
                var detection = botDModel?.Detection;
                if (!string.IsNullOrEmpty(detection))
                {
                    JObject jObject = JObject.Parse(detection);
                    IEnumerable<string> filteredKeys = jObject.Properties()
                        .Where(p => p.Value.Value<bool>("bot"))
                        .Select(p => p.Name);

                    return string.Join(",", filteredKeys);
                }
            }
            catch (Exception ex)
            {
                LogService.Error("Consumer GetBotDetectionTrue: " + ex.Message + "\n" + JsonConvert.SerializeObject(botDModel));
            }

            return string.Empty;
        }

        private static Flagged Browserless(string userAgent, bool emptyFP)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(userAgent) && IsHeadless(userAgent))
                {
                    return Flagged.Headless;
                }

                if (emptyFP)
                {
                    return Flagged.EmptyDevice;
                }

                if (string.IsNullOrWhiteSpace(userAgent))
                {
                    return Flagged.EmptyUserAgent;
                }
            }
            catch (Exception ex)
            {
                LogService.Error("Consumer CheckBrowserless: " + ex.Message + "\n" + JsonConvert.SerializeObject(userAgent));
            }

            return Flagged.Human;
        }

        private static bool IsHeadless(string userAgent)
        {
            return userAgent.ToLower(CultureInfo.InvariantCulture).Contains("headless");
        }
    }
}
