using Microsoft.EntityFrameworkCore;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Core.Domain.Errors;
using UrlShortener.Core.Validators;
using UrlShortener.Data.Repositories.Implementation;

namespace UrlShortener.Core.Domain.Results
{
    public static class ResultExtensions
    {

        public static Task<Result<T>> ToTaskResult<T>(this Error error)
        => Task.FromResult(Result<T>.Failure(error));

        // = //

        public static U Match<T, U>(this Result<T> result, Func<T, U> onSuccess, Func<Result<T>, U> onFailure)
            => result.IsSuccess ? onSuccess(result.Value) : onFailure(result);


        /* ===== Can Also be written like this
         public static U Match<T, U>(this Result<T> result, Func<T, U> onSuccess, Func<Result<T>, U> onFailure)
            => Match(result, () => onSuccess(result.Value), _ => onFailure(result));
         * ===== */

        public static U Match<U>(this Result result, Func<U> onSuccess, Func<Result, U> onFailure)
            => result.IsSuccess ? onSuccess() : onFailure(result);


        // = //

        // Result<T> 
        public static Result<U> Map<T, U>(this Result<T> result, Func<T, U> transform)
            => result.IsSuccess ? transform(result.Value) : result.Error;

        public async static Task<Result<U>> MapAsync<T, U>(this Result<T> result, Func<T, Task<U>> transform)
           => result.IsSuccess ? await transform(result.Value) : result.Error;

        // Result  
        public static Result<U> Map<U>(this Result result, Func<U> transform)
            => result.IsSuccess ? transform() : result.Error;
        public async static Task<Result<U>> MapAsync<U>(this Result result, Func<Task<U>> transform)
            => result.IsSuccess ? await transform() : result.Error;

        // Task<Result<T>> 
        public static Task<Result<U>> Map<T, U>(this Task<Result<T>> task, Func<T, U> transform)
            // 'ContinueWith' For row performence, but less safe.
            => task.ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return Error.Canceled;

                if (t.IsFaulted)
                    return Error.Unexpected;


