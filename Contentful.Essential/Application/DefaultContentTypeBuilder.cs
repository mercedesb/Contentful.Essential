using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Essential.Http;
using Contentful.Essential.Models;
using Contentful.Essential.Models.Attributes;
using Contentful.Essential.Models.Management;
using log4net.Core;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Contentful.Essential.Application
{
	public class DefaultContentTypeBuilder : IContentTypeBuilder
	{
		protected List<IHaveFieldValidation> _iHaveFieldValidationInstances;
		protected Dictionary<string, EditorInterfaceControl> _customEditors;

		public virtual async Task CreateOrUpdateContentTypes()
		{
			_iHaveFieldValidationInstances = GetIHaveFieldValidationInstances();
			foreach (var entry in ServiceLocator.Current.GetAllInstances<IEntry>())
			{
				try
				{
					_customEditors = new Dictionary<string, EditorInterfaceControl>();
					if (!string.IsNullOrWhiteSpace(entry.ContentTypeId))
					{
						ContentType contentType = await ContentDelivery.Instance.GetContentTypeAsync(entry.ContentTypeId);
						await Update(contentType, entry);
					}
				}
				catch (RuntimeBinderException ex)
				{
					await Create(entry);
				}
				catch (Exception ex)
				{
					SystemLog.Log(this, $"Unable to get content type: {entry.GetType()}", Level.Error, ex);
				}
			}
		}

		protected virtual async Task Create(IEntry entry)
		{
			Type entryType = entry.GetType();

			ContentType contentType = new ContentType();
			contentType.SystemProperties = new SystemProperties();
			contentType.SystemProperties.Id = entry.ContentTypeId;
			contentType.Name = !string.IsNullOrWhiteSpace(entry.ContentTypeName) ? entry.ContentTypeName : entryType.Name;
			contentType.Description = entry.ContentTypeDescription;

			string displayField;
			contentType.Fields = GetFields(entryType, out displayField);
			if (!contentType.Fields.Any())
				return;
			contentType.DisplayField = displayField != null ? displayField : contentType.Fields.First().Id;

			try
			{
				contentType = await ContentManagement.Instance.CreateOrUpdateContentTypeAsync(contentType);
				contentType = await ContentManagement.Instance.ActivateContentTypeAsync(contentType.SystemProperties.Id, contentType.SystemProperties.Version ?? 0);

				SystemLog.Log(this, $"Created content type: {entryType.GetType()}", Level.Info);

				await CustomizeEditorInterface(contentType);
			}
			catch (Exception ex)
			{
				SystemLog.Log(this, $"Unable to create content type: {entryType.GetType()}", Level.Error, ex);
			}
		}

		protected virtual async Task Update(ContentType currentContentType, IEntry entry)
		{
			bool contentTypeUpdated = false;
			Type entryType = entry.GetType();
			string displayField;
			List<Field> codeDefinedFields = GetFields(entryType, out displayField);
			IEnumerable<Field> addedFields = codeDefinedFields.Except(currentContentType.Fields, new FieldComparer());
			IEnumerable<Field> removedFields = currentContentType.Fields.Except(codeDefinedFields, new FieldComparer());
			IEnumerable<Field> updatedFields = currentContentType.Fields.Intersect(codeDefinedFields, new FieldComparer());

			if (addedFields.Any())
			{
				currentContentType.Fields.AddRange(addedFields);
				contentTypeUpdated = true;
			}
			if (removedFields.Any())
			{
				foreach (Field removed in removedFields)
				{
					removed.Omitted = true;
				}
				contentTypeUpdated = true;
			}

			foreach (Field fieldToUpdate in updatedFields)
			{
				Field codeUpdatedField = codeDefinedFields.First(f => f.Id == fieldToUpdate.Id);
				if (contentTypeUpdated) // we already have to make the API call, this won't cost us any extra so don't worry about checking for changes
				{
					fieldToUpdate.Validations = codeUpdatedField.Validations;
					continue;
				}

				// we may not need to make an API call, should check if there are updates and handle accordingly
				if (fieldToUpdate.Validations != null && codeUpdatedField.Validations != null)
				{
					// added or removed. (simplest use case)
					if (fieldToUpdate.Validations.Count() != codeUpdatedField.Validations.Count())
					{
						fieldToUpdate.Validations = codeUpdatedField.Validations;
						contentTypeUpdated = true;
					}
					// TODO: validations changed
				}
			}

			if (contentTypeUpdated)
			{
				// version is only populated in Management responses. only want to make this call if we need to update (since Management API calls $$)
				ContentType mgmtContentType = await ContentManagement.Instance.GetContentTypeAsync(currentContentType.SystemProperties.Id);
				mgmtContentType = await ContentManagement.Instance.CreateOrUpdateContentTypeAsync(currentContentType, null, mgmtContentType.SystemProperties.Version);
				mgmtContentType = await ContentManagement.Instance.ActivateContentTypeAsync(mgmtContentType.SystemProperties.Id, mgmtContentType.SystemProperties.Version ?? 0);
				SystemLog.Log(this, $"Updated content type: {entryType.GetType()}", Level.Info);
			}

			await CustomizeEditorInterface(currentContentType);
		}

		//TODO: editors were changed
		protected virtual async Task CustomizeEditorInterface(ContentType contentType)
		{
			if (!_customEditors.Any())
				return;

			EditorInterface editorInterface = await ContentManagement.Instance.GetEditorInterfaceAsync(contentType.SystemProperties.Id);

			foreach (KeyValuePair<string, EditorInterfaceControl> editor in _customEditors)
			{
				EditorInterfaceControl currentControl = editorInterface.Controls.First(f => f.FieldId == editor.Key);
				currentControl.WidgetId = editor.Value.WidgetId;
				currentControl.Settings = editor.Value.Settings;

			}
			editorInterface = await ContentManagement.Instance.UpdateEditorInterfaceAsync(editorInterface, contentType.SystemProperties.Id, editorInterface.SystemProperties.Version.Value);
		}

		protected virtual List<Field> GetFields(Type type, out string displayField)
		{
			List<Field> fields = new List<Field>();
			displayField = null;
			PropertyInfo[] props = type.GetProperties();

			foreach (PropertyInfo prop in props)
			{
				try
				{
					// Setter doesn't exist or isn't public. Don't add b/c editor won't be able to edit
					if (prop.GetSetMethod() == null)
						continue;
					if (!IsField(prop.PropertyType))
						continue;

					Field field = new Field();

					DisplayAttribute display = GetAttribute<DisplayAttribute>(prop);
					if (display != null)
						field.Name = display.Name;
					else
						field.Name = prop.Name;

					string id = Regex.Replace(field.Name, @"[^\w\._-]", string.Empty);
					//TODO: get rid of magic number
					field.Id = id.Length > 60 ? id.Substring(0, 60) : id;

					field.Required = Attribute.IsDefined(prop, typeof(RequiredAttribute));
					field.Localized = Attribute.IsDefined(prop, typeof(LocalizedAttribute));
					field.Omitted = Attribute.IsDefined(prop, typeof(OmittedAttribute));

					EditableAttribute editable = GetAttribute<EditableAttribute>(prop);
					field.Disabled = editable != null && !editable.AllowEdit;

					if (Attribute.IsDefined(prop, typeof(KeyAttribute)))
						displayField = field.Id;

					FieldLinkType? fieldLinkType;
					field.Type = GetFieldType(prop, out fieldLinkType);

					if (field.Type == SystemFieldTypes.Link)
						field.LinkType = fieldLinkType.ToString();

					if (field.Type == SystemFieldTypes.Array)
					{
						field.Items = new Schema();

						//TODO: fix this
						if (prop.PropertyType.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(prop.PropertyType.GetGenericTypeDefinition()))
						{
							Type itemType = prop.PropertyType.GetGenericArguments()[0]; // use this...
							FieldLinkType? fieldItemLinkType;
							string fieldItemType = GetFieldType(itemType, out fieldItemLinkType);

							if (fieldItemType == SystemFieldTypes.Link)
							{
								field.Items.Type = fieldItemType.ToString();
								field.Items.LinkType = fieldItemLinkType.ToString();
							}
							// only list of strings is supported
							else
							{
								field.Items.Type = SystemFieldTypes.Symbol.ToString();
							}
							List<IFieldValidator> itemValidators = GetValidations(prop, fieldItemType, fieldLinkType);
							if (itemValidators.Any())
								field.Items.Validations = itemValidators;
						}
					}

					List<IFieldValidator> validators = GetValidations(prop, field.Type, fieldLinkType);
					if (validators.Any())
						field.Validations = validators;

					fields.Add(field);

					KeyValuePair<string, EditorInterfaceControl> customEditor = GetCustomEditor(prop, field);
					if (!customEditor.Equals(default(KeyValuePair<string, EditorInterfaceControl>)))
						_customEditors.Add(customEditor.Key, customEditor.Value);
				}
				catch (Exception ex)
				{
					SystemLog.Log(this, $"Unable to create field: {prop.Name}", Level.Error, ex);
				}
			}
			return fields;
		}

		protected virtual bool IsField(Type type)
		{
			return type == typeof(string)
				|| type == typeof(bool)
				|| type == typeof(int)
				|| type == typeof(long)
				|| type == typeof(double)
				|| type == typeof(decimal)
				|| type == typeof(DateTime)
				|| type.IsEnum
				|| type == typeof(Location)
				|| type == typeof(Asset)
				|| typeof(IEntry).IsAssignableFrom(type)
				|| typeof(IEnumerable).IsAssignableFrom(type)
				|| (type.IsClass && type != typeof(SystemProperties));
		}

		protected virtual T GetAttribute<T>(PropertyInfo prop) where T : Attribute
		{
			object attrObj = prop.GetCustomAttributes(typeof(T), true).FirstOrDefault();
			T attr = attrObj != null ? attrObj as T : null;
			return attr;
		}

		// TODO: need to handle JSONobject and location
		protected virtual string GetFieldType(PropertyInfo prop, out FieldLinkType? linkType)
		{
			string fieldType = GetFieldType(prop.PropertyType, out linkType);

			if (prop.PropertyType == typeof(string) && Attribute.IsDefined(prop, typeof(LongStringAttribute)))
				return SystemFieldTypes.Text;

			return fieldType;
		}

		protected virtual string GetFieldType(Type type, out FieldLinkType? linkType)
		{
			linkType = null;

			if (type == typeof(string))
				return SystemFieldTypes.Symbol;
			if (type == typeof(int) || type == typeof(long))
				return SystemFieldTypes.Integer;
			if (type == typeof(double) || type == typeof(decimal))
				return SystemFieldTypes.Number;
			if (type == typeof(bool))
				return SystemFieldTypes.Boolean;
			if (type == typeof(DateTime))
				return SystemFieldTypes.Date;
			if (type.IsEnum)
				return SystemFieldTypes.Symbol;
			if (type == typeof(Location))
				return SystemFieldTypes.Location;
			if (type == typeof(Asset))
			{
				linkType = FieldLinkType.Asset;
				return SystemFieldTypes.Link;
			}
			if (typeof(IEntry).IsAssignableFrom(type))
			{
				linkType = FieldLinkType.Entry;
				return SystemFieldTypes.Link;
			}
			if (typeof(IEnumerable).IsAssignableFrom(type))
				return SystemFieldTypes.Array;
			if (type.IsClass && type != typeof(SystemProperties)) // || type.IsInterface
				return SystemFieldTypes.Object;

			return SystemFieldTypes.Symbol;
		}

		protected virtual List<IFieldValidator> GetValidations(PropertyInfo prop, string fieldType, FieldLinkType? linkType = null)
		{
			List<IFieldValidator> validators = new List<IFieldValidator>();

			foreach (var fieldValidationAttr in _iHaveFieldValidationInstances)
			{
				Type fieldValidationType = fieldValidationAttr.GetType();

				if (!Attribute.IsDefined(prop, fieldValidationType))
					continue;

				if (!fieldValidationAttr.ValidFieldTypes.Contains(fieldType))
					continue;

				if (linkType == null || fieldValidationAttr.ValidLinkTypes.Contains(linkType.Value))
				{
					object attrObj = prop.GetCustomAttributes(fieldValidationType, true).FirstOrDefault();
					IHaveFieldValidation attr = attrObj != null ? attrObj as IHaveFieldValidation : null;
					if (attr != null && attr.Validator != null)
						validators.Add(attr.Validator);
				}
			}

			if (prop.PropertyType.IsEnum)
			{
				validators.Add(new InValuesValidator(Enum.GetNames(prop.PropertyType)));
			}
			// handle enum argument to enumerable
			if (fieldType != SystemFieldTypes.Array && prop.PropertyType.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(prop.PropertyType.GetGenericTypeDefinition()) && prop.PropertyType.GetGenericArguments()[0].IsEnum)
			{
				validators.Add(new InValuesValidator(Enum.GetNames(prop.PropertyType.GetGenericArguments()[0])));
			}

			return validators;
		}

		protected virtual KeyValuePair<string, EditorInterfaceControl> GetCustomEditor(PropertyInfo prop, Field field)
		{
			if (Attribute.IsDefined(prop, typeof(EditorInterfaceControlAttribute)))
			{
				EditorInterfaceControlAttribute editUIAttr = GetAttribute<EditorInterfaceControlAttribute>(prop);
				EditorInterfaceControl customControl = new EditorInterfaceControl();
				customControl.FieldId = field.Id;
				customControl.WidgetId = editUIAttr.WidgetId;
				customControl.Settings = new EditorInterfaceControlSettings { HelpText = editUIAttr.HelpText };
				return new KeyValuePair<string, EditorInterfaceControl>(field.Id, customControl);
			}
			return default(KeyValuePair<string, EditorInterfaceControl>);
		}

		protected virtual List<IHaveFieldValidation> GetIHaveFieldValidationInstances()
		{
			try
			{
				return ServiceLocator.Current.GetAllInstances<IHaveFieldValidation>().ToList();
			}
			catch (Exception ex)
			{
				SystemLog.Log(this, $"Error getting all instances of IHaveFieldValidation from ServiceLocator", Level.Error, ex);
				return new List<IHaveFieldValidation>();
			}
		}
	}
}