using System;

namespace Contentful.Essential.Models.Attributes
{
	[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
	public class ContentTypeDefinitionAttribute : Attribute
	{
		public string ContentTypeId;
		public string Name;
		public string Description;
		public ContentTypeDefinitionAttribute(string contentTypeId, string name = "", string description = "")
		{
			ContentTypeId = contentTypeId;
			Name = name;
			Description = description;
		}
	}
}