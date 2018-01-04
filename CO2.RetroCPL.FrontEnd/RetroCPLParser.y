/**
* |------------------------------------------|
* | CO2.RetroCPL COMPILER OPTIMIZER 2 RETROC |
* | File: RetroCPLParser.y                   |
* | v1.00, December 2017                     |
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
			return ((Scanner)Scanner).helper.n_linecnt;
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

%token C_SC C_OK C_CK C_CM C_DP C_ES C_OP C_CP C_OB C_CB C_P C_A C_EM C_NS C_MS C_PS
%token C_AK C_SS C_PC C_LT C_GT C_UA C_PP C_QM

%token TYPEDEF EXTERN STATIC AUTO REGISTER
%token BYTE WORD LONGWORD SIGNED UNSIGNED CONST VOID VOLATILE
%token STRUCT UNION ENUM ELLIPSIS

%token CASE DEFAULT INTERRUPT IF ELSE SWITCH WHILE DO FOR GOTO CONTINUE BREAK RETURN


// ------------------------------------------
// Grammar rules
// ------------------------------------------
%%

translation_unit
	: external_declaration                                                         { $$ = new TranslationUnitSTN(1, n_linecnt, new SyntaxTreeNode[] { $1 }); SyntaxTree.Instance.setRoot($$); }
	| translation_unit external_declaration                                        { $1.addChildren(new SyntaxTreeNode[] { $2 }); $$ = $1;                   SyntaxTree.Instance.setRoot($$); }
	;

external_declaration
	: function_definition                                                          { $$ = new ExternalDeclarationSTN(1, n_linecnt, new SyntaxTreeNode[] { $1 }); }
	| declaration                                                                  { $$ = new ExternalDeclarationSTN(2, n_linecnt, new SyntaxTreeNode[] { $1 }); }
	| error	error_end                                                              { $$ = new ExternalDeclarationSTN(3, n_linecnt); this.yyerrok(); }
	;

declaration
	: declaration_specifiers C_SC                                                  { $$ = new DeclarationSTN(1, n_linecnt, new SyntaxTreeNode[] { $1 }    ); }
	| declaration_specifiers init_declarator_list C_SC                             { $$ = new DeclarationSTN(1, n_linecnt, new SyntaxTreeNode[] { $1, $2 }); }
	;

function_definition
	: declaration_specifiers direct_declarator declaration_list compound_statement { $$ = new FunctionDefinitionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1, $2, $3, $4 }); }
	| declaration_specifiers direct_declarator compound_statement                  { $$ = new FunctionDefinitionSTN(2, n_linecnt, new SyntaxTreeNode[] { $1, $2, $3     }); }
	| direct_declarator declaration_list compound_statement                        { $$ = new FunctionDefinitionSTN(3, n_linecnt, new SyntaxTreeNode[] { $1, $2, $3     }); }
	| direct_declarator compound_statement                                         { $$ = new FunctionDefinitionSTN(4, n_linecnt, new SyntaxTreeNode[] { $1, $2         }); }
	;

error_end
	: C_SC                                                                         { ; }
	| C_CP                                                                         { ; }
	| C_OK                                                                         { ; }
	| C_CK                                                                         { ; }
	;

declaration_specifiers
	: type_specifier                                                               { $$ = new DeclarationSpecifiersSTN(1, n_linecnt, new SyntaxTreeNode[] { $1     }); }
	| type_specifier declaration_specifiers                                        { $$ = new DeclarationSpecifiersSTN(2, n_linecnt, new SyntaxTreeNode[] { $1, $2 }); }
	| type_qualifier                                                               { $$ = new DeclarationSpecifiersSTN(3, n_linecnt, new SyntaxTreeNode[] { $1     }); }
	| type_qualifier declaration_specifiers                                        { $$ = new DeclarationSpecifiersSTN(4, n_linecnt, new SyntaxTreeNode[] { $1, $2 }); }
	;

init_declarator_list
	: init_declarator                                                              { $$ = new InitDeclaratorListSTN(1, n_linecnt, new SyntaxTreeNode[] { $1 }); }
	| init_declarator_list C_CM init_declarator                                    { $1.addChildren(new SyntaxTreeNode[] { $3 }); $$ = $1;                      }
	;

direct_declarator
	: IDENTIFIER                                                                   { $$ = new DirectDeclaratorSTN(1, n_linecnt, new SyntaxTreeNode[] { new IdentifierSTL(1, n_linecnt) });                                  }
	| C_OP direct_declarator C_CP                                                  { $$ = new DirectDeclaratorSTN(2, n_linecnt, new SyntaxTreeNode[] { $1                              }); }
	| direct_declarator C_OP parameter_list C_CP                                   { $$ = new DirectDeclaratorSTN(3, n_linecnt, new SyntaxTreeNode[] { $1, $3                          }); }
	| direct_declarator C_OP identifier_list C_CP                                  { $$ = new DirectDeclaratorSTN(4, n_linecnt, new SyntaxTreeNode[] { $1, $3                          }); }
	| direct_declarator C_OP C_CP                                                  { $$ = new DirectDeclaratorSTN(5, n_linecnt, new SyntaxTreeNode[] { $1                              }); }
	| direct_declarator C_OB constant_expression C_CB                              { $$ = new DirectDeclaratorSTN(6, n_linecnt, new SyntaxTreeNode[] { $1, $3                          }); }
	;

declaration_list
	: declaration                                                                  { $$ = new DeclarationListSTN(1, n_linecnt, new SyntaxTreeNode[] { $1 }); }
	| declaration_list declaration                                                 { $1.addChildren(new SyntaxTreeNode[] { $2 }); $$ = $1;                   }
	| error declaration                                                            { $$ = new DeclarationListSTN(3, n_linecnt, new SyntaxTreeNode[] { $2 }); }
	| declaration_list error declaration                                           { $1.addChildren(new SyntaxTreeNode[] { $3 }); $$ = $1;                   }
	;

compound_statement
	: C_OK C_CK                                                                    { $$ = new CompoundStatementSTN(1, n_linecnt);                                  }
	| C_OK statement_list C_CK                                                     { $$ = new CompoundStatementSTN(2, n_linecnt, new SyntaxTreeNode[] { $2 });     }
	| C_OK declaration_list C_CK                                                   { $$ = new CompoundStatementSTN(3, n_linecnt, new SyntaxTreeNode[] { $2 });     }
	| C_OK declaration_list statement_list C_CK                                    { $$ = new CompoundStatementSTN(4, n_linecnt, new SyntaxTreeNode[] { $2, $3 }); }
	;

type_specifier
	: VOID                                                                         { $$ = new TypeSpecifierSTN(1, n_linecnt); }
	| BYTE                                                                         { $$ = new TypeSpecifierSTN(2, n_linecnt); }
	| WORD                                                                         { $$ = new TypeSpecifierSTN(3, n_linecnt); }
	| LONGWORD                                                                     { $$ = new TypeSpecifierSTN(4, n_linecnt); }
	| SIGNED                                                                       { $$ = new TypeSpecifierSTN(5, n_linecnt); }
	| UNSIGNED                                                                     { $$ = new TypeSpecifierSTN(6, n_linecnt); }
	;

type_qualifier
	: CONST                                                                        { $$ = new TypeQualifierSTN(1, n_linecnt); }
	| VOLATILE                                                                     { $$ = new TypeQualifierSTN(2, n_linecnt); }
	| INTERRUPT                                                                    { $$ = new TypeQualifierSTN(3, n_linecnt); }
	;

init_declarator
	: direct_declarator                                                            { $$ = new InitDeclaratorSTN(1, n_linecnt, new SyntaxTreeNode[] { $1     }); }
	| direct_declarator C_ES initializer                                           { $$ = new InitDeclaratorSTN(2, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); }
	;

parameter_list
	: parameter_declaration                                                        { $$ = new ParameterListSTN(1, n_linecnt, new SyntaxTreeNode[] { $1 }); }
	| parameter_list C_CM parameter_declaration                                    { $1.addChildren(new SyntaxTreeNode[] { $3 }); $$ = $1;                 }
	;

identifier_list
	: IDENTIFIER                                                                   { $$ = new IdentifierListSTN(1, n_linecnt, new SyntaxTreeNode[] { new IdentifierSTL(1, n_linecnt) }); }
	| identifier_list C_CM IDENTIFIER                                              { $1.addChildren(new SyntaxTreeNode[] { new IdentifierSTL(1, n_linecnt) }); $$ = $1;                  }
	;

constant_expression
	: conditional_expression                                                       { $$ = new ConstantExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1 }); }
	;

statement_list
	: statement                                                                    { $$ = new StatementListSTN(1, n_linecnt, new SyntaxTreeNode[] { $1 }); }
	| statement_list statement                                                     { $1.addChildren(new SyntaxTreeNode[] { $2 }); $$ = $1;                 }
	;

initializer
	: assignment_expression                                                        { $$ = new InitializerSTN(1, n_linecnt, new SyntaxTreeNode[] { $1 }); }
	;

parameter_declaration
	: declaration_specifiers direct_declarator                                     { $$ = new ParameterDeclarationSTN(1, n_linecnt, new SyntaxTreeNode[] { $1, $2 }); }
	| declaration_specifiers                                                       { $$ = new ParameterDeclarationSTN(2, n_linecnt, new SyntaxTreeNode[] { $1     }); }
	;

conditional_expression
	: logical_or_expression                                                        { $$ = new ConditionalExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1         }); }
	| logical_or_expression C_QM expression C_DP conditional_expression            { $$ = new ConditionalExpressionSTN(2, n_linecnt, new SyntaxTreeNode[] { $1, $3, $5 }); }
	;

statement
	: labeled_statement                                                            { $$ = new StatementSTN(1, n_linecnt, new SyntaxTreeNode[] { $1 }); }
	| expression_statement                                                         { $$ = new StatementSTN(2, n_linecnt, new SyntaxTreeNode[] { $1 }); }
	| selection_statement                                                          { $$ = new StatementSTN(3, n_linecnt, new SyntaxTreeNode[] { $1 }); }
	| iteration_statement                                                          { $$ = new StatementSTN(4, n_linecnt, new SyntaxTreeNode[] { $1 }); }
	| jump_statement                                                               { $$ = new StatementSTN(5, n_linecnt, new SyntaxTreeNode[] { $1 }); }
	| error	error_end                                                              { $$ = new StatementSTN(6, n_linecnt, new SyntaxTreeNode[] {    }); }
	;

assignment_expression
	: conditional_expression                                                       { $$ = new AssignmentExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1         }); }
	| unary_expression assignment_operator assignment_expression                   { $$ = new AssignmentExpressionSTN(2, n_linecnt, new SyntaxTreeNode[] { $1, $2, $3 }); }
	| unary_expression LEFT_ARROW assignment_expression                            { $$ = new AssignmentExpressionSTN(3, n_linecnt, new SyntaxTreeNode[] { $1, $3     }); }
	;
	
logical_or_expression
	: logical_and_expression                                                       { $$ = new LogicalOrExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1     }); }
	| logical_or_expression OR_OP logical_and_expression                           { $$ = new LogicalOrExpressionSTN(2, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); }
	;

expression
	: assignment_expression                                                        { $$ = new ExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1     }); }
	| expression C_CM assignment_expression                                        { $$ = new ExpressionSTN(2, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); }
	;

labeled_statement
	: IDENTIFIER C_DP statement                                                    { $$ = new LabeledStatementSTN(1, n_linecnt, new SyntaxTreeNode[] { new IdentifierSTL(1, n_linecnt), $3 }); }
	| CASE constant_expression C_DP statement                                      { $$ = new LabeledStatementSTN(2, n_linecnt, new SyntaxTreeNode[] { $2, $4                              }); }
	| DEFAULT C_DP statement                                                       { $$ = new LabeledStatementSTN(3, n_linecnt, new SyntaxTreeNode[] { $3                                  }); }
	;

expression_statement
	: C_SC                                                                         { $$ = new ExpressionStatementSTN(1, n_linecnt);                              }
	| expression C_SC                                                              { $$ = new ExpressionStatementSTN(2, n_linecnt, new SyntaxTreeNode[] { $1 }); }
	;

selection_statement
	: IF C_OP expression C_CP compound_statement                                   { $$ = new SelectionStatementSTN(1, n_linecnt, new SyntaxTreeNode[] { $3, $5     }); }
	| IF C_OP expression C_CP compound_statement ELSE compound_statement           { $$ = new SelectionStatementSTN(2, n_linecnt, new SyntaxTreeNode[] { $1, $5, $7 }); }
	| SWITCH C_OP expression C_CP compound_statement                               { $$ = new SelectionStatementSTN(3, n_linecnt, new SyntaxTreeNode[] { $2, $5     }); }
	;

iteration_statement
	: WHILE C_OP expression C_CP statement                                         { $$ = new IterationStatementSTN(1, n_linecnt, new SyntaxTreeNode[] { $3, $5         }); }
	| DO statement WHILE C_OP expression C_CP C_SC                                 { $$ = new IterationStatementSTN(2, n_linecnt, new SyntaxTreeNode[] { $2, $5         }); }
	| FOR C_OP expression_statement expression_statement C_CP statement            { $$ = new IterationStatementSTN(3, n_linecnt, new SyntaxTreeNode[] { $3, $4, $6     }); }
	| FOR C_OP expression_statement expression_statement expression C_CP statement { $$ = new IterationStatementSTN(4, n_linecnt, new SyntaxTreeNode[] { $3, $4, $5, $7 }); }
	;

jump_statement
	: GOTO IDENTIFIER C_SC                                                         { $$ = new JumpStatementSTN(1, n_linecnt, new SyntaxTreeNode[] { new IdentifierSTL(1, n_linecnt) }); }
	| CONTINUE C_SC                                                                { $$ = new JumpStatementSTN(3, n_linecnt);                                                           }
	| BREAK C_SC                                                                   { $$ = new JumpStatementSTN(4, n_linecnt);                                                           }
	| RETURN C_SC                                                                  { $$ = new JumpStatementSTN(5, n_linecnt);                                                           }
	| RETURN expression C_SC                                                       { $$ = new JumpStatementSTN(6, n_linecnt, new SyntaxTreeNode[] { $2                              }); }
	;

unary_expression
	: postfix_expression                                                           { $$ = new UnaryExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1     }); }
	| INC_OP unary_expression                                                      { $$ = new UnaryExpressionSTN(2, n_linecnt, new SyntaxTreeNode[] { $2     }); }
	| DEC_OP unary_expression                                                      { $$ = new UnaryExpressionSTN(3, n_linecnt, new SyntaxTreeNode[] { $2     }); }
	| unary_operator cast_expression                                               { $$ = new UnaryExpressionSTN(4, n_linecnt, new SyntaxTreeNode[] { $1, $2 }); }
	| SIZEOF unary_expression                                                      { $$ = new UnaryExpressionSTN(5, n_linecnt, new SyntaxTreeNode[] { $2     }); }
	| SIZEOF C_OP type_specifier C_CP                                              { $$ = new UnaryExpressionSTN(6, n_linecnt, new SyntaxTreeNode[] { $3     }); }
	| LOBYTE unary_expression                                                      { $$ = new UnaryExpressionSTN(7, n_linecnt, new SyntaxTreeNode[] { $2     }); }
	| HIBYTE unary_expression                                                      { $$ = new UnaryExpressionSTN(8, n_linecnt, new SyntaxTreeNode[] { $2     }); }
	;

assignment_operator
	: C_ES                                                                         { $$ = new AssignmentOperatorSTN( 1, n_linecnt); }
	| MUL_ASSIGN                                                                   { $$ = new AssignmentOperatorSTN( 2, n_linecnt); /* Not supported by 6502 */ }
	| DIV_ASSIGN                                                                   { $$ = new AssignmentOperatorSTN( 3, n_linecnt); /* Not supported by 6502 */ }
	| MOD_ASSIGN                                                                   { $$ = new AssignmentOperatorSTN( 4, n_linecnt); /* Not supported by 6502 */ }
	| ADD_ASSIGN                                                                   { $$ = new AssignmentOperatorSTN( 5, n_linecnt); }
	| SUB_ASSIGN                                                                   { $$ = new AssignmentOperatorSTN( 6, n_linecnt); }
	| LEFT_ASSIGN                                                                  { $$ = new AssignmentOperatorSTN( 7, n_linecnt); }
	| RIGHT_ASSIGN                                                                 { $$ = new AssignmentOperatorSTN( 8, n_linecnt); }
	| AND_ASSIGN                                                                   { $$ = new AssignmentOperatorSTN( 9, n_linecnt); }
	| XOR_ASSIGN                                                                   { $$ = new AssignmentOperatorSTN(10, n_linecnt); }
	| OR_ASSIGN                                                                    { $$ = new AssignmentOperatorSTN(11, n_linecnt); }
	;

