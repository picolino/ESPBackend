using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using ESPBackend.DataAccessLayer.Interfaces;

namespace ESPBackend.DataAccessLayer
{
    public class DataContextFactory : IDataContextFactory
    {
        private readonly string connectionString;

        public DataContextFactory(string server, string database, string sqlLogin, string sqlPassword)
        {
            connectionString = BuildConnectionString(server, database, sqlLogin, sqlPassword);
        }

        private string BuildConnectionString(string serverInstanceName, string databaseName, string sqlLogin, string sqlPassword)
        {
            const string providerName = "System.Data.SqlClient";
            const string metadataTemplate = "res://{0}/DbModel.csdl|res://{0}/DbModel.ssdl|res://{0}/DbModel.msl";

            var sqlBuilder = new SqlConnectionStringBuilder
                             {
                                 DataSource = serverInstanceName,
                                 InitialCatalog = databaseName,
                                 IntegratedSecurity = false,
                                 UserID = sqlLogin,
                                 Password = sqlPassword,
                                 MultipleActiveResultSets = true,
                                 ConnectTimeout = 60
                             };

            var connectionStringBuilder = new EntityConnectionStringBuilder
                                          {
                                              Provider = providerName,
                                              ProviderConnectionString = sqlBuilder.ToString(),
                                              Metadata = string.Format(metadataTemplate, GetAssemblyName())
                                          };

            return connectionStringBuilder.ToString();
        }

        private string GetAssemblyName()
        {
            return typeof(DataContextFactory).Namespace;
        }

        public IDataContext Create()
        {
            return new Entities(connectionString);
        }
    }
}