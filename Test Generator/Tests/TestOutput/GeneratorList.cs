using System.Collections;

namespace Faker.Generators
{
	public class GeneratorList : IValueGenerator
	{
		public Type GeneratedType { get; } = typeof(IList);

		public object Generate(Type typeToGenerate, GeneratorContext context)
		{
			var len = context.Random.Next(1, 10);
			IList? list = (IList?)Activator.CreateInstance(typeToGenerate);

			if (list != null)
			{
				for (int i = 0; i < len; i++)
				{
					list.Add(context.Faker.Create(typeToGenerate.GetGenericArguments()[0]));
				}
			}
			return list;
		}

		public bool CanGenerate(Type type)
		{
			var n = nameof(IList);
			bool result = type.GetInterface(n) != null;
			return result;
		}

	}
}
