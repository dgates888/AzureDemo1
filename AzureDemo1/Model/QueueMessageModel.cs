namespace AzureDemo1.Model
{
    public class QueueMessageModel
    {
        public Guid? CorrelationId { get; set; }
        public string TaskType { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }
}
