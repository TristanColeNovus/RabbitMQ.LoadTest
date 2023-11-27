namespace RabbitMQ.LoadTester.BLL.Shared.Types
{
    /// <summary>
    /// File Writing Service Bus Message Type
    /// </summary>
    public class FileWrite
    {
        public string FileName { get; set; }
        public string Content { get; set; }
    }
}
