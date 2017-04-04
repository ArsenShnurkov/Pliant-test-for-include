using System;
using System.Diagnostics;
using Eto.Parse;
using Eto.Parse.Grammars;

partial class Globals
{
	static Grammar grammarForApplications = null;
	public static Grammar MonoGrammar
	{
		get
		{
			return GetMonoGrammar();
		}
	}
	static Grammar GetMonoGrammar()
	{
		if (grammarForApplications == null)
		{
			var fileContent = LoadFromResource(DefaultNamespaceName, "Grammar", "MonoGrammar1.ebnf");
			EbnfStyle style = (EbnfStyle)(
				(uint)EbnfStyle.Iso14977
				//& ~(uint) EbnfStyle.WhitespaceSeparator	
				| (uint)EbnfStyle.EscapeTerminalStrings);

			EbnfGrammar grammar;
			try
			{
				grammar = new EbnfGrammar(style);
				grammarForApplications = grammar.Build(fileContent, "a_file");
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
		return grammarForApplications;
	}
}
