using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using EventStore.ClientAPI;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace EsdbAsyncHandler {
    public class ProducerService : BackgroundService {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            var connection = ConnectionFactory.Create();
            var fixture    = new Fixture();

            await connection.ConnectAsync();

            while (!stoppingToken.IsCancellationRequested) {
                var data = Enumerable.Range(0, 100).Select(_ => CreateEvent());
                
                await connection.AppendToStreamAsync("test", ExpectedVersion.Any, data);

                await Task.Delay(100, stoppingToken);
            }

            EventData CreateEvent() {
                var e = fixture.Create<TestEvent>();

                return new EventData(
                    Guid.NewGuid(),
                    "TestEvent",
                    true,
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e)),
                    null
                );
            }
        }
    }
}
