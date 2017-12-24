namespace ESPBackend.Dto
{
    public class TestDataDto
    {
        public string TestString { get; set; }

        public override string ToString()
        {
            return $"{nameof(TestString)}: {TestString}";
        }
    }
}