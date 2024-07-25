using Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using UserActivity.Application.Configuration;
using UserActivity.Application.Interfaces;

namespace UserActivity.Infrastructure
{
    public class LoginEventConsumer : KafkaBaseConsumer<LoginEvent>
    {
        public LoginEventConsumer(IServiceScopeFactory scopeFactory, ILogger<LoginEventConsumer> logger)
            : base(scopeFactory, KafkaTopics.UserLoginTopic, logger)
        {
        }

        protected override async Task ProcessMessageAsync(LoginEvent message, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            var loginEventRepository = serviceProvider.GetRequiredService<ILoginEventRepository>();
            await loginEventRepository.LogLoginEvent(message);
        }
    }
}
