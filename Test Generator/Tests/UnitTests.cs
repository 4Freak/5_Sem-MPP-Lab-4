using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Dataflow;

namespace Tests
{
	public class Tests
	{
		[SetUp]
		public void Setup ()
		{
		}

		[Test]
		public async Task Test1 ()
		{
			int _idEndFilePaths;
			int _idResultDirectory;
			int _idMaxWritingTask;
			int _idMaxProcessingTask;
			int _idMaxReadingTask;
			
			var args = new string[] {
				"../../../../Tests/TestInput/FileSeeker.cs",
				"../../../../Tests/TestInput/GeneratorBool.cs",
				"../../../../Tests/TestInput/GeneratorByte.cs",
				"../../../../Tests/TestInput/GeneratorChar.cs",
				"../../../../Tests/TestInput/GeneratorDouble.cs",
				"../../../../Tests/TestInput/GeneratorFloat.cs",
				"../../../../Tests/TestInput/GeneratorInt.cs",
				"../../../../Tests/TestInput/GeneratorList.cs",
				"../../../../Tests/TestInput/GeneratorLong.cs",
				"../../../../Tests/TestInput/GeneratorObject.cs",
				"../../../../Tests/TestInput/GeneratorShort.cs",
				"../../../../Tests/TestInput/GeneratorString.cs",
				"../../../../Tests/TestOutput",
				"4",
				"4",
				"4"};

			_idEndFilePaths = args.Length - 5;
			_idResultDirectory = args.Length - 4;
			_idMaxWritingTask = args.Length - 3;
			_idMaxProcessingTask = args.Length - 2;
			_idMaxReadingTask = args.Length - 1;


			for (int i = 0; i < _idResultDirectory+1; i++)
			{
				args[i] = args[i].Replace('/', Path.DirectorySeparatorChar);
				args[i] = args[i].Replace('\\', Path.DirectorySeparatorChar);
			}

			var pipelineConfiguration = new PipelineConfiguration(Int32.Parse(args[_idMaxReadingTask]), Int32.Parse(args[_idMaxProcessingTask]), 
																  Int32.Parse(args[_idMaxWritingTask]));
			var pipeline = new Pipeline(pipelineConfiguration);
			List<string> filePaths = new List<string>();
			for (int i = 0; i < _idEndFilePaths+1; i++)
			{	
				filePaths.Add(args[i]);
			}

			await pipeline.PerformProcessing(filePaths, args[_idResultDirectory]);

			var dirInfo = new DirectoryInfo(args[_idResultDirectory]);
			var filesConut = dirInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly).Length;
			
			Assert.That(filesConut, Is.EqualTo(_idEndFilePaths+1));

			string sourceStr;
			using (var sr = new StreamReader("../../../../Tests/TestOutput/GeneratorBool.cs"))
			{
				sourceStr = sr.ReadToEnd();
			}

			Assert.IsNotNull(sourceStr);
			Assert.IsNotEmpty(sourceStr);
						
			var tree = CSharpSyntaxTree.ParseText(sourceStr);
			var root = tree.GetCompilationUnitRoot();

			var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>()
				.Where (resultClass => resultClass.Modifiers.Any(SyntaxKind.PublicKeyword)).ToList();
			
			var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToArray();
			
			Assert.Multiple( () =>
			{
				Assert.That(classes.Count, Is.EqualTo(1));
				Assert.That(classes[0].Identifier.Text, Is.EqualTo("GeneratorBool_Test"));
				Assert.That(methods.Length, Is.EqualTo(3));
				Assert.That(methods[0].Identifier.Text, Is.EqualTo("CanGenerate_Test"));
				Assert.That(methods[1].Identifier.Text, Is.EqualTo("CanGenerate1_Test"));
				Assert.That(methods[2].Identifier.Text, Is.EqualTo("Generate_Test"));
			});
		}
	}
}