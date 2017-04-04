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
	public static string GetFullText(Match node)
	{
		if (node.HasMatches)
		{
			var sb = new StringBuilder();
			foreach (var m in node.Matches)
			{
				sb.Append(GetFullText(m));
			}
			return sb.ToString();
		}
		else
		{
			return node.Text;
		}
	}

	public static string GetShortText(Match node)
	{
		if (string.Compare(node.Name, "glue") == 0
		   || string.Compare(node.Name, "ows") == 0
		   || string.Compare(node.Name, "iws") == 0)
		{
			return " ";
		}
		if (node.HasMatches)
		{
			var sb = new StringBuilder();
			foreach (var m in node.Matches)
			{
				sb.Append(GetShortText(m));
			}
			return sb.ToString();
		}
		else
		{
			return node.Text;
		}
	}
}
