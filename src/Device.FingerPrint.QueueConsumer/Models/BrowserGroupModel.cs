using System.Collections.Generic;

namespace Device.FingerPrint.QueueConsumer.Models
{
    public class BrowserGroupModel
    {
        public string? BrowserName { get; set; }
        public string? OsName { get; set; }
        public string? UserAgent { get; set; }
        public string? Vendor { get; set; }
        public List<string> VendorFlavors { get; set; } = new List<string>();
        public List<PluginModel> Plugins { get; set; } = new List<PluginModel>();
        public bool PdfViewerEnabled { get; set; }
    }

    public class PluginModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Filename { get; set; }
    }

    public class BrowserNameModel
    {
        public string? BrowserName { get; set; }
    }

    public class OSNameModel
    {
        public string? OsName { get; set; }
    }

    public class UserAgentModel
    {
        public string? UserAgent { get; set; }
    }

    public class VendorModel
    {
        public string? Vendor { get; set; }
    }

    public class VendorFlavorsModel
    {
        public List<string> VendorFlavors { get; set; } = new List<string>();
    }

    public class PluginsModel
    {
        public List<PluginModel> Plugins { get; set; } = new List<PluginModel>();
    }

    public class PdfViewerEnabledModel
    {
        public bool PdfViewerEnabled { get; set; }
    }
}
