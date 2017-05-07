%scanbasetype ScanBase  //names the ScanBaseclass to "ScanBase"
%using Grammar4      // include the Namespace of the scanner-class

%namespace Grammar4   // names the namespace of the Parser-class
%parsertype Parser      //names the Parserclass to "Parser"

%tokentype Tokens       //names the Tokensenumeration to "Tokens"

// Token definitions for GPLEX
%token SPACE
%token EOL
%token WORD
%token LSB, FORWARDSLASH, RSB

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
	: EOL { Console.Write("\n"); }
	;

word
	: WORD { Console.Write($<sVal>1); }
	;

spaces
    : SPACE { Console.Write(new string('_',$$.sVal.Length)); }
    ;
lsb
	: LSB { Console.Write("<"); }
	;
rsb
	: RSB { Console.Write(">"); }
	;
forwardslash
	: FORWARDSLASH { Console.Write("~/~"); }
	;

content
    : /* nothing */
    | nonempty_content
    ;

nonempty_content /* describe starting eols */
	: eols
	| eols noneol_content
	| noneol_content
    ;

noneol_content /* describe starting spaces */
	: spaces
	| spaces nonspace_content
	| nonspace_content
	;

nonspace_content
	: parts
	| parts eol
	| parts eol nonempty_content
	;

parts
    : instruction
    | lsb_spaces section
    | lsb_spaces section spaces
    ;
lsb_spaces
	: lsb
	| lsb spaces
	;
instruction
	: word
	| word spaces instruction_parameters
	;

instruction_parameters
	: instruction_word
	| instruction_word spaces instruction_parameters
	;

instruction_word
	: word
	;

section
	: section_start section_content section_end
	;

section_start
	: word rsb eol
	| word spaces rsb eol
	| word spaces nonspace_section_parameters eol
	;

section_content
    : lsb_spaces
    | section_nonempty_content
    ;

section_nonempty_content /* describe starting eols */
	: eols lsb_spaces
	| eols section_noneol_content
	| section_noneol_content
    ;

section_noneol_content /* describe starting spaces */
	: spaces lsb_spaces
	| spaces section_nonspace_content
	| section_nonspace_content
	;

section_nonspace_content
	: parts eol lsb_spaces
	| parts eol section_nonempty_content
	;

section_parameters_operation
	: lsb
    | rsb
	;

section_parameters_operations
	: section_parameters_operation spaces_section_parameters
	| section_parameters_operation nonspace_section_parameters
	;

spaces_section_parameters
	: spaces rsb
	| spaces nonspace_section_parameters
	| spaces section_parameters_operations
	;

nonspace_section_parameters
	: parameters_word rsb
	| parameters_word spaces_section_parameters
	| parameters_word section_parameters_operations
	;

parameters_word 
	: word
	;

section_end
	: forwardslash spaces word spaces rsb
	| forwardslash spaces word rsb
	| forwardslash word spaces rsb
	| forwardslash word rsb
	;

%% // User-code Section

// Don't forget to declare the Parser-Constructor
public Parser(Grammar4.Scanner scnr) : base(scnr) { }
