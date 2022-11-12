namespace Faker.Generators
{
	internal class GeneratorFloat : IValueGenerator
	{
		public Type GeneratedType {get; } = typeof(float);
		
		public object Generate(Type typeToGenerate, GeneratorContext context)
		{
			return (float)context.Random.Next(1, int.MaxValue) / int.MaxValue * 4;
		}

		public bool CanGenerate(Type type)
		{
			return type == GeneratedType;
		}
	}
}
