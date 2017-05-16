%scanbasetype ScanBase  //names the ScanBaseclass to "ScanBase"
%using Grammar5      // include the Namespace of the scanner-class

%namespace Grammar5   // names the namespace of the Parser-class
%parsertype Parser      //names the Parserclass to "Parser"

%tokentype Tokens       //names the Tokensenumeration to "Tokens"

// Token definitions for GPLEX
%token SPACE
%token EOL
%token BACKSLASH, FORWARDSLASH
%token LSB, RSB
%token DQUOTE
%token DSTRING
%token WORD

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

string
	: DSTRING { Console.Write("\u2CFA{0}\u2CFB", $$.sVal); }
	;

word
	: WORD { Console.Write($<sVal>1); }
	;

iws 
	: iws_element
	| iws_element iws
	;

iws_element
	: spaces
	| glue
	;

spaces
    : SPACE { Console.Write(new string('_',$$.sVal.Length)); }
    ;

glue
	: BACKSLASH eol
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
	: iws
	| iws nonspace_content
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
    | lsb_spaces section iws
    ;
lsb_spaces
	: lsb
	| lsb iws
	;
instruction
	: word
	| word iws instruction_parameters
	;

instruction_parameters
	: instruction_word
	| instruction_word iws instruction_parameters
	;

instruction_word
	: word
	| path
	| string
	;

path
	: root_path
	;

root_path
	: forwardslash word
	| forwardslash word root_path
	;

section
	: section_start section_content section_end
	;

section_start
	: word rsb eol
	| word iws rsb eol
	| word iws nonspace_section_parameters eol
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
	: iws lsb_spaces
	| iws section_nonspace_content
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
	: iws rsb
	| iws nonspace_section_parameters
	| iws section_parameters_operations
	;

nonspace_section_parameters
	: parameters_word rsb
	| parameters_word spaces_section_parameters
	| parameters_word section_parameters_operations
	;

parameters_word 
	: word
	| path
	| string
	;

section_end
	: forwardslash iws word iws rsb
	| forwardslash iws word rsb
	| forwardslash word iws rsb
	| forwardslash word rsb
	;

%% // User-code Section

// Don't forget to declare the Parser-Constructor
public Parser(Scanner scnr) : base(scnr) { }
