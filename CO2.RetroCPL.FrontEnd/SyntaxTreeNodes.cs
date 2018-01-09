using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO2.RetroCPL.Commons;

namespace CO2.RetroCPL.FrontEnd
{
    public class TranslationUnitSTN : SyntaxTreeNode
    {
        // : external_declaration
        // | translation_unit external_declaration
        // ;

        public TranslationUnitSTN(int rule, int line) : base(rule, line) { }
        public TranslationUnitSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            inheritTypeFromChildren();
            return type;
        }
    }

    public class ExternalDeclarationSTN : SyntaxTreeNode
    {
        // : declaration
        // | function_definition
        // ;

        public ExternalDeclarationSTN(int rule, int line) : base(rule, line) { }
        public ExternalDeclarationSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            type = children[0].type.Clone();
            return type;
        }
    }

    public class DeclarationSTN : SyntaxTreeNode
    {
        // : declaration_specifiers ';'
        // | declaration_specifiers init_declarator_list ';'
        // ;

        public DeclarationSTN(int rule, int line) : base(rule, line) { }
        public DeclarationSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //Get the specifiers and return error if the result is an error
            children[0].typeCheck();
            type = children[0].type.Clone();
            if (type.type == Constants.TYPE_ERR)
                return type;

            //If there are declarators to set, push the specifier, declare and pop it
            if (children.Count > 1)
            {
                SymbolsTable.Instance.pushTempSpecifier(type);
                children[1].typeCheck();
                SymbolsTable.Instance.popTempSpecifier();

                if (children[1].type.type == Constants.TYPE_ERR)
                    return children[1].type;
            }

            return type;
        }
    }

    public class FunctionDefinitionSTN : SyntaxTreeNode
    {
        // : declaration_specifiers direct_declarator declaration_list compound_statement
        // | declaration_specifiers direct_declarator compound_statement
        // DEPRECATED | direct_declarator declaration_list compound_statement
        // DEPRECATED | direct_declarator compound_statement
        // ;

        public FunctionDefinitionSTN(int rule, int line) : base(rule, line) { }
        public FunctionDefinitionSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //Get the specifiers
            children[0].typeCheck();
            type = children[0].type.Clone();

            //TODO get the declarator lex
            //TODO Add the new framework
            //TODO Add the parameter_list / identifier_list
            //TODO Check the compound_statement
            return type;
        }
    }

    public class DeclarationSpecifiersSTN : SyntaxTreeNode
    {
        // : type_specifier
        // | type_specifier declaration_specifiers
        // | type_qualifier
        // | type_qualifier declaration_specifiers
        // ;

        public DeclarationSpecifiersSTN(int rule, int line) : base(rule, line) { }
        public DeclarationSpecifiersSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            setMainType();
            setSpecifier();
            setQualifier();

            return type;
        }

        private void setMainType()
        {
            List<string> types = getTypes();
            if (types.Count <= 0)
                addError(ErrorMessages.ERR_SIN_MSG_02, line);
            else if (types.Count > 1)
                addError(ErrorMessages.ERR_SEM_MSG_07, string.Format("{0} / {1}", types[0], types[1]), line);
            else
                type.type = types[0];
        }

        private void setSpecifier()
        {
            List<string> specifiers = getSpecifiers();
            if (specifiers.Count > 1)
                addError(ErrorMessages.ERR_SEM_MSG_07, string.Format("{0} / {1}", specifiers[0], specifiers[1]), line);
            else if (specifiers.Count == 1)
            {
                switch(specifiers[0]){
                    case "signed":
                        type.b_unsigned = false;
                        break;
                    case "unsigned":
                        type.b_unsigned = true;
                        break;
                    default:
                        break;
                };
            }
        }

        private void setQualifier()
        {
            List<string> qualifiers = getQualifiers();
            if (qualifiers.Count > 1)
                addError(ErrorMessages.ERR_SEM_MSG_07, string.Format("{0} / {1}", qualifiers[0], qualifiers[1]), line);
            else if (qualifiers.Count == 1)
            {
                switch (qualifiers[0])
                {
                    case "const":
                        type.b_const = true;
                        break;
                    case "volatile":
                        type.b_volatile = true;
                        break;
                    case "interrupt":
                        type.b_interrupt = true;
                        break;
                    default:
                        break;
                };
            }
        }

        private List<string> getTypes()
        {
            List<string> output = new List<string>();

            foreach (SyntaxTreeNode child in children)
                if (child is TypeSpecifierSTN && child.rule < 5)
                    output.Add(child.lex);

            return output;
        }

        private List<string> getSpecifiers()
        {
            List<string> output = new List<string>();

            foreach (SyntaxTreeNode child in children)
                if (child is TypeSpecifierSTN && child.rule >= 5)
                    output.Add(child.lex);

            return output;
        }

        private List<string> getQualifiers()
        {
            List<string> output = new List<string>();

            foreach (SyntaxTreeNode child in children)
                if (child is TypeQualifierSTN)
                    output.Add(child.lex);

            return output;
        }
    }

    public class InitDeclaratorListSTN : SyntaxTreeNode
    {
        // : init_declarator
        // | init_declarator_list ',' init_declarator
        // ;

        public InitDeclaratorListSTN(int rule, int line) : base(rule, line) { }
        public InitDeclaratorListSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //These will be declared on DirectDeclaratorSTN
            foreach (SyntaxTreeNode child in children)
                child.typeCheck();

            //Return type
            type = SymbolsTable.Instance.peekTempSpecifier();
            return type;
        }
    }

    public class DirectDeclaratorSTN : SyntaxTreeNode
    {
        //TODO declararlos todos utilizando el SymbolsTable.Instance.peekTempSpecifier()

        // : IDENTIFIER
        // | '(' direct_declarator ')'
        // | IDENTIFIER '(' parameter_list ')'
        // | IDENTIFIER '(' identifier_list ')'
        // | IDENTIFIER '(' ')'
        // | direct_declarator '[' constant_expression ']'
        // ;

        public DirectDeclaratorSTN(int rule, int line) : base(rule, line) { }
        public DirectDeclaratorSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            lex = children[0].lex;

            switch (rule)
            {
                //TODO dar de alta en la tabla de símbolos
            };

            return new QualifiedType();
        }
    }

    public class DeclarationListSTN : SyntaxTreeNode
    {
        // : declaration
        // | declaration_list declaration
        // | error declaration
        // | declaration_list error declaration
        // ;

        public DeclarationListSTN(int rule, int line) : base(rule, line) { }
        public DeclarationListSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            foreach (SyntaxTreeNode child in children)
                child.typeCheck();

            return new QualifiedType();
        }
    }

    public class CompoundStatementSTN : SyntaxTreeNode
    {
        // : '{' '}'
        // | '{' statement_list '}'
        // | '{' declaration_list '}'
        // | '{' declaration_list statement_list '}'
        // ;

        public CompoundStatementSTN(int rule, int line) : base(rule, line) { }
        public CompoundStatementSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class TypeSpecifierSTN : SyntaxTreeNode
    {
        // : VOID
        // | BYTE
        // | WORD
        // | LONGWORD
        // | SIGNED
        // | UNSIGNED
        // ;

        public TypeSpecifierSTN(int rule, int line, string lex) : base(rule, line) { this.lex = lex; }
        public TypeSpecifierSTN(int rule, int line) : base(rule, line) { }
        public TypeSpecifierSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            return new QualifiedType();
        }
    }

    public class TypeQualifierSTN : SyntaxTreeNode
    {
        // : CONST
        // | VOLATILE
        // | INTERRUPT
        // ;

        public TypeQualifierSTN(int rule, int line, string lex) : base(rule, line) { this.lex = lex; }
        public TypeQualifierSTN(int rule, int line) : base(rule, line) { }
        public TypeQualifierSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            return new QualifiedType();
        }
    }

    public class InitDeclaratorSTN : SyntaxTreeNode
    {
        // : direct_declarator
        // | direct_declarator '=' initializer
        // ;

        public InitDeclaratorSTN(int rule, int line) : base(rule, line) { }
        public InitDeclaratorSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class ParameterListSTN : SyntaxTreeNode
    {
        // : parameter_declaration
        // | parameter_list ',' parameter_declaration
        // ;

        public ParameterListSTN(int rule, int line) : base(rule, line) { }
        public ParameterListSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class IdentifierListSTN : SyntaxTreeNode
    {
        // : IDENTIFIER
        // | identifier_list ',' IDENTIFIER
        // ;

        public IdentifierListSTN(int rule, int line) : base(rule, line) { }
        public IdentifierListSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class ConstantExpressionSTN : SyntaxTreeNode
    {
        // : conditional_expression
        // ;

        public ConstantExpressionSTN(int rule, int line) : base(rule, line) { }
        public ConstantExpressionSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class StatementListSTN : SyntaxTreeNode
    {
        // : statement
        // | statement_list statement
        // ;

        public StatementListSTN(int rule, int line) : base(rule, line) { }
        public StatementListSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class InitializerSTN : SyntaxTreeNode
    {
        // : assignment_expression
        // ;

        public InitializerSTN(int rule, int line) : base(rule, line) { }
        public InitializerSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class ParameterDeclarationSTN : SyntaxTreeNode
    {
        // : declaration_specifiers direct_declarator
        // | declaration_specifiers
        // ;

        public ParameterDeclarationSTN(int rule, int line) : base(rule, line) { }
        public ParameterDeclarationSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class ConditionalExpressionSTN : SyntaxTreeNode
    {
        // : logical_or_expression
        // | logical_or_expression C_QM expression C_DP conditional_expression
        // ;

        public ConditionalExpressionSTN(int rule, int line) : base(rule, line) { }
        public ConditionalExpressionSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class StatementSTN : SyntaxTreeNode
    {
        // : labeled_statement
        // | expression_statement
        // | selection_statement
        // | iteration_statement
        // | jump_statement
        // ;

        public StatementSTN(int rule, int line) : base(rule, line) { }
        public StatementSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class AssignmentExpressionSTN : SyntaxTreeNode
    {
        // : conditional_expression
        // | unary_expression assignment_operator assignment_expression
        // | unary_expression LEFT_ARROW assignment_expression
        // ;

        public AssignmentExpressionSTN(int rule, int line) : base(rule, line) { }
        public AssignmentExpressionSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class LogicalOrExpressionSTN : SyntaxTreeNode
    {
        // : logical_and_expression
        // | logical_or_expression OR_OP logical_and_expression
        // ;

        public LogicalOrExpressionSTN(int rule, int line) : base(rule, line) { }
        public LogicalOrExpressionSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class ExpressionSTN : SyntaxTreeNode
    {
        // : assignment_expression
        // | expression C_CM assignment_expression
        // ;

        public ExpressionSTN(int rule, int line) : base(rule, line) { }
        public ExpressionSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class LabeledStatementSTN : SyntaxTreeNode
    {
        // : IDENTIFIER C_DP statement
        // | CASE constant_expression C_DP statement
        // | DEFAULT C_DP statement
        // ;

        public LabeledStatementSTN(int rule, int line) : base(rule, line) { }
        public LabeledStatementSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class ExpressionStatementSTN : SyntaxTreeNode
    {
        // : C_SC
        // | expression C_SC
        // ;

        public ExpressionStatementSTN(int rule, int line) : base(rule, line) { }
        public ExpressionStatementSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class SelectionStatementSTN : SyntaxTreeNode
    {
        // : IF C_OP expression C_CP compound_statement
        // | IF C_OP expression C_CP compound_statement ELSE compound_statement
        // | SWITCH C_OP expression C_CP compound_statement
        // ;

        public SelectionStatementSTN(int rule, int line) : base(rule, line) { }
        public SelectionStatementSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class IterationStatementSTN : SyntaxTreeNode
    {
        // : WHILE C_OP expression C_CP statement
        // | DO statement WHILE C_OP expression C_CP C_SC
        // | FOR C_OP expression_statement expression_statement C_CP statement
        // | FOR C_OP expression_statement expression_statement expression C_CP statement
        // ;

        public IterationStatementSTN(int rule, int line) : base(rule, line) { }
        public IterationStatementSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class JumpStatementSTN : SyntaxTreeNode
    {
        // : GOTO IDENTIFIER C_SC
        // | CONTINUE C_SC
        // | BREAK C_SC
        // | RETURN C_SC
        // | RETURN expression C_SC
        // ;

        public JumpStatementSTN(int rule, int line) : base(rule, line) { }
        public JumpStatementSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
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

        public UnaryExpressionSTN(int rule, int line) : base(rule, line) { }
        public UnaryExpressionSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class AssignmentOperatorSTN : SyntaxTreeNode
    {
        // : '='
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

        public AssignmentOperatorSTN(int rule, int line) : base(rule, line) { }
        public AssignmentOperatorSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class LogicalAndExpressionSTN : SyntaxTreeNode
    {
        // : inclusive_or_expression
        // | logical_and_expression AND_OP inclusive_or_expression
        // ;

        public LogicalAndExpressionSTN(int rule, int line) : base(rule, line) { }
        public LogicalAndExpressionSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
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

        public PostfixExpressionSTN(int rule, int line) : base(rule, line) { }
        public PostfixExpressionSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
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

        public UnaryOperatorSTN(int rule, int line) : base(rule, line) { }
        public UnaryOperatorSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class CastExpressionSTN : SyntaxTreeNode
    {
        // : unary_expression
        // | C_OP type_specifier C_CP cast_expression
        // ;

        public CastExpressionSTN(int rule, int line) : base(rule, line) { }
        public CastExpressionSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class PrimaryExpressionSTN : SyntaxTreeNode
    {
        // : IDENTIFIER
        // | NUMBER_LITERAL
        // | STRING_LITERAL
        // | C_OP expression C_CP
        // ;

        public PrimaryExpressionSTN(int rule, int line) : base(rule, line) { }
        public PrimaryExpressionSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class InclusiveOrExpressionSTN : SyntaxTreeNode
    {
        // : exclusive_or_expression
        // | inclusive_or_expression C_PP exclusive_or_expression
        // ;

        public InclusiveOrExpressionSTN(int rule, int line) : base(rule, line) { }
        public InclusiveOrExpressionSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class ArgumentExpressionListSTN : SyntaxTreeNode
    {
        // : assignment_expression
        // | argument_expression_list C_CM assignment_expression
        // ;

        public ArgumentExpressionListSTN(int rule, int line) : base(rule, line) { }
        public ArgumentExpressionListSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class ExclusiveOrExpressionSTN : SyntaxTreeNode
    {
        // : and_expression
        // | exclusive_or_expression C_UA and_expression
        // ;

        public ExclusiveOrExpressionSTN(int rule, int line) : base(rule, line) { }
        public ExclusiveOrExpressionSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class AndExpressionSTN : SyntaxTreeNode
    {
        // : equality_expression
        // | and_expression C_A equality_expression
        // ;

        public AndExpressionSTN(int rule, int line) : base(rule, line) { }
        public AndExpressionSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class EqualityExpressionSTN : SyntaxTreeNode
    {
        // : relational_expression
        // | equality_expression EQ_OP relational_expression
        // | equality_expression NE_OP relational_expression
        // ;

        public EqualityExpressionSTN(int rule, int line) : base(rule, line) { }
        public EqualityExpressionSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class RelationalExpressionSTN : SyntaxTreeNode
    {
        // : shift_expression
        // | relational_expression C_GT shift_expression
        // | relational_expression C_LT shift_expression
        // | relational_expression LE_OP shift_expression
        // | relational_expression GE_OP shift_expression
        // ;

        public RelationalExpressionSTN(int rule, int line) : base(rule, line) { }
        public RelationalExpressionSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class ShiftExpressionSTN : SyntaxTreeNode
    {
        // : additive_expression
        // | shift_expression LEFT_OP additive_expression
        // | shift_expression RIGHT_OP additive_expression
        // ;

        public ShiftExpressionSTN(int rule, int line) : base(rule, line) { }
        public ShiftExpressionSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class AdditiveExpressionSTN : SyntaxTreeNode
    {
        // : multiplicative_expression
        // | additive_expression C_PS multiplicative_expression
        // | additive_expression C_MS multiplicative_expression
        // ;

        public AdditiveExpressionSTN(int rule, int line) : base(rule, line) { }
        public AdditiveExpressionSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class MultiplicativeExpressionSTN : SyntaxTreeNode
    {
        // : cast_expression
        // | multiplicative_expression C_AK cast_expression { /* Not supported by 6502 */ }
        // | multiplicative_expression C_SS cast_expression { /* Not supported by 6502 */ }
        // | multiplicative_expression C_PC cast_expression { /* Not supported by 6502 */ }
        // ;

        public MultiplicativeExpressionSTN(int rule, int line) : base(rule, line) { }
        public MultiplicativeExpressionSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class IdentifierSTL : SyntaxTreeNode
    {
        public IdentifierSTL(int rule, int line)
            : base(rule, line)
        {
            lex = SymbolsTable.Instance.popTempIdentifier();
        }
        public IdentifierSTL(int rule, int line, SyntaxTreeNode[] children)
            : base(rule, line, children)
        {
            lex = SymbolsTable.Instance.popTempIdentifier();
        }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }

    }

    public class NumberLiteralSTL : SyntaxTreeNode
    {
        public NumberLiteralSTL(int rule, int line)
            : base(rule, line)
        {
            getLiteralValue();
        }
        public NumberLiteralSTL(int rule, int line, SyntaxTreeNode[] children)
            : base(rule, line, children)
        {
            getLiteralValue();
        }

        private void getLiteralValue()
        {
            string newLiteral = SymbolsTable.Instance.popTempLiteral();
            type.type = Constants.TYPE_LITERAL;
            valueIsUsed = true;

            if (newLiteral[0] == '0' && newLiteral.Length > 1)
            {
                if (newLiteral[1] == 'x')
                    value = Convert.ToInt32(newLiteral, 16); //Hex
                else
                    value = Convert.ToInt32(newLiteral, 8);  //Oct
            }
            else if (newLiteral[0] != '\'')
                value = Convert.ToInt32(newLiteral);         //Decimal
            else
            {
                if (newLiteral.Length != 3)                  // Char
                    addError(ErrorMessages.ERR_SEM_MSG_05, newLiteral, line);
                else
                {
                    value = (int)newLiteral[1];
                    type.type = "byte";
                    type.b_unsigned = true;
                }
            }
        }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }

    }

    public class StringLiteralSTL : SyntaxTreeNode
    {
        public StringLiteralSTL(int rule, int line) : base(rule, line) { }
        public StringLiteralSTL(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }
}
