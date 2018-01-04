using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CO2.RetroCPL.FrontEnd;
using CO2.RetroCPL.Commons;

namespace CO2.RetroCPL
{
    class Program
    {
        static string programName;

        static int showSyntaxTree;
        static int showSymbolsTable;
        static int showObjectCode;
        static int performCodeTranslation;

        static int  optimizationGrade;
        static bool constShortcut;
        static bool showVersionFlag = false;
        //static bool allowRecursion;
        static int  errorShownGrade;



        static void Main(string[] args)
        {
            int output = 0;

            switch (parseargs(args))
            {
                case Constants.MAIN_RET_HELP:     // Help : Show usage
                    usage();
                    output = Constants.MAIN_RET_HELP;
                    break;
                case Constants.MAIN_RET_VER:      // Just asked to show the version
                    output = Constants.MAIN_RET_VER;
                    break;
                case Constants.MAIN_RET_BAD_ARGS: // Input error: Show usage
                    usage();
                    output = Constants.MAIN_RET_BAD_ARGS;
                    break;
                case Constants.MAIN_RET_FILE_ERR:	// Input file not found. Exit
                    output = Constants.MAIN_RET_FILE_ERR;
                    break;
                default:
                    break;
            }

            if (output == Constants.MAIN_RET_OK)
                process();

            Console.ReadKey();
        }

        private static void process()
        {
            FileStream file = null;
            Scanner scn = null;
            Parser parser = null;

            try
            {
                initialize();

                Console.WriteLine("Loading file {0}...", programName);
                file = new FileStream(programName, FileMode.Open);
                scn = new FrontEnd.Scanner(file);
                parser = new FrontEnd.Parser(scn);
                parser.Parse();

                if (ErrManager.Instance.existsError())
                    Console.WriteLine(ErrManager.Instance.toString());

                Console.WriteLine(SyntaxTree.toString());
                Console.WriteLine(SymbolsTable.Instance.toString());

                //program = parser.program;

                //if (program != null)
                //{
                //    program.Execute();
                //    Console.WriteLine();
                //}

                //outputFile.open(programName + ".asm");
                //if(outputFile.bad())
                //{
                //    inputFile.close();
                //    cout <<  MAIN_PARSEARGS_MSG_04 << programName << ".asm" << MAIN_PARSEARGS_MSG_05 << endl;
                //    return Constants.MAIN_RET_FILE_ERR;
                //}
            }
            finally
            {
                file.Close();
            }
        }

        private static void usage()
        {
            Console.WriteLine("Usage: CO2.RetroCPL [options] <input_file>");
            Console.WriteLine("Options:");
            Console.WriteLine("\t-e <number> Set the error messages displayed (3 by default)");
            Console.WriteLine("\t\t 0\tDoesn't display any messages");
            Console.WriteLine("\t\t 1\tDisplays error messages only");
            Console.WriteLine("\t\t 2\tDisplays also warnings");
            Console.WriteLine("\t\t 3\tDisplays also info messages");
            Console.WriteLine("\t-h Displays this help");
            //Console.WriteLine("\t-r Allow recursion");
            Console.WriteLine("\t-O <number> Set the optimization degree (2 by default)");
            Console.WriteLine("\t\t 0\tOptimizations disabled");
            Console.WriteLine("\t\t 1\tFew optimizations");
            Console.WriteLine("\t\t 2\tMid grade optimizations");
            Console.WriteLine("\t\t 3\tAll optimizations");
            Console.WriteLine("\t-T <number> Syntax Tree visibility   (0 by default)");
            Console.WriteLine("\t-S <number> Symbols table visibility (0 by default)");
            Console.WriteLine("\t-C <number> Object code visibility   (0 by default)");
            Console.WriteLine("\t-F <number> End code visibility      (2 by default)");
            Console.WriteLine("\t\t 0\tNo visibility");
            Console.WriteLine("\t\t 1\tDisplayed on console");
            Console.WriteLine("\t\t 2\tStored in a file");
            Console.WriteLine("\t-v Show the compiler version");
        }

