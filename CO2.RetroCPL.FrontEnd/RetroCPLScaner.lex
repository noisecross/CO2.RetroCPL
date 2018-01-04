/**
* |------------------------------------------|
* | CO2.RetroCPL COMPILER OPTIMIZER 2 RETROC |
* | File: RetroCPLScaner.lex                 |
* | v1.00, December 2017                     |
* | Author: Emilio Arango Delgado de Mendoza |
* | Based in Jeff Lee's work (1985)          |
* |------------------------------------------|
*/

%using CO2.RetroCPL.Commons;

%namespace CO2.RetroCPL.FrontEnd

%option stack, minimize, parser, verbose, persistbuffer, unicode, compressNext, embedbuffers

%{
	public ScanerHelper helper = new ScanerHelper();

	public override void yyerror(string format, params object[] args)
	{
		helper.lexErr(format, yytext);
	}

%}

// ------------------------------------------
// Definitions
// ------------------------------------------

//Base definitions
D	[0-9]
L	[a-zA-Z_]
H	[a-fA-F0-9]
IS	(u|U|l|L)*

//Single and multiline comments
StartComment	\/\*
EndComment		\*\/
LineComment		"//".*

//Literals
//The 'l' and 'u' at the end of a literal means long and unsigned
//The 'L' at the beginning of a string means double byte symbols
HexLiteral		0[xX]{H}+{IS}?
OctalLiteral	0{D}+{IS}?
IntegerLiteral	{D}+{IS}?
CharLiteral		L?'(\\.|[^\\'])+'
StringLiteral	L?\"(\\.|[^\\"])*\"

//Identifier
Identifier		{L}({L}|{D})*

//Spaces and End of Line
WhiteSpace	[ \t]
Eol			(\r\n?|\n)

// The states into which this FSA can pass.
%x CMMT		// Inside a comment.
%x CMMT2	// Inside a comment.

%%

// ------------------------------------------
// Rules
// ------------------------------------------

// Remove these lines 
{LineComment}+			{ helper.count(yytext); yy_push_state (CMMT2);        }
<CMMT2>{
	{Eol}               { helper.count(yytext); yy_pop_state ();              }
}
{StartComment}			{ helper.count(yytext); yy_push_state (CMMT);         }

// Inside a block comment.
<CMMT>{
	[^*\n]+				{ helper.count(yytext);                               }
	"*"					{ helper.count(yytext);                               }
	{EndComment}		{ helper.count(yytext); yy_pop_state();               }
	<<EOF>>				{ helper.lexErr(ErrorMessages.ERR_LEX_MSG_02, yytext);}
}

"auto"					{ helper.count(yytext); return(int) Tokens.AUTO;      }
"break"					{ helper.count(yytext); return(int) Tokens.BREAK;     }
"byte"					{ helper.count(yytext); return(int) Tokens.BYTE;      }
"case"					{ helper.count(yytext); return(int) Tokens.CASE;      }
"const"					{ helper.count(yytext); return(int) Tokens.CONST;     }
"continue"				{ helper.count(yytext); return(int) Tokens.CONTINUE;  }
"default"				{ helper.count(yytext); return(int) Tokens.DEFAULT;   }
"do"					{ helper.count(yytext); return(int) Tokens.DO;        }
"else"					{ helper.count(yytext); return(int) Tokens.ELSE;      }
"enum"					{ helper.count(yytext); return(int) Tokens.ENUM;      }
"extern"				{ helper.count(yytext); return(int) Tokens.EXTERN;    }
"for"					{ helper.count(yytext); return(int) Tokens.FOR;       }
"goto"					{ helper.count(yytext); return(int) Tokens.GOTO;      }
"if"					{ helper.count(yytext); return(int) Tokens.IF;        }
"interrupt"				{ helper.count(yytext); return(int) Tokens.INTERRUPT; }
"longword"				{ helper.count(yytext); return(int) Tokens.LONGWORD;  }
"register"				{ helper.count(yytext); return(int) Tokens.REGISTER;  }
"return"				{ helper.count(yytext); return(int) Tokens.RETURN;    }
"signed"				{ helper.count(yytext); return(int) Tokens.SIGNED;    }
"sizeof"				{ helper.count(yytext); return(int) Tokens.SIZEOF;    }
"static"				{ helper.count(yytext); return(int) Tokens.STATIC;    }
"struct"				{ helper.count(yytext); return(int) Tokens.STRUCT;    }
"switch"				{ helper.count(yytext); return(int) Tokens.SWITCH;    }
"typedef"				{ helper.count(yytext); return(int) Tokens.TYPEDEF;   }
"union"					{ helper.count(yytext); return(int) Tokens.UNION;     }
"unsigned"				{ helper.count(yytext); return(int) Tokens.UNSIGNED;  }
"void"                  { helper.count(yytext); return(int) Tokens.VOID;      }
"volatile"				{ helper.count(yytext); return(int) Tokens.VOLATILE;  }
"while"					{ helper.count(yytext); return(int) Tokens.WHILE;     }
"word"					{ helper.count(yytext); return(int) Tokens.WORD;      }

