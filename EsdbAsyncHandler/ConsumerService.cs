using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace EsdbAsyncHandler {
    public class ConsumerService : IHostedService {
        readonly ILogger<ConsumerService> _log;

        IEventStoreConnection               _connection;
        EventStoreStreamCatchUpSubscription _subscription;

        public ConsumerService(ILogger<ConsumerService> log) => _log = log;

        public async Task StartAsync(CancellationToken cancellationToken) {
            var mongoClient = new MongoClient("mongodb://mongoadmin:secret@localhost:27017");
            var collection  = mongoClient.GetDatabase("test").GetCollection<TestEvent>("test");

            _connection = ConnectionFactory.Create();
            await _connection.ConnectAsync();
            var options = new ReplaceOptions {IsUpsert = true};

            _subscription = _connection.SubscribeToStreamFrom(
                "test",
                StreamPosition.Start,
                CatchUpSubscriptionSettings.Default,
                EventAppeared
            );

            async Task EventAppeared(EventStoreCatchUpSubscription sub, ResolvedEvent re) {
                _log.LogDebug("Event appeared: {Version}", re.Event.EventNumber);

                try {
                    var data = JsonConvert.DeserializeObject<TestEvent>(Encoding.UTF8.GetString(re.Event.Data));

                    await collection.ReplaceOneAsync(x => x.Id == data.Id, data, options);
                }
                catch (Exception e) {
                    _log.LogError(e, "Error {Message}", e.Message);
                    throw;
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            _subscription.Stop();
            _connection.Close();
            return Task.CompletedTask;
        }
    }
}
