%scanbasetype ScanBase  //names the ScanBaseclass to "ScanBase"
%using Scanner      // include the Namespace of the scanner-class

%namespace Parser   // names the namespace of the Parser-class
%parsertype Parser      //names the Parserclass to "Parser"

%tokentype Tokens       //names the Tokensenumeration to "Tokens"

// Token definitions for GPLEX
%token SPACE
%token EOL
%token WORD

%union { 
		public int iVal;
		public string sVal;
}

%% //Grammar Rules Section

file
    : /* nothing */
	| eols_started_content
	| line_started_content
    ;

eols_started_content
	: eols
	| eols line_started_content
	;

eols
	: eol
	| eol eols
	; 

line_started_content
	: last_line_in_the_file
	| line_with_eol
	| line_with_eol line_started_content
	| line_with_eol eols_started_content
	;

last_line_in_the_file
	: parts { Console.Write("(E:{0},{1}+{2})", @1.StartLine, @1.StartColumn , @1.EndColumn - @1.StartColumn + 1); }
    ;

line_with_eol
	: parts eol { Console.Write("(I:{0},{1}+{2})", @1.StartLine, @1.StartColumn , @2.StartColumn + $2.sVal.Length - @1.StartColumn); }
	;

eol 
	: EOL { Console.Write("\\n"); }
	;

parts
	: space_parts
	| word_parts
	;

space_parts
	: space
	| space word_parts
	;

space
    : SPACE { Console.Write($$.sVal); }
    ;

word_parts
    : word
	| word space_parts
	;

word
	: WORD { Console.Write($<sVal>1); }
	;

%% // User-code Section

// Don't forget to declare the Parser-Constructor
public Parser(Scanner.Scanner scnr) : base(scnr) { }