{L}({L}|{D})*			{ helper.count(yytext); return(helper.checkTypeOrIdentifier(yytext, yyleng));         }

{HexLiteral}			{ helper.count(yytext); helper.newLiteral(yytext); return(int) Tokens.NUMBER_LITERAL; }
{OctalLiteral}			{ helper.count(yytext); helper.newLiteral(yytext); return(int) Tokens.NUMBER_LITERAL; }
{IntegerLiteral}		{ helper.count(yytext); helper.newLiteral(yytext); return(int) Tokens.NUMBER_LITERAL; }
{CharLiteral}			{ helper.count(yytext); helper.newLiteral(yytext); return(int) Tokens.NUMBER_LITERAL; }
{StringLiteral}			{ helper.count(yytext); helper.newLiteral(yytext); return(int) Tokens.STRING_LITERAL; }

"<-"					{ helper.count(yytext); return(int) Tokens.LEFT_ARROW;   }
">>="					{ helper.count(yytext); return(int) Tokens.RIGHT_ASSIGN; }
"<<="					{ helper.count(yytext); return(int) Tokens.LEFT_ASSIGN;  }
"+="					{ helper.count(yytext); return(int) Tokens.ADD_ASSIGN;   }
"-="					{ helper.count(yytext); return(int) Tokens.SUB_ASSIGN;   }
"*="					{ helper.count(yytext); return(int) Tokens.MUL_ASSIGN;   }
"/="					{ helper.count(yytext); return(int) Tokens.DIV_ASSIGN;   }
"%="					{ helper.count(yytext); return(int) Tokens.MOD_ASSIGN;   }
"&="					{ helper.count(yytext); return(int) Tokens.AND_ASSIGN;   }
"^="					{ helper.count(yytext); return(int) Tokens.XOR_ASSIGN;   }
"|="					{ helper.count(yytext); return(int) Tokens.OR_ASSIGN;    }
">>"					{ helper.count(yytext); return(int) Tokens.RIGHT_OP;     }
"<<"					{ helper.count(yytext); return(int) Tokens.LEFT_OP;      }
"++"					{ helper.count(yytext); return(int) Tokens.INC_OP;       }
"--"					{ helper.count(yytext); return(int) Tokens.DEC_OP;       }
"->"					{ helper.count(yytext); return(int) Tokens.PTR_OP;       }
"&&"					{ helper.count(yytext); return(int) Tokens.AND_OP;       }
"||"					{ helper.count(yytext); return(int) Tokens.OR_OP;        }
"<="					{ helper.count(yytext); return(int) Tokens.LE_OP;        }
">="					{ helper.count(yytext); return(int) Tokens.GE_OP;        }
"=="					{ helper.count(yytext); return(int) Tokens.EQ_OP;        }
"!="					{ helper.count(yytext); return(int) Tokens.NE_OP;        }
";"						{ helper.count(yytext); return(int) Tokens.C_SC;         }
("{"|"<%")				{ helper.count(yytext); return(int) Tokens.C_OK;         }
("}"|"%>")				{ helper.count(yytext); return(int) Tokens.C_CK;         }
","						{ helper.count(yytext); return(int) Tokens.C_CM;         }
":"						{ helper.count(yytext); return(int) Tokens.C_DP;         }
"="						{ helper.count(yytext); return(int) Tokens.C_ES;         }
"("						{ helper.count(yytext); return(int) Tokens.C_OP;         }
")"						{ helper.count(yytext); return(int) Tokens.C_CP;         }
("["|"<:")				{ helper.count(yytext); return(int) Tokens.C_OB;         }
("]"|":>")				{ helper.count(yytext); return(int) Tokens.C_CB;         }
"."						{ helper.count(yytext); return(int) Tokens.C_P;          }
"&"						{ helper.count(yytext); return(int) Tokens.C_A;          }
"!"						{ helper.count(yytext); return(int) Tokens.C_EM;         }
"~"						{ helper.count(yytext); return(int) Tokens.C_NS;         }
"-"						{ helper.count(yytext); return(int) Tokens.C_MS;         }
"+"						{ helper.count(yytext); return(int) Tokens.C_PS;         }
"*"						{ helper.count(yytext); return(int) Tokens.C_AK;         }
"/"						{ helper.count(yytext); return(int) Tokens.C_SS;         }
"%"						{ helper.count(yytext); return(int) Tokens.C_PC;         }
"<"						{ helper.count(yytext); return(int) Tokens.C_LT;         }
">"						{ helper.count(yytext); return(int) Tokens.C_GT;         }
"^"						{ helper.count(yytext); return(int) Tokens.C_UA;         }
"|"						{ helper.count(yytext); return(int) Tokens.C_PP;         }
"?"						{ helper.count(yytext); return(int) Tokens.C_QM;         }

[ \t\v\n\f\r]			{ helper.count(yytext); /* Ignore */   }
.						{ helper.count(yytext); helper.lexErr(ErrorMessages.ERR_LEX_MSG_00, yytext); }
%%