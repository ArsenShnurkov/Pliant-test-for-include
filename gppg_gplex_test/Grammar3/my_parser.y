%scanbasetype ScanBase  //names the ScanBaseclass to "ScanBase"
%using Grammar3      // include the Namespace of the scanner-class

%namespace Grammar3   // names the namespace of the Parser-class
%parsertype Parser      //names the Parserclass to "Parser"

%tokentype Tokens       //names the Tokensenumeration to "Tokens"

// Token definitions for GPLEX
%token SPACE
%token EOL
%token WORD
%token LSB, FORWADRSLASH, RSBI, RSB

%union { 
		public int iVal;
		public string sVal;
}

%% //Grammar Rules Section

content
    : /* nothing */
    | nonempty_content
    ;

nonempty_content /* describe starting eols */
	: eols
	| eols noneol_content
	| noneol_content
    ;

eols
	: eol
	| eol eols
	;

eol 
	: EOL { Console.Write("\\n"); }
	;

noneol_content /* describe starting spaces */
	: spaces
	| spaces nonspace_content
	| nonspace_content
	;

spaces
    : SPACE { Console.Write($$.sVal); }
    ;

nonspace_content
	: parts
	| parts eol
	| parts eol nonempty_content
	;

parts
    : instruction
    | section
    ;

instruction
	: word
	| word spaces instruction_parameters
	;

word
	: WORD { Console.Write($<sVal>1); }
	;

instruction_parameters
	: instruction_word
	| instruction_word spaces instruction_parameters
	;

instruction_word
	: word
	| LSB
	| RSB
	| RSBI
	;

section
	: section_start content section_end
	;

section_start
	: directive_opening section_name RSB eol
	| directive_opening section_name spaces RSB eol
	| directive_opening section_name spaces nonspace_section_parameters RSB eol
	;

directive_opening
	: lsb
	| lsb spaces
	;

section_name
	: WORD
	;

spaces_section_parameters
	: spaces
	| spaces nonspace_section_parameters
	;

nonspace_section_parameters
	: parameters_word
	| parameters_word spaces_section_parameters
	;

parameters_word /* может содержать ">" ! */
	: word
	| LSB { Console.Write($<sVal>1); }
	| RSBI { Console.Write($<sVal>1); }
	;

section_end
	: directive_closing section_name rsb
	| directive_closing spaces section_name rsb
	;

directive_closing
	: lsb FORWADRSLASH
	| lsb spaces FORWADRSLASH
	;
 
lsb
	: LSB
	;
rsb
	: RSB
	| RSBI
	;

%% // User-code Section

// Don't forget to declare the Parser-Constructor
public Parser(Grammar3.Scanner scnr) : base(scnr) { }
