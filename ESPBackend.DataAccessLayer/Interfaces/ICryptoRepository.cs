namespace ESPBackend.DataAccessLayer.Interfaces
{
    public interface ICryptoRepository
    {
        string GetAesKeyByUserId(string userId);
    }
}