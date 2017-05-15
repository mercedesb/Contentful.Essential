namespace Contentful.Essential.WebHooks
{
    public interface IWebHookHandler
    {
        string[] ForTypes { get; }
        string[] ForActions { get; }

        WebHookResponseMessage Process(WebHookRequestMessage request);
    }
}