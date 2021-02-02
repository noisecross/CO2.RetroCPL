/**
* |------------------------------------------|
* | CO2.RetroCPL COMPILER OPTIMIZER 2 RETROC |
* | File: RetroCPLParser.y                   |
* | v1.01, February 2021                     |
* | Author: Emilio Arango Delgado de Mendoza |
* | Based in Jeff Lee's work (1985)          |
* |------------------------------------------|
*/

%using CO2.RetroCPL.Commons;

%namespace CO2.RetroCPL.FrontEnd

%{
	int n_linecnt
	{
		get
		{
			return ((Scanner)Scanner).helper.getLineCounter();
		}
	}

	int n_chrcnt
	{
		get
		{
			return ((Scanner)Scanner).helper.getPreviousCharacterCounter();
		}
	}

	//public StatementList program = new StatementList();
%}


// ------------------------------------------
//  Bison declarations
// ------------------------------------------

%YYSTYPE SyntaxTreeNode

%start translation_unit

%token IDENTIFIER NUMBER_LITERAL STRING_LITERAL SIZEOF LOBYTE HIBYTE

%token PTR_OP INC_OP DEC_OP LEFT_OP RIGHT_OP LE_OP GE_OP EQ_OP NE_OP
%token AND_OP OR_OP MUL_ASSIGN DIV_ASSIGN MOD_ASSIGN ADD_ASSIGN
%token SUB_ASSIGN LEFT_ASSIGN RIGHT_ASSIGN AND_ASSIGN LEFT_ARROW
%token XOR_ASSIGN OR_ASSIGN TYPE_NAME

%token TYPEDEF EXTERN STATIC AUTO REGISTER
%token BYTE WORD LONGWORD SIGNED UNSIGNED CONST VOID VOLATILE
%token STRUCT UNION ENUM ELLIPSIS

%token CASE DEFAULT INTERRUPT IF ELSE SWITCH WHILE DO FOR GOTO CONTINUE BREAK RETURN


// ------------------------------------------
// Grammar rules
// ------------------------------------------
%%

translation_unit
	: external_declaration                                                         { $$ = new TranslationUnitSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1 }); SyntaxTree.Instance.setRoot($$); }
	| translation_unit external_declaration                                        { $1.addChildren(new SyntaxTreeNode[] { $2 }); $$ = $1;                             SyntaxTree.Instance.setRoot($$); }
	;

external_declaration
	: variables_declaration                                                        { $$ = new ExternalDeclarationSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1 }); }
	| function_prototyping                                                         { $$ = new ExternalDeclarationSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1 }); }
	| function_definition                                                          { $$ = new ExternalDeclarationSTN(3, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1 }); }
	| error	error_end                                                              { $$ = new ExternalDeclarationSTN(4, n_linecnt, n_chrcnt); this.yyerrok();              }
	;

variables_declaration
	: declaration_specifiers init_declarator_list ';'                              { $$ = new VariablesDeclarationSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $2 }); }
	;

function_definition
	: function_name_definition '(' parameter_list ')' compound_statement           { $$ = new FunctionDefinitionSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3, $5 }); }
	| function_name_definition '(' ')' compound_statement                          { $$ = new FunctionDefinitionSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $4 });     }
	;

function_prototyping
	: function_name_definition '(' abstract_parameter_list ')' ';'                 { $$ = new FunctionPrototypingSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); }
	| function_name_definition '(' ')' ';'                                         { $$ = new FunctionPrototypingSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1     }); }
	;

function_name_definition
	: declaration_specifiers pointer IDENTIFIER                                    { $$ = new FunctionNameDefinitionSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $2, new IdentifierSTL(3, n_linecnt, n_chrcnt) }); }
	| declaration_specifiers IDENTIFIER                                            { $$ = new FunctionNameDefinitionSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, new IdentifierSTL(2, n_linecnt, n_chrcnt) }); }
	;

error_end
	: ';'                                                                          { ; }
	| ')'                                                                          { ; }
	| '{'                                                                          { ; }
	| '}'                                                                          { ; }
	;

declaration_specifiers
	: type_specifier                                                               { $$ = new DeclarationSpecifiersSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1     }); }
	| type_specifier declaration_specifiers                                        { $2.addChildren(new SyntaxTreeNode[] { $1 }); $$ = $2;                                       }
	| type_qualifier                                                               { $$ = new DeclarationSpecifiersSTN(3, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1     }); }
	| type_qualifier declaration_specifiers                                        { $2.addChildren(new SyntaxTreeNode[] { $1 }); $$ = $2;                                       }
	;

