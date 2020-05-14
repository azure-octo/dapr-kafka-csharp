// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// ------------------------------------------------------------

namespace Dapr.Examples.Pubsub.Models
{
    using System;

    public class SampleMessage
    {
        public SampleMessage() { }
        public Guid CorrelationId { get; set; }
        public Guid MessageId { get; set; }
        public string Message { get; set; }
        public DateTime CreationDate { get; set; }
        public string Sentiment { get; set; }
        public DateTime PreviousAppTimestamp { get; set; }
    }
}
