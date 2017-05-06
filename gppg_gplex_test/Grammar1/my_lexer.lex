%using QUT.Gppg;         // include LexLocation class declaration
%using Parser;           // include the namespace of the generated Parser-class
%namespace Scanner       // names the Namespace of the generated Scanner-class
%visibility public       // visibility of the types "Tokens","ScanBase","Scanner"
%scannertype Scanner     // names the Scannerclass to "Scanner"
%scanbasetype ScanBase   // names the Scanbaseclass to "ScanBase"
%tokentype Tokens        // names the Tokenenumeration to "Tokens"

%option codePage:65001
/* out:Scanner.cs */
/*see the documentation of GPLEX for further Options you can use */

%{ //user-specified code will be copied in the Output-file
%}

EOL  \r\n?|\n
WS  [ \t]+
WORD [^ \t\n\r]+

%% //Rules Section

{EOL}         {yylval.sVal = yytext; return (int)Tokens.EOL;}
{WS}          {yylval.sVal = yytext; return (int)Tokens.SPACE;}
{WORD}        {yylval.sVal = yytext; return (int)Tokens.WORD;}

%{ //user-code that will be executed before return (with "finally")
	yylloc = new LexLocation(tokLin,tokCol,tokELin,tokECol);
%}
%% //User-code Section

