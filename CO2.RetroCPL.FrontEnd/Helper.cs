using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO2.RetroCPL.Commons;

namespace CO2.RetroCPL.FrontEnd
{
    class Helper
    {
        /*enum {
            NT_PRIMARY_EXPRESSION = 1, NT_POSTFIX_EXPRESSION, NT_ARGUMENT_EXPRESSION_LIST, NT_UNARY_EXPRESSION,
            NT_UNARY_OPERATOR, NT_CAST_EXPRESSION, NT_MULTIPLICATIVE_EXPRESSION, NT_ADDITIVE_EXPRESSION,
            NT_SHIFT_EXPRESSION, NT_RELATIONAL_EXPRESSION, NT_EQUALITY_EXPRESSION, NT_AND_EXPRESSION,
            NT_EXCLUSIVE_OR_EXPRESSION, NT_INCLUSIVE_OR_EXPRESSION, NT_LOGICAL_AND_EXPRESSION,
            NT_LOGICAL_OR_EXPRESSION, NT_CONDITIONAL_EXPRESSION, NT_ASSIGNMENT_EXPRESSION, NT_ASSIGNMENT_OPERATOR,
            NT_EXPRESSION, NT_CONSTANT_EXPRESSION, NT_DECLARATION, NT_DECLARATION_SPECIFIERS, NT_INIT_DECLARATOR_LIST,
            NT_INIT_DECLARATOR, NT_STORAGE_CLASS_SPECIFIER, NT_TYPE_SPECIFIER, NT_STRUCT_OR_UNION_SPECIFIER,
            NT_STRUCT_OR_UNION, NT_STRUCT_DECLARATION_LIST, NT_STRUCT_DECLARATION, NT_SPECIFIER_QUALIFIER_LIST,
            NT_STRUCT_DECLARATOR_LIST, NT_STRUCT_DECLARATOR, NT_ENUM_SPECIFIER, NT_ENUMERATOR_LIST, NT_ENUMERATOR,
            NT_TYPE_QUALIFIER, NT_DECLARATOR, NT_DIRECT_DECLARATOR, NT_POINTER, NT_TYPE_QUALIFIER_LIST, NT_PARAMETER_TYPE_LIST,
            NT_PARAMETER_LIST, NT_PARAMETER_DECLARATION, NT_IDENTIFIER_LIST, NT_TYPE_NAME, NT_ABSTRACT_DECLARATOR,
            NT_DIRECT_ABSTRACT_DECLARATOR, NT_INITIALIZER, NT_INITIALIZER_LIST, NT_STATEMENT, NT_LABELED_STATEMENT,
            NT_COMPOUND_STATEMENT, NT_DECLARATION_LIST, NT_STATEMENT_LIST, NT_EXPRESSION_STATEMENT, NT_SELECTION_STATEMENT,
            NT_ITERATION_STATEMENT, NT_JUMP_STATEMENT, NT_TRANSLATION_UNIT, NT_EXTERNAL_DECLARATION,
            NT_FUNCTION_DEFINITION
        }; */
    }

    class ScanerHelper
    {
        int n_chrcnt = 1;
        int n_linecnt = 1;

        public void lexErr(string message, string yytext)
        {
            string formattedMessage = string.Empty;
            formattedMessage += string.Format("\t {0:16} ({1}:{2}){3}", yytext, n_linecnt, n_chrcnt, Environment.NewLine);
            formattedMessage += string.Format("\t^ {0}", message);

            //c_addError(formattedMessage);
        }


        void yyerror(string message, string yytext)
        {           
            string formattedMessage = string.Empty;
            formattedMessage += string.Format("\t {0:16} ({1}:{2}){3}", yytext, n_linecnt, n_chrcnt, Environment.NewLine);
            formattedMessage += string.Format("\t^ {0}", message);

	        //c_addError(buf);
        }

        public void count(string yytext)
        {
            foreach (char c in yytext)
                count(c);
        }

        public void count(char c)
        {
            if (c == '\n')
            {
                n_chrcnt = 1;
                n_linecnt++;
            }
            else if (c == '\t')
                n_chrcnt += 4 - (n_chrcnt - 1 % 4);
            else
                n_chrcnt++;
        }

        public void newLiteral(string yytext)
        {
            //TODO
        }

        public int checkTypeOrIdentifier(string yytext, int yyleng)
        {
            //TODO
            //if (c_checkType(yytext) != 0)
            //{
            //	c_newType(yytext); 
            //	return TYPE_NAME;
            //}

            if (yyleng > Config.MAX_ID_LEN) // fflush(stdout);
                lexErr(ErrorMessages.ERR_LEX_MSG_01, yytext);

            //TODO
            //c_newIdentifier(yytext);
            return (int) Tokens.IDENTIFIER;
        }
    }
}
