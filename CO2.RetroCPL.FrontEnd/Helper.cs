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
using System.IO;

namespace CO2.RetroCPL.FrontEnd
{
    public class Helper
    {
        /// <summary>
        /// Parse the file given as input param
        /// </summary>
        /// <param name="fileName"></param>
        public static void ParseFile(string fileName)
        {
            try
            {
                if (!File.Exists(fileName))
                {
                    ErrManager.Instance.addError(string.Format("\t{0} ({1}){2}\t^{3}", fileName, 0, Environment.NewLine, ErrorMessages.ERR_SEM_MSG_1B));
                    return;
                }

                using (FileStream file = new FileStream(fileName, FileMode.Open))
                {
                    Scanner scn = new Scanner(file);
                    Parser parser = new Parser(scn);
                    parser.Parse();
                }

                SyntaxTree.Instance.typeCheck();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Initialize the SymbolsTable with the initial language types and sizes.
        /// </summary>
        public static void initLanguageRequirements()
        {
            SymbolsTable.Instance.addType(Constants.TYPE_ERR, -1);
            SymbolsTable.Instance.addType(Constants.TYPE_OK, 0);
            SymbolsTable.Instance.addType(Constants.TYPE_LITERAL, 0);
            SymbolsTable.Instance.addType("void", 0);
            SymbolsTable.Instance.addType(Constants.TYPE_BYTE, 1);
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
            List<STFramework> visitedFrameworks = new List<STFramework>();
            checkRecursion(symbolsTable.getFramework("main"), recursionEnabled, visitedFrameworks, symbolsTable, errManager);

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
            //if(errManager.existsError())
            //    return;

            if (visitedFrameworks.Contains(x))
            {
                // If isRecursive, just return
                if (x.recursive)
                    return;

                // Else, a cycle has been found. Set recursive every framework in the cycle return true
                // If recursion is not allowed, generate an error
                x.recursive = true;

                string message = ErrorMessages.ERR_SEM_MSG_16;
                if (!recursionEnabled) message += Environment.NewLine + "\t" + x.name;

                List<STFramework> visitedFrameworksCopy = visitedFrameworks.ToList();
                visitedFrameworksCopy.Reverse();
                string auxGraph = " -> " + x.name;
                foreach (var item in visitedFrameworksCopy)
                {
                    if (item == x) break;
                    if (!recursionEnabled) auxGraph = " -> " + item.name + auxGraph;
                    item.recursive = true;
                }
                message += auxGraph;

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

                        //TODO recover this
                        //Don't delete input symbols
                        //if (!framework.inputSymbols.Contains(symbol.lex))
                        //    SymbolsTable.Instance.delSymbol(symbol.lex, symbol.framework);
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
        private int n_preprechrcnt, n_prechrcnt, n_chrcnt = 1;
        private int n_linecnt = 1;

        /// <summary>
        /// n_preprechrcnt getter
        /// </summary>
        /// <returns></returns>
        public int getPreviousCharacterCounter()
        {
            return n_preprechrcnt;
        }

        /// <summary>
        /// n_linecnt getter
        /// </summary>
        /// <returns></returns>
        public int getLineCounter()
        {
            return n_linecnt;
        }

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
            n_preprechrcnt = n_prechrcnt = n_chrcnt = 1;
            n_linecnt++;
        }

        /// <summary>
        /// Keep a track of the position of the cursor in the input file.
        /// </summary>
        /// <param name="yytext">Text to advance.</param>
        public void count(string yytext, bool overwritePreviousCounters=true)
        {
            if (overwritePreviousCounters)
            {
                n_preprechrcnt = n_prechrcnt;
                n_prechrcnt = n_chrcnt;
            }

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
                n_prechrcnt = 1;
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
