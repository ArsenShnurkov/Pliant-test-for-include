using System;
//using ....;
using QUT.Gppg;
using Scanner;
using Parser;
using System.IO;

namespace NCParser
{
	class Program
	{
		static void Main(string[] args)
		{
			/*string pathTXT = @"C:\temp\testFile.txt";
			FileStream file = new FileStream(pathTXT, FileMode.Open);
			var scanner = new Scanner.Scanner();
			scanner.SetSource(file);
			var parser = new Parser.Parser(scanner);*/

			string simple_test = "A1 OR A2";
			var scanner = new Scanner.Scanner();
			scanner.SetSource(simple_test, 0);
			var parser = new Parser.Parser(scanner);
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
