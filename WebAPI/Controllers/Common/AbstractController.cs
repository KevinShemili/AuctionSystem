using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.Common {
	public abstract class AbstractController : ControllerBase {

		protected readonly IMediator _mediator;

		public AbstractController(IMediator mediator) {
			_mediator = mediator;
		}
	}
}
