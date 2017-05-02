using Contentful.Core.Models;
using System.Collections.Generic;

namespace Contentful.Essential.Models.Management
{
	public class FieldComparer : IEqualityComparer<Field>
	{
		public bool Equals(Field x, Field y)
		{
			return x.Id == y.Id;
		}

		public int GetHashCode(Field obj)
		{
			return obj.Id.GetHashCode();
		}
	}
}