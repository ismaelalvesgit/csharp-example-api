using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Example.integrationTests.Helpers
{
    static class TaskHelpers
    {
        public static async Task CancelAfter(IHostedService? task, TimeSpan time)
        {
            CancellationTokenSource source = new();
            source.CancelAfter(time);
            if (task != null) { 
                await Task.Run(() => task.StartAsync(source.Token), source.Token);
            }
        }

        public static async Task DropQueueAsync(IConfiguration configuration, IEnumerable<string> topics) 
        {
            var bootstrapServers = configuration.GetValue<string?>("Messaging:Kafka:Producers:Servers");
            var adminClient = new AdminClientBuilder(new AdminClientConfig() { BootstrapServers = bootstrapServers });
            await adminClient.Build().DeleteTopicsAsync(topics);
        }
    }
}
