namespace ESPBackend.DataAccessLayer.Interfaces
{
    public interface IDataContextFactory
    {
        IDataContext Create();
    }
}