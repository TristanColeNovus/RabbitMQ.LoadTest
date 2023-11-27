namespace RabbitMQ.LoadTester.BLL.Shared.Types
{
    /// <summary>
    /// File Appending Service Bus Message Type
    /// </summary>
    public class FileAppend
    {
        public string FileName { get; set; }
        public string Content { get; set; }
    }
}
