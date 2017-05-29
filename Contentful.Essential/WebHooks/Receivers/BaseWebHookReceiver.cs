using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contentful.Essential.WebHooks.Receivers
{
    public abstract class BaseWebHookReceiver : IWebHookHandler
    {
        public abstract string[] ForActions { get; }
<<<<<<< HEAD

=======
       
>>>>>>> master
        public abstract string[] ForTypes { get; }
        public abstract WebHookResponseMessage Process(WebHookRequestMessage request);

        public virtual bool IsMatch(WebHookRequestMessage request)
        {
            bool matchesType = ForTypes != null && (ForTypes.Contains("*") || ForTypes.Contains(request.ContentfulType));
            bool matchesAction = ForActions != null && (ForActions.Contains("*") || ForActions.Contains(request.ContentfulAction));

            return matchesType && matchesAction;
        }
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> master
