/**
* |------------------------------------------|
* | CO2.RetroCPL COMPILER OPTIMIZER 2 RETROC |
* | File: RetroCPLParser.y                   |
* | v1.00, December 2017                     |
* | Author: Emilio Arango Delgado de Mendoza |
* |------------------------------------------|
*/

%using CO2.RetroCPL.Commons;

%namespace CO2.RetroCPL.FrontEnd

%{
	//#define YYSTYPE object
	//SymbolTable symTable = SymbolTable.GetInstance;
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
%token BYTE WORD LONGWORD SIGNED UNSIGNED CONST VOLATILE
%token STRUCT UNION ENUM ELLIPSIS

%token CASE DEFAULT INTERRUPT IF ELSE SWITCH WHILE DO FOR GOTO CONTINUE BREAK RETURN


// ------------------------------------------
// Grammar rules
// ------------------------------------------
%%

translation_unit
	: external_declaration                                                         { }
	| translation_unit external_declaration                                        { }
	;

external_declaration
	: declaration                                                                  { }
	| function_definition                                                          { }
	| error	error_end                                                              { }
	;

declaration
	: declaration_specifiers C_SC                                                  { }
	| declaration_specifiers init_declarator_list C_SC                             { }
	;

function_definition
	: declaration_specifiers direct_declarator declaration_list compound_statement { }
	| declaration_specifiers direct_declarator compound_statement                  { }
	| direct_declarator declaration_list compound_statement                        { }
	| direct_declarator compound_statement                                         { }
	;

error_end
	: C_SC                                                                         { }
	| C_CP                                                                         { }
	| C_OK                                                                         { }
	| C_CK                                                                         { }
	;

declaration_specifiers
	: type_specifier                                                               { }
	| type_specifier declaration_specifiers                                        { }
	| type_qualifier                                                               { }
	| type_qualifier declaration_specifiers                                        { }
	;

init_declarator_list
	: init_declarator                                                              { }
	| init_declarator_list C_CM init_declarator                                    { }
	;

direct_declarator
	: IDENTIFIER                                                                   { }
	| C_OP direct_declarator C_CP                                                  { }
	| direct_declarator C_OP parameter_list C_CP                                   { }
	| direct_declarator C_OP identifier_list C_CP                                  { }
	| direct_declarator C_OP C_CP                                                  { }
	| direct_declarator C_OB constant_expression C_CB                              { }
	;

declaration_list
	: declaration                                                                  { }
	| declaration_list declaration                                                 { }
	| error declaration                                                            { }
	| declaration_list error declaration                                           { }
	;

compound_statement
	: C_OK C_CK                                                                    { }
	| C_OK statement_list C_CK                                                     { }
	| C_OK declaration_list C_CK                                                   { }
	| C_OK declaration_list statement_list C_CK                                    { }
	;

type_specifier
	: BYTE                                                                         { }
	| WORD                                                                         { }
	| LONGWORD                                                                     { }
	| SIGNED                                                                       { }
	| UNSIGNED                                                                     { }
	;

type_qualifier
	: CONST                                                                        { }
	| VOLATILE                                                                     { }
	| INTERRUPT                                                                    { }
	;

init_declarator
	: direct_declarator                                                            { }
	| direct_declarator C_ES initializer                                           { }
	;

parameter_list
	: parameter_declaration                                                        { }
	| parameter_list C_CM parameter_declaration                                    { }
	;

identifier_list
	: IDENTIFIER                                                                   { }
	| identifier_list C_CM IDENTIFIER                                              { }
	;

constant_expression
	: conditional_expression                                                       { }
	;

statement_list
	: statement                                                                    { }
	| statement_list statement                                                     { }
	;

initializer
	: assignment_expression                                                        { }
	;

parameter_declaration
	: declaration_specifiers direct_declarator                                     { }
	| declaration_specifiers                                                       { }
	;

conditional_expression
	: logical_or_expression                                                        { }
	| logical_or_expression C_QM expression C_DP conditional_expression            { }
	;

statement
	: labeled_statement                                                            { }
	| expression_statement                                                         { }
	| selection_statement                                                          { }
	| iteration_statement                                                          { }
	| jump_statement                                                               { }
	| error	error_end                                                              { }
	;

assignment_expression
	: conditional_expression                                                       { }
	| unary_expression assignment_operator assignment_expression                   { }
	| unary_expression LEFT_ARROW assignment_expression                            { }
	;
	
logical_or_expression
	: logical_and_expression                                                       { }
	| logical_or_expression OR_OP logical_and_expression                           { }
	;

expression
	: assignment_expression                                                        { }
	| expression C_CM assignment_expression                                        { }
	;

labeled_statement
	: IDENTIFIER C_DP statement                                                    { }
	| CASE constant_expression C_DP statement                                      { }
	| DEFAULT C_DP statement                                                       { }
	;

expression_statement
	: C_SC                                                                         { }
	| expression C_SC                                                              { }
	;

selection_statement
	: IF C_OP expression C_CP compound_statement                                   { }
	| IF C_OP expression C_CP compound_statement ELSE compound_statement           { }
	| SWITCH C_OP expression C_CP compound_statement                               { }
	;

iteration_statement
	: WHILE C_OP expression C_CP statement                                         { }
	| DO statement WHILE C_OP expression C_CP C_SC                                 { }
	| FOR C_OP expression_statement expression_statement C_CP statement            { }
	| FOR C_OP expression_statement expression_statement expression C_CP statement { }
	;

jump_statement
	: GOTO IDENTIFIER C_SC                                                         { }
	| CONTINUE C_SC                                                                { }
	| BREAK C_SC                                                                   { }
	| RETURN C_SC                                                                  { }
	| RETURN expression C_SC                                                       { }
	;

unary_expression
	: postfix_expression                                                           { }
	| INC_OP unary_expression                                                      { }
	| DEC_OP unary_expression                                                      { }
	| unary_operator cast_expression                                               { }
	| SIZEOF unary_expression                                                      { }
	| SIZEOF C_OP type_specifier C_CP                                              { }
	| LOBYTE unary_expression                                                      { }
	| HIBYTE unary_expression                                                      { }
	;

assignment_operator
	: C_ES                                                                         { }
	| MUL_ASSIGN                                                                   { /* Not supported by 6502 */ }
	| DIV_ASSIGN                                                                   { /* Not supported by 6502 */ }
	| MOD_ASSIGN                                                                   { /* Not supported by 6502 */ }
	| ADD_ASSIGN                                                                   { }
	| SUB_ASSIGN                                                                   { }
	| LEFT_ASSIGN                                                                  { }
	| RIGHT_ASSIGN                                                                 { }
	| AND_ASSIGN                                                                   { }
	| XOR_ASSIGN                                                                   { }
	| OR_ASSIGN                                                                    { }
	;

logical_and_expression
	: inclusive_or_expression                                                      { }
	| logical_and_expression AND_OP inclusive_or_expression                        { }
	;

postfix_expression
	: primary_expression                                                           { }
	| postfix_expression C_OB expression C_CB                                      { }
	| postfix_expression C_OP C_CP                                                 { }
	| postfix_expression C_OP argument_expression_list C_CP                        { }
	| postfix_expression INC_OP                                                    { }
	| postfix_expression DEC_OP                                                    { }
	;

unary_operator
	: C_A                                                                          { }
	| C_AK                                                                         { }
	| C_PS                                                                         { }
	| C_MS                                                                         { }
	| C_NS                                                                         { }
	| C_EM                                                                         { }
	;

cast_expression
	: unary_expression                                                             { }
	| C_OP type_specifier C_CP cast_expression                                     { }
	;

primary_expression
	: IDENTIFIER                                                                   { }
	| NUMBER_LITERAL                                                               { }
	| STRING_LITERAL                                                               { }
	| C_OP expression C_CP                                                         { }
	;

inclusive_or_expression
	: exclusive_or_expression                                                      { }
	| inclusive_or_expression C_PP exclusive_or_expression                         { }
	;

argument_expression_list
	: assignment_expression                                                        { }
	| argument_expression_list C_CM assignment_expression                          { }
	;

exclusive_or_expression
	: and_expression                                                               { }
	| exclusive_or_expression C_UA and_expression                                  { }
	;

and_expression
	: equality_expression                                                          { }
	| and_expression C_A equality_expression                                       { }
	;

equality_expression
	: relational_expression                                                        { }
	| equality_expression EQ_OP relational_expression                              { }
	| equality_expression NE_OP relational_expression                              { }
	;

relational_expression
	: shift_expression                                                             { }
	| relational_expression C_GT shift_expression                                  { }
	| relational_expression C_LT shift_expression                                  { }
	| relational_expression LE_OP shift_expression                                 { }
	| relational_expression GE_OP shift_expression                                 { }
	;
	
shift_expression
	: additive_expression                                                          { }
	| shift_expression LEFT_OP additive_expression                                 { }
	| shift_expression RIGHT_OP additive_expression                                { }
	;

additive_expression
	: multiplicative_expression                                                    { }
	| additive_expression C_PS multiplicative_expression                           { }
	| additive_expression C_MS multiplicative_expression                           { }
	;

multiplicative_expression
	: cast_expression                                                              { }
	| multiplicative_expression C_AK cast_expression                               { /* Not supported by 6502 */ }
	| multiplicative_expression C_SS cast_expression                               { /* Not supported by 6502 */ }
	| multiplicative_expression C_PC cast_expression                               { /* Not supported by 6502 */ }
	;
%%

// No argument CTOR. By deafult Parser's ctor requires scanner as param.
public Parser(Scanner scn) : base(scn) { }
