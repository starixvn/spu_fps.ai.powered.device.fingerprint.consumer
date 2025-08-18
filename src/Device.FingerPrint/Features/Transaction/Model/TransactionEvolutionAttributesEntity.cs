namespace Device.FingerPrint.Features.Transaction.Model
{
    public class TransactionEvolutionAttributesEntity
    {
        public ulong Id { get; set; }

        public string UserAgent { get; set; }

        public string BrowserFamilyCode { get; set; }

        public string BrowserVersion { get; set; }

        public string LocalStorage { get; set; }

        public string DoNotTrack { get; set; }

        public string Cookies { get; set; }

        public string Canvas { get; set; }

        public string Fonts { get; set; }

        public string Vendor { get; set; }

        public string GLRenderer { get; set; }

        public string Plugins { get; set; }

        public string Languages { get; set; }

        public string Accept { get; set; }

        public string Encoding { get; set; }

        public string Headers { get; set; }

        public string Resolution { get; set; }

        public string Timezone { get; set; }
    }
}
