using System;
using System.IO;
using System.Reflection;
using System.Text;
using Pliant.Ebnf;
using Pliant.Forest;
using Pliant.Runtime;
using Pliant.Tree;

class DefaultNamespaceName
{
	public static void Main(string[] args)
	{
		string grammarText = LoadFromResource(nameof(DefaultNamespaceName), "Grammar", "syntax2.ebnf");
		//string input = File.ReadAllText("/etc/apache2/httpd.conf", Encoding.UTF8);
		string input = "_0_1_0_0_1_1_";

 		var definition = new EbnfParser().Parse(grammarText);
		var grammar = new EbnfGrammarGenerator().Generate(definition);
		var parseEngine = new ParseEngine(grammar);
		var parseRunner = new ParseRunner(parseEngine, input);

		var recognized = false;
		var errorPosition = 0;
		while (!parseRunner.EndOfStream())
		{
			recognized = parseRunner.Read();
			if (!recognized)
			{
				errorPosition = parseRunner.Position;
				break;
			}
		}

		var accepted = false;
		if (recognized)
		{
			accepted = parseRunner.ParseEngine.IsAccepted();
			if (!accepted)
				errorPosition = parseRunner.Position;
		}
		Console.WriteLine($"Recognized: {recognized}, Accepted: {accepted}");
		if (!recognized || !accepted)
			Console.Error.WriteLine($"Error at position {errorPosition}");

		// get the parse forest root from the parse engine
		var parseForestRoot = parseEngine.GetParseForestRootNode();

		// create a internal tree node and supply the disambiguation algorithm for tree traversal.
		var parseTree = new InternalTreeNode(
			parseForestRoot,
			new SelectFirstChildDisambiguationAlgorithm());

		Console.WriteLine(parseTree.ToString());
	}
	public static string LoadFromResource(string default_namespace, string folder, string name_of_file)
	{
		// Resource ID: mptgitmodules.Resources.syntax4.ebnf
		var fullname = string.Format("{0}.{1}.{2}", default_namespace, folder, name_of_file);
		using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullname))
		{
			return LoadFromStream(s);
		}
	}
	public static string LoadFromStream(Stream s)
	{
		using (var sr = new StreamReader(s))
		{
			return LoadFromTextReader(sr);
		}
	}
	public static string LoadFromTextReader(TextReader sr)
	{
		var fileContent = sr.ReadToEnd();
		return fileContent;
	}
}
