namespace MyApp.Application;

using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using MyApp.Application.Exceptions;

public static class RxRetry
{
    public static IObservable<T> RetryWithBackoff<T>(
        this IObservable<T> source,
        int maxRetries,
        TimeSpan initialDelay,
        double factor = 2.0)
    {
        return source.RetryWhen(errors =>
            errors
                .Zip(Observable.Range(1, maxRetries), (error, attempt) => (error, attempt))
                .SelectMany(tuple => NextDelayOrError(tuple.error, tuple.attempt, maxRetries, initialDelay, factor))
        );

        static IObservable<int> NextDelayOrError(
            Exception error,
            int attempt,
            int maxRetries,
            TimeSpan initialDelay,
            double factor)
        {
            // Never retry auth redirect / token acquisition
            if (error is AuthenticationUnavailableException)
                return Observable.Throw<int>(error);
                
            // Last attempt -> propagate
            if (attempt == maxRetries)
                return Observable.Throw<int>(error);

            // No network / DNS / connection refused
            if (error is HttpRequestException { StatusCode: null } or HttpRequestException { InnerException: SocketException })
                return Observable.Throw<int>(error);

            if (error is HttpRequestException { StatusCode: HttpStatusCode status })
            {
                // Non-transient 4xx except 408/429
                if (status is >= HttpStatusCode.BadRequest and < HttpStatusCode.InternalServerError
                    && status is not HttpStatusCode.RequestTimeout
                    && status != (HttpStatusCode)429) // too many request
                {
                    return Observable.Throw<int>(error);
                }

                // Non-transient 501/505
                if (status is HttpStatusCode.NotImplemented or HttpStatusCode.HttpVersionNotSupported)
                    return Observable.Throw<int>(error);
            }

            var delayMs = initialDelay.TotalMilliseconds * Math.Pow(factor, attempt - 1);
            return Observable.Timer(TimeSpan.FromMilliseconds(delayMs)).Select(_ => attempt);
        }
    }
}
