using Microsoft.Extensions.Options;
using WebAPI.Application.Configuration;
using WebAPI.Application.Interfaces;

namespace WebAPI.Application.Services;
public class KafkaService<T> : IKafkaService<T>
{
    private readonly KafkaSettings _config;
    private readonly IKafkaProducer<T> _producer;

    public KafkaService(IOptions<KafkaSettings> config, IKafkaProducer<T> producer)
    {
        _config = config.Value;
        _producer = producer;
    }

    public async Task SendEventAsync(string topic, string key, T value)
    {
        await _producer.ProduceAsync(topic, key, value);
    }

}

