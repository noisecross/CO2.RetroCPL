using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO2.RetroCPL.Commons
{
    public class STFramework
    {
        public int id;
        public string name;
        public QualifiedType outputType = new QualifiedType("void"); //TODO fix this
        public List<QualifiedType> inputType = new List<QualifiedType>();
        public List<string> inputSymbols = new List<string>();
        public List<List<STEntry>> inputArguments = new List<List<STEntry>>();
        public Dictionary<string, STEntry> symbols = new Dictionary<string, STEntry>();
        public HashSet<string> labels = new HashSet<string>();
        public List<STFramework> dependencies = new List<STFramework>();
        public int codeSize = 0;
        public int symbolsSize = 0;
        public bool defined = false;
        public bool recursive = false;

        public STFramework(string inName, int inId)
        {
            name = inName;
            id = inId;
        }

        public bool setOutputType(QualifiedType inOutType)
        {
            if (outputType.type == "void") //TODO fix this
            {
                outputType = inOutType;
                return true;
            }
            return false;
        }

        public bool addInputType(QualifiedType inType)
        {
            inputType.Add(inType);
            inputArguments.Add(new List<STEntry>());
            return true;
        }

        public bool addInputType(QualifiedType inType, string inSymbol)
        {
            inputType.Add(inType);
            inputSymbols.Add(inSymbol);
            inputArguments.Add(new List<STEntry>());
            return true;
        }

        public bool addInputArgument(STEntry argument, int position)
        {
            inputArguments[position].Add(argument);
            return true;
        }

        public List<STEntry> getinputarguments(int position)
        {
            if (position < inputArguments.Count)
                return inputArguments[position];
            else
                return new List<STEntry>();
        }

        public void resetInputType()
        {
            inputType.Clear();
            inputArguments.Clear();
        }

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

        public bool addDependency(STFramework newDependency)
        {
            if (dependencies.Contains(newDependency)) return false;
            dependencies.Add(newDependency);
            return true;
        }

        public bool addSymbol(string lex)
        {
            if (symbols.ContainsKey(lex)) return false;
            symbols.Add(lex, new STEntry(lex, name));
            return true;
        }

        public bool addSymbol(string lex, QualifiedType inType)
        {
            if (symbols.ContainsKey(lex)) return false;
            symbols.Add(lex, new STEntry(lex, name, inType));
            return true;
        }

        public STEntry getSymbol(string lex)
        {
            if (symbols.ContainsKey(lex))
                return symbols[lex];
            else
                return null;
        }

        public List<STEntry> getSymbols()
        {
            List<STEntry> output = new List<STEntry>();

            foreach (var symbol in symbols)
                output.Add(symbol.Value);

            return output;
        }

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

        public bool existsSymbol(string lex)
        {
            return symbols.ContainsKey(lex);
        }

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

        public QualifiedType getType(string lex)
        {
            if (symbols.ContainsKey(lex))
                return symbols[lex].qType;
            else
                return new QualifiedType("");
        }

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

        public int getPointerDepth(string lex)
        {
            if (symbols.ContainsKey(lex))
                return symbols[lex].qType.pointerDepth;
            else
                return -1;
        }

        public bool isVolatile(string lex)
        {
            if (symbols.ContainsKey(lex))
                return symbols[lex].qType.b_volatile;
            else
                return false;
        }

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

        public bool isConst(string lex)
        {
            if (symbols.ContainsKey(lex))
                return symbols[lex].qType.b_const;
            else
                return false;
        }

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

        public bool isLiteral(string lex)
        {
            if (symbols.ContainsKey(lex))
                return symbols[lex].isLiteral;
            else
                return false;
        }

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

        public bool isUnsigned(string lex)
        {
            if (symbols.ContainsKey(lex))
                return symbols[lex].qType.b_unsigned;
            else
                return false;
        }

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

        public int getNUses(string lex)
        {
            if (symbols.ContainsKey(lex))
                return symbols[lex].n_uses;
            else
                return -1;
        }

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

        public int getAddress(string lex)
        {
            if (symbols.ContainsKey(lex))
                return symbols[lex].address;
            else
                return -1;
        }

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

        public bool addLabel(string newLabel)
        {
            if (existsLabel(newLabel)) return false;
            labels.Add(newLabel);
            return true;
        }

        public bool existsLabel(string label)
        {
            return labels.Contains(label);
        }

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
            output += string.Format("|{0:78}|{1}", " ", Environment.NewLine);
            if (!recursive)
                output += string.Format("| Framework: {0:64} |{1}", name, Environment.NewLine);
            else
                output += string.Format("| Framework: (R){0:61} |{1}", name, Environment.NewLine);
            output += string.Format("|{0:78}|{1}", " ", Environment.NewLine);
            output += string.Format("| Input:     {0:65} |{1}", inputType_string.Substring(0, Math.Min(64, inputType_string.Length)), Environment.NewLine);
            output += string.Format("| Output:    {0:65} |{1}", outputType.toString(), Environment.NewLine);
            output += string.Format("| Var Mem:   {0:5} bytes {1:53} |{2}", symbolsSize, " ", Environment.NewLine);
            output += string.Format("| Code Mem:  {0:5} bytes {1:53) |{2}", codeSize, " ", Environment.NewLine);
            output += string.Format("|{0:78}|{1}", " ", Environment.NewLine);
            output += string.Format("+------------------+--------------+-----+------------------+---+-----+--------+{0}", Environment.NewLine);
            output += string.Format("| Lexeme           | Type         | Arr | Framework        | V | #Us | MemAdd |{0}", Environment.NewLine);
            output += string.Format("+------------------+--------------+-----+------------------+---+-----+--------+{0}", Environment.NewLine);
            foreach (var item in symbols)
                output += item.Value.toString();
            output += string.Format("+------------------+--------------+-----+------------------+---+-----+--------+{0}", Environment.NewLine);
            output += string.Format("{0}", Environment.NewLine);

            return output;
        }
    }
}
