using System.Collections.Generic;

namespace Device.FingerPrint.QueueConsumer.Models
{
    public class PreferencesGroupModel
    {
        public List<string> Languages { get; set; } = new List<string>();
        public string? Timezone { get; set; }
        public bool TouchSupport { get; set; }
        public bool CookiesEnabled { get; set; }
        public bool LocalStorage { get; set; }
        public bool SessionStorage { get; set; }
        public bool OpenDatabase { get; set; }
        public bool IndexedDB { get; set; }
    }

    public class LanguagesModel
    {
        public List<string> Languages { get; set; } = new List<string>();
    }

    public class TimezoneModel
    {
        public string? Timezone { get; set; }
    }

    public class TouchSupportModel
    {
        public bool TouchSupport { get; set; }
    }

    public class CookiesEnabledModel
    {
        public bool CookiesEnabled { get; set; }
    }

    public class LocalStorageModel
    {
        public bool LocalStorage { get; set; }
    }

    public class SessionStorageModel
    {
        public bool SessionStorage { get; set; }
    }

    public class OpenDatabaseModel
    {
        public bool OpenDatabase { get; set; }
    }

    public class IndexedDBModel
    {
        public bool IndexedDB { get; set; }
    }
}