logical_and_expression
	: inclusive_or_expression                                                      { $$ = new LogicalAndExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1     }); }
	| logical_and_expression AND_OP inclusive_or_expression                        { $$ = new LogicalAndExpressionSTN(2, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); }
	;

postfix_expression
	: primary_expression                                                           { $$ = new PostfixExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1     }); }
	| postfix_expression C_OB expression C_CB                                      { $$ = new PostfixExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); }
	| postfix_expression C_OP C_CP                                                 { $$ = new PostfixExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1     }); }
	| postfix_expression C_OP argument_expression_list C_CP                        { $$ = new PostfixExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); }
	| postfix_expression INC_OP                                                    { $$ = new PostfixExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1     }); }
	| postfix_expression DEC_OP                                                    { $$ = new PostfixExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1     }); }
	;

unary_operator
	: C_A                                                                          { $$ = new UnaryOperatorSTN(1, n_linecnt); }
	| C_AK                                                                         { $$ = new UnaryOperatorSTN(2, n_linecnt); }
	| C_PS                                                                         { $$ = new UnaryOperatorSTN(3, n_linecnt); }
	| C_MS                                                                         { $$ = new UnaryOperatorSTN(4, n_linecnt); }
	| C_NS                                                                         { $$ = new UnaryOperatorSTN(5, n_linecnt); }
	| C_EM                                                                         { $$ = new UnaryOperatorSTN(6, n_linecnt); }
	;

