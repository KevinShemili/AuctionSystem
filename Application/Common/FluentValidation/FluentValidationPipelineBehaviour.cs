using FluentValidation;
using MediatR;

namespace Application.Common.FluentValidation {
	public class FluentValidationPipelineBehaviour<TRequest, TResponse> :
		IPipelineBehavior<TRequest, TResponse>
		where TRequest : IRequest<TResponse> {

		private readonly IEnumerable<IValidator<TRequest>> _validators;

		public FluentValidationPipelineBehaviour(IEnumerable<IValidator<TRequest>> validators) {
			_validators = validators;
		}

		public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {

			if (!_validators.Any())
				return await next(cancellationToken);

			var context = new ValidationContext<TRequest>(request);

			var validationResults = _validators
				.Select(x => x.Validate(context))
				.SelectMany(x => x.Errors)
				.Where(x => x != null)
				.ToList();

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