                var result = t.Result;
                return result.IsSuccess ? transform(result.Value) : Result<U>.Failure(result.Error);
            });

        public async static Task<Result<U>> MapAsync<T, U>(this Task<Result<T>> task, Func<T, Task<U>> transform)
        {
            var result = await task;
            return result.IsSuccess ? await transform(result.Value) : result.Error;
        }

        // Task<Result>
        public static Task<Result<U>> Map<U>(this Task<Result> task, Func<U> transform)
            => task.ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return Error.Canceled;

                if (t.IsFaulted)
                    return Error.Unexpected;

                var result = t.Result;
                return result.IsSuccess ? transform() : Result<U>.Failure(result.Error);
            });
        public async static Task<Result<U>> MapAsync<U>(this Task<Result> task, Func<Task<U>> transform)
        {
            var result = await task;
            return result.IsSuccess ? await transform() : result.Error;
        }

        // = //

        // Result<T>
        public static Result<U> Bind<T, U>(this Result<T> result, Func<T, Result<U>> func)
            => result.IsSuccess ? func(result.Value) : result.Error;
        public static Result Bind<T>(this Result<T> result, Func<T, Result> func)
            => result.IsSuccess ? func(result.Value) : result.Error;
        public static Task<Result<U>> BindAsync<T, U>(this Result<T> result, Func<T, Task<Result<U>>> func)
            => result.IsSuccess ? func(result.Value) : result.Error.ToTaskResult<U>();
        public static Task<Result> BindAsync<T>(this Result<T> result, Func<T, Task<Result>> func)
            => result.IsSuccess ? func(result.Value) : result.Error; // <= implicit declared in Error

        // Result
        public static Result<U> Bind<U>(this Result result, Func<Result<U>> func)
            => result.IsSuccess ? func() : result.Error;    
        public static Result Bind(this Result result, Func<Result> func)
            => result.IsSuccess ? func() : result.Error;
        public static Task<Result<U>> BindAsync<U>(this Result result, Func<Task<Result<U>>> func)
            => result.IsSuccess ? func() : result.Error.ToTaskResult<U>();
        public static Task<Result> BindAsync(this Result result, Func<Task<Result>> func)
            => result.IsSuccess ? func() : result.Error;

        // Task<Result<T>>
        public async static Task<Result<U>> Bind<T, U>(this Task<Result<T>> task, Func<T, Result<U>> func)
        {
            var result = await task;
            return result.IsSuccess ? func(result.Value) : result.Error;
        }
        public async static Task<Result> Bind<T>(this Task<Result<T>> task, Func<T, Result> func)
        {
            var result = await task;
            return result.IsSuccess ? func(result.Value) : result.Error;
        }
        public async static Task<Result<U>> BindAsync<T, U>(this Task<Result<T>> task, Func<T, Task<Result<U>>> func)
        {
            var result = await task;
            return result.IsSuccess ? await func(result.Value) : result.Error;
        }
        public async static Task<Result> BindAsync<T>(this Task<Result<T>> task, Func<T, Task<Result>> func)
        {
            var result = await task;
            return result.IsSuccess ? await func(result.Value) : result.Error;
        }

        // Task<Result>
        public async static Task<Result<U>> Bind<U>(this Task<Result> task, Func<Result<U>> func)
        {
            var result = await task;
            return result.IsSuccess ? func() : result.Error;
        }
        public async static Task<Result> Bind(this Task<Result> task, Func<Result> func)
        {
            var result = await task;
            return result.IsSuccess ? func() : result.Error;
        }
        public async static Task<Result<U>> BindAsync<U>(this Task<Result> task, Func<Task<Result<U>>> func)
        {
            var result = await task;
            return result.IsSuccess ? await func() : result.Error;
        }
        public async static Task<Result> BindAsync(this Task<Result> task, Func<Task<Result>> func)
        {
            var result = await task;
            return result.IsSuccess ? await func() : result.Error;
        }


        // =====> Suggestion for BindSafe() <= TryCatch inside ?

        // = //

        // Result<T>
        public static Result<U> Ensure<U>(this Result<U> result, Func<U, bool> predicate, Func<Error, Result<U>> onFailure)
            => result.IsFailure || predicate(result.Value) ? result : onFailure(result.Error);
        public async static Task<Result<U>> EnsureAsync<U>(this Result<U> result, Func<U, Task<bool>> predicate, Func<Error, Result<U>> onFailure)
            => result.IsFailure || await predicate(result.Value) ? result : onFailure(result.Error);

        // Result
        public static Result Ensure(this Result result, Func<bool> predicate, Func<Error, Result> onFailure)
            => result.IsFailure || predicate() ? result : onFailure(result.Error);
        public async static Task<Result> EnsureAsync(this Result result, Func<Task<bool>> predicate, Func<Error, Result> onFailure)
            => result.IsFailure || await predicate() ? result : onFailure(result.Error);

        // Task<Result<T>>
        public async static Task<Result<U>> Ensure<U>(this Task<Result<U>> task, Func<U, bool> predicate, Func<Error, Result<U>> onFailure)
        {
            var result = await task;
            if (result.IsFailure || predicate(result.Value)) return result; 
            
            return onFailure(result.Error);
        }

        public async static Task<Result<U>> EnsureAsync<U>(this Task<Result<U>> task, Func<U, Task<bool>> predicate, Func<Error, Result<U>> onFailure)
        {
            var result = await task;
            return result.IsFailure || await predicate(result.Value) ? result : onFailure(result.Error);
        }

        // Task<Result>
        public async static Task<Result> Ensure(this Task<Result> task, Func<bool> predicate, Func<Error, Result> onFailure)
        {
            var result = await task;
            return result.IsFailure || predicate() ? result : onFailure(result.Error);
        }
        public async static Task<Result> EnsureAsync(this Task<Result> task, Func<Task<bool>> predicate, Func<Error, Result> onFailure)
        {
            var result = await task;
            return result.IsFailure || await predicate() ? result : onFailure(result.Error);
        }


        // = //

        // Result 
        public static Result Tap(this Result result, Action action) 
        { if (result.IsSuccess) action(); return result; }

        public async static Task<Result> TapAsync(this Result result, Func<Task> action)
        { if (result.IsSuccess) await action(); return result; }

        // Result<T>
        public static Result<U> Tap<U>(this Result<U> result, Action<U> action)
        { if (result.IsSuccess) action(result.Value); return result; }

        public async static Task<Result<U>> TapAsync<U>(this Result<U> result, Func<U, Task> action)
        { if (result.IsSuccess) await action(result.Value); return result; }

        // Task<Result>
        public async static Task<Result> Tap(this Task<Result> task, Action action)
        { var result = await task; if (result.IsSuccess) action(); return result; }

        public async static Task<Result> TapAsync(this Task<Result> task, Func<Task> action)
        { var result = await task; if (result.IsSuccess) await action(); return result; }

        // Task<Result<T>>
        public async static Task<Result<U>> Tap<U>(this Task<Result<U>> task, Action<U> action)
        { var result = await task; if(result.IsSuccess) action(result.Value); return result; }

        public async static Task<Result<U>> TapAsync<U>(this Task<Result<U>> task, Func<U, Task> action)
        { var result = await task; if(result.IsSuccess) await action(result.Value); return result; }


        // = //

        // Result 
        public static Result TapError(this Result result, Action action)
        { if (result.IsFailure) action(); return result; }

        public async static Task<Result> TapErrorAsync(this Result result, Func<Task> action)
        { if (result.IsFailure) await action(); return result; }

        // Result<T>
        public static Result<U> TapError<U>(this Result<U> result, Action<U> action)
        { if (result.IsFailure) action(result.Value); return result; }

        public async static Task<Result<U>> TapErrorAsync<U>(this Result<U> result, Func<U, Task> action)
        { if (result.IsFailure) await action(result.Value); return result; }

        // Task<Result>
        public async static Task<Result> TapError(this Task<Result> task, Action action)
        { var result = await task; if (result.IsFailure) action(); return result; }

        public async static Task<Result> TapErrorAsync(this Task<Result> task, Func<Task> action)
        { var result = await task; if (result.IsFailure) await action(); return result; }

        // Task<Result<T>>
        public async static Task<Result<U>> TapError<U>(this Task<Result<U>> task, Action<U> action, Func<Error, bool> atCondition)
        { var result = await task; if (result.IsFailure && atCondition(result.Error)) action(result.Value); return result; }

        public async static Task<Result<U>> TapErrorAsync<U>(this Task<Result<U>> task, Func<U, Task> action)
        { var result = await task; if (result.IsFailure) await action(result.Value); return result; }

        // = //

        // Result
        public async static Task<Result> TryCatch(this Task<Result> task, Func<Exception, Error> onFailure)
        {
            try   { return await task; }
            catch (TaskCanceledException) { return Error.Canceled; }
            catch (Exception ex) { return onFailure.Invoke(ex); }
        }

        public async static Task<Result<U>> TryCatch<U>(this Task<Result<U>> task, Func<Exception, Error> onFailure)
        {
            try   { return await task; }
            catch (TaskCanceledException) { return Error.Canceled; }
            catch (Exception ex) { return onFailure.Invoke(ex); }
        }

        public async static Task<Result<U>> TryCatchAsync<U>(Func<Task<U>> action, Func<Exception, Error> onFailure)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                return onFailure.Invoke(ex);
            }
        }

        public async static Task<Result> TryCatchAsync(Func<Task> action, Func<Exception, Error> onFailure)
        {
            try
            {
                await action();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return onFailure.Invoke(ex);
            }
        }

        public async static Task<Result<U>> TryCatchAsync<U>(U Value, Func<U, Task<U>> transform, Func<Exception, Error> onFailure)
           => await TryCatchAsync(() => transform(Value), onFailure);

        public async static Task<Result<U>> TryCatchAsync<T, U>(T Value, Func<T, Task<U>> transform, Func<Exception, Error> onFailure)
            => await TryCatchAsync(() => transform(Value), onFailure);

        public static Result<U> TryCatch<U>(Func<U> action, Func<Exception, Error> onFailure)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                return onFailure.Invoke(ex);
            }
        }

        public static Result TryCatch(Action action, Func<Exception, Error> onFailure)
        {
            try
            {
                action();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return onFailure.Invoke(ex);
            }
        }

        public static Result<U> TryCatch<U>(U Value, Func<U, U> action, Func<Exception, Error> onFailure)
            => TryCatch(() => action(Value), onFailure);

        // = //

        private async static Task<Result<U>> WithTransactionCore<U>(Result<U> result, IRepositoryManager unitOfWork, Func<U, Task> action)
        {

            if (result.IsFailure)
                return result;

            await using var tx = await unitOfWork.BeginTransactionAsync();

            try
            {
                await action(result.Value);

                await tx.CommitAsync();

                return result;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();

                throw;
            }

        }

        public async static Task<Result<U>> WithTransaction<U>(this Task<Result<U>> task, IRepositoryManager unitOfWork, Func<U, Task> action)
            => await WithTransactionCore(await task, unitOfWork, action);

        public static Task<Result<U>> WithTransaction<U>(this Result<U> result, IRepositoryManager unitOfWork, Func<U, Task> action)
            => WithTransactionCore(result, unitOfWork, action);


        private async static Task<Result> WithTransactionCore(Result result, IRepositoryManager unitOfWork, Func<Task> action)
        {
            if (result.IsFailure)
                return result;

            await using var tx = await unitOfWork.BeginTransactionAsync();

            try
            {
                await action();

                await tx.CommitAsync();

                return result;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();

                throw;
            }

        }

        public async static Task<Result> WithTransaction(Task<Result> task, IRepositoryManager unitOfWork, Func<Task> action)
            => await WithTransactionCore(await task, unitOfWork, action);

        public static Task<Result> WithTransaction(Result result, IRepositoryManager unitOfWork, Func<Task> action)
            =>  WithTransactionCore(result, unitOfWork, action);


        // = //

        

        
    }
}
