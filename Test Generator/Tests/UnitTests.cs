using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Dataflow;

namespace Tests
{
	public class Tests
	{
		private int _idEndFilePaths;
		private int _idResultDirectory;
		private int _idMaxWritingTask;
		private int _idMaxProcessingTask;
		private int _idMaxReadingTask;

		private string[] args;

		private int _classesCount = 4;

		[SetUp]
		public async Task Setup ()
		{
			
			args = new string[] {
				"../../../../Tests/TestInput/FileSeeker.cs",			// Does not exist
				"../../../../Tests/TestInput/ManyClasses.cs",			// Many classes inside
				"../../../../Tests/TestInput/ManyNamespaces.cs",		// Many namespaces inside
				"../../../../Tests/TestInput/NoMethods.cs",				// No methods inside
				"../../../../Tests/TestInput/FileScopedNamespace.cs",   // FileScoped namespace inside
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
		}

		[Test]
		public void TestFileCount ()
		{
			var dirInfo = new DirectoryInfo(args[_idResultDirectory]);
			var filesConut = dirInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly).Length;
			
			Assert.That(filesConut, Is.EqualTo(_classesCount));
		}

		[Test]
		public void TestEmptyFiles()
		{
			var dirInfo = new DirectoryInfo(args[_idResultDirectory]);
			var files = dirInfo.GetFiles();
			foreach (var file in files)
			{
				if (file.Name == "FileSeeker.cs" ||
					file.Name == "NoMethods.cs")
				{
					Assert.Fail();
				}
			}
			Assert.Pass();
		}

		[Test]
		public void TestFileContent()
		{
			string sourceStr;
			using (var sr = new StreamReader("../../../../Tests/TestOutput/ManyClasses_GeneratorBool.cs"))
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
				Assert.That(methods[1].Identifier.Text, Is.EqualTo("CanGenerate_1_Test"));
				Assert.That(methods[2].Identifier.Text, Is.EqualTo("Generate_Test"));
			
				// Check for no parameters
				foreach (var method in methods)
				{
					Assert.That(method.ParameterList.Parameters.Count, Is.EqualTo(0));
				} 
			});
		}
	}
}