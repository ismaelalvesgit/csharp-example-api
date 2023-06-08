namespace Example.BackgroundTasks.Model
{
    public class ConsumerMessageError
    {
        public string? Message { get; set; }
        public List<Exception>? Exceptions { get; set; }
    }
}
