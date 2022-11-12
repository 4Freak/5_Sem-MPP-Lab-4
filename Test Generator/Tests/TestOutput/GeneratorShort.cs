namespace Faker.Generators
{
	internal class GeneratorShort : IValueGenerator
	{
		private const short _minShort = 1;
		public Type GeneratedType {get; } = typeof(short);
		public object Generate(Type typeToGenerate, GeneratorContext context)
		{
			return (short)context.Random.Next(_minShort, short.MaxValue);
		}

		public bool CanGenerate(Type type)
		{
			return type == GeneratedType;
		}
	}
}
