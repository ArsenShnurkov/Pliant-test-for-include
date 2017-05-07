%scanbasetype ScanBase  //names the ScanBaseclass to "ScanBase"
%using Grammar4      // include the Namespace of the scanner-class

%namespace Grammar4   // names the namespace of the Parser-class
%parsertype Parser      //names the Parserclass to "Parser"

%tokentype Tokens       //names the Tokensenumeration to "Tokens"

// Token definitions for GPLEX
%token SPACE
%token EOL
%token WORD
%token LSB, FORWARDSLASH, RSBI, RSB

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
rsbi
	: RSBI { Console.Write("~>~"); }
	;
rsb_or_rsbi
	: rsb
	| rsbi
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
	| parts spaces
	| parts eol
	| parts spaces eol
	| parts eol nonempty_content
	| parts spaces eol nonempty_content
	;

parts
    : instruction
    | lsb_spaces section
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
	| lsb
	| rsb
	| rsbi
	;

section
	: section_start section_content section_end
	;

section_start
	: word rsb eol
	| word spaces rsb eol
	| word spaces nonspace_section_parameters rsb eol
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
	| lsb
	| rsbi
	;

section_end
	: forwardslash spaces word spaces rsb_or_rsbi {Console.Write("1");} 
	| forwardslash spaces word rsb_or_rsbi {Console.Write("2");}
	| forwardslash word spaces rsb_or_rsbi {Console.Write("3");}
	| forwardslash word rsb_or_rsbi {Console.Write("4");}
	;

%% // User-code Section

// Don't forget to declare the Parser-Constructor
public Parser(Grammar4.Scanner scnr) : base(scnr) { }
