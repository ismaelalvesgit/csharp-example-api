namespace Example.Domain.Models
{
    public class ProducerData <TEntity>
    {
        public string Topic { get; set; }
        public TEntity Data { get; set; }
    }
}
