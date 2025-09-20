using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using UrlShortener.Core.Domain.Exceptions.Base;
using UrlShortener.Core.Services.Interfaces;

namespace UrlShortener.Api.Middlewares
{

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILoggerManager logger)
        {
            _next = next;
            _logger = logger;
        }

        // This was static ! not sure if its important
        private async Task WriteError(HttpContext context, int statusCode, string message, Exception ex = null)
        {
                _logger.LogWarn("Exception Caught:{message}|{statusCode}|{CorrelationId}", ex, args: [message, statusCode, context.TraceIdentifier]);
                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsJsonAsync(new { error = message });
        }

        public async Task Invoke(HttpContext context)
        {

            try
            {
                await _next(context);
            }
            catch (HttpStatusCodeException ex)
            {
                await WriteError(context, ex.StatusCode, "HttpStatus Code Exception", ex);
            } 
            catch(JsonException ex)
            {
                await WriteError(context, StatusCodes.Status400BadRequest, "Invalid JSON format.", ex);
            }
            catch (SecurityTokenExpiredException ex)
            {
                await WriteError(context, StatusCodes.Status400BadRequest, "Expired Token.", ex);
            }
            catch(TypeLoadException ex)
            {
                await WriteError(context, StatusCodes.Status400BadRequest, "Loading a type that does not exist in assembly.", ex);
            }
            catch(AuthException ex)
            {
                await WriteError(context, StatusCodes.Status401Unauthorized, "Auth Exception.",  ex);
            }
            catch (DomainException ex)
            {
                await WriteError(context, StatusCodes.Status500InternalServerError, "Domain Exception.",  ex);
            }
            catch (InfrastructureException ex)
            {
                await WriteError(context, StatusCodes.Status500InternalServerError, "Infrastructure Exception", ex);
            }
            catch (TaskCanceledException ex) when (!context.RequestAborted.IsCancellationRequested)
            {
                await WriteError(context, StatusCodes.Status504GatewayTimeout, "Request timed out.", ex);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await WriteError(context, StatusCodes.Status409Conflict, "The resource was modified by another process.", ex);
            }
            catch (DbUpdateException ex)
            {
                await WriteError(context, StatusCodes.Status500InternalServerError, "A database error occurred.", ex);
            }
            catch (Exception ex)
            {
                await WriteError(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.", ex);
            }
        }
    }
}
