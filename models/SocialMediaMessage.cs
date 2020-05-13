using System;

namespace Dapr.Examples.Pubsub.Models
{
    public class SocialMediaMessage
    {
        public SocialMediaMessage() { }
        public Guid CorrelationId { get; set; }
        public Guid MessageId { get; set; }
        public string Message { get; set; }
        public DateTime CreationDate { get; set; }
        public string Sentiment { get; set; }
        public DateTime PreviousAppTimestamp { get; set; }
    }
}