init_declarator_list
	: init_declarator                                                              { $$ = new InitDeclaratorListSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1 }); }
	| init_declarator_list ',' init_declarator                                     { $1.addChildren(new SyntaxTreeNode[] { $3 }); $$ = $1;                                }
	;

type_name
	: declaration_specifiers                                                       { $$ = new TypeNameSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1     }); }
	| declaration_specifiers pointer                                               { $$ = new TypeNameSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $2 }); }
	;

declarator
	: pointer direct_declarator                                                    { $$ = new DeclaratorSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $2 }); }
	| direct_declarator                                                            { $$ = new DeclaratorSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1     }); }
	;

pointer
    : '*'                                                                          { $$ = new PointerSTN(1, n_linecnt, n_chrcnt);                    }
    | '*' pointer                                                                  { $2.addChildren(new SyntaxTreeNode[] { $1 }); $$ = $2;           }
    ;

direct_declarator
	: IDENTIFIER                                                                   { $$ = new DirectDeclaratorSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { new IdentifierSTL(1, n_linecnt, n_chrcnt)     }); }
	| IDENTIFIER '[' constant_expression ']'                                       { $$ = new DirectDeclaratorSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { new IdentifierSTL(1, n_linecnt, n_chrcnt), $3 }); }
	;

type_specifier
	: VOID                                                                         { $$ = new TypeSpecifierSTN(1, n_linecnt, n_chrcnt, "void");     }
	| BYTE                                                                         { $$ = new TypeSpecifierSTN(2, n_linecnt, n_chrcnt, "byte");     }
	| WORD                                                                         { $$ = new TypeSpecifierSTN(3, n_linecnt, n_chrcnt, "word");     }
	| LONGWORD                                                                     { $$ = new TypeSpecifierSTN(4, n_linecnt, n_chrcnt, "longword"); }
	| SIGNED                                                                       { $$ = new TypeSpecifierSTN(5, n_linecnt, n_chrcnt, "signed");   }
	| UNSIGNED                                                                     { $$ = new TypeSpecifierSTN(6, n_linecnt, n_chrcnt, "unsigned"); }
	;

type_qualifier
	: CONST                                                                        { $$ = new TypeQualifierSTN(1, n_linecnt, n_chrcnt, "const");     }
	| VOLATILE                                                                     { $$ = new TypeQualifierSTN(2, n_linecnt, n_chrcnt, "volatile");  }
	| INTERRUPT                                                                    { $$ = new TypeQualifierSTN(3, n_linecnt, n_chrcnt, "interrupt"); }
	;

init_declarator
	: declarator                                                                   { $$ = new InitDeclaratorSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1     }); }
	| declarator '=' initializer                                                   { $$ = new InitDeclaratorSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); }
	;

declaration_list
	: variables_declaration                                                        { $$ = new DeclarationListSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1 }); }
	| declaration_list variables_declaration                                       { $1.addChildren(new SyntaxTreeNode[] { $2 }); $$ = $1;                             }
	| error variables_declaration                                                  { $$ = new DeclarationListSTN(3, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $2 }); }
	| declaration_list error variables_declaration                                 { $1.addChildren(new SyntaxTreeNode[] { $3 }); $$ = $1;                             }
	;

compound_statement
	: '{' '}'                                                                      { $$ = new CompoundStatementSTN(1, n_linecnt, n_chrcnt);                                  }
	| '{' statement_list '}'                                                       { $$ = new CompoundStatementSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $2 });     }
	| '{' declaration_list '}'                                                     { $$ = new CompoundStatementSTN(3, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $2 });     }
	| '{' declaration_list statement_list '}'                                      { $$ = new CompoundStatementSTN(4, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $2, $3 }); }
	;

parameter_list
	: parameter_declaration                                                        { $$ = new ParameterListSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1 }); }
	| parameter_list ',' parameter_declaration                                     { $1.addChildren(new SyntaxTreeNode[] { $3 }); $$ = $1;                           }
	;

abstract_parameter_list
	: abstract_parameter_declaration                                               { $$ = new AbstractParameterListSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1 }); }
	| abstract_parameter_list ',' abstract_parameter_declaration                   { $1.addChildren(new SyntaxTreeNode[] { $3 }); $$ = $1;                                   }
	;

constant_expression
	: conditional_expression                                                       { $$ = new ConstantExpressionSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1 }); }
	;

statement_list
	: statement                                                                    { $$ = new StatementListSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1 }); }
	| statement_list statement                                                     { $1.addChildren(new SyntaxTreeNode[] { $2 }); $$ = $1;                           }
	;