        private static int parseargs(string[] argv)
        {
            // Usage: CO2.RetroCPL [options] <input_file>
            int    argc = argv.Count();

	        if (argc < 1)
		        return Constants.MAIN_RET_BAD_ARGS;

	        if (argc == 1 && argv[0][0]== '-'){
		        switch(argv[0][1]){
		
		        case 'h':	// Show help
			        if (argv[0][2] != '\0')
				        return Constants.MAIN_RET_BAD_ARGS;
			        return Constants.MAIN_RET_HELP;
		        case 'v':  // Show version
			        if (argv[0][2] != '\0')
				        return Constants.MAIN_RET_BAD_ARGS;
			        showVersionFlag = true;
			        break;
		        default:
			        return Constants.MAIN_RET_BAD_ARGS;
		        }
	        }

            for (int i = 0; i < argc - 1; i++)
            {
                try
                {
                    int result = parseOneArg(argv[i]);
                    if (result != Constants.MAIN_RET_OK)
                        return result;
                }
                catch
                {
                    return Constants.MAIN_RET_BAD_ARGS;
                }
            }

            if (showVersionFlag)
            {
                Console.WriteLine("{0}{1}{0}", Environment.NewLine, Constants.VERSION);
                if (argc == 2)
                    return Constants.MAIN_RET_VER;
            }
	
            //if(allowRecursion){
            //    cout << endl << MAIN_PARSEARGS_MSG_06 << endl << endl;
            //}

            programName = argv[argc - 1];
            if (!File.Exists(programName))
            {
                Console.WriteLine("Input Error: File {0} was not found", programName);
                return Constants.MAIN_RET_FILE_ERR;
            }

	        // OK
	        return Constants.MAIN_RET_OK;
        }

        private static int parseOneArg(string argv)
        {
            int auxInt;

            if (argv[0] != '-')
                return Constants.MAIN_RET_BAD_ARGS;

            switch (argv[1])
            {
                case 'O':	// Set the optimization grade
                    auxInt = int.Parse(argv[2].ToString());
                    if (auxInt < 0 || auxInt > 3)
                        return Constants.MAIN_RET_BAD_ARGS;
                    if (argv[3] != '\0')
                        return Constants.MAIN_RET_BAD_ARGS;
                    optimizationGrade = auxInt;
                    constShortcut = (optimizationGrade > 0);
                    break;

                case 'h':	// Show help
                    if (argv[2] != '\0')
                        return Constants.MAIN_RET_BAD_ARGS;
                    return Constants.MAIN_RET_HELP;

                //case 'r':  // Allow recursion
                //    if (argv[2] != '\0')
                //        return Constants.MAIN_RET_BAD_ARGS;
                //    allowRecursion = true;
                //    break;

                case 'e':	// Set the error messages shown
                    auxInt = int.Parse(argv[2].ToString());
                    if (auxInt < 0 || auxInt > 3)
                        return Constants.MAIN_RET_BAD_ARGS;
                    if (argv[3] != '\0')
                        return Constants.MAIN_RET_BAD_ARGS;
                    errorShownGrade = auxInt;
                    break;

                case 'T':	// Set the syntax tree visibility
                    auxInt = int.Parse(argv[2].ToString());
                    if (auxInt < 0 || auxInt > 2)
                        return Constants.MAIN_RET_BAD_ARGS;
                    if (argv[3] != '\0')
                        return Constants.MAIN_RET_BAD_ARGS;
                    showSyntaxTree = auxInt;
                    break;

                case 'S':	// Set the symbols table visibility
                    auxInt = int.Parse(argv[2].ToString());
                    if (auxInt < 0 || auxInt > 2)
                        return Constants.MAIN_RET_BAD_ARGS;
                    if (argv[3] != '\0')
                        return Constants.MAIN_RET_BAD_ARGS;
                    showSymbolsTable = auxInt;
                    break;

                case 'C':	// Set the intermediate code visibility
                    auxInt = int.Parse(argv[2].ToString());
                    if (auxInt < 0 || auxInt > 2)
                        return Constants.MAIN_RET_BAD_ARGS;
                    if (argv[3] != '\0')
                        return Constants.MAIN_RET_BAD_ARGS;
                    showObjectCode = auxInt;
                    break;

                case 'F':	// Set the final code visibility
                    auxInt = int.Parse(argv[2].ToString());
                    if (auxInt < 0 || auxInt > 2)
                        return Constants.MAIN_RET_BAD_ARGS;
                    if (argv[3] != '\0')
                        return Constants.MAIN_RET_BAD_ARGS;
                    performCodeTranslation = auxInt;
                    break;

                case 'v':  // Show version
                    if (argv[2] != '\0')
                        return Constants.MAIN_RET_BAD_ARGS;
                    showVersionFlag = true;
                    break;

                default:
                    return Constants.MAIN_RET_BAD_ARGS;
            };

            /* OK */
            return Constants.MAIN_RET_OK;
        }

        private static void initialize()
        {
	        SymbolsTable symbolsTable = new SymbolsTable();
            ErrManager   errManager   = new ErrManager(errorShownGrade > 1, errorShownGrade > 2);
            SyntaxTree   syntaxTree   = SyntaxTree.Instance;

	        Helper.initLanguageRequirements();
        }
    }
}