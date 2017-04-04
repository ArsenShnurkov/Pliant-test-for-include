using System;
using System.Diagnostics;
using System.IO;

public class Context
{
	StreamWriter fs;
	public Context()
	{
		fs = new StreamWriter("list_of_files.log.txt");
	}
	public void WriteFileName(string filename)
	{
		fs.WriteLine("Loadind file " + filename);
		fs.Flush();
	}
	public void WriteInclusion(string textOfDirective, string sourceFile, int line, int position)
	{
		FileInfo fi = new FileInfo(sourceFile);
		fs.WriteLine("Include directive encountered at " + fi.Name + $" ({line},{position})");
		fs.WriteLine("Text: " + textOfDirective);
		fs.Flush();
	}
}

partial class Globals
{
	static Context context = new Context();
	public static Context Context
	{
		get
		{
			return context;
		}
	}
}
