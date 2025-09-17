using System;

namespace Device.FingerPrint.Features.LogInfo.Model
{
    public class LogInfoModel
    {
        public int LogType { get; set; }

        public string? Environment { get; set; }

        public string? ScriptVersion { get; set; }

        public string? SessionKey { get; set; }

        public string? Tagging { get; set; }

        public string? LoginName { get; set; }

        public string? SubscriberName { get; set; }

        public DateTime TransTime { get; set; }

        public string? UserAgent { get; set; }

        public string? URL { get; set; }
    }
}
