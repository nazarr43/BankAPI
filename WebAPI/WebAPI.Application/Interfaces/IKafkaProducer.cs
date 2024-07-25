namespace WebAPI.Application.Interfaces;
public interface IKafkaProducer<T>
{
    Task ProduceAsync(string topic, string key, T value);
}

