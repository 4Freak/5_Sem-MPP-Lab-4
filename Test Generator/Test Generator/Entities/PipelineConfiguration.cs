namespace Test_Generator
{
	public class PipelineConfiguration
	{
		
		public int	MaxReadingTask {get; }
		public int MaxProcessingTask {get; }
		public int MaxWritingTask {get; }

		public PipelineConfiguration(int maxReadingTask, int maxProcessingTask, int maxWritingTask)
		{
			MaxReadingTask = maxReadingTask;
			MaxProcessingTask = maxProcessingTask;
			MaxWritingTask = maxWritingTask;
		}
	}
}
