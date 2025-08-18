namespace Device.FingerPrint.Features.Offset.Services
{
    public interface IOffsetService
    {
        void UpdateOffset(long Id, long OffsetValue);
    }

    public class OffsetService : IOffsetService
    {
        private readonly IOffsetDataAccess offsetDataAccess;

        public OffsetService(IOffsetDataAccess offsetDataAccess)
        {
            this.offsetDataAccess = offsetDataAccess;
        }

        public void UpdateOffset(long Id, long OffsetValue)
        {
            offsetDataAccess.UpdateOffset(Id, OffsetValue);
        }
    }
}
