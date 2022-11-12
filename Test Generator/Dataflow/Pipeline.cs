using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using TestGeneratorLib;

namespace Test_Generator
{
	public class Pipeline
	{
		private readonly PipelineConfiguration _pipelineConfiguration;
		private readonly TestGenerator _testGenerator;

		public Pipeline(PipelineConfiguration pipelineConfiguration)
		{
			_pipelineConfiguration = pipelineConfiguration;
			_testGenerator = new TestGenerator();
		}

		public async Task PerformProcessing(IEnumerable<string> filePaths, string resultDirecoty)
		{
			var linkOptions = new DataflowLinkOptions {PropagateCompletion = true };
			
			var readFiles = new TransformBlock<string, FileContent>(
				async path => new FileContent(path, await ReadFileContent(path)),
				new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = _pipelineConfiguration.MaxReadingTask });
			
			var processFiles = new TransformBlock<FileContent, FileContent>(
				sourceFileContent => new FileContent(sourceFileContent.Path, ProcessFileContent(sourceFileContent.Content)),
				new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = _pipelineConfiguration.MaxProcessingTask });

			var writeFiles = new ActionBlock<FileContent>(
				async sourceFileContent => await WriteFile(sourceFileContent, resultDirecoty),
				new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = _pipelineConfiguration.MaxWritingTask });
			
			readFiles.LinkTo(processFiles, linkOptions);
			processFiles.LinkTo(writeFiles, linkOptions);
			
			foreach (var filePath in filePaths)
			{
				readFiles.Post(filePath);
			}

			readFiles.Complete();

			await writeFiles.Completion;
		}

		private async Task<string> ReadFileContent(string filePath)
		{
			try
			{
				string result;
				using (var sr = new StreamReader(filePath))
				{
					result =  await sr.ReadToEndAsync();
				}
				return result;
			}
			catch (FileNotFoundException ex)
			{
				Console.WriteLine($"File not found: {ex.FileName}");
				return "";
			}
		}

		private string ProcessFileContent(string content)
		{
			var testContent = _testGenerator.Generate(content);
			return testContent.Content;
		}

		private async Task WriteFile (FileContent fileContent, string resultDirectory)
		{
			if (!Directory.Exists(resultDirectory))
			{
				Directory.CreateDirectory(resultDirectory);
			}
			var resultFilePath =  resultDirectory + Path.DirectorySeparatorChar + fileContent.Path.Substring(fileContent.Path.LastIndexOf(Path.DirectorySeparatorChar) + 1);
			using (var sw = new StreamWriter(resultFilePath))
			{
				await sw.WriteAsync(fileContent.Content);	
			}
		}
	}
}
