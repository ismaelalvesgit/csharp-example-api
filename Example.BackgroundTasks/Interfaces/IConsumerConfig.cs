namespace Example.BackgroundTasks.Interfaces
{
    public interface IConsumerConfig<T>
    {
        string? Topic { get; set; }
        string? GroupId { get; set; }
    }
}