initializer
	: assignment_expression                                                        { $$ = new InitializerSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1 }); }
	;

parameter_declaration
	: declaration_specifiers pointer IDENTIFIER                                    { $$ = new ParameterDeclarationSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $2, new IdentifierSTL(3, n_linecnt, n_chrcnt) }); }
	| declaration_specifiers IDENTIFIER                                            { $$ = new ParameterDeclarationSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, new IdentifierSTL(2, n_linecnt, n_chrcnt)     }); }
	;

abstract_parameter_declaration
	: declaration_specifiers pointer                                               { $$ = new AbstractParameterDeclarationSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $2 }); }
	| declaration_specifiers                                                       { $$ = new AbstractParameterDeclarationSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1     }); }
	;

conditional_expression
	: logical_or_expression                                                        { $$ = new ConditionalExpressionSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1         }); }
	| logical_or_expression '?' expression ':' conditional_expression              { $$ = new ConditionalExpressionSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3, $5 }); }
	;

statement
	: labeled_statement                                                            { $$ = new StatementSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1 }); }
	| expr_statement                                                               { $$ = new StatementSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1 }); }
	| selection_statement                                                          { $$ = new StatementSTN(3, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1 }); }
	| iteration_statement                                                          { $$ = new StatementSTN(4, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1 }); }
	| jump_statement                                                               { $$ = new StatementSTN(5, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1 }); }
	| error	error_end                                                              { $$ = new StatementSTN(6, n_linecnt, n_chrcnt, new SyntaxTreeNode[] {    }); }
	;

assignment_expression
	: conditional_expression                                                       { $$ = new AssignmentExpressionSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1         }); }
	| unary_expression assignment_operator assignment_expression                   { $$ = new AssignmentExpressionSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $2, $3 }); }
	| unary_expression LEFT_ARROW assignment_expression                            { $$ = new AssignmentExpressionSTN(3, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3     }); }
	;
	
logical_or_expression
	: logical_and_expression                                                       { $$ = new LogicalOrExpressionSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1     }); }
	| logical_or_expression OR_OP logical_and_expression                           { $$ = new LogicalOrExpressionSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); }
	;

expression
	: assignment_expression                                                        { $$ = new ExpressionSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1     }); }
	| expression ',' assignment_expression                                         { $$ = new ExpressionSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); }
	;

labeled_statement
	: IDENTIFIER ':' statement                                                     { $$ = new LabeledStatementSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { new IdentifierSTL(1, n_linecnt, n_chrcnt), $3 }); }
	;

expr_statement
	: ';'                                                                          { $$ = new ExpressionStatementSTN(1, n_linecnt, n_chrcnt);                              }
	| expression ';'                                                               { $$ = new ExpressionStatementSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1 }); }
	;

selection_statement
	: IF '(' expression ')' compound_statement                                     { $$ = new SelectionStatementSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $3, $5     }); }
	| IF '(' expression ')' compound_statement ELSE compound_statement             { $$ = new SelectionStatementSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $3, $5, $7 }); }
	| SWITCH '(' expression ')' '{' case_statement_list '}'                        { $$ = new SelectionStatementSTN(3, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $3, $6     }); }
	;

case_statement_list
	: case_statement                                                               { $$ = new CaseStatementListSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1 }); }
	| case_statement_list case_statement                                           { $1.addChildren(new SyntaxTreeNode[] { $2 }); $$ = $1;                               }
	;

case_statement
	: CASE constant_expression ':'                                                 { $$ = new CaseStatementSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $2     }); }
	| CASE constant_expression ':' statement_list                                  { $$ = new CaseStatementSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $2, $4 }); }
	| DEFAULT ':' statement_list                                                   { $$ = new CaseStatementSTN(3, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $3     }); }
	;

iteration_statement
	: WHILE '(' expression ')' compound_statement                                  { $$ = new IterationStatementSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $3, $5         }); }
	| DO compound_statement WHILE '(' expression ')' ';'                           { $$ = new IterationStatementSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $2, $5         }); }
	| FOR '(' expr_statement expr_statement ')' compound_statement                 { $$ = new IterationStatementSTN(3, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $3, $4, $6     }); }
	| FOR '(' expr_statement expr_statement expression ')' compound_statement      { $$ = new IterationStatementSTN(4, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $3, $4, $5, $7 }); }
	;

