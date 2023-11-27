using MDL.ServiceBus.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.LoadTester.NEW.Morphis
{
    public abstract class NovusMessageWriter
    {
        private static readonly IDictionary<Type, Type> mapOfKnownWriters = new Dictionary<Type, Type>
        {
            { typeof(Command), typeof(NovusCommandMessageWriter) },
            { typeof(Chaining), typeof(NovusChainingMessageWriter) },
            { typeof(FileWrite), typeof(NovusFileWriteMessageWriter) },
            { typeof(FileAppend), typeof(NovusFileAppendMessageWriter) }
        };

        public static NovusMessageWriter Create<T>(T message)
        {
            mapOfKnownWriters.TryGetValue(typeof(T), out Type writerType);

            if (writerType != null)
                return (NovusMessageWriter)Activator.CreateInstance(writerType, message);

            return new NullNovusMessageWriter();
        }

        protected string FromBase64(string base64string)
        {
            return
                Encoding.UTF8.GetString(
                    Convert.FromBase64String(base64string));
        }

        public abstract String WriteToString();

        private class NullNovusMessageWriter : NovusMessageWriter
        {
            public override string WriteToString()
            {
                return string.Empty;
            }
        }
    }

    public class NovusCommandMessageWriter : NovusMessageWriter
    {
        private readonly Command message;

        public NovusCommandMessageWriter(Command message)
        {
            this.message = message;
        }

        public override string WriteToString()
        {
            return "(exec): " + FromBase64(message.CommandString);
        }
    }

    public class NovusChainingMessageWriter : NovusMessageWriter
    {
        private readonly Chaining message;

        public NovusChainingMessageWriter(Chaining message)
        {
            this.message = message;
        }

        public override string WriteToString()
        {
            string result = string.Format(
                "Year: {0}, " +
                "Transaction Number: {1}, " +
                "Quote Number: {2}, " +
                "Policy Number: {3}, " +
                "Claim Reference: {4}, " +
                "Document Type: {5}, " +
                "Partner Id: {6}",
                    message.Year,
                    message.TransactionNumber,
                    message.QuoteNumber,
                    message.PolicyNumber,
                    message.ClaimReference,
                    message.DocumentType,
                    message.PartnerId);

            return "(chaining): " + result;
        }
    }

    public class NovusFileAppendMessageWriter : NovusMessageWriter
    {
        private readonly FileAppend message;

        public NovusFileAppendMessageWriter(FileAppend message)
        {
            this.message = message;
        }

        public override string WriteToString()
        {
            return "(appending to file): " + message.FileName;
        }
    }

    public class NovusFileWriteMessageWriter : NovusMessageWriter
    {
        private readonly FileWrite message;

        public NovusFileWriteMessageWriter(FileWrite message)
        {
            this.message = message;
        }

        public override string WriteToString()
        {
            return "(writing to file): " + message.FileName;
        }
    }
}
