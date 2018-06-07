using ESPBackend.DataAccessLayer;
using ESPBackend.DataAccessLayer.Interfaces;

namespace ESPBackend.Application
{
    public static class RepositoryFactory
    {
        private static IDataContextFactory DataContextFactory { get; set; }

        public static ITestDataRepository TestDataRepository { get; private set; }
        public static ICryptoRepository CryptoRepository { get; private set; }


        public static void CreateDataContextFactory(string server, string database, string sqlLogin, string sqlPassword)
        {
            DataContextFactory = new DataContextFactory(server, database, sqlLogin, sqlPassword);
            CreateRepositories();
        }

        private static void CreateRepositories()
        {
            TestDataRepository = new TestDataRepository(DataContextFactory);
            CryptoRepository = new CryptoRepository(DataContextFactory);
        }
    }
}