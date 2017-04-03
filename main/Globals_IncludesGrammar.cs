using System;
using System.Diagnostics;
using Eto.Parse;
using Eto.Parse.Grammars;

partial class Globals
{
	const string DefaultNamespaceName = "DefaultNamespaceName";
	const string nameOfTheStartingRule = "file";

	static Grammar grammarForIncludes = null;
	public static Grammar IncludesGrammar
	{
		get
		{
			return GetIncludesGrammar();
		}
	}
	static Grammar GetIncludesGrammar()
	{
		if (grammarForIncludes == null)
		{
			var fileContent = LoadFromResource(DefaultNamespaceName, "Grammar", "syntax8.ebnf");
			EbnfStyle style = (EbnfStyle)(
				(uint)EbnfStyle.Iso14977
				//& ~(uint) EbnfStyle.WhitespaceSeparator	
				| (uint)EbnfStyle.EscapeTerminalStrings);

			EbnfGrammar grammar;
			try
			{
				grammar = new EbnfGrammar(style);
				grammarForIncludes = grammar.Build(fileContent, nameOfTheStartingRule);
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
		}
		return grammarForIncludes;
	}
}
