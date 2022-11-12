using System.Text;

namespace Faker.Generators
{
	internal class GeneratorString : IValueGenerator
	{
		private const int _minLen = 1;
		private const int _maxLen = 20;

		private const int _minVal = (int)'a';
		private const int _maxVal = (int)'z';

		public Type GeneratedType {get; } = typeof(string);
		
		public object Generate(Type typeToGenerate, GeneratorContext context)
		{
			var rndStr = new StringBuilder();
			int Len = context.Random.Next(_minLen, _maxLen);
			for (int i = 0; i < Len; i++)
			{
				rndStr.Append((char)context.Random.Next(_minVal, _maxVal));
			}
			return rndStr.ToString();
		}

		public bool CanGenerate(Type type)
		{
			return type == GeneratedType;
		}
	}
}
