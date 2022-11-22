namespace TestGeneratorLib
{
	public class TestContent
	{
		public string NamespaceName;
		public string ClassName;
		public string Content { get; }

		public TestContent(string namespaceName, string className, string content)
		{
			NamespaceName = namespaceName.Replace("\r\n", "");
			ClassName = className.Replace("\r\n", "");
			Content = content;
		}
	}
}
