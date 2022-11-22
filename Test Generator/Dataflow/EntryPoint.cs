namespace Dataflow
{
	public class EntryPoint
	{
		private static int _idEndFilePaths;
		private static int _idResultDirectory;
		private static int _idMaxReadingTask;
		private static int _idMaxProcessingTask;
		private static int _idMaxWritingTask;
		public static async Task Main(string[] args)
		{
			if (args.Length == 0) 
			{
				args = new string[] {
					"../../../../Tests/TestInput/GeneratorBool.cs",
					"../../../../Tests/TestInput/GeneratorByte.cs",
					"../../../../Tests/TestOutput",
					"4",
					"4",
					"4"};
			}
			
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
			
			Console.WriteLine("End. Press Enter");
			Console.ReadLine();
		}
	}
}
