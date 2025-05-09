using Application.Common.Broadcast;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace WebAPI.Controllers {

	[ApiController]
	[Route("api/broadcast")]
	public class BroadcastController : ControllerBase {

		private readonly IBroadcastService _broadcastService;

		public BroadcastController(IBroadcastService broadcastService) {
			_broadcastService = broadcastService;
		}

		[HttpGet("stream")]
		public async Task Stream() {

			Response.Headers.Add("Cache-Control", "no-cache");
			Response.ContentType = "text/event-stream";

			try {
				await foreach (var msg in _broadcastService.Reader.ReadAllAsync(HttpContext.RequestAborted)) {

					var json = JsonSerializer.Serialize(msg.Payload);

					await Response.WriteAsync($"event: {msg.Topic}\n");
					await Response.WriteAsync($"data: {json}\n\n");
					await Response.Body.FlushAsync(HttpContext.RequestAborted);
				}
			}
			catch (OperationCanceledException) { }
		}
	}
}