cast_expression
	: unary_expression                                                             { $$ = new CastExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1     }); }
	| C_OP type_specifier C_CP cast_expression                                     { $$ = new CastExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $2, $4 }); }
	;

primary_expression
	: IDENTIFIER                                                                   { $$ = new PrimaryExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { new IdentifierSTL(1, n_linecnt)    }); }
	| NUMBER_LITERAL                                                               { $$ = new PrimaryExpressionSTN(2, n_linecnt, new SyntaxTreeNode[] { new NumberLiteralSTL(1, n_linecnt) }); }
	| STRING_LITERAL                                                               { $$ = new PrimaryExpressionSTN(3, n_linecnt, new SyntaxTreeNode[] { new StringLiteralSTL(1, n_linecnt) }); }
	| C_OP expression C_CP                                                         { $$ = new PrimaryExpressionSTN(4, n_linecnt, new SyntaxTreeNode[] { $2                                 }); }
	;

inclusive_or_expression
	: exclusive_or_expression                                                      { $$ = new InclusiveOrExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1     }); }
	| inclusive_or_expression C_PP exclusive_or_expression                         { $$ = new InclusiveOrExpressionSTN(2, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); }
	;

argument_expression_list
	: assignment_expression                                                        { $$ = new ArgumentExpressionListSTN(1, n_linecnt, new SyntaxTreeNode[] { $1 }); }
	| argument_expression_list C_CM assignment_expression                          { $1.addChildren(new SyntaxTreeNode[] { $3 }); $$ = $1;                          }
	;

