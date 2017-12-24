using System.ComponentModel.DataAnnotations.Schema;

namespace ESPBackend.Domain
{
    [Table("TestData")]
    public class TestData
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string TestString { get; set; }
        
    }
}