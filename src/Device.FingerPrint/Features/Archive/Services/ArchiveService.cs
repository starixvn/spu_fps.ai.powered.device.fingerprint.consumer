namespace Device.FingerPrint.Features.Archive.Services
{
    public interface IArchiveService
    {
        void UpdateModifiedDate(int batchSize);
    }

    public class ArchiveService : IArchiveService
    {
        private readonly IArchiveDataAccess archiveDataAccess;

        public ArchiveService(IArchiveDataAccess archiveDataAccess)
        {
            this.archiveDataAccess = archiveDataAccess;
        }

        public void UpdateModifiedDate(int batchSize)
        {
            archiveDataAccess.UpdateModifiedDate(batchSize);
        }
    }
}
