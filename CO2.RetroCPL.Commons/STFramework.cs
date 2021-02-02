/**
* |------------------------------------------|
* | CO2.RetroCPL COMPILER OPTIMIZER 2 RETROC |
* | File: STFramework.cs                     |
* | v1.00, January 2018                      |
* | Author: Emilio Arango Delgado de Mendoza |
* |------------------------------------------|
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO2.RetroCPL.Commons
{
    public class STFramework
    {
        public int                         id;
        public string                      name;
        public QualifiedType               outputType     = new QualifiedType("void");
        public List<QualifiedType>         inputType      = new List<QualifiedType>();
        public List<string>                inputSymbols   = new List<string>();
        public List<List<STEntry>>         inputArguments = new List<List<STEntry>>();
        public Dictionary<string, STEntry> symbols        = new Dictionary<string, STEntry>();
        public HashSet<string>             labels         = new HashSet<string>();
        public List<STFramework>           dependencies   = new List<STFramework>();
        public int                         codeSize       = 0;
        public int                         symbolsSize    = 0;
        public bool                        defined        = false;
        public bool                        recursive      = false;
        
        #region Framework methods

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="inName">New framework name.</param>
        /// <param name="inId">Id for the new framework.</param>
        public STFramework(string inName, int inId)
        {
            name = inName;
            id = inId;
        }

        /// <summary>
        /// Set the output type to the framework.
        /// </summary>
        /// <param name="inOutType">Type to set.</param>
        /// <returns></returns>
        public bool setOutputType(QualifiedType inOutType)
        {
            if (outputType.type == "void")
            {
                outputType = inOutType;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add an input type to the framework.
        /// </summary>
        /// <param name="inType">Type to add.</param>
        /// <returns>True if the type wasn't set before.</returns>
        public bool addInputType(QualifiedType inType)
        {
            inputType.Add(inType);
            inputArguments.Add(new List<STEntry>());
            return true;
        }

        /// <summary>
        /// Add an input type to the framework.
        /// </summary>
        /// <param name="inType">Type to add.</param>
        /// <param name="inSymbol">Symbol to add.</param>
        /// <returns>True if the type wasn't set before.</returns>
        public bool addInputType(QualifiedType inType, string inSymbol)
        {
            inputType.Add(inType);
            inputSymbols.Add(inSymbol);
            inputArguments.Add(new List<STEntry>());

            addSymbol(inSymbol, inType);
            return true;
        }

        /// <summary>
        /// Add an argument in a given position to perform future optimizations.
        /// </summary>
        /// <param name="argument">The pointer to the STEntry argument.</param>
        /// <param name="position">The place in the argument list where the symbol is allocated.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool addInputArgument(STEntry argument, int position)
        {
            inputArguments[position].Add(argument);
            return true;
        }

        /// <summary>
        /// Get the symbol list related with the argument in a given position to perform future optimizations.
        /// </summary>
        /// <param name="position">The argument place in the argument list.</param>
        /// <returns>The list of symbols related to that argument.</returns>
        public List<STEntry> getinputarguments(int position)
        {
            if (position < inputArguments.Count)
                return inputArguments[position];
            else
                return new List<STEntry>();
        }

        /// <summary>
        /// Reset the input types of the framework.
        /// </summary>
        public void resetInputType()
        {
            inputType.Clear();
            inputArguments.Clear();
        }

        /// <summary>
        /// Calculates the memory needed by the framework to store all symbols.
        /// </summary>
        /// <param name="st">Symbols table containing the type sizes.</param>
        /// <returns>The number of bytes used to store all symbols.</returns>
        public int calculateSize(SymbolsTable st)
        {
            HashSet<int> usedLocations = new HashSet<int>();
            symbolsSize = 0;

            foreach (var symbol in symbols)
            {
                int symSize = st.getTypeSize(symbol.Value.qType);
                int address = symbol.Value.address;

                if (address < 0)
                    break;

                /* Add 1 byte to 1 byte symbols */
                if (!usedLocations.Contains(address))
                {
                    usedLocations.Add(address);
                    symbolsSize++;
                }

                /* Add more bytes to bigger symbols */
                for (int i = 1; i < symSize; i++)
                {
                    if (address < 0x0100)
                    {
                        address += 0x0001;
                    }
                    else
                    {
                        address += 0x0100;
                    }

                    if (!usedLocations.Contains(address))
                    {
                        usedLocations.Add(address);
                        symbolsSize++;
                    }
                }
            }

            return symbolsSize;
        }

        /// <summary>
        ///  Stores a new dependency in the framework.
        /// </summary>
        /// <param name="newDependency">The new dependency.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool addDependency(STFramework newDependency)
        {
            if (dependencies.Contains(newDependency)) return false;
            dependencies.Add(newDependency);
            return true;
        }

        /// <summary>
        /// Add a new label to the framework namespace.
        /// </summary>
        /// <param name="newLabel">The name of the new label.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool addLabel(string newLabel)
        {
            if (existsLabel(newLabel)) return false;
            labels.Add(newLabel);
            return true;
        }

        /// <summary>
        /// Check if a label exists in the framework namespace.
        /// </summary>
        /// <param name="label">The name of the label to check.</param>
        /// <returns>True if the label exists.</returns>
        public bool existsLabel(string label)
        {
            return labels.Contains(label);
        }

        #endregion

        #region Frameworks and Symbols

        /// <summary>
        /// Add a new symbol to the framework.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool addSymbol(string lex)
        {
            if (symbols.ContainsKey(lex)) return false;
            symbols.Add(lex, new STEntry(lex, name));
            return true;
        }

        /// <summary>
        /// Add a new symbol to the framework.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="inType">The type of the symbol.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool addSymbol(string lex, QualifiedType inType)
        {
            if (symbols.ContainsKey(lex)) return false;
            symbols.Add(lex, new STEntry(lex, name, inType));
            return true;
        }

        /// <summary>
        /// Get a pointer to a given symbol of the framework.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <returns>The symbol if the operation was performed successfully.</returns>
        public STEntry getSymbol(string lex)
        {
            if (symbols.ContainsKey(lex))
                return symbols[lex];
            else
                return null;
        }
        
        /// <summary>
        /// Get the list of symbols in the framework.
        /// </summary>
        /// <returns>The symbol list.</returns>
        public List<STEntry> getSymbols()
        {
            List<STEntry> output = new List<STEntry>();

            foreach (var symbol in symbols)
                output.Add(symbol.Value);

            return output;
        }

        /// <summary>
        /// Delete a symbol from the framework.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool delSymbol(string lex)
        {
            if (symbols.ContainsKey(lex))
            {
                symbols.Remove(lex);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Check the existence of a symbol with a certain name in the current framework.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <returns>True if a symbol with the given name exists.</returns>
        public bool existsSymbol(string lex)
        {
            return symbols.ContainsKey(lex);
        }

        /// <summary>
        /// Set the type of a certain symbol.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="type"></param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool setType(string lex, string type)
        {
            if (symbols.ContainsKey(lex))
            {
                symbols[lex].qType.type = type;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Get the type of a certain symbol.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <returns>The type of the symbol.</returns>
        public QualifiedType getType(string lex)
        {
            if (symbols.ContainsKey(lex))
                return symbols[lex].qType;
            else
                return new QualifiedType("");
        }

        /// <summary>
        /// Set the pointer depth of a certain symbol.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="depth">The depth to set.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool setPointerDepth(string lex, int depth)
        {
            if (symbols.ContainsKey(lex))
            {
                symbols[lex].qType.pointerDepth = depth;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Get the pointer depth of a certain symbol.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <returns>The pointer depth of the symbol or -1 if the symbol doesn't exist.</returns>
        public int getPointerDepth(string lex)
        {
            if (symbols.ContainsKey(lex))
                return symbols[lex].qType.pointerDepth;
            else
                return -1;
        }

        /// <summary>
        /// Check if a certain symbol is volatile.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <returns>The volatile flag of the symbol.</returns>
        public bool isVolatile(string lex)
        {
            if (symbols.ContainsKey(lex))
                return symbols[lex].qType.b_volatile;
            else
                return false;
        }

        /// <summary>
        /// Set a certain symbol as volatile.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool setVolatile(string lex)
        {
            if (symbols.ContainsKey(lex))
            {
                symbols[lex].qType.b_volatile = true;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Check if a certain symbol is const.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <returns>The const flag of the symbol.</returns>
        public bool isConst(string lex)
        {
            if (symbols.ContainsKey(lex))
                return symbols[lex].qType.b_const;
            else
                return false;
        }

        /// <summary>
        /// Set a certain symbol as const.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool setConst(string lex)
        {
            if (symbols.ContainsKey(lex))
            {
                symbols[lex].qType.b_const = true;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Check if a certain symbol is literal.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <returns>The literal flag of the symbol.</returns>
        public bool isLiteral(string lex)
        {
            if (symbols.ContainsKey(lex))
                return symbols[lex].isLiteral;
            else
                return false;
        }

        /// <summary>
        /// Set a certain symbol as literal.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool setLiteral(string lex)
        {
            if (symbols.ContainsKey(lex))
            {
                symbols[lex].isLiteral = true;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Check if a certain symbol is unsigned.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <returns>The unsigned flag of the symbol.</returns>
        public bool isUnsigned(string lex)
        {
            if (symbols.ContainsKey(lex))
                return symbols[lex].qType.b_unsigned;
            else
                return false;
        }

        /// <summary>
        /// Set a certain symbol as unsigned.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool setUnsigned(string lex)
        {
            if (symbols.ContainsKey(lex))
            {
                symbols[lex].qType.b_unsigned = true;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Set a certain symbol as signed.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool clearUnsigned(string lex)
        {
            if (symbols.ContainsKey(lex))
            {
                symbols[lex].qType.b_unsigned = false;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Get the number of uses from a certain symbol.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <returns>The number of uses or -1 if the operation was performed with failures.</returns>
        public int getNUses(string lex)
        {
            if (symbols.ContainsKey(lex))
                return symbols[lex].n_uses;
            else
                return -1;
        }

        /// <summary>
        /// Set the number of uses from a certain symbol.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="inNUses">Tthe number of uses to set.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool setNUses(string lex, int inNUses)
        {
            if (symbols.ContainsKey(lex))
            {
                symbols[lex].n_uses = inNUses;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Get the current proposed memory address of a certain symbol.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <returns>The current proposed memory address of the symbol.</returns>
        public int getAddress(string lex)
        {
            if (symbols.ContainsKey(lex))
                return symbols[lex].address;
            else
                return -1;
        }

        /// <summary>
        /// Set a proposed memory address to a certain symbol.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="address">The new address to set.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool setAddress(string lex, int address)
        {
            if (symbols.ContainsKey(lex))
            {
                symbols[lex].address = address;
                return true;
            }
            else
                return false;
        }

        #endregion

        /// <summary>
        /// Pretty printer.
        /// </summary>
        /// <returns>String containing the text formated framework.</returns>
        public string toString()
        {
            /* <future> print labels */
            string output = string.Empty;
            string inputType_string = string.Empty;

            if (name == Constants.GLOBAL_FRAMEWORK && symbols.Count == 0)
            {
                output += Environment.NewLine;
                return output;
            }

            foreach (var item in inputType)
                inputType_string += item.toString() + ", ";
            if (inputType_string == "")
                inputType_string = "void";
            else
                inputType_string = inputType_string.Substring(0, inputType_string.Length - 2);

            output += string.Format("+-----------------------------------------------------------------------------+{0}", Environment.NewLine);
            output += string.Format("|{0,77}|{1}", " ", Environment.NewLine);
            if (!recursive)
                output += string.Format("| Framework: {0,-64} |{1}", name, Environment.NewLine);
            else
                output += string.Format("| Framework: (R){0,-61} |{1}", name, Environment.NewLine);
            output += string.Format("|{0,77}|{1}", " ", Environment.NewLine);
            output += string.Format("| Input:     {0,-64} |{1}", inputType_string.Substring(0, Math.Min(64, inputType_string.Length)), Environment.NewLine);
            output += string.Format("| Output:    {0,-64} |{1}", outputType.toString(), Environment.NewLine);
            if(codeSize > 0)
            {
                output += string.Format("| Var Mem:   {0,5} bytes {1,52} |{2}", symbolsSize, " ", Environment.NewLine);
                output += string.Format("| Code Mem:  {0,5} bytes {1,52} |{2}", codeSize, " ", Environment.NewLine);
            }
            output += string.Format("|{0,77}|{1}", " ", Environment.NewLine);
            output += string.Format("+------------------+--------------+-----+------------------+---+-----+--------+{0}", Environment.NewLine);
            output += string.Format("| Symbol           | Type         | Arr | Framework        | V | #Us | MemAdd |{0}", Environment.NewLine);
            output += string.Format("+------------------+--------------+-----+------------------+---+-----+--------+{0}", Environment.NewLine);
            foreach (var item in symbols)
                output += item.Value.toString();
            output += string.Format("+------------------+--------------+-----+------------------+---+-----+--------+{0}", Environment.NewLine);
            output += string.Format("{0}", Environment.NewLine);

            return output;
        }
    }
}
