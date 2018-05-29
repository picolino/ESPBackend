using System;
using ESPBackend.DataAccessLayer.Interfaces;

namespace ESPBackend.DataAccessLayer
{
    public class CryptoRepository : ICryptoRepository
    {
        private readonly IDataContextFactory dataContextFactory;

        public CryptoRepository(IDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        public string GetAesKeyByUserId(string userId)
        {
            using (var dataContext = dataContextFactory.Create())
            {
                return dataContext.GetAesKeyForUser(userId);
            }
        }
    }
}