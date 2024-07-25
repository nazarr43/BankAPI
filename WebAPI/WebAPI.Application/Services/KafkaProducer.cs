using Confluent.Kafka;
using Newtonsoft.Json;
using WebAPI.Application.Configuration;
using WebAPI.Application.Interfaces;

namespace WebAPI.Application.Services;
public class KafkaProducer<T> : IKafkaProducer<T>
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(IProducer<string, string> producer)
    {
        _producer = producer;
    }

    public async Task ProduceAsync(string topic, string key, T value)
    {
        var serializedValue = JsonConvert.SerializeObject(value);
        var message = new Message<string, string>
        {
            Key = key,
            Value = serializedValue
        };

        await _producer.ProduceAsync(topic, message);
    }
}

