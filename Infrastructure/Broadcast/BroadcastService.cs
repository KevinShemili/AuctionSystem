using Application.Common.Broadcast;
using System.Threading.Channels;

namespace Infrastructure.Broadcast {

	// Simple broadcast mechanism
	public class BroadcastService : IBroadcastService {

		// Channel is 
		// 1. Thread safe
		// 2. Lock free
		// Its designed for producer consumer scenarios and in process messaging. Unbounded means the channel will grow as needed.
		private readonly Channel<BroadcastMessage> _channel = Channel.CreateUnbounded<BroadcastMessage>();

		// Exposes the ChannelReader to consumers, allowing other services to subscribe and read broadcast messages
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
