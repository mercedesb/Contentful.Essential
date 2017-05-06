namespace Contentful.Essential.Models
{
    public interface IContentType
    {
        string ContentTypeId { get; }
        string ContentTypeName { get; }
        string ContentTypeDescription { get; }
    }
}