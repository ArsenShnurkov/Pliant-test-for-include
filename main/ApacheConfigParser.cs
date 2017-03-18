using System;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Eto.Parse;
using Eto.Parse.Grammars;

class DefaultNamespaceName
{
	const string nameOfTheStartingRule = "file";
	static public KeyValuePair<string, Match> LoadConfig(string configName)
	{
		var fileContent = LoadFromResource(nameof(DefaultNamespaceName), "Grammar", "syntax8.ebnf");

		EbnfStyle style = (EbnfStyle)(
			(uint)EbnfStyle.Iso14977
			//& ~(uint) EbnfStyle.WhitespaceSeparator	
			| (uint)EbnfStyle.EscapeTerminalStrings);

		EbnfGrammar grammar;
		Grammar parser;
		try
		{
			grammar = new EbnfGrammar(style);
			parser = grammar.Build(fileContent, nameOfTheStartingRule);
		}
		catch (Exception ex)
		{
			Trace.WriteLine(ex.ToString());
			/*
			System.ArgumentException: the topParser specified is not found in this ebnf
			  at Eto.Parse.Grammars.EbnfGrammar.Build (System.String bnf, System.String startParserName) [0x00048] in <filename unknown>:0 
			  at Globals.Main (System.String[] args) [0x0002b] in /var/calculate/remote/distfiles/egit-src/SqlParser-on-EtoParse.git/test1/Program.cs:20 
			*/
			throw;
		}

		var originalContent = File.ReadAllText(configName, Encoding.UTF8);
		var match = parser.Match(originalContent);
		if (match.Success == false)
		{
			throw new FormatException($"invalid format of file {configName}");
		}
		var fi = new FileInfo(configName);
		string path = fi.Directory.FullName;
		// Здесь надо проанализировать аспарсенное, извлечь маски и повызывать LoadConfigsByMask
		var list = match.Find("include_directive");
		foreach (var include_directive in list)
		{
			int index = match.Matches.FindIndex(m => m == include_directive);
			string mask = include_directive["include_path"].Text;
			// читаем файлы
			var content = LoadConfigsByMask(mask, path);
			// заменяем директиву на контент файлов
			match.Matches.RemoveAt(index);
			foreach (var p in content) // вставляем несколько узлов
			{
				var tree = p.Value;
				match.Matches.Insert(index, tree);
				index++;
			}
		}
		// Здесь надо склеить результат
		// А тут мы результат возвращаем (вопрос, можно ли заменить узел в дереве? MatchCollection это List, значит заменить можно)
		return new KeyValuePair<string, Match>(configName, match);
	}

	static public IEnumerable<KeyValuePair<string, Match>> LoadConfigsByMask(string mask, string path)
	{
		var res = new List<KeyValuePair<string, Match>>();
		string[] files = Directory.GetFiles(path, mask);
		for (int i = 0; i < files.Length; ++i)
		{
			var filename = files[i];
			var pair = LoadConfig(filename);
			res.Add(pair);
		}
		return res;
	}

	static void Main(string[] args)
	{
		try
		{
			string apacheConfigFileName = ConfigurationManager.AppSettings["ApacheConfigFileName"].ToString();
			var match = LoadConfig(apacheConfigFileName).Value;

			string full_text = GetFullText(match);
			Console.WriteLine(full_text);

			var sb_short = new StringBuilder();
			var matches = match.Find("other_directive", true);
			foreach (var m in matches)
			{
				sb_short.AppendLine(GetShortText(m));
			}
			string short_text = sb_short.ToString();
			Console.WriteLine(short_text);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
	}

	public static string GetFullText(Match node)
	{
		var sb = new StringBuilder();
		foreach (var m in node.Matches)
		{
			if (string.Compare(m.Name, "include_directive") == 0)
			{
				sb.AppendLine(GetFullText(m));
			}
			else
			{
				sb.AppendLine(m.Text);
			}
		}
		return sb.ToString();
	}

	public static string GetShortText(Match node)
	{
		/*if (string.Compare(node.Name, "other_keyword") == 0 && string.Compare(node.Text, "CustomLog") == 0)
		{
			Debugger.Break();
		}*/
		if (string.Compare(node.Name, "glue") == 0
		   || string.Compare(node.Name, "ows") == 0
		   || string.Compare(node.Name, "iws") == 0)
		{
			return " ";
		}
		if (node.HasMatches)
		{
			StringBuilder buffer = new StringBuilder(node.Length);
			foreach (var m in node.Matches)
			{
				buffer.Append(GetShortText(m));
			}
			return buffer.ToString();
		}
		else
		{
			return node.Text;
		}
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
