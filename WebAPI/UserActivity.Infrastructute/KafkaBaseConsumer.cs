using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace UserActivity.Infrastructure
{
    public abstract class KafkaBaseConsumer<TMessage> : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _topic;
        private readonly ILogger<KafkaBaseConsumer<TMessage>> _logger;

        protected KafkaBaseConsumer(IServiceScopeFactory scopeFactory, string topic, ILogger<KafkaBaseConsumer<TMessage>> logger)
        {
            _scopeFactory = scopeFactory;
            _topic = topic;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() => ConsumeMessages(cancellationToken), cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task ConsumeMessages(CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var consumer = scope.ServiceProvider.GetRequiredService<IConsumer<string, string>>();
                consumer.Subscribe(_topic);

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            var result = consumer.Consume(cancellationToken);
                            if (result != null)
                            {
                                TMessage message = default;

                                try
                                {
                                    message = JsonConvert.DeserializeObject<TMessage>(result.Message.Value);
                                }
                                catch (JsonException jsonEx)
                                {
                                    _logger.LogError(jsonEx, "Failed to deserialize message: {Message}", result.Message.Value);
                                    continue;
                                }

                                if (message != null)
                                {
                                    await ProcessMessageAsync(message, scope.ServiceProvider, cancellationToken);
                                }
                            }
                        }
                        catch (ConsumeException consumeEx)
                        {
                            _logger.LogError(consumeEx, "Error consuming Kafka message");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    consumer.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while consuming Kafka message");
                }
            }
        }

        protected abstract Task ProcessMessageAsync(TMessage message, IServiceProvider serviceProvider, CancellationToken cancellationToken);
    }
}
