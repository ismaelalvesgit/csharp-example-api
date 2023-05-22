using Example.BackgroundTasks.Interfaces;

namespace Example.BackgroundTasks.Model
{
    public class ConsumerConfig<T> : IConsumerConfig<T>
    {
        public string topic { get; set; }
        public string? groupId { get; set; }
    }
}
