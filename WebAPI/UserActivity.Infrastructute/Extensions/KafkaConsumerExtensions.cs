using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using UserActivity.Application.Configuration;

namespace UserActivity.Infrastructure.Extensions
{
    public static class KafkaConsumerExtensions
    {
        public static IServiceCollection AddKafkaConsumer<TKey, TValue>(this IServiceCollection services, Action<ConsumerConfig> configureConsumer)
        {
            services.AddSingleton<IConsumer<TKey, TValue>>(sp =>
            {
                var kafkaSettings = sp.GetRequiredService<IOptions<KafkaSettings>>().Value;
                var config = new ConsumerConfig
                {
                    BootstrapServers = kafkaSettings.BootstrapServers,
                    GroupId = kafkaSettings.ConsumerGroupId
                };
                configureConsumer(config);
                return new ConsumerBuilder<TKey, TValue>(config).Build();
            });

            return services;
        }
    }
}
