using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;



namespace TestGeneratorLib
{
	public class TestGenerator
	{
		public List<TestContent> Generate(string sourceStr)
		{
			var result = new List<TestContent>();

			var tree = CSharpSyntaxTree.ParseText(sourceStr);
			if (tree == null)
			{
				return result;
			}
			var root = tree.GetCompilationUnitRoot();

			var sourceNamespaces = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
			var sourceFileScopedNamespaces = root.DescendantNodes().OfType<FileScopedNamespaceDeclarationSyntax>();
			bool isHasFilescopedNamespace = false;
			FileScopedNamespaceDeclarationSyntax sourceFileScopedNamespace = null;
			foreach(var sfsn in sourceFileScopedNamespaces)
			{
				if (sfsn != null)
				{
					sourceFileScopedNamespace = FileScopedNamespaceDeclaration(QualifiedName((sfsn).Name, IdentifierName("Tests")));
					isHasFilescopedNamespace = true;
				}
			}

			var sourceUsings = root.DescendantNodes().OfType<UsingDirectiveSyntax>();
			var resultUsings = new SyntaxList<UsingDirectiveSyntax>(sourceUsings)
				.AddRange(sourceNamespaces.Select(GetUsingsFromNamespaces))
				.Add(UsingDirective(ParseName("System")))
				.Add(UsingDirective(ParseName("System.Generic.Collections")))
				.Add(UsingDirective(ParseName("System.Linq")))
				.Add(UsingDirective(ParseName("System.Text")))
				.Add(UsingDirective(ParseName("NUnit.Framework")));

			var resultClasses = root.DescendantNodes().OfType<ClassDeclarationSyntax>()
				.Where (resultClass => resultClass.Modifiers.Any(SyntaxKind.PublicKeyword)).ToList();

			foreach (var resultClass in resultClasses)
			{
				var resultMembers = AssembleNamespaces(resultClass, isHasFilescopedNamespace);

				string namespaceName;
				CompilationUnitSyntax resultUnit = null;
				if (isHasFilescopedNamespace)
				{
					resultUnit = CompilationUnit()
						.WithMembers(SingletonList<MemberDeclarationSyntax>(sourceFileScopedNamespace
						.WithUsings(resultUsings)
						.AddMembers(resultMembers)));
						
					namespaceName = sourceFileScopedNamespace.Name.ToString();
				}
				else
				{
					resultUnit = CompilationUnit()
						.WithUsings(resultUsings)
						.AddMembers(resultMembers);
					namespaceName = (resultMembers as NamespaceDeclarationSyntax).Name.ToString();
				}

				result.Add(new TestContent(namespaceName,
											resultClass.Identifier.Text, 
											resultUnit.NormalizeWhitespace().ToFullString()));;			
			}
			return result;
		}
		private UsingDirectiveSyntax GetUsingsFromNamespaces (NamespaceDeclarationSyntax namespaceDeclaration)
		{
			return UsingDirective(namespaceDeclaration.Name);
		}

		private MemberDeclarationSyntax AssembleNamespaces (ClassDeclarationSyntax classDeclaration, bool isHasFileScopedNamespace)
		{
			// Check for filescoped namespaces
			var resultClass = AssembleClass(classDeclaration);
			if (isHasFileScopedNamespace)
			{
				return resultClass;
			}

			NamespaceDeclarationSyntax currNamespace = classDeclaration.Parent as NamespaceDeclarationSyntax;
			NamespaceDeclarationSyntax resultNamespaces;
			var classNamespaces = new List<NamespaceDeclarationSyntax>();	

			// Get all namespaces
			while (currNamespace != null)
			{
				classNamespaces.Add(NamespaceDeclaration(QualifiedName((currNamespace).Name, IdentifierName("Tests"))));
				currNamespace = currNamespace.Parent as NamespaceDeclarationSyntax;
			}

			if (classNamespaces.Count == 0)
			{
				classNamespaces.Add(NamespaceDeclaration(IdentifierName("Tests")));
			}

			classNamespaces[0] = classNamespaces[0].AddMembers(resultClass);

			// If more than 1 namespace than add it
			for (int i = 1; i < classNamespaces.Count; i++)
			{
				classNamespaces[i] = classNamespaces[i].AddMembers(classNamespaces[i-1]);
			}
			resultNamespaces = classNamespaces[classNamespaces.Count-1];

			return resultNamespaces;
		}

		private MemberDeclarationSyntax AssembleClass(ClassDeclarationSyntax classDeclaration)
		{
			var resultAttribute = SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("TestFixture")))));
			
			var resultModifiers = TokenList(Token(SyntaxKind.PublicKeyword));
			
			var resultMethods = AssembleMethods(classDeclaration);

			var resultClass = ClassDeclaration(classDeclaration.Identifier.Text + "_Test")
				.WithAttributeLists(resultAttribute)
				.WithModifiers(resultModifiers)
				.AddMembers(resultMethods);
			
			return resultClass;
		}

		private MemberDeclarationSyntax[] AssembleMethods(SyntaxNode syntaxNode)
		{
			var resultAttribute = SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("Test")))));
			
			var resultModifiers = TokenList(Token(SyntaxKind.PublicKeyword));

			var resultReturnedType = PredefinedType(Token(SyntaxKind.VoidKeyword));

			var resultBody = Block(ExpressionStatement(InvocationExpression(MemberAccessExpression(
				SyntaxKind.SimpleMemberAccessExpression,IdentifierName("Assert"), IdentifierName("Fail")))
				.WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("autogenerated"))))))));
			
			var resultMembers = new List<MemberDeclarationSyntax>();
			var sourceMethods = syntaxNode.DescendantNodes().OfType<MethodDeclarationSyntax>()
				.Where(sourceMethod => sourceMethod.Modifiers.Any(SyntaxKind.PublicKeyword)).ToList();

			sourceMethods.Sort((method1, method2) => string.Compare(method1.Identifier.Text, method2.Identifier.Text, StringComparison.Ordinal));

			int i = 0;
			string prevId = sourceMethods[0].Identifier.Text;
			while (i < sourceMethods.Count)
			{
				int overloadCount = 0;
				while (	(i < sourceMethods.Count) &&
						((overloadCount == 0) || (sourceMethods[i].Identifier.Text == prevId)))
				{
					string overloadID = sourceMethods[i].Identifier.Text + (overloadCount == 0 ? "" :"_" + overloadCount.ToString()) + "_Test";
					
					resultMembers.Add(MethodDeclaration(resultReturnedType, overloadID)
						.WithAttributeLists(resultAttribute)
						.WithModifiers(resultModifiers)
						.WithBody(resultBody));

					overloadCount++;
					prevId = sourceMethods[i].Identifier.Text;					
					i++;
				}
			}
			
			return resultMembers.ToArray();
		}
	}
}
