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
%token SSLENGINE, ON, VIRTUALHOST, SERVERNAME

%union { 
		public int iVal;
		public string sVal;
}

%start content

%{
class VirtualHostInfo
{
	public bool IsSSLEnabled;
	public string DomainName;
}
List<VirtualHostInfo> hosts = new List<VirtualHostInfo>();
%}

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
	: WORD { /*Console.Write($<sVal>1);*/ }
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
	| ssl_engine_instruction
	| server_name_instruction
	;

ssl_engine_instruction
	: SSLENGINE iws ON { SslEngineActivated(); }
	;

server_name_instruction
	: SERVERNAME iws instruction_parameters { ServerNameSet($3.sVal); }
	;

instruction_parameters
	: instruction_parameters_word
	| instruction_parameters_word iws instruction_parameters
	;

instruction_parameters_word
	: word
	| path
	| string
	| ON
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
	| virtualhost_section_start
	;

virtualhost_section_start
	: VIRTUALHOST iws nonspace_section_parameters eol { VirtualHostStart(); }
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
	: forwardslash iws section_end_word iws rsb
	| forwardslash iws section_end_word rsb
	| forwardslash section_end_word iws rsb
	| forwardslash section_end_word rsb
	;

section_end_word
	: word
	| VIRTUALHOST { VirtualHostEnd(); }
	;

%% // User-code Section

// Don't forget to declare the Parser-Constructor
public Parser(Scanner scnr) : base(scnr) { }

void SslEngineActivated()
{
	Console.WriteLine("SSLEngine activated");
	hosts[hosts.Count-1].IsSSLEnabled = true;
}

void ServerNameSet(string name)
{
	Console.WriteLine("ServerName {0}", name);
	hosts[hosts.Count-1].DomainName = name;
}

void VirtualHostStart()
{
	hosts.Add(new VirtualHostInfo());
	Console.WriteLine("VirtualHostStart");
}

void VirtualHostEnd()
{
	Console.WriteLine("VirtualHostEnd");
}

public IEnumerable<string> GetHosts()
{
	var res = new List<string>();
	for (int i = 0; i < hosts.Count; i++)
	{
		if (hosts[i].IsSSLEnabled)
		{
			res.Add(hosts[i].DomainName);
		}
	}
	return res;
}