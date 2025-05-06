using System.Threading.Channels;

namespace Application.Common.Broadcast {
	public interface IBroadcastService {
		ChannelReader<BroadcastMessage> Reader { get; }
		ValueTask PublishAsync(string topic, object payload);
	}
}
