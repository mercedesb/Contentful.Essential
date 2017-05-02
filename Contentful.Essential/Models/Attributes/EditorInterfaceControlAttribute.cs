using System;

namespace Contentful.Essential.Models.Attributes
{
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
	public class EditorInterfaceControlAttribute : Attribute
	{
		public string WidgetId { get; set; }
		public string HelpText { get; set; }
		public EditorInterfaceControlAttribute(string widgetId, string helpText = "")
		{
			WidgetId = widgetId;
			HelpText = helpText;
		}
	}
}