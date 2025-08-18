using System;

namespace Device.FingerPrint.Features.Transaction.Model
{
    public class TransactionEvolutionEntity : TransactionEvolutionAttributesEntity
    {
        public ulong RawFingerPrintId { get; set; }

        public ulong CookieId { get; set; }

        public DateTime TransTime { get; set; }

        public string FPIds { get; set; }
    }
}
