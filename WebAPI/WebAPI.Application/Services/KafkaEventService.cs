using Contracts;
using WebAPI.Application.Interfaces;

namespace WebAPI.Application.Services;
public class KafkaEventService : IKafkaEventService
{
    private readonly IKafkaService<LoginEvent> _kafkaService;
    private const string _loginEventsTopic = "user-login-topic";

    public KafkaEventService(IKafkaService<LoginEvent> kafkaService)
    {
        _kafkaService = kafkaService;
    }
    public async Task PublishLoginEventAsync(LoginEvent loginEvent)
    {
        await _kafkaService.SendEventAsync(_loginEventsTopic, loginEvent.UserId, loginEvent);
    }
}

