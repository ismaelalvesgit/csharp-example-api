using Example.Domain.Models;

namespace Example.Domain.Interfaces.Services
{
    public interface IProducerService
    {
        Task ProduceAsync<TEntity>(ProducerData<TEntity> producerConfig);
    }
}
