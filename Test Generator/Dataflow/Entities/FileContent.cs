namespace Dataflow
{
	public class FileContent
	{
		public string Path { get; }
		public string Content { get; }

		public FileContent(string path, string content) 
		{
			Path = path;
			Content = content;
		}
	}
}
