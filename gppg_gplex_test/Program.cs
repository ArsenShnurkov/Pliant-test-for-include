﻿using System;
using QUT.Gppg;
using System.IO;
using System.Reflection;
using System.Text;

namespace NCParser
{
	class Program
	{
		static void Main(string[] args)
		{
			PropertyInfo gac = typeof(System.Environment).GetProperty(
					"GacPath", BindingFlags.Static | BindingFlags.NonPublic);

			if (gac != null)
			{
				MethodInfo get_gac = gac.GetGetMethod(true);
				string gac_path = (string)get_gac.Invoke(null, null);
				string extensions_path = Path.GetFullPath(Path.Combine(
							gac_path, Path.Combine("..", "xbuild")));
				Console.WriteLine(extensions_path);
			}
			/*string pathTXT = @"C:\temp\testFile.txt";
			FileStream file = new FileStream(pathTXT, FileMode.Open);
			var scanner = new Scanner.Scanner();
			scanner.SetSource(file);
			var parser = new Parser.Parser(scanner);*/

			var ms = new FileStream("/var/calculate/remote/distfiles/egit-src/Pliant-test-for-include.git/main/bin/Debug/cache.txt", FileMode.Open);
			var scanner = new Grammar2.Scanner();
			scanner.SetSource(ms);
			var parser = new Grammar2.Parser(scanner);
			if (parser.Parse())
			{
				Console.WriteLine("Success");
			}
			else
			{
				Console.WriteLine("Fail");
			}
		}
	}
}
