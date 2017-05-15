using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;
using System.Linq;

namespace Contentful.Essential.WebHooks
{
    public static partial class WebHookDispatcher
    {
        /// <summary>
        /// Compares the request to available handlers, executes matches, and returns log entires
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static List<WebHookResponseMessage> Process(WebHookRequestMessage request)
        {
            var log = new List<WebHookResponseMessage>();
            var handlers = ServiceLocator.Current.GetAllInstances<IWebHookHandler>().Where(h => IsMatch(h, request));

            foreach (var currHandler in handlers)
            {
                var result = currHandler.Process(request);

                if (result != null)
                {
                    result.Source = result.Source ?? currHandler.GetType().Name; // If they don't explicitly set a source, use the handler class name
                    log.Add(result);
                }
            }

            return log;
        }

        public static bool IsMatch(IWebHookHandler handler, WebHookRequestMessage request)
        {
            bool matchesType = handler.ForTypes != null && (handler.ForTypes.Contains("*") || handler.ForTypes.Contains(request.ContentfulType));
            bool matchesAction = handler.ForActions != null && (handler.ForActions.Contains("*") || handler.ForActions.Contains(request.ContentfulAction));

            return matchesType && matchesAction;
        }
    }
}
