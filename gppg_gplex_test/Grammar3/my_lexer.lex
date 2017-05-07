%using QUT.Gppg;         // include LexLocation class declaration
%using Grammar3;         // include the namespace of the generated Parser-class
%namespace Grammar3      // names the Namespace of the generated Scanner-class
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
WORD [^ \t\n\r<]+
LSB [<]
RSB [>]

%% //Rules Section

{EOL}         {yylval.sVal = yytext; return (int)Tokens.EOL;}
{WS}          {yylval.sVal = yytext; return (int)Tokens.SPACE;}
{LSB}         {return (int)Tokens.LSB;}
{RSB}/{EOL}   {return (int)Tokens.RSB;}
{RSB}         {return (int)Tokens.RSBI;}
[/]          {return (int)Tokens.FORWADRSLASH;}
{WORD}        {yylval.sVal = yytext; return (int)Tokens.WORD;}

%{ //user-code that will be executed before return (with "finally")
	yylloc = new LexLocation(tokLin,tokCol,tokELin,tokECol);
%}
%% //User-code Section

