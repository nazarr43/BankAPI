using Contracts;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using StackExchange.Redis;
using UserActivity.Application.Configuration;
using UserActivity.Application.Interfaces;

namespace UserActivity.Infrastructure.Repository
{
    public class LoginEventRepository : ILoginEventRepository
    {
        private readonly IMongoCollection<LoginEvent> _loginEvents;

        public LoginEventRepository(IMongoDatabase database, IOptions<MongoDBSettings> mongoDBSettings)
        {
            _loginEvents = database.GetCollection<LoginEvent>(mongoDBSettings.Value.CollectionName);
        }

        public async Task<IEnumerable<LoginEvent>> GetLoginCountAsync(DateTime startDate, DateTime endDate, string userId)
        {
            var filter = Builders<LoginEvent>.Filter.And(
                Builders<LoginEvent>.Filter.Gte(le => le.Timestamp, startDate),
                Builders<LoginEvent>.Filter.Lte(le => le.Timestamp, endDate)
            );

            if (!string.IsNullOrEmpty(userId))
            {
                filter = Builders<LoginEvent>.Filter.And(
                    filter,
                    Builders<LoginEvent>.Filter.Eq(le => le.UserId, userId)
                );
            }
            var loginEvents = await _loginEvents.Find(filter).ToListAsync();
            return loginEvents;
        }
        public async Task LogLoginEvent(LoginEvent loginEvent)
        {
            await _loginEvents.InsertOneAsync(loginEvent);
        }
    }
}
