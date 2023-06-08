using Example.BackgroundTasks.Interfaces;

namespace Example.BackgroundTasks.Model
{
    public class ConsumerConfig<T> : IConsumerConfig<T>
    {
        public string? Topic { get; set; }
        public string? GroupId { get; set; }
    }
}
