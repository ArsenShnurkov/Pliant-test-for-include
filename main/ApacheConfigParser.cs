using System;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Eto.Parse;
using Eto.Parse.Grammars;
using System.Drawing;

partial class Globals
{
	const string cachefilename = "cache.txt";
	static void Main(string[] args)
	{
		try
		{
			//if (File.Exists(cachefilename) == false)
			{
				string apacheConfigFileName = ConfigurationManager.AppSettings["ApacheConfigFileName"].ToString();
				var match = LoadConfig(apacheConfigFileName).Value;

				string full_text = GetFullText(match);
				//Console.WriteLine(full_text);
				File.WriteAllText("full_text.txt", full_text);

				var sb_short = new StringBuilder();
				var matches = match.Find("other_directive", true);
				foreach (var m in matches)
				{
					sb_short.AppendLine(GetShortText(m));
				}
				string shortened_content = sb_short.ToString();
				File.WriteAllText(cachefilename, shortened_content);
			}
			string short_text = File.ReadAllText(cachefilename);
			var res = ExtractPairsFromPreparsedText(short_text);
			Console.WriteLine(res.Count);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
	}

	/// <remarks>
	/// Считывает файл, разбирает по упрощённой грамматике и возвращает пару ("имя файла", дерево разбора)
	/// </remarks>
	static public KeyValuePair<string, Match> LoadConfig(string configName)
	{
		if (File.Exists(configName) == false)
		{
			throw new FileNotFoundException(configName);
		}
		/*
		 * Выводим название файла. Хорошо бы, наверное, выводить не в stdout на консоль, а в специальный файл?
		 * Контексты для кого придумали в C# ?
		*/
		Context.WriteFileName(configName);
		var originalContent = File.ReadAllText(configName, Encoding.UTF8);

		var match = IncludesGrammar.Match(originalContent);
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

			Point pt = GetLineNumberAndPosition(originalContent, include_directive.Index);
			Context.WriteInclusion(include_directive.Text, configName, pt.X, pt.Y);
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

	public static List<KeyValuePair<string, string>> ExtractPairsFromPreparsedText(string originalContent)
	{
		var res = new List<KeyValuePair<string, string>>();
		var fileContent = LoadFromResource(DefaultNamespaceName, "Grammar", "syntax10.ebnf");

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

		var match = parser.Match(originalContent);
		if (match.Success == false)
		{
			throw new FormatException($"Error when reparsing {match.ErrorMessage}");
		}
		var m = match.Matches.Find("virtual_host_section", true);
		return res;
	}

	static public IEnumerable<KeyValuePair<string, Match>> LoadConfigsByMask(string mask, string path)
	{
		var res = new List<KeyValuePair<string, Match>>();
		string[] files = Directory.GetFiles(path, mask); // can throw exception if mask contains wrong path
		if (files.Length == 0 && mask.Contains("*") == false)
		{
			throw new FileNotFoundException(mask);
		}
		Array.Sort(files, StringComparer.InvariantCulture); // see https://superuser.com/questions/705297/in-what-order-does-apache-load-conf-files-and-which-ones
		for (int i = 0; i < files.Length; ++i)
		{
			var filename = files[i];
			var pair = LoadConfig(filename);
			res.Add(pair);
		}
		return res;
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
		if (string.Compare(node.Name, "section_start") == 0 && string.Compare(node.Text, "section_end") == 0)
		{
			Debugger.Break();
		}
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
