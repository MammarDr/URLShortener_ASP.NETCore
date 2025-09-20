using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Diagnostics.CodeAnalysis;
using UrlShortener.Core.Domain.Errors;
using UrlShortener.Models.DTOs.Paging;
using UrlShortener.Models.Enums;

namespace UrlShortener.Core.Domain.Results
{

    public class Result
    {
        
        protected Result(bool isSuccess, Error err)
        {
            IsSuccess = isSuccess;
            Error = err;
        }
        public bool IsSuccess { get; private set;  }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        List<string> ValidationErrors { get; }

        public static Result Success() => new(true, Error.None);
        public static Result Failure(Error err) => new(false, err);

        public static Result<T> Success<T>(T value) => Result<T>.Success(value);
        public static Result<T> Failure<T>(Error err) => Result<T>.Failure(err);


        public void AddValidation(string error)
        {
            //Errors.Add(error);
        }
    }

    

}
