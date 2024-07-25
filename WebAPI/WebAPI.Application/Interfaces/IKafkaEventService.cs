using Contracts;

namespace WebAPI.Application.Interfaces;
public interface IKafkaEventService
{
    Task PublishLoginEventAsync(LoginEvent loginEvent);
}

