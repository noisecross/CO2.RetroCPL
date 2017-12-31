using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO2.RetroCPL.FrontEnd
{
    class SyntaxTree
    {

    }

    public class SyntaxTreeNode
    {
        public SyntaxTreeNode() { }

        public void pollo()
        {
            List<SyntaxTreeNode> list = new List<SyntaxTreeNode>();
            list.Add(new TranslationUnitSTN());
        }
    }

    public class TranslationUnitSTN : SyntaxTreeNode
    {
        // : external_declaration
        // | translation_unit external_declaration
        // ;

        public TranslationUnitSTN() { }
    }

    public class ExternalDeclarationSTN : SyntaxTreeNode
    {
        // : declaration
        // | function_definition
        // ;
    }

    public class DeclarationSTN : SyntaxTreeNode
    {
        // : declaration_specifiers C_SC
        // | declaration_specifiers init_declarator_list C_SC
        // ;
    }

    public class FunctionDefinitionSTN : SyntaxTreeNode
    {
        // : declaration_specifiers direct_declarator declaration_list compound_statement
        // | declaration_specifiers direct_declarator compound_statement
        // | direct_declarator declaration_list compound_statement
        // | direct_declarator compound_statement
        // ;
    }

    public class DeclarationSpecifiersSTN : SyntaxTreeNode
    {
        // : type_specifier
        // | type_specifier declaration_specifiers
        // | type_qualifier
        // | type_qualifier declaration_specifiers
        // ;
    }

    public class InitDeclaratorListSTN : SyntaxTreeNode
    {
        // : init_declarator
        // | init_declarator_list C_CM init_declarator
        // ;
    }

    public class DirectDeclaratorSTN : SyntaxTreeNode
    {
        // : IDENTIFIER
        // | C_OP direct_declarator C_CP
        // | direct_declarator C_OP parameter_list C_CP
        // | direct_declarator C_OP identifier_list C_CP
        // | direct_declarator C_OP C_CP
        // | direct_declarator C_OB constant_expression C_CB
        // ;
    }

    public class DeclarationListSTN : SyntaxTreeNode
    {
        // : declaration
        // | declaration_list declaration
        // | error declaration
        // | declaration_list error declaration
        // ;
    }

    public class CompoundStatementSTN : SyntaxTreeNode
    {
        // : C_OK C_CK
        // | C_OK statement_list C_CK
        // | C_OK declaration_list C_CK
        // | C_OK declaration_list statement_list C_CK
        // ;
    }

    public class TypeSpecifierSTN : SyntaxTreeNode
    {
        // : BYTE
        // | WORD
        // | LONGWORD
        // | SIGNED
        // | UNSIGNED
        // ;
    }

    public class TypeQualifierSTN : SyntaxTreeNode
    {
        // : CONST
        // | VOLATILE
        // | INTERRUPT
        // ;
    }

    public class InitDeclaratorSTN : SyntaxTreeNode
    {
        // : direct_declarator
        // | direct_declarator C_ES initializer
        // ;
    }

    public class ParameterListSTN : SyntaxTreeNode
    {
        // : parameter_declaration
        // | parameter_list C_CM parameter_declaration
        // ;
    }

    public class IdentifierListSTN : SyntaxTreeNode
    {
        // : IDENTIFIER
        // | identifier_list C_CM IDENTIFIER
        // ;
    }

    public class ConstantExpressionSTN : SyntaxTreeNode
    {
        // : conditional_expression
        // ;
    }

    public class StatementListSTN : SyntaxTreeNode
    {
        // : statement
        // | statement_list statement
        // ;
    }

    public class InitializerSTN : SyntaxTreeNode
    {
        // : assignment_expression
        // ;
    }

    public class ParameterDeclarationSTN : SyntaxTreeNode
    {
        // : declaration_specifiers direct_declarator
        // | declaration_specifiers
        // ;
    }

    public class ConditionalExpressionSTN : SyntaxTreeNode
    {
        // : logical_or_expression
        // | logical_or_expression C_QM expression C_DP conditional_expression
        // ;
    }

    public class StatementSTN : SyntaxTreeNode
    {
        // : labeled_statement
        // | expression_statement
        // | selection_statement
        // | iteration_statement
        // | jump_statement
        // ;
    }

    public class AssignmentExpressionSTN : SyntaxTreeNode
    {
        // : conditional_expression
        // | unary_expression assignment_operator assignment_expression
        // | unary_expression LEFT_ARROW assignment_expression
        // ;
    }

    public class LogicalOrExpressionSTN : SyntaxTreeNode
    {
        // : logical_and_expression
        // | logical_or_expression OR_OP logical_and_expression
        // ;
    }

    public class ExpressionSTN : SyntaxTreeNode
    {
        // : assignment_expression
        // | expression C_CM assignment_expression
        // ;
    }

    public class LabeledStatementSTN : SyntaxTreeNode
    {
        // : IDENTIFIER C_DP statement
        // | CASE constant_expression C_DP statement
        // | DEFAULT C_DP statement
        // ;
    }

    public class ExpressionStatementSTN : SyntaxTreeNode
    {
        // : C_SC
        // | expression C_SC
        // ;
    }

    public class SelectionStatementSTN : SyntaxTreeNode
    {
        // : IF C_OP expression C_CP compound_statement
        // | IF C_OP expression C_CP compound_statement ELSE compound_statement
        // | SWITCH C_OP expression C_CP compound_statement
        // ;
    }

    public class IterationStatementSTN : SyntaxTreeNode
    {
        // : WHILE C_OP expression C_CP statement
        // | DO statement WHILE C_OP expression C_CP C_SC
        // | FOR C_OP expression_statement expression_statement C_CP statement
        // | FOR C_OP expression_statement expression_statement expression C_CP statement
        // ;
    }

    public class JumpStatementSTN : SyntaxTreeNode
    {
        // : GOTO IDENTIFIER C_SC
        // | CONTINUE C_SC
        // | BREAK C_SC
        // | RETURN C_SC
        // | RETURN expression C_SC
        // ;
    }

    public class UnaryExpressionSTN : SyntaxTreeNode
    {
        // : postfix_expression
        // | INC_OP unary_expression
        // | DEC_OP unary_expression
        // | unary_operator cast_expression
        // | SIZEOF unary_expression
        // | SIZEOF C_OP type_specifier C_CP
        // | LOBYTE unary_expression
        // | HIBYTE unary_expression
        // ;
    }

    public class AssignmentOperatorSTN : SyntaxTreeNode
    {
        // : C_ES
        // | MUL_ASSIGN { /* Not supported by 6502 */ }
        // | DIV_ASSIGN { /* Not supported by 6502 */ }
        // | MOD_ASSIGN { /* Not supported by 6502 */ }
        // | ADD_ASSIGN
        // | SUB_ASSIGN
        // | LEFT_ASSIGN
        // | RIGHT_ASSIGN
        // | AND_ASSIGN
        // | XOR_ASSIGN
        // | OR_ASSIGN
        // ;
    }

    public class LogicalAndExpressionSTN : SyntaxTreeNode
    {
        // : inclusive_or_expression
        // | logical_and_expression AND_OP inclusive_or_expression
        // ;
    }

    public class PostfixExpressionSTN : SyntaxTreeNode
    {
        // : primary_expression
        // | postfix_expression C_OB expression C_CB
        // | postfix_expression C_OP C_CP
        // | postfix_expression C_OP argument_expression_list C_CP
        // | postfix_expression INC_OP
        // | postfix_expression DEC_OP
        // ;
    }

    public class UnaryOperatorSTN : SyntaxTreeNode
    {
        // : C_A
        // | C_AK
        // | C_PS
        // | C_MS
        // | C_NS
        // | C_EM
        // ;
    }

    public class CastExpressionSTN : SyntaxTreeNode
    {
        // : unary_expression
        // | C_OP type_specifier C_CP cast_expression
        // ;
    }

    public class PrimaryExpressionSTN : SyntaxTreeNode
    {
        // : IDENTIFIER
        // | NUMBER_LITERAL
        // | STRING_LITERAL
        // | C_OP expression C_CP
        // ;
    }

    public class InclusiveOrExpressionSTN : SyntaxTreeNode
    {
        // : exclusive_or_expression
        // | inclusive_or_expression C_PP exclusive_or_expression
        // ;
    }

    public class ArgumentExpressionListSTN : SyntaxTreeNode
    {
        // : assignment_expression
        // | argument_expression_list C_CM assignment_expression
        // ;
    }

    public class ExclusiveOrExpressionSTN : SyntaxTreeNode
    {
        // : and_expression
        // | exclusive_or_expression C_UA and_expression
        // ;
    }

    public class AndExpressionSTN : SyntaxTreeNode
    {
        // : equality_expression
        // | and_expression C_A equality_expression
        // ;
    }

    public class EqualityExpressionSTN : SyntaxTreeNode
    {
        // : relational_expression
        // | equality_expression EQ_OP relational_expression
        // | equality_expression NE_OP relational_expression
        // ;
    }

    public class RelationalExpressionSTN : SyntaxTreeNode
    {
        // : shift_expression
        // | relational_expression C_GT shift_expression
        // | relational_expression C_LT shift_expression
        // | relational_expression LE_OP shift_expression
        // | relational_expression GE_OP shift_expression
        // ;
    }

    public class ShiftExpressionSTN : SyntaxTreeNode
    {
        // : additive_expression
        // | shift_expression LEFT_OP additive_expression
        // | shift_expression RIGHT_OP additive_expression
        // ;
    }

    public class AdditiveExpressionSTN : SyntaxTreeNode
    {
        // : multiplicative_expression
        // | additive_expression C_PS multiplicative_expression
        // | additive_expression C_MS multiplicative_expression
        // ;
    }

    public class MultiplicativeExpressionSTN : SyntaxTreeNode
    {
        // : cast_expression
        // | multiplicative_expression C_AK cast_expression { /* Not supported by 6502 */ }
        // | multiplicative_expression C_SS cast_expression { /* Not supported by 6502 */ }
        // | multiplicative_expression C_PC cast_expression { /* Not supported by 6502 */ }
        // ;
    }
}
