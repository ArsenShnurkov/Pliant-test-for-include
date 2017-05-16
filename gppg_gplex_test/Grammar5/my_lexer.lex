%using QUT.Gppg;         // include LexLocation class declaration
%using Grammar4;         // include the namespace of the generated Parser-class
%namespace Grammar5      // names the Namespace of the generated Scanner-class
%visibility public       // visibility of the types "Tokens","ScanBase","Scanner"
%scannertype Scanner     // names the Scannerclass to "Scanner"
%scanbasetype ScanBase   // names the Scanbaseclass to "ScanBase"
%tokentype Tokens        // names the Tokenenumeration to "Tokens"

%option codePage:utf-8
/* out:Scanner.cs */
/*see the documentation of GPLEX for further Options you can use */

%{ //user-specified code will be copied in the Output-file
%}

EOL  \r\n?|\n
WS  [ \t]+
WORD [^\\/" \t\n\r><]+
LSB [<]
RSB [>]
DQUOTE ["]
DSTRING {DQUOTE}([^"\\]*(\\.[^"\\]*)*){DQUOTE}

%% //Rules Section

{EOL}         {yylval.sVal = yytext; return (int)Tokens.EOL;}
{WS}          {yylval.sVal = yytext; return (int)Tokens.SPACE;}
{LSB}         {return (int)Tokens.LSB;}
{RSB}         {return (int)Tokens.RSB;}
{WORD}        {yylval.sVal = yytext; return (int)Tokens.WORD;}
{DSTRING}     {yylval.sVal = yytext; return (int)Tokens.DSTRING;}
["]           {return (int)Tokens.DQUOTE;}
[/]           {return (int)Tokens.FORWARDSLASH;}
[\\]           {return (int)Tokens.BACKSLASH;}

%{ //user-code that will be executed before return (with "finally")
	yylloc = new LexLocation(tokLin,tokCol,tokELin,tokECol);
%}
%% //User-code Section

