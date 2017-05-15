namespace Contentful.Essential.WebHooks
{
    public class WebHookResponseMessage
    {
        public string Source { get; set; }
        public string Message { get; set; }

        public WebHookResponseMessage(string message)
        {
            Message = message;
        }
    }
}
