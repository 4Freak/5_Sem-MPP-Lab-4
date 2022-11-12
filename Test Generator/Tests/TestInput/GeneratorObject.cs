using System.Reflection;

namespace Faker.Generators
{
	public class GeneratorObject
	{
		private readonly IFaker _faker;

		public GeneratorObject(IFaker faker)
		{
			_faker = faker;
		}

		public object CreateObject(Type type)
		{
			var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
				.OrderByDescending(j => j.GetParameters().Length)
				.ToList();
			
			foreach (var constructor in constructors)
			{
				try
				{
					var arguments = constructor.GetParameters()
                        .Select(i => i.ParameterType)
						.Select(_faker.Create)
                        .ToArray();

					return constructor.Invoke(arguments);
				}
				catch
				{}
			}

			if (type.IsValueType)
			{
				return Activator.CreateInstance(type);
			}

			throw new FakerException("Cannot use public constructors");
		}
		public object FillObject(object item)
		{
			var fields = item.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
			foreach (var field in fields)
			{
				if (Equals(field.GetValue(item), GetDefaultValue(field.FieldType)))
				{
					field.SetValue(item, _faker.Create(field.FieldType));
				}
			}

			var properties = item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach (var property in properties)
			{
				if (Equals(property.GetValue(item), GetDefaultValue(property.PropertyType)))
				{
					property.SetValue(item, _faker.Create(property.PropertyType));
				}				
			}
			return item;
		}

		public static object GetDefaultValue(Type type)
		{
			if (type.IsValueType)
			{
				return Activator.CreateInstance(type);
			}
			else
			{
				return null;
			}
		}
	}
}
