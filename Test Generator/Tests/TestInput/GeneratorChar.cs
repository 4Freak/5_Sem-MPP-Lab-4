namespace Faker.Generators
{
	public class GeneratorChar : IValueGenerator
	{
		private const int _minChar = 1;
		private const int _maxChar = 26;
	
		public Type GeneratedType {get; } = typeof(char);
		
		public object Generate(Type typeToGenerate, GeneratorContext context)
		{
			return (char)(context.Random.Next(_minChar, _maxChar) + (int)'a');		
		}

		public bool CanGenerate(Type type)
		{
			return type == GeneratedType;
		}
	}
}
