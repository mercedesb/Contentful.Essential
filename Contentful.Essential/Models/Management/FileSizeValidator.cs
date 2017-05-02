using Contentful.Core.Models.Management;

namespace Contentful.Essential.Models.Management
{
	public class FileSizeValidator : IFieldValidator
	{
		protected const int BYTES_IN_KB = 1024;
		protected const int BYTES_IN_MB = 1048576;
		/// <summary>
		/// The minimum allowed size of the file.
		/// </summary>
		public int? Min { get; set; }

		/// <summary>
		/// The maximum allowed size of the file.
		/// </summary>
		public int? Max { get; set; }
		/// <summary>
		/// The custom error message that should be displayed.
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Initializes a new instance of <see cref="T:Contentful.Essential.Models.Management.FileSizeValidator" />.
		/// </summary>
		/// <param name="min">The minimum size of the file.</param>
		/// <param name="max">The maximum size of the file.</param>
		/// <param name="minUnit">The unit measuring the minimum file size.</param>
		/// <param name="maxUnit">The unit measuring the maximum file size.</param>
		/// <param name="message">The custom error message for this validation.</param>
		public FileSizeValidator(int? min, int? max, FileSizeUnit minUnit = FileSizeUnit.Byte, FileSizeUnit maxUnit = FileSizeUnit.Byte, string message = null)
		{
			this.Min = GetCalculatedByteSize(min, minUnit);
			this.Max = GetCalculatedByteSize(max, maxUnit);
			this.Message = message;
		}
		public object CreateValidator()
		{
			return new { assetFileSize = new { min = this.Min, max = this.Max }, message = this.Message };
		}

		protected virtual int? GetCalculatedByteSize(int? value, FileSizeUnit unit)
		{
			if (value != null)
			{
				if (unit == FileSizeUnit.KB)
					value = value * 1000;
				if (unit == FileSizeUnit.MB)
					value = value * 1048576;
			}
			return value;
		}
	}
}