using Confluent.Kafka;
using FluentValidation;
using Newtonsoft.Json;

namespace Example.BackgroundTasks.Services
{
    public abstract class ConsumerBaseService : IHostedService, IDisposable
    {
        private readonly ILogger<ConsumerBaseService> _logger;
        private readonly ConsumerConfig _consumerConfig;
        private readonly string _topic;
        private readonly string _groupId;
        private IConsumer<Ignore, string>? _consumer;
        private readonly int _maxNumAttempts;
        private readonly bool _enableRetryOnFailure;
        private readonly int _retryIntervalInSec;

        protected ConsumerBaseService(string topic, string groupId, IConfiguration configuration, ILogger<ConsumerBaseService> logger)
        {
            _topic = topic;
            _groupId = groupId;
            _logger = logger;
            _enableRetryOnFailure = configuration.GetValue<bool>("Messaging:Kafka:Consumers:EnableRetryOnFailure");
            _maxNumAttempts = configuration.GetValue<int>("Messaging:Retry:Count");
            _retryIntervalInSec = configuration.GetValue<int>("Messaging:Retry:Delay");
            _consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = configuration.GetValue<string>("Messaging:Kafka:Consumers:Servers"),
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };
        }

        protected ConsumerBaseService(string topic, IConfiguration configuration, IHostEnvironment environment, ILogger<ConsumerBaseService> logger)
        {
            _topic = topic;
            _groupId = environment.ApplicationName;
            _logger = logger;
            _enableRetryOnFailure = configuration.GetValue<bool>("Messaging:Kafka:Consumers:EnableRetryOnFailure");
            _maxNumAttempts = configuration.GetValue<int>("Messaging:Retry:Count");
            _retryIntervalInSec = configuration.GetValue<int>("Messaging:Retry:Delay");
            _consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = configuration.GetValue<string>("Messaging:Kafka:Consumers:Servers"),
                GroupId = environment.ApplicationName,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
        }

        public virtual async Task ExecuteAsync(string data)
        {
            await Task.CompletedTask;  // do the work
        }

        protected async Task ProcessQueue(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Start consumer topic: {topic} groupId: {groupId}", _topic, _groupId);
            _consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
            _consumer.Subscribe(_topic);
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var message = _consumer.Consume(stoppingToken);

                        // Don't want to block consume loop, so starting new Task for each message  
                        await Task.Run(async () =>
                        {
                            var currentNumAttempts = 0;
                            var committed = false;

                            while (currentNumAttempts < _maxNumAttempts)
                            {
                                currentNumAttempts++;

                                try
                                {
                                    _logger.LogInformation("Topic: {topic} Menssage: {message} Offset: {offset}", _topic, message.Message.Value, message.TopicPartitionOffset);
                                    await ExecuteAsync(message.Message.Value);
                                    committed = Commit(message);
                                    if (committed) break;
                                }
                                catch (Exception ex)
                                {
                                    LoggerError(ex);
                                }

                                if (currentNumAttempts < _maxNumAttempts)
                                {
                                    if (!_enableRetryOnFailure)
                                    {
                                        committed = Commit(message);
                                        break;
                                    }
                                    // Delay between tries
                                    LoggerError($"Retry: {currentNumAttempts}");
                                    await Task.Delay(TimeSpan.FromSeconds(_retryIntervalInSec));
                                }
                            }

                            if (!committed)
                            {
                                Commit(message);
                                // Publish in deadleare queue
                            }
                        }, stoppingToken);
                    }
                    catch (ConsumeException ex)
                    {
                        LoggerError(ex);
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                LoggerError(ex);
                _consumer.Close();
            }
        }

        public virtual void Dispose()
        {
            _consumer?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await ProcessQueue(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer?.Close();
            await Task.CompletedTask;
        }

        private bool Commit(ConsumeResult<Ignore, string> message)
        {
            try
            {
                _consumer?.Commit(message);
                return true;
            }
            catch (KafkaException ex)
            {
                LoggerError(ex);
                return false;
            }
        }

        private void LoggerError(Exception ex)
        {
            _logger.LogError("Failed consumer topic: {topic} {message}", _topic, ex.Message);
        }

        private void LoggerError(string ex)
        {
            _logger.LogError("Failed consumer topic: {topic} {ex}", _topic, ex);
        }

        public T? Deserialize<T, Y>(string data)
        {
            var deserialzer = JsonConvert.DeserializeObject<T>(data);

            if (Activator.CreateInstance(typeof(Y)) is IValidator<T> validation && deserialzer != null)
            {
                validation.ValidateAndThrow(deserialzer);
            }

            return deserialzer;
        }
    }
}
