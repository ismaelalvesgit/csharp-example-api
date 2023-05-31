using Confluent.Kafka;
using Example.Domain.Interfaces.Services;
using Example.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Example.Application.Services
{
    public class ProducerService : IProducerService
    {
        private readonly ILogger<ProducerService> _logger;
        private readonly ProducerConfig _producerConfig;

        public ProducerService(IConfiguration configuration, ILogger<ProducerService> logger)
        {
            _logger = logger;
            _producerConfig = new ProducerConfig()
            {
                BootstrapServers = configuration.GetValue<string>("Messaging:Kafka:Producers:Servers"),
                AllowAutoCreateTopics = true,
                MessageTimeoutMs = configuration.GetValue<int>("Messaging:Kafka:Producers:Timeout")
            };
        }

        public async Task<Guid> ProduceAsync<TEntity>(ProducerData<TEntity> producerConfig)
        {
            var identifier = Guid.NewGuid();
            _logger.LogInformation("Send topic: {topic} identifier: {identifier}", producerConfig.Topic, identifier);
            var producer = new ProducerBuilder<Null, string>(_producerConfig).Build();
            var data = Serializer(producerConfig.Data, identifier);
            await producer.ProduceAsync(producerConfig.Topic, new Message<Null, string> { Value = data });
            producer.Dispose();

            return identifier;
        }

        private static string Serializer<TEntity>(TEntity entity, Guid? identifier)
        {
            Dictionary<string, object?>? valuePairs = entity?.GetType()
                .GetProperties()
                .Where(x => x.GetValue(entity) != null)
                .ToDictionary(d => ToCamelCase(d.Name), p => p.GetValue(entity));
            valuePairs?.Add("identifier", identifier ?? Guid.NewGuid());
            return JsonConvert.SerializeObject(valuePairs);
        }

        private static string ToCamelCase(string str)
        {
            var words = str.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries);
            var leadWord = Regex.Replace(words[0], @"([A-Z])([A-Z]+|[a-z0-9]+)($|[A-Z]\w*)",
                m =>
                {
                    return m.Groups[1].Value.ToLower() + m.Groups[2].Value.ToLower() + m.Groups[3].Value;
                });
            var tailWords = words.Skip(1)
                .Select(word => char.ToUpper(word[0]) + word[1..])
                .ToArray();
            return $"{leadWord}{string.Join(string.Empty, tailWords)}";
        }
    }
}
