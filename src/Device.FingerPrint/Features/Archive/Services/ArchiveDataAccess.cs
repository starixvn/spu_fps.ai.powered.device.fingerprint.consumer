using Device.FingerPrint.Features.Archive.Criterias;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Archive.Services
{
    public interface IArchiveDataAccess
    {
        void UpdateModifiedDate(int batchSize);
    }

    public class ArchiveDataAccess : IArchiveDataAccess
    {
        private readonly IDynamicRepository repository;

        public ArchiveDataAccess()
        {
            repository = new DynamicRepository();
        }

        public void UpdateModifiedDate(int batchSize)
        {
            var criteria = new UpdateModifiedDateCriteria
            {
                BatchSize = batchSize
            };

            repository.Execute(criteria);
        }
    }
}
