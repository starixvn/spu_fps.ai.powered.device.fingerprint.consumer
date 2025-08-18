using Device.FingerPrint.Features.Offset.Criterias;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Offset.Services
{

    public interface IOffsetDataAccess
    {
        void UpdateOffset(long Id, long OffsetValue);
    }

    public class OffsetDataAccess : IOffsetDataAccess
    {
        private readonly IDynamicRepository repository;

        public OffsetDataAccess()
        {
            repository = new DynamicRepository();
        }

        public void UpdateOffset(long Id, long OffsetValue)
        {
            var criteria = new UpdateOffsetCriteria
            {
                Id = Id,
                OffsetValue = OffsetValue
            };

            repository.Execute(criteria);
        }
    }
}
