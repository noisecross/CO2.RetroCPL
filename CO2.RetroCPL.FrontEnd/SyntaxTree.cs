using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO2.RetroCPL.Commons;

namespace CO2.RetroCPL.FrontEnd
{
    public class SyntaxTree
    {
        SyntaxTreeNode _root = null;

        private static SyntaxTree _uniqueSyntaxTree = null;
        public  static SyntaxTree Instance
        {
            get
            {
                if (_uniqueSyntaxTree == null)
                    _uniqueSyntaxTree = new SyntaxTree();
                return _uniqueSyntaxTree;
            }
        }

        public void setRoot(SyntaxTreeNode root)
        {
            _root = root;
        }

        public static string toString()
        {
            if (_uniqueSyntaxTree == null || Instance._root == null)
                return string.Empty;

            return Instance._root.toString();
        }
    }

    public class SyntaxTreeNode
    {
        protected int                  rule        = -1;
        protected int                  line        = -1;
        public    int                  value       = 0;
        public    bool                 valueIsUsed = false;
        protected bool                 returnDone  = false;
        public    string               lex         = string.Empty;
        protected List<SyntaxTreeNode> children    = new List<SyntaxTreeNode>();
        public    QualifiedType        type        = new QualifiedType();

        public SyntaxTreeNode(){ }
        public SyntaxTreeNode(int rule, int line)
        {
            this.rule = rule;
            this.line = line;
        }
        public SyntaxTreeNode(int rule, int line, SyntaxTreeNode[] children)
        {
            this.rule = rule;
            this.line = line;

            foreach (SyntaxTreeNode item in children)
                this.children.Add(item);
        }

        public void addChildren(SyntaxTreeNode[] children)
        {
            foreach (SyntaxTreeNode item in children)
                this.children.Add(item);
        }

        public string getNodeType() { return this.GetType().Name; }

        public virtual QualifiedType typeCheck() { return new QualifiedType(); }

        protected static void addError(string errMessage, string lex, int line)
        {
            string errOutput = string.Format("\t{0} ({1}){2}\t^{3}", lex, line, Environment.NewLine, errMessage);
            ErrManager.Instance.addError(errOutput);
        }

        public string toString()
        {
            return toString("   ");
        }

        private string toString(string prefix)
        {
            string output = string.Format("{0}+-{1} : ({2})", Helper.substr(prefix, (prefix.Length - 3)), getNodeType(), type.toString());
            if (lex != "")   output += string.Format(" ({0})", lex);
            if (valueIsUsed) output += string.Format(" ({0})", value);
            output += Environment.NewLine;

            int size = children.Count;
            foreach (var item in children)
            {
                size--;
                string newPrefix = prefix + ((size > 0) ? "|  " : "   ");

                if (item == null)
                    output += string.Format("{0}<ERROR>", newPrefix);
                else
                    output += string.Format("{0}", item.toString(newPrefix));
            }

            return output;
        }
    }

    public class TranslationUnitSTN : SyntaxTreeNode
    {
        // : external_declaration
        // | translation_unit external_declaration
        // ;

        public TranslationUnitSTN(int rule, int line) : base(rule, line) { }
        public TranslationUnitSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
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
            //TODO
            return new QualifiedType();
        }
    }

    public class DeclarationSTN : SyntaxTreeNode
    {
        // : declaration_specifiers C_SC
        // | declaration_specifiers init_declarator_list C_SC
        // ;

        public DeclarationSTN(int rule, int line) : base(rule, line) { }
        public DeclarationSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class FunctionDefinitionSTN : SyntaxTreeNode
    {
        // : declaration_specifiers direct_declarator declaration_list compound_statement
        // | declaration_specifiers direct_declarator compound_statement
        // | direct_declarator declaration_list compound_statement
        // | direct_declarator compound_statement
        // ;
        
        public FunctionDefinitionSTN(int rule, int line) : base(rule, line) { }
        public FunctionDefinitionSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }
        
        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
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
            //TODO
            return new QualifiedType();
        }
    }

    public class InitDeclaratorListSTN : SyntaxTreeNode
    {
        // : init_declarator
        // | init_declarator_list C_CM init_declarator
        // ;

        public InitDeclaratorListSTN(int rule, int line) : base(rule, line) { }
        public InitDeclaratorListSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
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

        public DirectDeclaratorSTN(int rule, int line) : base(rule, line) { }
        public DirectDeclaratorSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
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
            //TODO
            return new QualifiedType();
        }
    }

    public class CompoundStatementSTN : SyntaxTreeNode
    {
        // : C_OK C_CK
        // | C_OK statement_list C_CK
        // | C_OK declaration_list C_CK
        // | C_OK declaration_list statement_list C_CK
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
        // : BYTE
        // | WORD
        // | LONGWORD
        // | SIGNED
        // | UNSIGNED
        // ;

        public TypeSpecifierSTN(int rule, int line) : base(rule, line) { }
        public TypeSpecifierSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class TypeQualifierSTN : SyntaxTreeNode
    {
        // : CONST
        // | VOLATILE
        // | INTERRUPT
        // ;

        public TypeQualifierSTN(int rule, int line) : base(rule, line) { }
        public TypeQualifierSTN(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children) { }

        public override QualifiedType typeCheck()
        {
            //TODO
            return new QualifiedType();
        }
    }

    public class InitDeclaratorSTN : SyntaxTreeNode
    {
        // : direct_declarator
        // | direct_declarator C_ES initializer
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
        // | parameter_list C_CM parameter_declaration
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
        // | identifier_list C_CM IDENTIFIER
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
        public IdentifierSTL(int rule, int line) : base(rule, line)
        {
            lex = SymbolsTable.Instance.popTempIdentifier();
        }
        public IdentifierSTL(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children)
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
        public NumberLiteralSTL(int rule, int line) : base(rule, line)
        {
            getLiteralValue();
        }
        public NumberLiteralSTL(int rule, int line, SyntaxTreeNode[] children) : base(rule, line, children)
        {
            getLiteralValue();
        }

        private void getLiteralValue()
        {
            string newLiteral = SymbolsTable.Instance.popTempLiteral();
            type.type   = Constants.TYPE_LITERAL;
		    valueIsUsed = true;
            
            if(newLiteral[0] == '0' && newLiteral.Length > 1)
            {
			    if(newLiteral[1] == 'x')
                    value = Convert.ToInt32(newLiteral, 16); //Hex
                else
				    value = Convert.ToInt32(newLiteral, 8);  //Oct
		    }
            else if(newLiteral[0] != '\'')
                value = Convert.ToInt32(newLiteral);         //Decimal
		    else
            {
                if (newLiteral.Length != 3)                  // Char
                    addError(ErrorMessages.ERR_SEM_MSG_05, newLiteral, line);
			    else
                {
				    value           = (int) newLiteral[1];
				    type.type       = "byte";
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
