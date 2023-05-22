namespace Example.BackgroundTasks.Interfaces
{
    public interface IConsumerConfig<T>
    {
        string topic { get; set; }
        string? groupId { get; set; }
    }
}
