namespace Application.Common.ResultPattern {

	// Here is were we implement the result pattern. 
	// The idea is to explicitly represent a success or failure state of an operation,
	// without throwing an eexception. Exceptions are expensive and should be avoided,
	// for simple logic related error handling.

	public sealed class Result<T> {

		private Result(bool isSuccess, T value, Error error) {

			if (isSuccess && error != Error.None ||
				!isSuccess && error == Error.None) {
				throw new ArgumentException("Invalid error", nameof(error));
			}

			IsSuccess = isSuccess;
			Value = value;
			Error = error;
		}

		public bool IsSuccess { get; }
		public bool IsFailure => !IsSuccess;
		public T Value { get; }
		public Error Error { get; }

		public static Result<T> Success(T value) => new(true, value, Error.None);
		public static Result<T> Failure(Error error) => new(false, default!, error);
	}
}
