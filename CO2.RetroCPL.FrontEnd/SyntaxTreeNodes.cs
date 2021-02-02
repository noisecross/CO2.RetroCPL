using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO2.RetroCPL.Commons;

namespace CO2.RetroCPL.FrontEnd
{
    // Make clone of the type when copiying, not when returning

    public class TranslationUnitSTN : SyntaxTreeNode
    {
        // : external_declaration
        // | translation_unit external_declaration
        // ;

        public TranslationUnitSTN(int rule, int line, int column) : base(rule, line, column) { }
        public TranslationUnitSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            inheritTypeFromChildren();
            return type;
        }
    }

    public class ExternalDeclarationSTN : SyntaxTreeNode
    {
        // : variables_declaration
        // | function_prototyping
        // | function_definition
        // | error error_end
        // ;

        public ExternalDeclarationSTN(int rule, int line, int column) : base(rule, line, column) { }
        public ExternalDeclarationSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            if (rule == 4) return new QualifiedType(Constants.TYPE_ERR);

            type = children[0].typeCheck().Clone();
            return type;
        }
    }

    public class VariablesDeclarationSTN : SyntaxTreeNode
    {
        // : declaration_specifiers init_declarator_list ';'
        // ;

        public VariablesDeclarationSTN(int rule, int line, int column) : base(rule, line, column) { }
        public VariablesDeclarationSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            //Get the specifiers and return error if the result is an error
            children[0].typeCheck();
            type = children[0].type.Clone();
            if (type.type == Constants.TYPE_ERR)
                return type;

            symbolsTable.pushTempQualifier(type);
            children[1].typeCheck();
            symbolsTable.popTempQualifier();

            if (children[1].type.type == Constants.TYPE_ERR)
                return children[1].type;

            return type;
        }
    }

    public class FunctionDefinitionSTN : SyntaxTreeNode
    {
        //function_definition
        //	: function_name_definition '(' parameter_list ')' compound_statement
        //	| function_name_definition '(' ')' compound_statement
        //	;

        public FunctionDefinitionSTN(int rule, int line, int column) : base(rule, line, column) { }
        public FunctionDefinitionSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            SyntaxTreeNode compoundStatement;

            type = children[0].typeCheck().Clone();
            string functionName = children[0].lex;
            symbolsTable.currentFramework = functionName;

            // If the framework already exists, check the differences
            if (symbolsTable.existsFramework(functionName))
                if (!symbolsTable.getOutputType(functionName).sameOutput(type))
                    addError(ErrorMessages.ERR_SEM_MSG_0B, functionName, line, column);

            // Add the new framework, the input types and the output type
            if (rule == 1)
            {
                children[1].typeCheck();
                compoundStatement = children[2];
            }
            else
            {
                symbolsTable.addFramework(functionName);
                compoundStatement = children[1];
            }
            symbolsTable.setOutputType(functionName, type); //TODO comprobar corrección

            // Add the "return" symbol
            if (symbolsTable.getTypeSize(type) > 0)
                symbolsTable.addSymbol(Constants.SEM_RETURN_SYMBOL, type);

            //Check the compound_statement
            syntaxTree.labelsExpected.Clear();
            compoundStatement.typeCheck();
            checkLabelsDefinition();

            // Tag the function name
            lex = functionName;

            // Last checks
            if (symbolsTable.getTypeSize(type) != 0 && !compoundStatement.returnDone)
                addWarning(ErrorMessages.WAR_SEM_MSG_03, functionName, line, column);
            symbolsTable.setAsDefined(functionName);

            symbolsTable.currentFramework = Constants.GLOBAL_FRAMEWORK;
            return type;
        }

        private static void checkLabelsDefinition()
        {
            foreach(var label in syntaxTree.labelsExpected)
            {
                if (!symbolsTable.existsLabel(symbolsTable.currentFramework, label.Key))
                    addError(ErrorMessages.ERR_SEM_MSG_12, label.Key, label.Value.Key, label.Value.Value);
            }
        }
    }

    public class FunctionPrototypingSTN : SyntaxTreeNode
    {
        //function_prototyping
        //	: function_name_definition '(' abstract_parameter_list ')' ';'
        //	| function_name_definition '(' ')' ';'
        //	;

        public FunctionPrototypingSTN(int rule, int line, int column) : base(rule, line, column) { }
        public FunctionPrototypingSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            type = children[0].typeCheck().Clone();
            string functionName = children[0].lex;

            if (symbolsTable.existsFramework(functionName))
            {
                if (!symbolsTable.getOutputType(functionName).sameOutput(type))
                {
                    addError(ErrorMessages.ERR_SEM_MSG_0B, functionName, line, column);
                    return type;
                }
            }
            else
            {
                // Add the new framework
                symbolsTable.addFramework(functionName);
                symbolsTable.setOutputType(functionName, type);
            }

            // Add input types
            if(rule == 1)
            {
                symbolsTable.currentFramework = functionName;
                children[1].typeCheck();
                symbolsTable.currentFramework = Constants.GLOBAL_FRAMEWORK;
            }

            return type;
        }
    }

    public class FunctionNameDefinitionSTN : SyntaxTreeNode
    {
	    // : declaration_specifiers pointer IDENTIFIER
	    // | declaration_specifiers IDENTIFIER
	    // ;

        public FunctionNameDefinitionSTN(int rule, int line, int column) : base(rule, line, column) { }
        public FunctionNameDefinitionSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            children[0].typeCheck();

            if (rule == 1)
            {
                type = CheckPointerAndCombineWithQualifiedType(children[0].type, children[1]);
                lex = children[2].lex;
            }
            else
            {
                type = children[0].type.Clone();
                lex = children[1].lex;
            }

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

        public DeclarationSpecifiersSTN(int rule, int line, int column) : base(rule, line, column) { }
        public DeclarationSpecifiersSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

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
                addError(ErrorMessages.ERR_SIN_MSG_02, line, column);
            else if (types.Count > 1)
                addError(ErrorMessages.ERR_SEM_MSG_07, string.Format("{0} / {1}", types[0], types[1]), line, column);
            else
                type.type = types[0];
        }

        private void setSpecifier()
        {
            List<string> specifiers = getSpecifiers();
            if (specifiers.Count > 1)
                addError(ErrorMessages.ERR_SEM_MSG_07, string.Format("{0} / {1}", specifiers[0], specifiers[1]), line, column);
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
                addError(ErrorMessages.ERR_SEM_MSG_07, string.Format("{0} / {1}", qualifiers[0], qualifiers[1]), line, column);
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

        public InitDeclaratorListSTN(int rule, int line, int column) : base(rule, line, column) { }
        public InitDeclaratorListSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            //These will be declared on DirectDeclaratorSTN
            foreach (SyntaxTreeNode child in children)
                child.typeCheck();

            //Return type
            type = symbolsTable.peekTempQualifier().Clone();
            return type;
        }
    }

    public class TypeNameSTN : SyntaxTreeNode
    {
        //type_name
        //    : declaration_specifiers
        //    | declaration_specifiers pointer
        //    ;

        public TypeNameSTN(int rule, int line, int column) : base(rule, line, column) { }
        public TypeNameSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            children[0].typeCheck();
            type = children[0].type.Clone();

            if (rule == 2)
            {
                children[1].typeCheck();
                type.pointerDepth = children[1].value;
            }

            return type;
        }
    }

    public class DeclaratorSTN : SyntaxTreeNode
    {
        //declarator
        //    : pointer direct_declarator
        //    | direct_declarator
        //    ;

        public DeclaratorSTN(int rule, int line, int column) : base(rule, line, column) { }
        public DeclaratorSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            type = symbolsTable.peekTempQualifier();

            if (rule == 1)
            {
                QualifiedType currentModifiedType = CheckPointerAndCombineWithQualifiedType(type, children[0]);
                symbolsTable.pushTempQualifier(currentModifiedType);
                children[1].typeCheck();
                symbolsTable.popTempQualifier();
                //children[1].typeCheck();
                inheritPropertiesFromNode(children[1]);
            }
            else
            {
                children[0].typeCheck();
                if (children[0].valueIsUsed)
                {
                    valueIsUsed = true;
                    value = children[0].value;
                }
                lex = children[0].lex;
            }

            return type;
        }
    }

    public class PointerSTN : SyntaxTreeNode
    {
        //pointer
        //    : '*'
        //    | '*' pointer
        //    ;

        public PointerSTN(int rule, int line, int column) : base(rule, line, column) { }
        public PointerSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            valueIsUsed = true;
            value = 1 + children.Count();
            return type;
        }
    }

    public class DirectDeclaratorSTN : SyntaxTreeNode
    {
        // : IDENTIFIER
        // | IDENTIFIER '[' constant_expression ']'
        // ;

        public DirectDeclaratorSTN(int rule, int line, int column) : base(rule, line, column) { }
        public DirectDeclaratorSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            lex = children[0].lex;
            type = symbolsTable.peekTempQualifier();

            // Declare an array
            if (rule == 2)
            {
                type.pointerDepth++;

                children[1].typeCheck();
                type.arrSize = (children[1].valueIsUsed) ? children[1].value : 0xFF;
            }

            // Check incompatible qualifiers
            if (type.b_interrupt)
            {
                type.b_interrupt = false;
                addError(ErrorMessages.ERR_SEM_MSG_08, lex, line, column);
            }

            // Declare and check redeclarations
            if(!symbolsTable.addSymbol(lex, type))
                addError(ErrorMessages.ERR_SEM_MSG_09, lex, line, column);

            return type;
        }
    }

    public class DeclarationListSTN : SyntaxTreeNode
    {
        // : variables_declaration
        // | declaration_list variables_declaration
        // | error variables_declaration
        // | declaration_list error variables_declaration
        // ;

        public DeclarationListSTN(int rule, int line, int column) : base(rule, line, column) { }
        public DeclarationListSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            foreach (SyntaxTreeNode child in children)
                child.typeCheck();

            return type;
        }
    }

    public class CompoundStatementSTN : SyntaxTreeNode
    {
        // : '{' '}'
        // | '{' statement_list '}'
        // | '{' declaration_list '}'
        // | '{' declaration_list statement_list '}'
        // ;

        public CompoundStatementSTN(int rule, int line, int column) : base(rule, line, column) { }
        public CompoundStatementSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            foreach (SyntaxTreeNode child in children)
                child.typeCheck();

            switch (rule)
            {
                case 2:
                    returnDone = children[0].returnDone;
                    break;
                case 4:
                    returnDone = children[1].returnDone;
                    break;
                default:
                    break;
            }

            return type;
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

        public TypeSpecifierSTN(int rule, int line, int column, string lex) : base(rule, line, column) { this.lex = lex; }
        public TypeSpecifierSTN(int rule, int line, int column) : base(rule, line, column) { }
        public TypeSpecifierSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            return type;
        }
    }

    public class TypeQualifierSTN : SyntaxTreeNode
    {
        // : CONST
        // | VOLATILE
        // | INTERRUPT
        // ;

        public TypeQualifierSTN(int rule, int line, int column, string lex) : base(rule, line, column) { this.lex = lex; }
        public TypeQualifierSTN(int rule, int line, int column) : base(rule, line, column) { }
        public TypeQualifierSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            return type;
        }
    }

    public class InitDeclaratorSTN : SyntaxTreeNode
    {
        // : direct_declarator
        // | direct_declarator '=' initializer
        // ;

        public InitDeclaratorSTN(int rule, int line, int column) : base(rule, line, column) { }
        public InitDeclaratorSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            type = children[0].typeCheck().Clone();
            lex = children[0].lex;

            if(rule == 2)
            {
                children[1].typeCheck();
                if (children[1].valueIsUsed)
                {
                    valueIsUsed = true;
                    value = children[1].value;
                }

                string auxMessage = lex;
                if (valueIsUsed)
                    auxMessage += " = " + valueIsUsed;
                else if (!string.IsNullOrEmpty(children[1].lex))
                    auxMessage += " = " + children[1].lex;
                promoteType(type, children[1].type, line, column, children[1], auxMessage);
            }

            return type;
        }
    }

    public class ParameterListSTN : SyntaxTreeNode
    {
        // : parameter_declaration
        // | parameter_list ',' parameter_declaration
        // ;

        public ParameterListSTN(int rule, int line, int column) : base(rule, line, column) { }
        public ParameterListSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            string currentFramework = symbolsTable.currentFramework;

            // Check if this function was declared already
            if (!symbolsTable.existsFramework(currentFramework))
            {
                // If it wasn't, declare it and set the input types
                symbolsTable.addFramework(currentFramework);
                foreach (SyntaxTreeNode child in children)
                {
                    child.typeCheck();
                    symbolsTable.addInputType(currentFramework, child.type.Clone(), child.lex);
                }
            }
            else
            {
                // If it was, check for input differences and declare the input symbols
                foreach (SyntaxTreeNode child in children)
                    child.typeCheck();

                if (!checkInputTypeCorrectness(currentFramework, this))
                    addError(ErrorMessages.ERR_SEM_MSG_01, lex, line, column);

                symbolsTable.resetInputType(currentFramework);
                foreach (SyntaxTreeNode child in children)
                    symbolsTable.addInputType(currentFramework, child.type.Clone(), child.lex);
            }

            return type;
        }
    }

    public class AbstractParameterListSTN : SyntaxTreeNode
    {
        //: abstract_parameter_declaration
	    //| abstract_parameter_list ',' abstract_parameter_declaration
	    //;

        public AbstractParameterListSTN(int rule, int line, int column) : base(rule, line, column) { }
        public AbstractParameterListSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            string currentFramework = symbolsTable.currentFramework;

            foreach (SyntaxTreeNode child in children)
            {
                child.typeCheck();
                symbolsTable.addInputType(currentFramework, child.type.Clone());
            }

            return type;
        }
    }

    public class ConstantExpressionSTN : SyntaxTreeNode
    {
        // : conditional_expression
        // ;

        public ConstantExpressionSTN(int rule, int line, int column) : base(rule, line, column) { }
        public ConstantExpressionSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            type = children[0].typeCheck();
            valueIsUsed = children[0].valueIsUsed;
            value = children[0].value;

            if (!valueIsUsed)
                addError(ErrorMessages.ERR_SEM_MSG_18, line, column);

            return type;
        }
    }

    public class StatementListSTN : SyntaxTreeNode
    {
        // : statement
        // | statement_list statement
        // ;

        public StatementListSTN(int rule, int line, int column) : base(rule, line, column) { }
        public StatementListSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            bool breakHasBeenFound = false;

            foreach (SyntaxTreeNode child in children)
			{
                child.typeCheck();

                // Stop looking for a returnDone if happens a 'break'
                if (child.rule == 5 && child.children[0].rule == 3) // jump_statement => BREAK ';'
                    breakHasBeenFound = true;

                if (!breakHasBeenFound && child.returnDone)
					returnDone = true;
			}

            return type;
        }
    }

    public class InitializerSTN : SyntaxTreeNode
    {
        // : assignment_expression
        // ;

        public InitializerSTN(int rule, int line, int column) : base(rule, line, column) { }
        public InitializerSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            children[0].typeCheck();
            inheritPropertiesFromNode(children[0]);
            return type;
        }
    }

    public class ParameterDeclarationSTN : SyntaxTreeNode
    {
        // : declaration_specifiers pointer IDENTIFIER
        // | declaration_specifiers IDENTIFIER
        // ;

        public ParameterDeclarationSTN(int rule, int line, int column) : base(rule, line, column) { }
        public ParameterDeclarationSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            children[0].typeCheck();

            if(rule == 1)
            {
                type = CheckPointerAndCombineWithQualifiedType(children[0].type, children[1]);
                lex = children[2].lex;
            }
            else
            {
                type = children[0].type.Clone();
                lex = children[1].lex;
            }

            return type;
        }
    }

    public class AbstractParameterDeclarationSTN : SyntaxTreeNode
    {
	    //: declaration_specifiers pointer
	    //| declaration_specifiers
	    //;

        public AbstractParameterDeclarationSTN(int rule, int line, int column) : base(rule, line, column) { }
        public AbstractParameterDeclarationSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            children[0].typeCheck();

            if (rule == 1)
                type = CheckPointerAndCombineWithQualifiedType(children[0].type, children[1]);
            else
                type = children[0].type.Clone();

            return type;
        }
    }

    public class ConditionalExpressionSTN : SyntaxTreeNode
    {
        // : logical_or_expression
        // | logical_or_expression '?' expression ':' conditional_expression
        // ;

        public ConditionalExpressionSTN(int rule, int line, int column) : base(rule, line, column) { }
        public ConditionalExpressionSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            children[0].typeCheck();

            if (rule == 1)
            {
                // Perform the logical_or_expression and return
                inheritPropertiesFromNode(children[0]);
                return type;
            }


            // Otherwise, perform the conditional (ternary)
            children[1].typeCheck();
            children[2].typeCheck();

            // Shortcut values known at compile time
            if (children[0].valueIsUsed)
            {
                if(children[0].value != 0)
                    inheritPropertiesFromNode(children[1]);
                else
                    inheritPropertiesFromNode(children[2]);
            }
            else
                type = promoteType(children[1].type, children[2].type, line, column);

            return type;
        }
    }

    public class StatementSTN : SyntaxTreeNode
    {
        // : labeled_statement
        // | expression_statement
        // | selection_statement
        // | iteration_statement
        // | jump_statement
        // | error 
        // ;

        public StatementSTN(int rule, int line, int column) : base(rule, line, column) { }
        public StatementSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            if (rule == 6) return new QualifiedType(Constants.TYPE_ERR);

            children[0].typeCheck();
            inheritPropertiesFromNode(children[0]);
            returnDone = children[0].returnDone;

            return type;
        }
    }

    public class AssignmentExpressionSTN : SyntaxTreeNode
    {
        // : conditional_expression
        // | unary_expression assignment_operator assignment_expression
        // | unary_expression LEFT_ARROW assignment_expression
        // ;

        public AssignmentExpressionSTN(int rule, int line, int column) : base(rule, line, column) { }
        public AssignmentExpressionSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            children[0].typeCheck();

            if (rule == 1)
            {
                // Perform the conditional_expression and return
                inheritPropertiesFromNode(children[0]);
                return type;
            }

            if (rule == 2)
            {
                children[2].typeCheck();
                checkAssignmentExpression();
                type = children[0].type.Clone();
            }
            else
            {
                children[0].tryPromoteLiteral();
                children[1].typeCheck();
                children[1].tryPromoteLiteral();
                if (children[0].type.type != "word" && children[2].type.type != "byte")
                    addError(ErrorMessages.ERR_LEX_MSG_00, "", line, column);
                type = children[2].type.Clone();
            }

            return type;
        }

        private void checkAssignmentExpression()
        {
            // | unary_expression assignment_operator assignment_expression
            bool bypass = false;

            children[0].tryPromoteLiteral();
            string auxMessage = children[0].lex;
            if (children[2].valueIsUsed)
                auxMessage += " " + children[1].lex + " " + children[2].value;
            else if(!string.IsNullOrEmpty(children[2].lex))
                auxMessage += " " + children[1].lex + " " + children[2].lex;
            promoteType(children[0].type, children[2].type, line, column, children[2], auxMessage);

            switch (children[1].rule)
            {
                case 1: // : '='
                    if (children[2].valueIsUsed)
                    {
                        valueIsUsed = true;
                        value = children[2].value;
                    }
                    else if (children[0].rule == 4 && children[0].children[0].rule == 2)
                        bypass = true; // (*x) = ...   => A bypass can be performed to allow overlap
                    else
                        bypass = digForAnUnaryAmpersand(children[2]); // ... = (&x)   => A bypass can be performed to allow overlap
                    break;
                case 2: // | MUL_ASSIGN
                case 3: // | DIV_ASSIGN
                case 4: // | MOD_ASSIGN
                    addError(ErrorMessages.ERR_SEM_MSG_1A, children[1].lex, line, column);
                    break;
                case 5: // | ADD_ASSIGN
                    performOperation((x, y) => { return x + y; });
                    break;
                case 6: // | SUB_ASSIGN
                    performOperation((x, y) => { return x - y; });
                    break;
                case 7: // | LEFT_ASSIGN
                case 8: // | RIGHT_ASSIGN
                    if (symbolsTable.getTypeSize(children[0].type) > 1)
                        addError(ErrorMessages.ERR_SEM_MSG_05, children[1].lex, line, column);
                    if (children[2].valueIsUsed && (children[2].value > 7 || children[2].value < 0))
                    {
                        addWarning(ErrorMessages.WAR_SEM_MSG_00, children[1].lex, line, column);
                        value = 0;
                    }
                    else
                    {
                        if(children[1].rule == 7)
                            performOperation((x, y) => { return x << y; });
                        else
                            performOperation((x, y) => { return x >> y; });
                    }
                    if (symbolsTable.getTypeSize(children[0].type) > 1)
                        addError(ErrorMessages.ERR_SEM_MSG_05, children[1].lex, line, column);
                    break;
                case 9: // | AND_ASSIGN
                    performOperation((x, y) => { return x & y; });
                    break;
                case 10: // | XOR_ASSIGN
                    performOperation((x, y) => { return x ^ y; });
                    break;
                case 11: // | OR_ASSIGN
                    performOperation((x, y) => { return x | y; });
                    break;
            }

            checkConstantProperty(children[0].lex, line, column, bypass);
        }

        private void performOperation(Func<int, int, int> op)
        {
            if(children[0].valueIsUsed && children[2].valueIsUsed)
            {
                valueIsUsed = true;
                value = op(children[0].value, children[2].value);
            }
        }

        private bool digForAnUnaryAmpersand(SyntaxTreeNode node)
        {
            if (node.children.Count == 0)
                return false;

            if(node.GetType().Equals(typeof(UnaryExpressionSTN)))
            {
                if (node.rule == 4 && node.children[0].rule == 1 && node.children[1].lex != "")
                    return true;
                else
                    return false;
            }

            if (node.rule != 1)
                return false;

            return digForAnUnaryAmpersand(node.children[0]);
        }
    }

    public class LogicalOrExpressionSTN : SyntaxTreeNode
    {
        // : logical_and_expression
        // | logical_or_expression OR_OP logical_and_expression
        // ;

        public LogicalOrExpressionSTN(int rule, int line, int column) : base(rule, line, column) { }
        public LogicalOrExpressionSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            //Only executed if both childen valuIsUsed and rule != 1
            expressionCheck(calculateExpression, false);
            if (rule != 1)
                type = new QualifiedType() { type = Constants.TYPE_BYTE, b_unsigned = true };
            return type;
        }

        private void calculateExpression()
        {
            valueIsUsed = true;
            value = ((children[0].value != 0) || (children[1].value != 0)) ? 1 : 0;
        }
    }

    public class ExpressionSTN : SyntaxTreeNode
    {
        // : assignment_expression
        // | expression ',' assignment_expression
        // ;

        public ExpressionSTN(int rule, int line, int column) : base(rule, line, column) { }
        public ExpressionSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            children[0].typeCheck();

            //The comma operator evaluates a series of expressions.
            //The value of the comma group is the value of the last element in the list.
            if (rule == 1)
                inheritPropertiesFromNode(children[0]);
            else
            {
                children[1].typeCheck();
                inheritPropertiesFromNode(children[1]);
            }

            return type;
        }
    }

    public class LabeledStatementSTN : SyntaxTreeNode
    {
        // : IDENTIFIER ':' statement
        // ;

        public LabeledStatementSTN(int rule, int line, int column) : base(rule, line, column) { }
        public LabeledStatementSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            //A jump-statement must reside in the same function and can appear before only one
            //statement in the same function. The set of identifier names following a goto has
            //its own name space so the names do not interfere with other identifiers. Labels
            //cannot be redeclared.

            // IDENTIFIER ':' statement
            if (!symbolsTable.addLabel(symbolsTable.currentFramework, children[0].lex))
                addError(ErrorMessages.ERR_SEM_MSG_0C, symbolsTable.currentFramework + ":" + children[0].lex, line, column);
            children[1].typeCheck();
            returnDone = children[1].returnDone;
            type = children[1].type.Clone();

            return type;
        }
    }

    public class ExpressionStatementSTN : SyntaxTreeNode
    {
        // : ';'
        // | expression ';'
        // ;

        public ExpressionStatementSTN(int rule, int line, int column) : base(rule, line, column) { }
        public ExpressionStatementSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
			if(rule == 2)
				type = children[0].typeCheck();
			
            return type;
        }
    }

    public class SelectionStatementSTN : SyntaxTreeNode
    {
        // : IF '(' expression ')' compound_statement
        // | IF '(' expression ')' compound_statement ELSE compound_statement
        // | SWITCH '(' expression ')' '{' case_statement_list '}'

        public SelectionStatementSTN(int rule, int line, int column) : base(rule, line, column) { }
        public SelectionStatementSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            switch (rule)
			{
				case 1: // IF '(' expression ')' compound_statement
                    typeCheckRule1();
					break;
				case 2: // IF '(' expression ')' compound_statement ELSE compound_statement
                    typeCheckRule2();
					break;
				case 3: // SWITCH '(' expression ')' '{' case_statement_list '}'
                    typeCheckRule3();
					break;
				default:
					break;
			}

            return type;
        }
		
		private void typeCheckRule1()
		{
            // IF '(' expression ')' compound_statement
            children[0].typeCheck();
			children[1].typeCheck();
			// @returnDone always false
			if (children[0].type.type != Constants.TYPE_ERR)
				type = children[1].type;
			else
				type.type = Constants.TYPE_ERR;

		}
		
		private void typeCheckRule2()
		{
            // IF '(' expression ')' compound_statement ELSE compound_statement
            children[0].typeCheck();
			children[1].typeCheck();
			children[2].typeCheck();
			returnDone = children[1].returnDone && children[2].returnDone;
			if ((children[0].type.type == Constants.TYPE_ERR) ||
				(children[1].type.type == Constants.TYPE_ERR) ||
				(children[2].type.type == Constants.TYPE_ERR))
				type.type = Constants.TYPE_ERR;
		}
		
		private void typeCheckRule3()
		{
            // SWITCH '(' expression ')' '{' case_statement_list '}'
            children[0].typeCheck();

            // expression must be 'byte'
            children[0].tryPromoteLiteral();
            if (children[0].type.type != Constants.TYPE_BYTE)
                addError(ErrorMessages.ERR_SEM_MSG_05, children[0].value.ToString(), line, column);

            // Check the statement
            syntaxTree.breakable++;
            children[1].typeCheck();
			syntaxTree.breakable--;
			
			returnDone = children[1].returnDone;
			if ((children[0].type.type == Constants.TYPE_ERR) ||
				(children[1].type.type == Constants.TYPE_ERR))
				type.type = Constants.TYPE_ERR;
		}
    }

    public class CaseStatementListSTN : SyntaxTreeNode
    {
        // : case_statement
        // | case_statement_list case_statement
        // ;

        public CaseStatementListSTN(int rule, int line, int column) : base(rule, line, column) { }
        public CaseStatementListSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            returnDone = true;

            foreach (SyntaxTreeNode child in children)
            {
                child.typeCheck();
                if (!child.returnDone)
                    returnDone = false;
            }

            return type;
        }
    }

    public class CaseStatementSTN : SyntaxTreeNode
    {
        // : CASE constant_expression ':'
        // | CASE constant_expression ':' statement_list
        // | DEFAULT ':' statement_list
        // ;

        public CaseStatementSTN(int rule, int line, int column) : base(rule, line, column) { }
        public CaseStatementSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            children[0].typeCheck();

            if (rule < 3)
            {
                // | CASE constant_expression ':'
                // | CASE constant_expression ':' statement_list

                // constant_expression must be 'byte'
                children[0].tryPromoteLiteral();
                if (children[0].type.type != Constants.TYPE_BYTE)
                    addError(ErrorMessages.ERR_SEM_MSG_05, children[0].value.ToString(), children[0].getLine(), children[0].getColumn());

                if (rule == 2)
                {
                    children[1].typeCheck();
                    returnDone = children[1].returnDone;
                }
            }
            else
            {
                // | DEFAULT ':' statement_list
                returnDone = children[0].returnDone;
                type = children[0].type;
            }

            return type;
        }
    }

    public class IterationStatementSTN : SyntaxTreeNode
    {
        // : WHILE '(' expression ')' statement
        // | DO statement WHILE '(' expression ')' ';'
        // | FOR '(' expression_statement expression_statement ')' statement
        // | FOR '(' expression_statement expression_statement expression ')' statement
        // ;

        public IterationStatementSTN(int rule, int line, int column) : base(rule, line, column) { }
        public IterationStatementSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
			switch(rule)
			{
				case 1: // WHILE '(' expression ')' statement
					typeCheckRule1();
					break;
				case 2: // DO statement WHILE '(' expression ')' ';'
					typeCheckRule2();
					break;
				case 3:	// FOR '(' expression_statement expression_statement ')' statement
					typeCheckRule3();
					break;
				case 4:	// FOR '(' expression_statement expression_statement expression ')' statement
					typeCheckRule4();
					break;
				default:
					break;
			}
			
            return type;
        }
		
		private void typeCheckRule1()
		{
			// WHILE '(' expression ')' statement
			children[0].typeCheck();
			if (children[0].valueIsUsed && value == 0)
				addWarning(ErrorMessages.WAR_SEM_MSG_04, "while", line, column);
			else
			{
				syntaxTree.continuable++;
				syntaxTree.breakable++;
				children[1].typeCheck();
				syntaxTree.continuable--;
				syntaxTree.breakable--;
				
				returnDone = children[1].returnDone;
				type       = children[1].type;
			}
		}
		
		private void typeCheckRule2()
		{
			// DO statement WHILE '(' expression ')' ';'
			syntaxTree.continuable++;
			syntaxTree.breakable++;
			children[0].typeCheck();
			syntaxTree.continuable--;
			syntaxTree.breakable--;
			
			children[1].typeCheck();
			returnDone = children[0].returnDone;
			
			if (symbolsTable.getTypeSize(children[1].type) != 1)
			{
				addError(ErrorMessages.ERR_SEM_MSG_05, "do-while(expression)", line, column);
				type.type = Constants.TYPE_ERR;
			}
			else
				type = children[0].type;
		}

		private void typeCheckRule3()
		{
			// FOR '(' expression_statement expression_statement ')' statement
			children[0].typeCheck();
			children[1].typeCheck();
			if(children[1].valueIsUsed && children[1].value == 0)
			{
				/* loop condition always false */
				addWarning(ErrorMessages.WAR_SEM_MSG_04, "for", line, column);
				type = children[0].type;
			}
			else
			{
				/* if (children[1]->symbol - syntaxTree.getNTerminals() == NT_EXPRESSION_STATEMENT)
					/* loop condition always true */
				syntaxTree.continuable++;
				syntaxTree.breakable++;
				children[2].typeCheck();
				syntaxTree.continuable--;
				syntaxTree.breakable--;
				
				returnDone = children[2].returnDone;
				if ((children[0].type.type != Constants.TYPE_ERR) && (children[1].type.type != Constants.TYPE_ERR))
					type = children[2].type;
				else
					type.type = Constants.TYPE_ERR;
			}
		}

		private void typeCheckRule4()
		{
			// FOR '(' expression_statement expression_statement expression ')' statement
			children[0].typeCheck();
			children[1].typeCheck();
			children[2].typeCheck();
			if(children[1].valueIsUsed && children[1].value == 0)
            {
				/* loop condition always false */
				addWarning(ErrorMessages.WAR_SEM_MSG_04, "for", line, column);
				if ((children[0].type.type != Constants.TYPE_ERR) && (children[2].type.type != Constants.TYPE_ERR))
					type = children[0].type;
				else
					type.type = Constants.TYPE_ERR;
			}else{
				/* if (children[1]->symbol - syntaxTree.getNTerminals() == NT_EXPRESSION_STATEMENT)
					/* loop condition always true */
				syntaxTree.continuable++;
				syntaxTree.breakable++;
				children[3].typeCheck();
				syntaxTree.continuable--;
				syntaxTree.breakable--;
				returnDone = children[3].returnDone;
				
				if ((children[0].type.type != Constants.TYPE_ERR) &&
					(children[1].type.type != Constants.TYPE_ERR) &&
					(children[2].type.type != Constants.TYPE_ERR))
					type = children[3].type;
				else
					type.type = Constants.TYPE_ERR;
			}
		}
    }

    public class JumpStatementSTN : SyntaxTreeNode
    {
        // : GOTO IDENTIFIER ';'
        // | CONTINUE ';'
        // | BREAK ';'
        // | RETURN ';'
        // | RETURN expression ';'
        // ;

        public JumpStatementSTN(int rule, int line, int column) : base(rule, line, column) { }
        public JumpStatementSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
			switch(rule)
			{
				case 1:	// GOTO IDENTIFIER ';'
					syntaxTree.labelsExpected.Add(children[0].lex, new KeyValuePair<int, int>(line, column));
					break;
				case 2:	// CONTINUE ';'
					if (syntaxTree.continuable < 1)
						addError(ErrorMessages.ERR_SEM_MSG_0D, "continue", line, column);
					break;
				case 3:	// BREAK ';'
					if (syntaxTree.breakable < 1)						
						addError(ErrorMessages.ERR_SEM_MSG_0D, "break", line, column);
					break;
				case 4:	// RETURN ';'
					returnDone = true;
					if (symbolsTable.getTypeSize(symbolsTable.getOutputType(symbolsTable.currentFramework)) != 0)
						addWarning(ErrorMessages.WAR_SEM_MSG_03, "return;", line, column);
					break;
				case 5:	// RETURN expression ';'
					children[0].typeCheck();
					type = children[0].type.Clone();
					returnDone = true;
					QualifiedType fraweworkType = symbolsTable.getOutputType(symbolsTable.currentFramework);
                    promoteType(fraweworkType, children[0].type, line, column, children[0]);
					//if (!fraweworkType.sameOutput(children[0].type))
					//	addError(ErrorMessages.ERR_SEM_MSG_06,
					//		string.Format("return {0} is not compatible with output {1}", type.toString(), fraweworkType.toString()),
					//		line, column);
					symbolsTable.incNUses(Constants.SEM_RETURN_SYMBOL, symbolsTable.currentFramework, 1);
					break;
                default:
                    break;
			}
			
            return type;
        }
    }

    public class UnaryExpressionSTN : SyntaxTreeNode
    {
        // : postfix_expression
        // | INC_OP unary_expression
        // | DEC_OP unary_expression
        // | unary_operator cast_expression
        // | SIZEOF unary_expression
        // | SIZEOF '(' type_specifier ')'
        // | SIZEOF '(' type_specifier pointer ')'
        // | LOBYTE unary_expression
        // | HIBYTE unary_expression
        // ;

        public UnaryExpressionSTN(int rule, int line, int column) : base(rule, line, column) { }
        public UnaryExpressionSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            children[0].typeCheck();

            if (rule == 1)
            {
                // Perform the postfix_expression and return
                inheritPropertiesFromNode(children[0]);
                return type;
            }

            switch (rule)
            {
                case 2: // | INC_OP unary_expression
                    inheritPropertiesFromNode(children[0]);
                    if (valueIsUsed) ++value;
                    break;
                case 3: // | DEC_OP unary_expression
                    inheritPropertiesFromNode(children[0]);
                    if (valueIsUsed) --value;
                    break;
                case 4: // | unary_operator cast_expression
                    checkUnaryOperatorExpression();
                    break;
                case 5: // | SIZEOF unary_expression
                case 6: // | SIZEOF '(' type_specifier ')'
                    type.type = Constants.TYPE_LITERAL;
                    value = symbolsTable.getTypeSize(children[0].type);
                    if (value < 0)
                        addError(ErrorMessages.ERR_SEM_MSG_03, children[0].type.toString(), line, column);
                    else
                        valueIsUsed = true;
                    break;
                case 7: // | SIZEOF '(' type_specifier pointer ')'
                    type.type = Constants.TYPE_LITERAL;
                    children[0].type.pointerDepth = children[1].children.Count + 1;
                    value = symbolsTable.getTypeSize(children[0].type);
                    if (value < 0)
                        addError(ErrorMessages.ERR_SEM_MSG_03, children[0].type.toString(), line, column);
                    else
                        valueIsUsed = true;
                    break;
                case 8: // | LOBYTE unary_expression
                case 9: // | HIBYTE unary_expression
                    type.type = Constants.TYPE_BYTE;
                    type.b_unsigned = true;
                    if (symbolsTable.getTypeSize(children[0].type) != 2)
                        addWarning(ErrorMessages.WAR_SEM_MSG_09, children[0].type.toString(), line, column);

                    if (children[0].valueIsUsed)
                    {
                        valueIsUsed = true;
                        value = (rule == 8) ? children[0].value & 0x00FF : (children[0].value & 0xFF00) >> 8;
                    }
                    break;
                default:
                    break;
            }

            return type;
        }

        private void checkUnaryOperatorExpression()
        {
            // | unary_operator cast_expression
            children[1].typeCheck();
            inheritPropertiesFromNode(children[1]);

            switch (children[0].rule)
            {
                case 1: // : '&' -> Address of
                    type.pointerDepth++;
                    // Pointers to functions are not allowed!
                    if (lex == "" || !symbolsTable.existsSymbol(lex, symbolsTable.currentFramework))
                        addError(ErrorMessages.ERR_SEM_MSG_06, "&", line, column);
                    else
                        lex = "";
                    valueIsUsed = false;
                    break;
                case 2: // | '*' -> Pointer to
                    type.pointerDepth--;
                    valueIsUsed = false;
                    break;
                case 3: // | '+'
                    // Same type
                    break;
                case 4: // | '-'
                    if (type.pointerDepth > 0)
                        addError(ErrorMessages.ERR_SEM_MSG_06, "-(" + type.toString() + ")", line, column);
                    type.b_unsigned = false;
                    if (valueIsUsed) value = -value;
                    break;
                case 5: // | '~'
                    // Same type: Be careful in the code generation
                    if (symbolsTable.getTypeSize(type) > 1)
                        addError(ErrorMessages.ERR_SEM_MSG_05, "~", line, column);
                    if (valueIsUsed) value = ~(byte)value;
                    break;
                case 6: // | '!'
                    // Same type
                    if (symbolsTable.getTypeSize(type) > 1)
                        addError(ErrorMessages.ERR_SEM_MSG_05, "!", line, column);
                    if (valueIsUsed) value = (value == 0) ? 1 : 0;
                    break;
                default:
                    break;
            }
        }
    }

    public class AssignmentOperatorSTN : SyntaxTreeNode
    {
        // : '='
        // | MUL_ASSIGN // Not supported by 6502
        // | DIV_ASSIGN // Not supported by 6502
        // | MOD_ASSIGN // Not supported by 6502
        // | ADD_ASSIGN
        // | SUB_ASSIGN
        // | LEFT_ASSIGN
        // | RIGHT_ASSIGN
        // | AND_ASSIGN
        // | XOR_ASSIGN
        // | OR_ASSIGN
        // ;

        public AssignmentOperatorSTN(int rule, int line, int column) : base(rule, line, column) { }
        public AssignmentOperatorSTN(int rule, int line, int column, string lex) : base(rule, line, column) { this.lex = lex; }
        public AssignmentOperatorSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            //Here goes nothing!
            return type;
        }
    }

    public class LogicalAndExpressionSTN : SyntaxTreeNode
    {
        // : inclusive_or_expression
        // | logical_and_expression AND_OP inclusive_or_expression
        // ;

        public LogicalAndExpressionSTN(int rule, int line, int column) : base(rule, line, column) { }
        public LogicalAndExpressionSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            //Only executed if both childen valuIsUsed and rule != 1
            expressionCheck(calculateExpression, false);
            if (rule != 1) type = new QualifiedType() { type = Constants.TYPE_BYTE, b_unsigned = true };
            return type;
        }

        private void calculateExpression()
        {
            valueIsUsed = true;
            value = ((children[0].value != 0) && (children[1].value != 0)) ? 1 : 0;
        }
    }

    public class PostfixExpressionSTN : SyntaxTreeNode
    {
        // : primary_expression
        // | postfix_expression '[' expression ']'
        // | IDENTIFIER '(' ')'
        // | IDENTIFIER '(' argument_expression_list ')'
        // | postfix_expression INC_OP
        // | postfix_expression DEC_OP
        // ;

        public PostfixExpressionSTN(int rule, int line, int column) : base(rule, line, column) { }
        public PostfixExpressionSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            children[0].typeCheck();
            lex = children[0].lex;

            switch (rule)
            {
                case 1: // primary_expression
                    typeCheckRule1();
                    break;
                case 2: // postfix_expression '[' expression ']'
                    typeCheckRule2();
                    break;
                case 3: // postfix_expression '(' ')'
                    typeCheckRule3();
                    break;
                case 4: // postfix_expression '(' argument_expression_list ')'
                    typeCheckRule4();
                    break;
                case 5: // postfix_expression INC_OP
                    typeCheckRule5();
                    break;
                case 6: // postfix_expression DEC_OP
                    typeCheckRule6();
                    break;
            }

            return type;
        }

        private void typeCheckRule1()
        {
            // primary_expression
            inheritPropertiesFromNode(children[0]);
        }

        private void typeCheckRule2()
        {
            // postfix_expression '[' expression ']'

            //Make sure this is a pointer
            if (children[0].type.pointerDepth < 1)
                addError(ErrorMessages.ERR_SEM_MSG_19, "'[' ']'", line, column);

            //Dereference the content of the pointer
            type = children[0].type.Clone();
            type.pointerDepth -= 1;
            type.arrSize = 0;

            children[1].typeCheck();

            if (!children[1].valueIsUsed) return;

            //Check the index is valid in the bound
            if (children[0].type.arrSize > 0 && children[0].type.arrSize <= children[1].value)
                addWarning(ErrorMessages.WAR_SEM_MSG_08, "'[' ']'", line, column);
            
            //FIXME If here, this is an address known in compilation time
        }

        private void typeCheckRule3()
        {
            // IDENTIFIER '(' ')'

            //Check the input types are right
            if (symbolsTable.existsFramework(lex))
            {
                //Get type and add to current framework a dependency of lex framework
                type = symbolsTable.getOutputType(lex).Clone();
                if (lex != symbolsTable.currentFramework)
                    symbolsTable.addDependency(symbolsTable.currentFramework, lex);
                else
                {
                    //Avoid recursion
                    addError(ErrorMessages.ERR_SEM_MSG_16, lex, line, column);
                    symbolsTable.getFramework(symbolsTable.currentFramework).recursive = true;
                }

                //Check input type are right
                if (symbolsTable.getInputType(lex).Count != 0)
                    addError(ErrorMessages.ERR_SEM_MSG_01, lex, line, column);
            }
            else
                addError(ErrorMessages.ERR_SEM_MSG_02, lex, line, column);

        }

        private void typeCheckRule4()
        {
            // IDENTIFIER '(' argument_expression_list ')'

            //Check the input types are right
            children[1].typeCheck();
            if (symbolsTable.existsFramework(lex))
            {
                //Get type and add to current framework a dependency of lex framework
                type = symbolsTable.getOutputType(lex).Clone();
                if (lex != symbolsTable.currentFramework)
                    symbolsTable.addDependency(symbolsTable.currentFramework, lex);
                else
                {
                    //Avoid recursion
                    addError(ErrorMessages.ERR_SEM_MSG_16, lex, line, column);
                    symbolsTable.getFramework(symbolsTable.currentFramework).recursive = true;
                }

                //Check input type are right
                if (!checkInputTypeCorrectness(lex, children[1]))
                    addError(ErrorMessages.ERR_SEM_MSG_01, lex, line, column);
            }
            else
                addError(ErrorMessages.ERR_SEM_MSG_02, lex, line, column);
        }

        private void typeCheckRule5()
        {
            // postfix_expression INC_OP
            checkConstantProperty(children[0].lex, line, column, false);
            inheritPropertiesFromNode(children[0]);
        }

        private void typeCheckRule6()
        {
            // postfix_expression DEC_OP
            checkConstantProperty(children[0].lex, line, column, false);
            inheritPropertiesFromNode(children[0]);
        }
    }

    public class UnaryOperatorSTN : SyntaxTreeNode
    {
        // : '&'
        // | '*'
        // | '+'
        // | '-'
        // | '~'
        // | '!'
        // ;

        public UnaryOperatorSTN(int rule, int line, int column) : base(rule, line, column) { }
        public UnaryOperatorSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            // Here goes nothing!
            return type;
        }
    }

    public class CastExpressionSTN : SyntaxTreeNode
    {
        // : unary_expression
        // | '(' type_specifier ')' cast_expression
        // ;

        public CastExpressionSTN(int rule, int line, int column) : base(rule, line, column) { }
        public CastExpressionSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            children[0].typeCheck();

            switch (rule)
            {
                case 1:
                    // unary_expression
                    inheritPropertiesFromNode(children[0]);
                    break;
                case 2:
                    // '(' type_specifier ')' cast_expression
                    castExpression();
                    break;
                default:
                    break;
            }

            return type;
        }

        private void castExpression()
        {
            // '(' type_specifier ')' cast_expression
            type = children[0].type.Clone();

            children[1].typeCheck();
            lex = children[1].lex;

            if (children[1].valueIsUsed)
            {
                valueIsUsed = true;
                value = children[1].value;
            }

            if (children[1].type.isLiteral())
                children[1].tryPromoteLiteral(type);
            else if (symbolsTable.getTypeSize(type) < symbolsTable.getTypeSize(children[1].type))
                addWarning(ErrorMessages.WAR_SEM_MSG_06, string.Format("cast ({0}) {1}", type.toString(), children[1].type.toString()), line, column);
            else if ((type.b_unsigned || type.pointerDepth > 0) != (children[1].type.b_unsigned || children[1].type.pointerDepth > 0))
                addWarning(ErrorMessages.WAR_SEM_MSG_01, string.Format("cast ({0}) {1}", type.toString(), children[1].type.toString()), line, column);
        }
    }

    public class PrimaryExpressionSTN : SyntaxTreeNode
    {
        // : IDENTIFIER
        // | NUMBER_LITERAL
        // | STRING_LITERAL
        // | '(' expression ')'
        // ;

        public PrimaryExpressionSTN(int rule, int line, int column) : base(rule, line, column) { }
        public PrimaryExpressionSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            if (rule == 1)
            {
                lex = children[0].lex;
                if (symbolsTable.existsSymbol(lex, symbolsTable.currentFramework))
                {
                    type = symbolsTable.getSymbol(lex, symbolsTable.currentFramework).qType.Clone();
                    symbolsTable.incNUses(children[0].lex, symbolsTable.currentFramework);
                }
                else
                    addError(ErrorMessages.ERR_SEM_MSG_00, lex, line, column);
            }
            else
            {
                if (rule == 4)
                    children[0].typeCheck();

                inheritPropertiesFromNode(children[0]);
            }

            return type;
        }
    }

    public class InclusiveOrExpressionSTN : SyntaxTreeNode
    {
        // : exclusive_or_expression
        // | inclusive_or_expression '|' exclusive_or_expression
        // ;

        public InclusiveOrExpressionSTN(int rule, int line, int column) : base(rule, line, column) { }
        public InclusiveOrExpressionSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            expressionCheck(calculateExpression);
            return type;
        }

        private void calculateExpression()
        {
            valueIsUsed = true;
            value = children[0].value | children[1].value;
        }
    }

    public class ArgumentExpressionListSTN : SyntaxTreeNode
    {
        // : assignment_expression
        // | argument_expression_list ',' assignment_expression
        // ;

        public ArgumentExpressionListSTN(int rule, int line, int column) : base(rule, line, column) { }
        public ArgumentExpressionListSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            foreach (SyntaxTreeNode child in children)
                child.typeCheck();

            return type;
        }
    }

    public class ExclusiveOrExpressionSTN : SyntaxTreeNode
    {
        // : and_expression
        // | exclusive_or_expression '^' and_expression
        // ;

        public ExclusiveOrExpressionSTN(int rule, int line, int column) : base(rule, line, column) { }
        public ExclusiveOrExpressionSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            expressionCheck(calculateExpression);
            return type;
        }

        private void calculateExpression()
        {
            valueIsUsed = true;
            value = children[0].value ^ children[1].value;
        }
    }

    public class AndExpressionSTN : SyntaxTreeNode
    {
        // : equality_expression
        // | and_expression '&' equality_expression
        // ;

        public AndExpressionSTN(int rule, int line, int column) : base(rule, line, column) { }
        public AndExpressionSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            expressionCheck(calculateExpression);
            return type;
        }

        private void calculateExpression()
        {
            valueIsUsed = true;
            switch (rule)
            {
                case 2:
                    value = children[0].value & children[1].value;
                    break;
                default:
                    break;
            }
        }
    }

    public class EqualityExpressionSTN : SyntaxTreeNode
    {
        // : relational_expression
        // | equality_expression EQ_OP relational_expression
        // | equality_expression NE_OP relational_expression
        // ;

        public EqualityExpressionSTN(int rule, int line, int column) : base(rule, line, column) { }
        public EqualityExpressionSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            expressionCheck(calculateExpression);
            if (rule != 1) type = new QualifiedType() { type = Constants.TYPE_BYTE, b_unsigned = true };
            return type;
        }

        private void calculateExpression()
        {
            valueIsUsed = true;
            switch (rule)
            {
                case 2:
                    value = (children[0].value == children[1].value) ? 1 : 0;
                    break;
                case 3:
                    value = (children[0].value != children[1].value) ? 1 : 0;
                    break;
                default:
                    break;
            }
        }
    }

    public class RelationalExpressionSTN : SyntaxTreeNode
    {
        // : shift_expression
        // | relational_expression '>' shift_expression
        // | relational_expression '<' shift_expression
        // | relational_expression LE_OP shift_expression
        // | relational_expression GE_OP shift_expression
        // ;

        public RelationalExpressionSTN(int rule, int line, int column) : base(rule, line, column) { }
        public RelationalExpressionSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            expressionCheck(calculateExpression);
            if (rule != 1) type = new QualifiedType(){ type = Constants.TYPE_BYTE, b_unsigned = true };
            return type;
        }

        private void calculateExpression()
        {
            valueIsUsed = true;
            switch (rule)
            {
                case 2:
                    value = (children[0].value > children[1].value) ? 1 : 0;
                    break;
                case 3:
                    value = (children[0].value < children[1].value) ? 1 : 0;
                    break;
                case 4:
                    value = (children[0].value >= children[1].value) ? 1 : 0;
                    break;
                case 5:
                    value = (children[0].value <= children[1].value) ? 1 : 0;
                    break;
                default:
                    break;
            }
        }
    }

    public class ShiftExpressionSTN : SyntaxTreeNode
    {
        // : additive_expression
        // | shift_expression LEFT_OP additive_expression
        // | shift_expression RIGHT_OP additive_expression
        // ;

        public ShiftExpressionSTN(int rule, int line, int column) : base(rule, line, column) { }
        public ShiftExpressionSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            expressionCheck(calculateExpression);

            //Check the operands are byte sized
            if (rule > 1 && symbolsTable.getTypeSize(type) > 1)
                addError(ErrorMessages.ERR_SEM_MSG_05, (rule == 2) ? "<<" : ">>", line, column);

            return type;
        }

        private void calculateExpression()
        {
            valueIsUsed = true;
            switch (rule)
            {
                case 2:
                    value = children[0].value << children[1].value;
                    if (symbolsTable.getTypeSize(type.type) < symbolsTable.getSizeToFitValue(value, value < 0))
                        addWarning(ErrorMessages.WAR_SEM_MSG_06, "<<", line, column);
                    break;
                case 3:
                    value = children[0].value >> children[1].value;
                    if (symbolsTable.getTypeSize(type.type) < symbolsTable.getSizeToFitValue(value, value < 0))
                        addWarning(ErrorMessages.WAR_SEM_MSG_06, ">>", line, column);
                    break;
                default:
                    break;
            }
        }
    }

    public class AdditiveExpressionSTN : SyntaxTreeNode
    {
        // : multiplicative_expression
        // | additive_expression '+' multiplicative_expression
        // | additive_expression '-' multiplicative_expression
        // ;

        public AdditiveExpressionSTN(int rule, int line, int column) : base(rule, line, column) { }
        public AdditiveExpressionSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            expressionCheck(calculateExpression);
            return type;
        }

        private void calculateExpression()
        {
            valueIsUsed = true;
            switch (rule)
            {
                case 2:
                    value = children[0].value + children[1].value;
                    if (!type.isLiteral() && symbolsTable.getTypeSize(type.type) < symbolsTable.getSizeToFitValue(value, value < 0))
                        addWarning(ErrorMessages.WAR_SEM_MSG_06, "+", line, column);
                    break;
                case 3:
                    value = children[0].value - children[1].value;
                    if (!type.isLiteral() && symbolsTable.getTypeSize(type.type) < symbolsTable.getSizeToFitValue(value, value < 0))
                        addWarning(ErrorMessages.WAR_SEM_MSG_06, "-", line, column);
                    break;
                default:
                    break;
            }
        }
    }

    public class MultiplicativeExpressionSTN : SyntaxTreeNode
    {
        // : cast_expression
        // | multiplicative_expression '*' cast_expression { /* Not supported by RetroC */ }
        // | multiplicative_expression '/' cast_expression { /* Not supported by RetroC */ }
        // | multiplicative_expression '%' cast_expression { /* Not supported by RetroC */ }
        // ;

        public MultiplicativeExpressionSTN(int rule, int line, int column) : base(rule, line, column) { }
        public MultiplicativeExpressionSTN(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children) { }

        public override QualifiedType typeCheck()
        {
            //Only executed if both childen valuIsUsed and rule != 1
            expressionCheck(calculateExpression);

            if (!valueIsUsed && rule > 1)
            {
                string[] opLex = new string[] { "", "", "*", "/", "%" };
                addError(ErrorMessages.ERR_SEM_MSG_1A, opLex[rule], line, column);
            }

            return type;
        }

        private void calculateExpression()
        {
            valueIsUsed = true;
            switch (rule)
            {
                case 2:
                    value = children[0].value * children[1].value;
                    if (symbolsTable.getTypeSize(type.type) < symbolsTable.getSizeToFitValue(value, value < 0))
                        addWarning(ErrorMessages.WAR_SEM_MSG_06, "*", line, column);
                    break;
                case 3:
                    value = children[0].value / children[1].value;
                    break;
                case 4:
                    value = children[0].value % children[1].value;
                    break;
                default:
                    break;
            }
        }
    }

    public class IdentifierSTL : SyntaxTreeNode
    {
        public IdentifierSTL(int rule, int line, int column) : base(rule, line, column)
        {
            lex = symbolsTable.popTempIdentifier();
        }
        public IdentifierSTL(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children)
        {
            lex = symbolsTable.popTempIdentifier();
        }

        public override QualifiedType typeCheck()
        {
            return type;
        }
    }

    public class NumberLiteralSTL : SyntaxTreeNode
    {
        public NumberLiteralSTL(int rule, int line, int column) : base(rule, line, column)
        {
            getLiteralValue();
        }
        public NumberLiteralSTL(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children)
        {
            getLiteralValue();
        }

        private void getLiteralValue()
        {
            string newLiteral = symbolsTable.popTempLiteral();
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
                    addError(ErrorMessages.ERR_SEM_MSG_05, newLiteral, line, column);
                else
                {
                    value = (int)newLiteral[1];
                    type.type = Constants.TYPE_BYTE;
                    type.b_unsigned = true;
                }
            }
        }

        public override QualifiedType typeCheck()
        {
            return type;
        }
    }

    public class StringLiteralSTL : SyntaxTreeNode
    {
        public StringLiteralSTL(int rule, int line, int column) : base(rule, line, column)
        {
            getLiteralValue();
        }
        public StringLiteralSTL(int rule, int line, int column, SyntaxTreeNode[] children) : base(rule, line, column, children)
        {
            getLiteralValue();
        }

        private void getLiteralValue()
        {
            lex = symbolsTable.popTempLiteral();
            type.type = Constants.TYPE_STRING;
            type.arrSize = lex.Length;
        }

        public override QualifiedType typeCheck()
        {
            return type;
        }
    }
}
