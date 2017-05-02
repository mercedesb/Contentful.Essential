using Contentful.Essential.Models;
using StructureMap;

namespace Contentful.Essential.Application
{
	/// <summary>
	///     Service locator bootstrapper
	/// </summary>
	public class Bootstrapper : Registry
	{
		/// <summary>
		///     Instantiates a new instance of the <see cref="Bootstrapper" /> class.
		/// </summary>
		public Bootstrapper()
		{
			Scan(
				scan =>
				{
					scan.TheCallingAssembly();
					scan.WithDefaultConventions();
					scan.AddAllTypesOf<IEntry>();
					scan.AddAllTypesOf<IHaveFieldValidation>();
				});

			For<IContentTypeBuilder>().Use<DefaultContentTypeBuilder>();

			For(typeof(IRepository<>)).Use(typeof(BaseContentRepository<>));
		}
	}
}