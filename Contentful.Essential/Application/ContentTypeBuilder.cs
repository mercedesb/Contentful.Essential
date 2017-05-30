﻿using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Essential.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Contentful.CodeFirst
{
    /// <summary>
    /// The main class of Contentful.CodeFirst. Allows you to configure and call the Contentful API to create content types from your code.
    /// </summary>
    public class ContentTypeBuilder
    {
        /// <summary>
        /// Loads all types with a <see cref="ContentTypeAttribute"/> in a given assembly.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to load types from.</param>
        /// <returns>An enumerable of types loaded.</returns>
        public static IEnumerable<Type> LoadTypes(string assemblyName)
        {
            return LoadTypes(Assembly.Load(new AssemblyName(assemblyName)));
        }

        /// <summary>
        /// Loads all types with a <see cref="ContentTypeAttribute"/> in a given assembly.
        /// </summary>
        /// <param name="assembly">The assembly to load types from.</param>
        /// <returns>An enumerable of types loaded.</returns>
        public static IEnumerable<Type> LoadTypes(Assembly assembly)
        {
            var types = assembly.ExportedTypes.Where(c => c.GetTypeInfo().IsClass && c.GetTypeInfo().GetCustomAttribute<ContentTypeAttribute>() != null);

            return types;
        }

        /// <summary>
        /// Initializes content types ready to be sent to the Contentful API from a number of types.
        /// </summary>
        /// <param name="types">The types to be transformed into content types. Could be chained to the output of the <see cref="LoadTypes(Assembly)"/> method.</param>
        /// <returns>An enumerable of <see cref="ContentType"/> ready to be created/updated in Contentful.</returns>
        public static IEnumerable<ContentType> InitializeContentTypes(IEnumerable<Type> types, bool camelCaseFieldIds = false)
        {
            types = types.OrderBy(c => c.GetTypeInfo().GetCustomAttribute<ContentTypeAttribute>()?.Order ?? 0);

            foreach (var type in types)
            {
                var attribute = type.GetTypeInfo().GetCustomAttribute<ContentTypeAttribute>();

                var id = attribute.Id ?? type.Name;
                var name = attribute.Name ?? type.Name;

                var displayField = attribute.DisplayField;
                var description = attribute.Description;

                var contentType = new ContentType()
                {
                    SystemProperties = new SystemProperties
                    {
                        Id = id
                    },

                    Name = name,

                    DisplayField = displayField,

                    Description = description,

                    Fields = new List<Field>()
                };
                foreach (var prop in type.GetTypeInfo().DeclaredProperties)
                {
                    if (prop.SetMethod == null || prop.GetCustomAttribute<IgnoreContentFieldAttribute>() != null)
                    {
                        continue;
                    }

                    var fieldAttribute = prop.GetCustomAttribute<ContentFieldAttribute>() ?? new ContentFieldAttribute();
                    var field = new Field()
                    {
                        Id = fieldAttribute.Id ?? (camelCaseFieldIds ? prop.Name.ToCamelCase() : prop.Name),
                        Name = fieldAttribute.Name ?? prop.Name,
                        Type = fieldAttribute.Type ?? FieldTypeConverter.Convert(prop.PropertyType),
                        Disabled = fieldAttribute.Disabled,
                        Omitted = fieldAttribute.Omitted,
                        Localized = fieldAttribute.Localized,
                        Required = fieldAttribute.Required,
                        LinkType = fieldAttribute.LinkType ?? FieldTypeConverter.ConvertLinkType(prop.PropertyType),
                        Validations = new List<IFieldValidator>()
                    };
                    var validationAttributes = prop.GetCustomAttributes<ContentfulValidationAttribute>();
                    var isCollectionProperty = typeof(ICollection).GetTypeInfo().IsAssignableFrom(prop.PropertyType.GetTypeInfo());

                    if (isCollectionProperty)
                    {
                        field.Items = new Schema()
                        {
                            LinkType = fieldAttribute.ItemsLinkType ?? FieldTypeConverter.ConvertItemLinkType(prop.PropertyType),
                            Type = fieldAttribute.ItemsType ?? FieldTypeConverter.ConvertItemType(prop.PropertyType),
                            Validations = new List<IFieldValidator>()
                        };
                    }

                    foreach (var validation in validationAttributes)
                    {
                        if (isCollectionProperty && validation is SizeAttribute == false)
                        {
                            field.Items.Validations.Add(validation.Validator);
                        }
                        else
                        {
                            field.Validations.Add(validation.Validator);
                        }
                    }



                    contentType.Fields.Add(field);
                }

                yield return contentType;
            }
        }

        /// <summary>
        /// Creates a number of content types in Contentful.
        /// </summary>
        /// <param name="contentTypes">The content types to create.</param>
        /// <param name="configuration">The configuration for the creation process.</param>
        /// <param name="client">The optional client to use for creation.</param>
        /// <returns>A list of created or updated content types.</returns>
        public static async Task<List<ContentType>> CreateContentTypes(IEnumerable<ContentType> contentTypes, ContentfulCodeFirstConfiguration configuration, IContentfulManagementClient client = null)
        {
            var managementClient = client;

            if (managementClient == null)
            {
                var httpClient = new HttpClient();
                managementClient = new ContentfulManagementClient(httpClient, configuration.ApiKey, configuration.SpaceId);
            }

            var createdTypes = new List<ContentType>();

            var existingContentTypes = (await managementClient.GetContentTypesAsync()).ToList();

            if (configuration.ForceUpdateContentTypes == false)
            {
                //remove any pre-existing content types from the list to be created.
                contentTypes = contentTypes.Where(c => !existingContentTypes.Any(x => x.SystemProperties.Id == c.SystemProperties.Id));
            }

            foreach (var contentType in contentTypes)
            {
                //make sure to add correct version for existing content types
                contentType.SystemProperties.Version = existingContentTypes.FirstOrDefault(c => c.SystemProperties.Id == contentType.SystemProperties.Id)?.SystemProperties.Version;

                var createdContentType = await managementClient.CreateOrUpdateContentTypeAsync(contentType, version: contentType.SystemProperties.Version);

                if (configuration.PublishAutomatically)
                {
                    createdContentType = await managementClient.ActivateContentTypeAsync(createdContentType.SystemProperties.Id, createdContentType.SystemProperties.Version ?? 1);
                }

                createdTypes.Add(createdContentType);
            }

            return createdTypes;
        }

        /// <summary>
        /// Creates content types in Contentful from any types with a <see cref="ContentTypeAttribute"/> found in an Assembly.
        /// </summary>
        /// <param name="assemblyName">The assembly to load types from.</param>
        /// <param name="configuration">The configuration for the creation process.</param>
        /// <param name="client">The optional client to use for creation.</param>
        /// <returns>A list of created or updated content types.</returns>
        public static async Task<List<ContentType>> CreateContentTypesFromAssembly(string assemblyName, MWContentfulCodeFirstConfiguration configuration, IContentfulManagementClient client = null)
        {
            var types = LoadTypes(assemblyName);
            var contentTypesToCreate = InitializeContentTypes(types, configuration.CamelcaseFieldIdsAutomatically);
            var createdContentTypes = await CreateContentTypes(contentTypesToCreate, configuration, client);
            return createdContentTypes;
        }
    }
}