exclusive_or_expression
	: and_expression                                                               { $$ = new ExclusiveOrExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1     }); }
	| exclusive_or_expression C_UA and_expression                                  { $$ = new ExclusiveOrExpressionSTN(2, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); }
	;

and_expression
	: equality_expression                                                          { $$ = new AndExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1     }); }
	| and_expression C_A equality_expression                                       { $$ = new AndExpressionSTN(2, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); }
	;

equality_expression
	: relational_expression                                                        { $$ = new EqualityExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1     }); }
	| equality_expression EQ_OP relational_expression                              { $$ = new EqualityExpressionSTN(2, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); }
	| equality_expression NE_OP relational_expression                              { $$ = new EqualityExpressionSTN(3, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); }
	;

relational_expression
	: shift_expression                                                             { $$ = new RelationalExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1     }); }
	| relational_expression C_GT shift_expression                                  { $$ = new RelationalExpressionSTN(2, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); }
	| relational_expression C_LT shift_expression                                  { $$ = new RelationalExpressionSTN(3, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); }
	| relational_expression LE_OP shift_expression                                 { $$ = new RelationalExpressionSTN(4, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); }
	| relational_expression GE_OP shift_expression                                 { $$ = new RelationalExpressionSTN(5, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); }
	;
	
