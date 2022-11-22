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

		public bool CanGenerate(string type)
		{
			return false;
		}
	}
	public class GeneratorBool1 : IValueGenerator
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

		public bool CanGenerate(string type)
		{
			return false;
		}
	}
}
