%scanbasetype ScanBase  //names the ScanBaseclass to "ScanBase"
%using Grammar2      // include the Namespace of the scanner-class

%namespace Grammar2   // names the namespace of the Parser-class
%parsertype Parser      //names the Parserclass to "Parser"

%tokentype Tokens       //names the Tokensenumeration to "Tokens"

// Token definitions for GPLEX
%token SPACE
%token EOL
%token WORD
%token LSB, BACKSLASH, RSB

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
	| eols line_started_content
	| line_started_content
    ;

line_started_content
	: line_with_eol
	| last_line_in_the_file
	| line_with_eol nonempty_content
	;

line_with_eol
	: parts eol { Console.Write("(I:{0},{1}+{2})", @1.StartLine, @1.StartColumn , @2.StartColumn + $2.sVal.Length - @1.StartColumn); }
	;

last_line_in_the_file
	: parts { Console.Write("(E:{0},{1}+{2})", @1.StartLine, @1.StartColumn , @1.EndColumn - @1.StartColumn + 1); }
    ;

parts /* describes leading spaces */
	: spaces
	| spaces meat_started_parts
	| meat_started_parts
	;

meat_started_parts
    : instruction
    | section
    ;

instruction
	: word
	| word spaces instruction_parameters
	;

instruction_parameters
	: word
	| word spaces instruction_parameters
	;

section
	: section_start section_content
	;

section_start
	: LSB section_name RSB eol
	| LSB section_name RSB eol eols
	| LSB section_name spaces section_parameters RSB eol
	| LSB section_name spaces section_parameters RSB eol eols
	;

section_content
	: line_in_the_section_with_eol  section_end
	| line_in_the_section_with_eol section_content  section_end
	| line_in_the_section_with_eol eols  section_end
	| line_in_the_section_with_eol eols section_content  section_end
	;

section_end
	: LSB BACKSLASH section_name RSB
	| LSB spaces BACKSLASH section_name RSB
	;
line_in_the_section_with_eol
	: parts eol
	;

section_name
	: section_name_word
	;

section_parameters
	: section_name_word
	| section_name_word spaces section_parameters
	;

section_name_word /* не может содержать ">" ! Надо переписать грамматику, чтобы могло. ? */
	: WORD
	;

word
	: WORD { Console.Write($<sVal>1); }
	| LSB { Console.Write($<sVal>1); }
	| RSB { Console.Write($<sVal>1); }
	;

spaces
    : SPACE { Console.Write($$.sVal); }
    ;

eols
	: eol
	| eol eols
	;

eol 
	: EOL { Console.Write("\\n"); }
	;

%% // User-code Section

// Don't forget to declare the Parser-Constructor
public Parser(Grammar2.Scanner scnr) : base(scnr) { }
