using Application.Common.Broadcast;
using System.Threading.Channels;

namespace Infrastructure.Broadcast {
	public class BroadcastService : IBroadcastService {

		private readonly Channel<BroadcastMessage> _channel = Channel.CreateUnbounded<BroadcastMessage>();

		public ChannelReader<BroadcastMessage> Reader => _channel.Reader;

		public ValueTask PublishAsync(string topic, object payload) {
			var result = _channel.Writer.WriteAsync(new BroadcastMessage {
				Topic = topic,
				Payload = payload
			});

			return result;
		}
	}
}
