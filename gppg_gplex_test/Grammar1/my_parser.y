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
	| line_parts
	| eol_parts
    ;

line_parts
	: line 
	| line eol_parts
	;

eol_parts
    : eols 
	| eols line_parts
    ;

line
	: parts { $$ = $1; @$ = @1; Console.Write("<${0}${1}..${2}${3}>", @1.StartLine, @1.StartColumn, @1.EndLine, @1.EndColumn); }
    ;

eols
	: eol
	| eol eols
	; 

eol 
	: EOL { $$ = $1; @$ = @1; Console.WriteLine(".\n"); }
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
    : SPACE { $$ = $1; @$ = @1; Console.Write("_"); }
    ;

word_parts
    : word
	| word space_parts
	;

word
	: WORD { $$ = $1; @$ = @1; Console.Write($<sVal>1); }
	;

%% // User-code Section

// Don't forget to declare the Parser-Constructor
public Parser(Scanner.Scanner scnr) : base(scnr) { }
