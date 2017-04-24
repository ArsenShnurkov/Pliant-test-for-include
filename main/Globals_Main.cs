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

/*class Publication
{
	public string company;
	public int year;
}

interface IPublicable
{
	string Author { get; }
}

struct Book : IPublicable
{
	public Publication FirstPublication;
	public List<Publication> AllPublications;
	string author;

	string IPublicable.Author
	{
		get
		{
			return author;
		}
	}
}

class BookCover
{
	Book Content;
}

struct BookInfo
{
	string Title;
}

static class SimpleBookExtensions
{
	public static void Print(this BookInfo info)
	{
	}
	public static void Print(ref this BookInfo info)
	{
	}
}*/

partial class Globals
{

	const string cachefilename = "cache.txt";
	static void Main(string[] args)
	{
		try
		{
			/*
			var atm = AvotTestGrammar.Match("000111000");
			if (atm.Success == false)
			{
				throw new FormatException($"Error when reparsing {atm.ErrorMessage}");
			}
			var v_atm = atm.Find("v_directive", true);
			*/

			if (File.Exists(cachefilename) == false)
			{
				string apacheConfigFileName = Path.Combine(ConfigurationManager.AppSettings["ApacheConfigFilePath"].ToString(),"httpd.conf");;
				var match = LoadConfig(apacheConfigFileName).Value;

				string html_text = GetHtml(match, "test1");
				File.WriteAllText("parse_markup.htm", html_text);

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
		Context.WriteFileName(configName);
		var originalContent = File.ReadAllText(configName, Encoding.UTF8);

		var match = IncludesGrammar.Match(originalContent);
		if (match.Success == false)
		{
			throw new FormatException($"invalid format of file {configName}");
		}

		// Getting ServerRoot directive
		string path = null;
		var ServerRootMatches = match.Find("serverroot_directive", true);
		foreach (var ServerRootDirective in ServerRootMatches)
		{
			if (path == null)
			{
				path = ServerRootDirective["include_path"].Matches[0].Text;
				if (path.StartsWith("\"", StringComparison.InvariantCulture) && path.EndsWith("\"", StringComparison.InvariantCulture))
				{
					path = path.Substring(1, path.Length - 2);
				}
			}
			else
			{
				throw new FormatException("several ServerRoot entries encountered");
			}
		}
		if (path != null)
		{
			ServerRoot = path;
		}
		Trace.WriteLine($"ServerRoot={path}");

		new TreeCleaner().RemoveExtraNodes(match);

		// Здесь надо проанализировать распарсенное, извлечь маски и повызывать LoadConfigsByMask
		var list = match.Find("include_directive", true);
		foreach (var include_directive in list)
		{
			int index = match.Matches.FindIndex(m => m.Index == include_directive.Index);

			Point pt = GetLineNumberAndPosition(originalContent, include_directive.Index);
			Context.WriteInclusion(include_directive.Text, configName, pt.X, pt.Y);
			string mask = include_directive["include_path"].Text;
			// читаем файлы
			var content = LoadConfigsByMask(mask, ServerRoot);

			// match.Matches[index].Tag = content; // Простановка тега почему-то не срабатывает.
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
		var watch = System.Diagnostics.Stopwatch.StartNew();
		var match = MonoGrammar.Match(originalContent);
		// the code that you want to measure comes here
		watch.Stop();
		var elapsedMs = watch.ElapsedMilliseconds;
		Console.WriteLine($"parse time is {elapsedMs}");
		if (match.Success == false)
		{
			throw new FormatException($"Error when reparsing {match.ErrorMessage}");
		}
		new TreeCleaner().RemoveExtraNodes(match);
		var m = match.Matches.Find("a_section", true);
		foreach (var virthost in m)
		{
			var servername = virthost.Find("v_instruction_servername", true);
			var addmonoapplications = virthost.Find("v_instruction_addmonoapplications", true);
		}
		return res;
	}

	static public IEnumerable<KeyValuePair<string, Match>> LoadConfigsByMask(string mask, string path)
	{
		var res = new List<KeyValuePair<string, Match>>();
		string separator = new string(Path.DirectorySeparatorChar, 1);
		if (mask.StartsWith(separator, StringComparison.InvariantCulture) == false)
		{
			mask = Path.Combine(path, mask);
		}
		path = Path.GetDirectoryName(mask);
		mask = mask.Substring(path.Length);
		if (mask.StartsWith(separator, StringComparison.InvariantCulture))
		{
			mask = mask.Substring(separator.Length);
		}
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
}
