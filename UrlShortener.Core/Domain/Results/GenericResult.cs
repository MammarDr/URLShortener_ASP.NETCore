using System.Diagnostics.CodeAnalysis;
using UrlShortener.Core.Domain.Errors;


namespace UrlShortener.Core.Domain.Results
{
    public class Result<T> : Result
    {
        private readonly T? value;
        private Result(T? value, bool isSuccess, Error err) : base(isSuccess, err)
        {
            this.value = value;
        }

        /* public T Value => IsSuccess
                 ? value!
                 : throw new InvalidOperationException
                     ("The value of the failure result can't be accessed");*/

        public T Value => value!;

        public static Result<T> Success(T value) => new(value, true, Error.None);
        public static Result<T> Failure(Error err) => new(default, false, err);

        public static Result<T> Create(T value) => Success(value);

        public static implicit operator Result<T>(T? value)
            => Success(value);

        public static implicit operator Result<T>(Error? err)
            => err is not null ? Failure(err) : Failure(Error.Unexpected);

    }
}
