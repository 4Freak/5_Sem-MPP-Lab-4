namespace FileScopedFaker;

	public class GeneratorByte : IValueGenerator
	{
	    
		private const int _minByte = 1;
		public Type GeneratedType {get; } = typeof(byte);
		
		public object Generate(Type typeToGenerate, GeneratorContext context)
		{
			Byte [] buffer = new Byte[1];
			context.Random.NextBytes(buffer);
			return buffer[0];
		}

		public bool CanGenerate(Type type)
		{
			return type == GeneratedType;
		}
	}

