namespace Contentful.Essential
{
	public class Constants
	{
	}
	public enum FieldLinkType
	{
		Entry,
		Asset
	}

    public class WebHookActions
    {
        public const string AutoSave = "auto_save";
        public const string Archive = "archive";
        public const string Create = "create";
        public const string Delete = "delete";
        public const string Publish = "publish";
        public const string Save = "save";
        public const string Unarchive = "unarchive";
        public const string Unpublish = "unpublish";
    }

    public class WebHookTypes
    {
        public const string Asset = "Asset";
        public const string ContentType = "ContentType";
        public const string Entry = "Entry";
    }
}