namespace RabbitMQ.LoadTester.BLL.Shared.Types
{
    /// <summary>
    /// File Renaming Service Bus Message Type
    /// </summary>
    public class FileRename
    {
        public string ExistingFileName { get; set; }
        public string RenameFileName { get; set; }
    }
}
