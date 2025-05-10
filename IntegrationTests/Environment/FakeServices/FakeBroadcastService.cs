using Application.Common.Broadcast;
using System.Threading.Channels;

namespace IntegrationTests.Environment.FakeServices {
	public class FakeBroadcastService : IBroadcastService {
		public ChannelReader<BroadcastMessage> Reader => throw new NotImplementedException();

		public ValueTask PublishAsync(string topic, object payload) {
			return new ValueTask(Task.CompletedTask);
		}
	}
}
