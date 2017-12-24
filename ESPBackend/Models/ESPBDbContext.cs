using System.Data.Entity;
using ESPBackend.Domain;

namespace ESPBackend.Models
{
    public class ESPBDbContext : DbContext
    {
        public ESPBDbContext() : base("ESPBDbContext")
        {
            
        }

        public virtual DbSet<TestData> TestData { get; set; }
    }
}