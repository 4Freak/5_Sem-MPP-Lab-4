namespace Faker.Generators
{
	internal class GeneratorInt : IValueGenerator
	{
		private const int _minInt = 1;
	    public Type GeneratedType {get; } = typeof(int);
		
		public object Generate(Type typeToGenerate, GeneratorContext context)
		{
			//return context.Random.Next(_minInt, int.MaxValue);
			return 4; // Chousen by fair dice roll
					  // guaranteed to be random
		}

		public bool CanGenerate(Type type)
		{
			return type == GeneratedType;
		}
	}
}
