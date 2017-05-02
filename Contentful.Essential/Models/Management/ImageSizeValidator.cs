using Contentful.Core.Models.Management;

namespace Contentful.Essential.Models.Management
{
	public class ImageSizeValidator : IFieldValidator
	{
		protected const int BYTES_IN_KB = 1024;
		protected const int BYTES_IN_MB = 1048576;
		/// <summary>
		/// The minimum allowed width of the image (in px).
		/// </summary>
		public int? MinWidth { get; set; }

		/// <summary>
		/// The maximum allowed width of the image (in px).
		/// </summary>
		public int? MaxWidth { get; set; }
		/// <summary>
		/// The minimum allowed height of the iamge (in px).
		/// </summary>
		int? MinHeight { get; set; }

		/// <summary>
		/// The maximum allowed height of the image (in px).
		/// </summary>
		public int? MaxHeight { get; set; }
		/// <summary>
		/// The custom error message that should be displayed.
		/// </summary>
		public string Message { get; set; }


		/// <summary>
		/// Initializes a new instance of <see cref="T:Contentful.Essential.Models.Management.ImageSizeValidator" />.
		/// </summary>
		/// <param name="minWidth">The minimum width of the image.</param>
		/// <param name="maxWidth">The maximum width of the image.</param>
		/// <param name="minHeight">The minimum height of the image.</param>
		/// <param name="maxHeight">The maximum height of the image.</param>
		/// <param name="message">The custom error message for this validation.</param>
		public ImageSizeValidator(int? minWidth, int? maxWidth, int? minHeight, int? maxHeight, string message = null)
		{
			this.MinWidth = minWidth;
			this.MaxWidth = maxWidth;
			this.MinHeight = minHeight;
			this.MaxHeight = maxHeight;
			this.Message = message;
		}
		public object CreateValidator()
		{
			return new { assetImageDimensions = new { width = new { min = this.MinWidth, max = this.MaxWidth }, height = new { min = this.MinHeight, max = this.MaxHeight } }, message = this.Message };
		}
	}
}