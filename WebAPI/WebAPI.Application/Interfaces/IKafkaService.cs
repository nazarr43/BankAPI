namespace WebAPI.Application.Interfaces;
public interface IKafkaService<T>
{
    Task SendEventAsync(string topic, string key, T value);
}

