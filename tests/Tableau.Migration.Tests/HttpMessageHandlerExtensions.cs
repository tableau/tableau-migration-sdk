using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;

namespace Tableau.Migration.Tests
{
    public static class HttpMessageHandlerExtensions
    {
        public static IImmutableList<HttpMessageHandler> Flatten(this HttpMessageHandler handler)
        {
            var handlers = new List<HttpMessageHandler>()
            {
                handler
            };

            if (handler is DelegatingHandler delegatingHandler)
            {
                var innerHandler = delegatingHandler.InnerHandler;

                if (innerHandler is not null)
                    handlers.AddRange(innerHandler.Flatten());
            }

            return handlers.ToImmutableList();
        }
    }
}
