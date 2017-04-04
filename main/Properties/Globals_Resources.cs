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
	/// <remarks>
	/// This string constant is used to load resources
	/// </remarks>
	const string DefaultNamespaceName = "DefaultNamespaceName";

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
