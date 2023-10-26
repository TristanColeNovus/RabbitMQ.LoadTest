using System;

namespace RabbitMQ.LoadTester.BLL.Shared.Types
{
    /// <summary>
    /// Chaining Service Bus Message Type
    /// </summary>
    public class Chaining
    {
        public string ProgramShortcut { get; set; }
        public string LogOn { get; set; }   // Oracle DB User Name
        public string PolicyNumber { get; set; }
        public string Year { get; set; }
        public bool IsEndorsementPending { get; set; }
        public DateTime? EndorsementEffectiveDate { get; set; }
        public string DocumentType { get; set; }
        public string TransactionNumber { get; set; }
        public string QuoteNumber { get; set; }
        public string PartnerId { get; set; }
        public string ClaimReference { get; set; }
        public string Section { get; set; }
        public string ClaimNumber { get; set; }
        public string BaPolNum { get; set; }
        public string BaYear { get; set; }
        public string Mvn { get; set; }
        public string SectionTitle { get; set; }
    }
}
