using System.Diagnostics;

namespace UrlShortener.Api.Middlewares
{
    public class ProfilingMiddlewarecs
    {
        public readonly RequestDelegate _next;
        public ProfilingMiddlewarecs(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Stopwatch _stopwatch = new();
            _stopwatch.Start();
            await _next(context);
            _stopwatch.Stop();
            Console.WriteLine(_stopwatch.ElapsedMilliseconds);
        }
    }
}