jump_statement
	: GOTO IDENTIFIER ';'                                                          { $$ = new JumpStatementSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { new IdentifierSTL(1, n_linecnt, n_chrcnt) }); }
	| CONTINUE ';'                                                                 { $$ = new JumpStatementSTN(2, n_linecnt, n_chrcnt);                                                                     }
	| BREAK ';'                                                                    { $$ = new JumpStatementSTN(3, n_linecnt, n_chrcnt);                                                                     }
	| RETURN ';'                                                                   { $$ = new JumpStatementSTN(4, n_linecnt, n_chrcnt);                                                                     }
	| RETURN expression ';'                                                        { $$ = new JumpStatementSTN(5, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $2                                        }); }
	;

unary_expression
	: postfix_expression                                                           { $$ = new UnaryExpressionSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1     }); }
	| INC_OP unary_expression                                                      { $$ = new UnaryExpressionSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $2     }); }
	| DEC_OP unary_expression                                                      { $$ = new UnaryExpressionSTN(3, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $2     }); }
	| unary_operator cast_expression                                               { $$ = new UnaryExpressionSTN(4, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $2 }); }
	| SIZEOF unary_expression                                                      { $$ = new UnaryExpressionSTN(5, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $2     }); }
	| SIZEOF '(' type_specifier ')'                                                { $$ = new UnaryExpressionSTN(6, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $3     }); }
	| SIZEOF '(' type_specifier pointer ')'                                        { $$ = new UnaryExpressionSTN(7, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $3, $4 }); }
	| LOBYTE unary_expression                                                      { $$ = new UnaryExpressionSTN(8, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $2     }); }
	| HIBYTE unary_expression                                                      { $$ = new UnaryExpressionSTN(9, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $2     }); }
	;

assignment_operator
	: '='                                                                          { $$ = new AssignmentOperatorSTN( 1, n_linecnt, n_chrcnt, "=");   }
	| MUL_ASSIGN                                                                   { $$ = new AssignmentOperatorSTN( 2, n_linecnt, n_chrcnt, "*="); /* Not supported by RetroC */ }
	| DIV_ASSIGN                                                                   { $$ = new AssignmentOperatorSTN( 3, n_linecnt, n_chrcnt, "/="); /* Not supported by RetroC */ }
	| MOD_ASSIGN                                                                   { $$ = new AssignmentOperatorSTN( 4, n_linecnt, n_chrcnt, "%="); /* Not supported by RetroC */ }
	| ADD_ASSIGN                                                                   { $$ = new AssignmentOperatorSTN( 5, n_linecnt, n_chrcnt, "+=");  }
	| SUB_ASSIGN                                                                   { $$ = new AssignmentOperatorSTN( 6, n_linecnt, n_chrcnt, "-=");  }
	| LEFT_ASSIGN                                                                  { $$ = new AssignmentOperatorSTN( 7, n_linecnt, n_chrcnt, "<<="); }
	| RIGHT_ASSIGN                                                                 { $$ = new AssignmentOperatorSTN( 8, n_linecnt, n_chrcnt, ">>="); }
	| AND_ASSIGN                                                                   { $$ = new AssignmentOperatorSTN( 9, n_linecnt, n_chrcnt, "&=");  }
	| XOR_ASSIGN                                                                   { $$ = new AssignmentOperatorSTN(10, n_linecnt, n_chrcnt, "^=");  }
	| OR_ASSIGN                                                                    { $$ = new AssignmentOperatorSTN(11, n_linecnt, n_chrcnt, "|=");  }
	;

logical_and_expression
	: inclusive_or_expression                                                      { $$ = new LogicalAndExpressionSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1     }); }
	| logical_and_expression AND_OP inclusive_or_expression                        { $$ = new LogicalAndExpressionSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); }
	;

postfix_expression
	: primary_expression                                                           { $$ = new PostfixExpressionSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1                                              }); }
	| postfix_expression '[' expression ']'                                        { $$ = new PostfixExpressionSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3                                          }); }
	| IDENTIFIER '(' ')'                                                           { $$ = new PostfixExpressionSTN(3, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { new IdentifierSTL(1, n_linecnt, n_chrcnt)     }); }
	| IDENTIFIER '(' argument_expression_list ')'                                  { $$ = new PostfixExpressionSTN(4, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { new IdentifierSTL(1, n_linecnt, n_chrcnt), $3 }); }
	| postfix_expression INC_OP                                                    { $$ = new PostfixExpressionSTN(5, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1                                              }); }
	| postfix_expression DEC_OP                                                    { $$ = new PostfixExpressionSTN(6, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1                                              }); }
	;

