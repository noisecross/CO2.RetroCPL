/**
* |------------------------------------------|
* | CO2.RetroCPL COMPILER OPTIMIZER 2 RETROC |
* | File: Helper.cs                          |
* | v1.00, January 2018                      |
* | Author: Emilio Arango Delgado de Mendoza |
* |------------------------------------------|
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO2.RetroCPL.Commons;

namespace CO2.RetroCPL.FrontEnd
{
    public class Helper
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

        /// <summary>
        /// Initialize the SymbolsTable with the initial language types and sizes.
        /// </summary>
        public static void initLanguageRequirements()
        {
            SymbolsTable.Instance.addType(Constants.TYPE_ERR, -1);
            SymbolsTable.Instance.addType(Constants.TYPE_OK, 0);
            SymbolsTable.Instance.addType(Constants.TYPE_LITERAL, 0);
            SymbolsTable.Instance.addType("void", 0);
            SymbolsTable.Instance.addType("byte", 1);
            SymbolsTable.Instance.addType("word", 2);

            SymbolsTable.Instance.addFramework(Constants.GLOBAL_FRAMEWORK);
            SymbolsTable.Instance.currentFramework = Constants.GLOBAL_FRAMEWORK;
            SymbolsTable.Instance.setAsDefined(Constants.GLOBAL_FRAMEWORK);

            /* FUTURE <pre_compiler> add the constants __DATE__, __TIME__, __STDC__, ...*/
        }

        /// <summary>
        /// Perform the final tasks to ensure the semantic tree represents a valid one.
        /// The current configuration is done to fit a RetroCPL language.
        /// In order to build a different front-end, this piece of code should be changed.
        /// </summary>
        /// <param name="recursionEnabled">Flag which can allow recursion.</param>
        /// <returns>True if the analysis allow to start with code generation.</returns>
        public static bool validateSemanticTree(bool recursionEnabled)
        {
            SymbolsTable symbolsTable = SymbolsTable.Instance;
            ErrManager errManager = ErrManager.Instance;

            // Ensure a 'main' framework
            if (!symbolsTable.existsFramework("main"))
            {
                errManager.addError(ErrorMessages.ERR_SEM_MSG_0A);
                return false;
            }
            
            // Ensure that main don't have input arguments
            if (symbolsTable.getInputType("main").Count > 0)
            {
                errManager.addError(ErrorMessages.ERR_SEM_MSG_0E);
                return false;
            }

            // Ensure that main output is 'void'
            if (symbolsTable.getTypeSize(symbolsTable.getOutputType("main")) != 0)
            {
                errManager.addError(ErrorMessages.ERR_SEM_MSG_0E);
                return false;
            }


            // Ensure that every declared framework is also defined
            foreach (var framework in symbolsTable.getFrameworks())
            {
                if (!framework.defined)
                    errManager.addError(string.Format("{0}: {1}", framework.name, ErrorMessages.ERR_SEM_MSG_15));
            }

            // Ensure there are no call cycles if recursion disabled or mark frameworks otherwise
            if (!errManager.existsError())
            {
                List<STFramework> visitedFrameworks = new List<STFramework>();
                checkRecursion(symbolsTable.getFramework("main"), recursionEnabled, visitedFrameworks, symbolsTable, errManager);
            }

            // Ensure that no error have been detected
            if (errManager.existsError())
                return false;

            return true;
        }

        /// <summary>
        /// Look for call cycles and recursion and mark the frameworks.
        /// </summary>
        /// <param name="x">Node under check.</param>
        /// <param name="recursionEnabled">Flag to enable recursion.</param>
        /// <param name="visitedFrameworks">List containing the currently visited frameworks.</param>
        /// <param name="symbolsTable">Main instance of the symbols table.</param>
        /// <param name="errManager">Main instance of the errors manager.</param>
        private static void checkRecursion(STFramework x, bool recursionEnabled, List<STFramework> visitedFrameworks, SymbolsTable symbolsTable, ErrManager errManager)
        {
            // bool output = false;
            if(errManager.existsError())
                return;

            if (visitedFrameworks.Contains(x))
            {
                // If isRecursive, just return
                if(x.recursive)
                    return;
                
                // Else, a cycle has been found. Set recursive every framework in the cycle return true
                // If recursion is not allowed, generate an error
                x.recursive = true;
                
                string message = ErrorMessages.ERR_SEM_MSG_16;
                if (!recursionEnabled) message += " " + x.name;

                List<STFramework> visitedFrameworksCopy = visitedFrameworks.ToList();
                visitedFrameworksCopy.Reverse();
                foreach (var item in visitedFrameworksCopy)
                {
                    if (item == x) break;
                    if (!recursionEnabled) message += " -> " + item.name;
                    item.recursive = true;
                }
                
                if (!recursionEnabled)
                {
                    message += ErrorMessages.ERR_SEM_MSG_17;
                    errManager.addError(message);
                }
                return;
            }

            visitedFrameworks.Add(x);
            foreach (var item in x.dependencies)
                checkRecursion(item, recursionEnabled, visitedFrameworks, symbolsTable, errManager);
            visitedFrameworks.Remove(x);
        }

        /// <summary>
        /// Deletes every unused symbol of the symbols table.
        /// </summary>
        public static void killUnusedSymbols()
        {
            foreach(var framework in SymbolsTable.Instance.getFrameworks())
                foreach(var symbol in framework.getSymbols().ToList())
                    if (symbol.n_uses == 0 && !symbol.qType.b_volatile && symbol.qType.pointerDepth == 0)
                    {
                        string message = string.Format("{0} \'{1}\' @ {2}", ErrorMessages.WAR_SEM_MSG_07, symbol.lex, symbol.framework);
                        ErrManager.Instance.addWarning(message);
                        SymbolsTable.Instance.delSymbol(symbol.lex, symbol.framework);
                    }
        }

        /// <summary>
        /// Truncate the string to match the given size at maximum.
        /// </summary>
        public static string substr(string input, int size)
        {
            return input.Substring(0, Math.Min(size, input.Length));
        }
    }

    public class ScanerHelper
    {
        public int n_chrcnt = 1;
        public int n_linecnt = 1;

        /// <summary>
        /// Add a new error message to the main ErrManager instance.
        /// </summary>
        /// <param name="message">Message retrived by the scanner.</param>
        /// <param name="yytext">Text which thrown the error.</param>
        public void lexErr(string message, string yytext)
        {
            string formattedMessage = string.Empty;
            formattedMessage += string.Format("\t{0} ({1}:{2}){3}", yytext, n_linecnt, n_chrcnt - yytext.Length, Environment.NewLine);
            formattedMessage += string.Format("\t^ {0}", message);

            ErrManager.Instance.addError(formattedMessage);
        }

        /// <summary>
        /// Add a new error message to the main ErrManager instance.
        /// </summary>
        /// <param name="message">Message retrived by the parser.</param>
        /// <param name="yytext">Text which thrown the error.</param>
        public void yyerror(string message, string yytext)
        {           
            string formattedMessage = string.Empty;
            formattedMessage += string.Format("\t{0} ({1}:{2}){3}", yytext, n_linecnt, n_chrcnt - yytext.Length, Environment.NewLine);
            formattedMessage += string.Format("\t^ {0}", message);

            ErrManager.Instance.addError(formattedMessage);
        }

        public void incrementLine(string yytext)
        {
            n_chrcnt = 1;
            n_linecnt++;
        }

        /// <summary>
        /// Keep a track of the position of the cursor in the input file.
        /// </summary>
        /// <param name="yytext">Text to advance.</param>
        public void count(string yytext)
        {
            foreach (char c in yytext)
                count(c);
        }

        /// <summary>
        /// Keep a track of the position of the cursor in the input file.
        /// </summary>
        /// <param name="c">Character to advance.</param>
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

        /// <summary>
        /// Add a new temp literal to the symbols table.
        /// </summary>
        /// <param name="yytext">Literal text.</param>
        public void newLiteral(string yytext)
        {
            SymbolsTable.Instance.pushTempLiteral(yytext);
        }

        /// <summary>
        /// Check if a text is a type name or an identifier.
        /// </summary>
        /// <param name="yytext">Text to check.</param>
        /// <param name="yyleng">Length of the text.</param>
        /// <returns>Token.</returns>
        public int checkTypeOrIdentifier(string yytext, int yyleng)
        {
            if (SymbolsTable.Instance.existsType(yytext))
            {
                SymbolsTable.Instance.pushTempType(yytext);
                return (int) Tokens.TYPE_NAME;
            }

            if (yyleng > Config.MAX_ID_LEN) // fflush(stdout);
                lexErr(ErrorMessages.ERR_LEX_MSG_01, yytext);

            SymbolsTable.Instance.pushTempIdentifier(yytext);
            return (int) Tokens.IDENTIFIER;
        }
    }
}