shift_expression
	: additive_expression                                                          { $$ = new ShiftExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1     }); }
	| shift_expression LEFT_OP additive_expression                                 { $$ = new ShiftExpressionSTN(2, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); }
	| shift_expression RIGHT_OP additive_expression                                { $$ = new ShiftExpressionSTN(3, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); }
	;

additive_expression
	: multiplicative_expression                                                    { $$ = new AdditiveExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1     }); }
	| additive_expression C_PS multiplicative_expression                           { $$ = new AdditiveExpressionSTN(2, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); }
	| additive_expression C_MS multiplicative_expression                           { $$ = new AdditiveExpressionSTN(3, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); }
	;

multiplicative_expression
	: cast_expression                                                              { $$ = new MultiplicativeExpressionSTN(1, n_linecnt, new SyntaxTreeNode[] { $1     });                             }
	| multiplicative_expression C_AK cast_expression                               { $$ = new MultiplicativeExpressionSTN(2, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); /* Not supported by 6502 */ }
	| multiplicative_expression C_SS cast_expression                               { $$ = new MultiplicativeExpressionSTN(3, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); /* Not supported by 6502 */ }
	| multiplicative_expression C_PC cast_expression                               { $$ = new MultiplicativeExpressionSTN(4, n_linecnt, new SyntaxTreeNode[] { $1, $3 }); /* Not supported by 6502 */ }
	;
%%

// No argument CTOR. By deafult Parser's ctor requires scanner as param.
public Parser(Scanner scn) : base(scn) { }
