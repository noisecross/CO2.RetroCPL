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

        static int displaySyntaxTree;
        static int displaySymbolsTable;
        static int displayObjectCode;
        static int performCodeTranslation;

        static int  optimizationDegree;
        static bool constShortcut;
        static bool displayVersionFlag = false;
        //static bool allowRecursion;
        static int  errorDisplayDegree = 2;



        /// <summary>
        /// Code went to this compiler because it
        /// wanted to live deliberately...
        /// It wanted to live deep and suck
        /// out all the marrow of Retro Consoles!
        /// </summary>
        /// <param name="args"></param>
        static int Main(string[] args)
        {
            Constants.MainReturn output = checkInputArgs(args);

            if (output == Constants.MainReturn.OK)
                process();

            Console.ReadKey();
            return (int)output;
        }

        private static void process()
        {
            try
            {
                initialize();

                Console.WriteLine("Loading file {0}...", programName);
                Helper.ParseFile(programName);

                Console.WriteLine(SymbolsTable.Instance.toString());
                //Console.WriteLine(SyntaxTree.toString());

                ErrManager.Instance.printStatus();

                if (!ErrManager.Instance.existsError())
                    Common.WriteLineConsoleColoured("Operation performed succesfully!", ConsoleColor.Green);

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
            catch (Exception e)
            {
                Console.WriteLine("Error running CO2.RetroCPL:");
                Common.WriteConsoleException(e, true);
            }
        }

        private static Constants.MainReturn checkInputArgs(string[] args)
        {
            Constants.MainReturn output = 0;

            switch (parseargs(args))
            {
                // Help : Display usage
                case Constants.MainReturn.HELP:
                    usage();
                    output = Constants.MainReturn.HELP;
                    break;

                // Just asked to display the version
                case Constants.MainReturn.VER:
                    output = Constants.MainReturn.VER;
                    break;

                // Input error: Display usage
                case Constants.MainReturn.BAD_ARGS:
                    usage();
                    output = Constants.MainReturn.BAD_ARGS;
                    break;

                // Input file not found. Exit
                case Constants.MainReturn.FILE_ERR:
                    output = Constants.MainReturn.FILE_ERR;
                    break;

                default:
                    break;
            }

            return output;
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
            Console.WriteLine("\t-v Display the compiler version");
        }

        private static Constants.MainReturn parseargs(string[] argv)
        {
            // Usage: CO2.RetroCPL [options] <input_file>
            int    argc = argv.Count();

	        if (argc < 1)
		        return Constants.MainReturn.BAD_ARGS;

	        if (argc == 1 && argv[0][0]== '-'){
		        switch(argv[0][1]){
		
		        case 'h':	// Display help
			        if (argv[0][2] != '\0')
				        return Constants.MainReturn.BAD_ARGS;
			        return Constants.MainReturn.HELP;
		        case 'v':  // Display version
			        if (argv[0][2] != '\0')
				        return Constants.MainReturn.BAD_ARGS;
			        displayVersionFlag = true;
			        break;
		        default:
			        return Constants.MainReturn.BAD_ARGS;
		        }
	        }

            for (int i = 0; i < argc - 1; i++)
            {
                try
                {
                    Constants.MainReturn result = parseOneArg(argv[i]);
                    if (result != Constants.MainReturn.OK)
                        return result;
                }
                catch
                {
                    return Constants.MainReturn.BAD_ARGS;
                }
            }

            if (displayVersionFlag)
            {
                Console.WriteLine("{0}{1}{0}", Environment.NewLine, Constants.VERSION);
                if (argc == 2)
                    return Constants.MainReturn.VER;
            }
	
            //if(allowRecursion){
            //    cout << endl << MAIN_PARSEARGS_MSG_06 << endl << endl;
            //}

            programName = argv[argc - 1];
            if (!File.Exists(programName))
            {
                Console.Write("Input Error: ");
                Common.WriteLineConsoleColoured("File " + programName + " was not found", ConsoleColor.Red);
                return Constants.MainReturn.FILE_ERR;
            }

	        // OK
	        return Constants.MainReturn.OK;
        }

        private static Constants.MainReturn parseOneArg(string argv)
        {
            int auxInt;

            if (argv[0] != '-')
                return Constants.MainReturn.BAD_ARGS;

            switch (argv[1])
            {
                case 'O':	// Set the optimization grade
                    auxInt = int.Parse(argv[2].ToString());
                    if (auxInt < 0 || auxInt > 3)
                        return Constants.MainReturn.BAD_ARGS;
                    if (argv[3] != '\0')
                        return Constants.MainReturn.BAD_ARGS;
                    optimizationDegree = auxInt;
                    constShortcut = (optimizationDegree > 0);
                    break;

                case 'h':	// Display help
                    if (argv[2] != '\0')
                        return Constants.MainReturn.BAD_ARGS;
                    return Constants.MainReturn.HELP;

                //case 'r':  // Allow recursion
                //    if (argv[2] != '\0')
                //        return Constants.MAIN_RET_BAD_ARGS;
                //    allowRecursion = true;
                //    break;

                case 'e':	// Set the error messages displayed
                    auxInt = int.Parse(argv[2].ToString());
                    if (auxInt < 0 || auxInt > 3)
                        return Constants.MainReturn.BAD_ARGS;
                    if (argv[3] != '\0')
                        return Constants.MainReturn.BAD_ARGS;
                    errorDisplayDegree = auxInt;
                    break;

                case 'T':	// Set the syntax tree visibility
                    auxInt = int.Parse(argv[2].ToString());
                    if (auxInt < 0 || auxInt > 2)
                        return Constants.MainReturn.BAD_ARGS;
                    if (argv[3] != '\0')
                        return Constants.MainReturn.BAD_ARGS;
                    displaySyntaxTree = auxInt;
                    break;

                case 'S':	// Set the symbols table visibility
                    auxInt = int.Parse(argv[2].ToString());
                    if (auxInt < 0 || auxInt > 2)
                        return Constants.MainReturn.BAD_ARGS;
                    if (argv[3] != '\0')
                        return Constants.MainReturn.BAD_ARGS;
                    displaySymbolsTable = auxInt;
                    break;

                case 'C':	// Set the intermediate code visibility
                    auxInt = int.Parse(argv[2].ToString());
                    if (auxInt < 0 || auxInt > 2)
                        return Constants.MainReturn.BAD_ARGS;
                    if (argv[3] != '\0')
                        return Constants.MainReturn.BAD_ARGS;
                    displayObjectCode = auxInt;
                    break;

                case 'F':	// Set the final code visibility
                    auxInt = int.Parse(argv[2].ToString());
                    if (auxInt < 0 || auxInt > 2)
                        return Constants.MainReturn.BAD_ARGS;
                    if (argv[3] != '\0')
                        return Constants.MainReturn.BAD_ARGS;
                    performCodeTranslation = auxInt;
                    break;

                case 'v':  // Display version
                    if (argv[2] != '\0')
                        return Constants.MainReturn.BAD_ARGS;
                    displayVersionFlag = true;
                    break;

                default:
                    return Constants.MainReturn.BAD_ARGS;
            };

            /* OK */
            return Constants.MainReturn.OK;
        }

        private static void initialize()
        {
	        SymbolsTable symbolsTable = new SymbolsTable();
            ErrManager   errManager   = new ErrManager(errorDisplayDegree > 1, errorDisplayDegree > 2);
            SyntaxTree   syntaxTree   = SyntaxTree.Instance;

	        Helper.initLanguageRequirements();
        }
    }
}