unary_operator
	: '&'                                                                          { $$ = new UnaryOperatorSTN(1, n_linecnt, n_chrcnt); }
	| '*'                                                                          { $$ = new UnaryOperatorSTN(2, n_linecnt, n_chrcnt); }
	| '+'                                                                          { $$ = new UnaryOperatorSTN(3, n_linecnt, n_chrcnt); }
	| '-'                                                                          { $$ = new UnaryOperatorSTN(4, n_linecnt, n_chrcnt); }
	| '~'                                                                          { $$ = new UnaryOperatorSTN(5, n_linecnt, n_chrcnt); }
	| '!'                                                                          { $$ = new UnaryOperatorSTN(6, n_linecnt, n_chrcnt); }
	;

cast_expression
	: unary_expression                                                             { $$ = new CastExpressionSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1     }); }
	| '(' type_name ')' cast_expression                                            { $$ = new CastExpressionSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $2, $4 }); }
	;

primary_expression
	: IDENTIFIER                                                                   { $$ = new PrimaryExpressionSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { new IdentifierSTL(1, n_linecnt, n_chrcnt)    }); }
	| NUMBER_LITERAL                                                               { $$ = new PrimaryExpressionSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { new NumberLiteralSTL(1, n_linecnt, n_chrcnt) }); }
	| STRING_LITERAL                                                               { $$ = new PrimaryExpressionSTN(3, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { new StringLiteralSTL(1, n_linecnt, n_chrcnt) }); }
	| '(' expression ')'                                                           { $$ = new PrimaryExpressionSTN(4, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $2                                             }); }
	;

inclusive_or_expression
	: exclusive_or_expression                                                      { $$ = new InclusiveOrExpressionSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1     }); }
	| inclusive_or_expression '|' exclusive_or_expression                          { $$ = new InclusiveOrExpressionSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); }
	;

argument_expression_list
	: assignment_expression                                                        { $$ = new ArgumentExpressionListSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1 }); }
	| argument_expression_list ',' assignment_expression                           { $1.addChildren(new SyntaxTreeNode[] { $3 }); $$ = $1;                                    }
	;

exclusive_or_expression
	: and_expression                                                               { $$ = new ExclusiveOrExpressionSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1     }); }
	| exclusive_or_expression '^' and_expression                                   { $$ = new ExclusiveOrExpressionSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); }
	;

and_expression
	: equality_expression                                                          { $$ = new AndExpressionSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1     }); }
	| and_expression '&' equality_expression                                       { $$ = new AndExpressionSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); }
	;

equality_expression
	: relational_expression                                                        { $$ = new EqualityExpressionSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1     }); }
	| equality_expression EQ_OP relational_expression                              { $$ = new EqualityExpressionSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); }
	| equality_expression NE_OP relational_expression                              { $$ = new EqualityExpressionSTN(3, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); }
	;

relational_expression
	: shift_expression                                                             { $$ = new RelationalExpressionSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1     }); }
	| relational_expression '>' shift_expression                                   { $$ = new RelationalExpressionSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); }
	| relational_expression '<' shift_expression                                   { $$ = new RelationalExpressionSTN(3, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); }
	| relational_expression LE_OP shift_expression                                 { $$ = new RelationalExpressionSTN(4, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); }
	| relational_expression GE_OP shift_expression                                 { $$ = new RelationalExpressionSTN(5, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); }
	;
	
shift_expression
	: additive_expression                                                          { $$ = new ShiftExpressionSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1     }); }
	| shift_expression LEFT_OP additive_expression                                 { $$ = new ShiftExpressionSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); }
	| shift_expression RIGHT_OP additive_expression                                { $$ = new ShiftExpressionSTN(3, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); }
	;

additive_expression
	: multiplicative_expression                                                    { $$ = new AdditiveExpressionSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1     }); }
	| additive_expression '+' multiplicative_expression                            { $$ = new AdditiveExpressionSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); }
	| additive_expression '-' multiplicative_expression                            { $$ = new AdditiveExpressionSTN(3, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); }
	;

multiplicative_expression
	: cast_expression                                                              { $$ = new MultiplicativeExpressionSTN(1, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1     });                             }
	| multiplicative_expression '*' cast_expression                                { $$ = new MultiplicativeExpressionSTN(2, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); /* Not supported by RetroC */ }
	| multiplicative_expression '/' cast_expression                                { $$ = new MultiplicativeExpressionSTN(3, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); /* Not supported by RetroC */ }
	| multiplicative_expression '%' cast_expression                                { $$ = new MultiplicativeExpressionSTN(4, n_linecnt, n_chrcnt, new SyntaxTreeNode[] { $1, $3 }); /* Not supported by RetroC */ }
	;
%%

// No argument CTOR. By deafult Parser's ctor requires scanner as param.
public Parser(Scanner scn) : base(scn) { }
