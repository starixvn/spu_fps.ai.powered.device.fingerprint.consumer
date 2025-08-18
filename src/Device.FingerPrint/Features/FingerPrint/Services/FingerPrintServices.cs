using Device.FingerPrint.Features.FingerPrint.Entities;
using System;
using System.Collections.Generic;

namespace Device.FingerPrint.Features.FingerPrint.Services
{
    public interface IFingerPrintServices
    {
        IEnumerable<FPFingerPrintEntity> GetFPFingerPrint(DateTime fromDate, DateTime toDate, int batchSize, ulong lastID);
    }

    public class FingerPrintServices : IFingerPrintServices
    {
        private readonly IFingerPrintDataAccess _dataAcess;

        public FingerPrintServices(IFingerPrintDataAccess dataAcess)
        {
            _dataAcess = dataAcess;
        }

        public IEnumerable<FPFingerPrintEntity> GetFPFingerPrint(DateTime fromDate, DateTime toDate, int batchSize, ulong lastID)
        {
            return _dataAcess.GetFPFingerPrint(fromDate, toDate, batchSize, lastID);
        }
    }
}
