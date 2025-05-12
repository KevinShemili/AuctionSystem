using FluentValidation;
using MediatR;

namespace Application.Common.FluentValidation {

	// We already have the Mediator request pipeline setup. 
	// We further setup the FluentValidation pipeline behaviour to validate requests.
	// This FluentValidation pipeline lives inside the Mediator pipeline.
	public class FluentValidationPipelineBehaviour<TRequest, TResponse> :
		IPipelineBehavior<TRequest, TResponse>
		where TRequest : IRequest<TResponse> {

		private readonly IEnumerable<IValidator<TRequest>> _validators;

		public FluentValidationPipelineBehaviour(IEnumerable<IValidator<TRequest>> validators) {
			_validators = validators;
		}

		// The method that gets called whenever a request is processed through MediatR.
		// It intercepts the request before it reaches its handler and performs 
		// validation using the provided validators.
		public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {

			if (!_validators.Any())
				return await next(cancellationToken);

			// Create a validation context that holds the request data
			var context = new ValidationContext<TRequest>(request);

			// Execute each validator and collect all validation errors
			var validationResults = _validators
				.Select(x => x.Validate(context))
				.SelectMany(x => x.Errors)
				.Where(x => x != null)
				.ToList();

			// If any validation errors were found group them by property name
			if (validationResults.Count != 0) {
				var errorsDictionary = validationResults
					.GroupBy(
						x => x.PropertyName,
						(propertyName, errorMessages) => new {
							Key = propertyName,
							Values = errorMessages.Distinct().ToArray()
						})
					.ToDictionary(x => x.Key, x => x.Values);

				var errorMessages = string.Join(" ", errorsDictionary.Select(kv =>
					string.Join(" ", kv.Value.Select(e => e.ErrorMessage))));

				throw new ValidationException(errorMessages);
			}

			return await next(cancellationToken);
		}
	}
}
