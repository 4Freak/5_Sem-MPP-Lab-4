namespace Faker.Generators
{
	public class GeneratorBool : IValueGenerator
	{
		public Type GeneratedType {get; } = typeof(bool);
		
		public object Generate(Type typeToGenerate, GeneratorContext context)
		{
			return true;
		}

		public bool CanGenerate(Type type)
		{
			return type == GeneratedType;
		}
	}
}
