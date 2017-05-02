using Contentful.Core.Models.Management;
using System;

namespace Contentful.Essential.Models.Management
{
	public class DateRangeValidator : IFieldValidator
	{
		protected string _min;

		/// <summary>
		/// The minimum allowed date.
		/// </summary>
		public DateTime? Min
		{
			get
			{
				return DateTime.Parse(_min);
			}
		}

		protected string _max;

		/// <summary>
		/// The maximum allowed date.
		/// </summary>
		public DateTime? Max
		{
			get
			{
				return DateTime.Parse(_max);

			}
		}

		//public DateTime? Before { get; set; }
		//public DateTime? After { get; set; }

		/// <summary>
		/// The custom error message that should be displayed.
		/// </summary>
		public string Message { get; set; }


		/// <summary>
		/// Initializes a new instance of <see cref="T:Contentful.Essential.Models.Management.DateRangeValidator" />.
		/// </summary>
		/// <param name="minWidth">The minimum width of the image.</param>
		/// <param name="maxWidth">The maximum width of the image.</param>
		/// <param name="message">The custom error message for this validation.</param>
		public DateRangeValidator(string min, string max, string message = null)
		{
			_min = min;
			_max = max;
			//this.Before = before;
			//this.After = after;
			this.Message = message;
		}
		public object CreateValidator()
		{
			return new { dateRange = new { min = _min, max = _max }, message = this.Message };
		}
	}
}