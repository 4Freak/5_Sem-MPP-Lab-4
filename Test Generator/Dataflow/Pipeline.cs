using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks.Dataflow;
using TestGeneratorLib;

namespace Dataflow
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
			
			var processFiles = new TransformBlock<FileContent, List<FileContent>>(
				sourceFileContent => ProcessFilesContents(sourceFileContent),
				new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = _pipelineConfiguration.MaxProcessingTask });

			var writeFiles = new ActionBlock<List<FileContent>>(
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

		private List<FileContent> ProcessFilesContents(FileContent sourceFileContent)
		{
			var testContents = _testGenerator.Generate(sourceFileContent.Content);
			var result = new List<FileContent>();
			foreach(var testContent in testContents)
			{
				var resultPath = sourceFileContent.Path.Insert(sourceFileContent.Path.LastIndexOf("."), "_" + testContent.NamespaceName + "_" + testContent.ClassName);
				result.Add(new FileContent(resultPath, testContent.Content));
			}
			return result;
		}

		private async Task WriteFile (List<FileContent> fileContents, string resultDirectory)
		{
			if (!Directory.Exists(resultDirectory))
			{
				Directory.CreateDirectory(resultDirectory);
			}
			foreach (var fileContent in fileContents)
			{
				var resultFilePath =  resultDirectory + Path.DirectorySeparatorChar + fileContent.Path.Substring(fileContent.Path.LastIndexOf(Path.DirectorySeparatorChar) + 1);
				using (var sw = new StreamWriter(resultFilePath))
				{
					await sw.WriteAsync(fileContent.Content);	
				}
			}
		}
	}
}
