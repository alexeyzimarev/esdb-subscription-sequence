using System;
using EventStore.ClientAPI;

namespace EsdbAsyncHandler {
    public static class ConnectionFactory {
        public static IEventStoreConnection Create() => EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"));
    }
}
