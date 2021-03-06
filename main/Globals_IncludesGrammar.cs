﻿using System;
using System.Configuration;
using System.Diagnostics;
using Eto.Parse;
using Eto.Parse.Grammars;

partial class Globals
{
	/// <summary>
	/// This value is used to calculate relative paths from config in the same way as Apache does
	/// </summary>
	/// <value>The server root.</value>
	public static string ServerRoot
	{
		get
		{
			return ConfigurationManager.AppSettings["ApacheConfigFilePath"].ToString();
		}
		set
		{
			/* ignore value */
		}
	}

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
			var fileContent = LoadFromResource(DefaultNamespaceName, "Grammar", "IncludesGrammar1.ebnf");
			EbnfStyle style = (EbnfStyle)(
				(uint)EbnfStyle.Iso14977
				//& ~(uint) EbnfStyle.WhitespaceSeparator	
				| (uint)EbnfStyle.EscapeTerminalStrings);

			EbnfGrammar grammar;
			try
			{
				grammar = new EbnfGrammar(style);
				grammarForIncludes = grammar.Build(fileContent, "file");
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
