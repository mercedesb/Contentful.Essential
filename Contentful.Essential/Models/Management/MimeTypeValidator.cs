using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using System.Collections.Generic;
using System.Linq;

namespace Contentful.Essential.Models.Management
{
	/// <summary>
	/// Represents a validator that validates that an asset is of a certain <see cref="T:Contentful.Core.Search.MimeTypeRestriction" />
	/// Overrides the Contentful SDK because there is currently a bug
	/// </summary>
	public class MimeTypeValidator : Contentful.Core.Models.Management.MimeTypeValidator, IFieldValidator
	{
		public MimeTypeValidator(IEnumerable<MimeTypeRestriction> mimeTypes, string message = null) : base(mimeTypes, message)
		{
		}

		/// <summary>
		/// Creates a representation of this validator that can be easily serialized.
		/// </summary>
		/// <returns>The object to serialize.</returns>
		public new object CreateValidator()
		{
			return new { linkMimetypeGroup = MimeTypes.Select(mt => mt.ToString().ToLower()), message = this.Message };
		}
	}
}