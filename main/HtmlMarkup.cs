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
using System.CodeDom.Compiler;

partial class Globals
{
	public static string GetHtml(Match node, string title)
	{
		var sb = new StringBuilder();
		using (TextWriter tw1 = new StringWriter(sb))
		{
			using (IndentedTextWriter writer = new IndentedTextWriter(tw1))
			{
				writer.Write($"<!DOCTYPE html><html>");
				writer.Write($"<head><title>{title}</title></head>");
				writer.Write($"<body>");
				writer.Indent++;
				var currentPath = new Stack<string>();
				GetHtmlMarkup(node, writer, currentPath);
				writer.Indent--;
				writer.Write($"</body></html>");
			}
		}
		return sb.ToString();
	}
	public static void GetHtmlMarkup(Match node, IndentedTextWriter writer, Stack<string> currentPath)
	{
		if (string.Compare(node.Name, "newline") == 0
		   || string.Compare(node.Name, "cr") == 0
		   || string.Compare(node.Name, "lf") == 0)
		{
			writer.WriteLine($"<br title=\"{GetTitle(node)}\">");
			return;
		}

		if (string.Compare(node.Name, "comment") == 0)
		{
			writer.Write($"<span title=\"{EscapeForTitle(GetFullTitle(currentPath))}\">");
			writer.WriteLine(EscapeForHtml(node.Text));
			writer.Write("</span>");
			return;
		}

		currentPath.Push(GetTitle(node));
		writer.WriteLine($"<span title=\"{EscapeForTitle(GetFullTitle(currentPath))}\">");
		if (node.HasMatches )
		{
			writer.Indent++;
			foreach (var m in node.Matches)
			{
				GetHtmlMarkup(m, writer, currentPath);
			}
			writer.Indent--;
		}
		else
		{
			writer.WriteLine(EscapeForHtml(node.Text));
		}
		writer.WriteLine("</span>");
		currentPath.Pop();
	}

	static string GetTitle(Match node)
	{
		var sb = new StringBuilder();
		sb.Append($"{node.Name} {node.Index}..{node.Index+node.Length}");
		return sb.ToString();
	}
	static string GetFullTitle(Stack<string> path)
	{
		var sb = new StringBuilder();
		foreach (string s in path)
		{
			sb.Append(s);
			sb.Append(Environment.NewLine);
		}
		return sb.ToString();
	}

	public static string EscapeForHtml(string text)
	{
		var sb = new StringBuilder(text.Length*2);
		for (int i = 0; i < text.Length; ++i)
		{
			switch (text[i])
			{
				case '"': sb.Append("&quot;"); break;
				case '\'': sb.Append("&apos;"); break;
				case '<': sb.Append("&lt;"); break;
				case '>': sb.Append("&gt;"); break;
				case '&': sb.Append("&amp;"); break;
				default:sb.Append(text[i]); break;
			}
		}
		return sb.ToString();
	}
	public static string EscapeForTitle(string text)
	{
		var sb = new StringBuilder(text.Length * 2);
		for (int i = 0; i < text.Length; ++i)
		{
			switch (text[i])
			{
				case '"': sb.Append("&quot;"); break;
				case '\'': sb.Append("&apos;"); break;
				case '<': sb.Append("&lt;"); break;
				case '>': sb.Append("&gt;"); break;
				case '&': sb.Append("&amp;"); break;
				case (char)10: sb.Append("&#10;"); break;
				case (char)13: sb.Append("&#13;"); break;
				default: sb.Append(text[i]); break;
			}
		}
		return sb.ToString();
	}
}
