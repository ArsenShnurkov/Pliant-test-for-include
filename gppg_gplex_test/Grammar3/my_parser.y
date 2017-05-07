%scanbasetype ScanBase  //names the ScanBaseclass to "ScanBase"
%using Grammar3      // include the Namespace of the scanner-class

%namespace Grammar3   // names the namespace of the Parser-class
%parsertype Parser      //names the Parserclass to "Parser"

%tokentype Tokens       //names the Tokensenumeration to "Tokens"

// Token definitions for GPLEX
%token SPACE
%token EOL
%token WORD
%token LSB, FORWARDSLASH, RSBI, RSB
%token LSS, LSE

%union { 
		public int iVal;
		public string sVal;
}

%start content

%% //Grammar Rules Section

eols
	: eol
	| eol eols
	;

eol 
	: EOL { Console.Write("\\n"); }
	;

content
    : /* nothing */
    | nonempty_content
    ;

nonempty_content /* describe starting eols */
	: eols
	| eols nonspace_content
    ;

nonspace_content
	: parts
	| parts eol
	| parts eol nonempty_content
	;

parts
    : WORD
    | LSB section
    ;

section
	: section_start section_end
	| section_start section_content section_end
	;

section_start
	: WORD RSB eol
	;

section_end
	: FORWARDSLASH WORD rsb
	;

rsb
	: RSB
	| RSBI
	;

section_content
	: parts eol LSB
	| parts eol section_content
	;

%% // User-code Section

// Don't forget to declare the Parser-Constructor
public Parser(Grammar3.Scanner scnr) : base(scnr) { }
