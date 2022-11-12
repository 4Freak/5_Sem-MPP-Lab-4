namespace Faker.Generators
{
	internal class GeneratorLong : IValueGenerator
	{
		private const long _minLong = 1;
		public Type GeneratedType {get; } = typeof(long);
		
		public object Generate(Type typeToGenerate, GeneratorContext context)
		{
			return context.Random.NextInt64(_minLong, long.MaxValue);
		}

		public bool CanGenerate(Type type)
		{
			return type == GeneratedType;
		}
	}
}
