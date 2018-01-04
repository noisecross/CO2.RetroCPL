using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO2.RetroCPL.Commons
{
    public class SymbolsTable
    {
        Dictionary<string, STFramework>  frameworks      = new Dictionary<string, STFramework>();
        Dictionary<string, string>       constants       = new Dictionary<string, string>();
        List<Type>                       types           = new List<Commons.Type>();
        List<string>                     tempIdentifiers = new List<string>();
        List<string>                     tempTypes       = new List<string>();
        List<string>                     tempLiterals    = new List<string>();
        int                              idCounter       = 0;
        int                              memSize         = 0;

        public string currentFramework = string.Empty;

        private static SymbolsTable _uniqueSymbolsTable = null;
        public  static SymbolsTable Instance
        {
            get
            {
                if (_uniqueSymbolsTable == null)
                    _uniqueSymbolsTable = new SymbolsTable();
                return _uniqueSymbolsTable;
            }
        }

        public SymbolsTable() {
            _uniqueSymbolsTable = this;
        }
 
        public bool addFramework(string framework)
        {
            if (frameworks.ContainsKey(framework)) return false;
 
            STFramework newSymbol = new STFramework(framework, idCounter++);
            frameworks.Add(framework, newSymbol);

            return true;
        }
 
        public bool existsFramework(string framework)
        {
            return frameworks.ContainsKey(framework);
        }
 
        public STFramework getFramework(string framework)
        {
            return frameworks[framework];
        }
 
        public List<STFramework> getFrameworks()
        {
            return frameworks.Values.ToList();
        }

        public bool addConstant(string name, string constant)
        {
            if (constants.ContainsKey(name)) return false;
            constants.Add(name, constant);

            return true;
        }
        
        public bool existsConstant(string name)
        {
            return constants.ContainsKey(name);
        }
 
        public string getConstant(string name)
        {
            return constants[name];
        }
 
        public bool addType(string name, int size)
        {
            types.Add(new Type(name, size));
            return true;
        }

        public bool existsType(string name)
        {
            return types.Where(_ => _.type == name).Count() > 0;
        }
 
        public int getTypeSize(string name)
        {
            var type = types.Where(_ => _.type == name);
            if (type == null || type.Count() <= 0) return -255;
            return type.First().size;
        }
 
        public int getTypeSize(QualifiedType inType)
        {
            int output = -255;

            if (inType.pointerDepth == 0)
                output = getTypeSize(inType.type);
            else                
                output = 2; // The size of a pointer

            output *= inType.arrSize;
 
            return output;
        }
 
        public void pushTempIdentifier(string name)
        {
            tempIdentifiers.Add(name);
        }
 
        public string popTempIdentifier()
        {
            //string output = tempIdentifiers.back();
            //tempIdentifiers.pop_back();
            string output = tempIdentifiers.Last();
            tempIdentifiers.RemoveAt(tempIdentifiers.Count - 1);
            return output;
        }
 
        public void pushTempLiteral(string literalValue)
        {
            tempLiterals.Add(literalValue);
        }

        public string popTempLiteral()
        {
            string output = tempLiterals.Last();
            tempLiterals.RemoveAt(tempLiterals.Count - 1);
            return output;
        }
 
        public void pushTempType(string name)
        {
            tempIdentifiers.Add(name);
        }
 
        public string popTempType(){
            string output = tempTypes.Last();
            tempTypes.RemoveAt(tempTypes.Count - 1);
            return output;
        }
 
        public void resetNUses()
        {
            foreach(var framework in frameworks)
                foreach (var symbol in framework.Value.getSymbols())
                    symbol.n_uses = 0;
        }

        public int getId(string framework)
        {
            if (!existsFramework(framework)) return -1;
            return getFramework(framework).id;
        }

        public QualifiedType getOutputType(string framework)
        {
            if (!existsFramework(framework)) return null;
            return getFramework(framework).outputType;
        }
 
        public bool setOutputType(string framework, QualifiedType type)
        {
            if (!existsFramework(framework)) return false;
            getFramework(framework).outputType = type;

            return true;
        }

        public List<QualifiedType> getInputType(string framework)
        {
            if (!existsFramework(framework)) return new List<QualifiedType>();
            return getFramework(framework).inputType;
        }

        public List<string> getInputSymbols(string framework)
        {
            if (!existsFramework(framework)) return new List<string>();
            return getFramework(framework).inputSymbols;
        }

        public bool addInputType(string framework, QualifiedType inType)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).addInputType(inType);
        }

        public bool addInputType(string framework, QualifiedType inType, string inSymbol)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).addInputType(inType, inSymbol);
        }

        public bool addInputArgument(string framework, STEntry argument, int position)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).addInputArgument(argument, position);
        }

        public List<STEntry> getInputArguments(string framework, int position)
        {
            if (!existsFramework(framework)) return new List<STEntry>();
            if (getFramework(framework).inputArguments.Count <= position) return new List<STEntry>();
            return getFramework(framework).inputArguments[position];
        }

        public void resetInputType(string framework)
        {
            getFramework(framework).resetInputType();
        }

        public int calculateSize(string framework)
        {
            if (!existsFramework(framework)) return -1;
            return getFramework(framework).calculateSize(this);
        }

        public bool addDependency(string framework, STFramework dependency)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).addDependency(dependency);
        }

        public bool addDependency(string framework, string dependency)
        {
            if (!existsFramework(framework)) return false;
            if (!existsFramework(dependency)) return false;
            return getFramework(framework).addDependency(getFramework(dependency));
        }

        public List<STFramework> getDependencies(string framework)
        {
            if (!existsFramework(framework)) return new List<STFramework>();
            return getFramework(framework).dependencies;
        }

        public bool addLabel(string framework, string newLabel)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).addLabel(newLabel);
        }

        public bool existsLabel(string framework, string label)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).existsLabel(label);
        }

        public void setRecursive(string framework)
        {
            if (!existsFramework(framework)) return;
            getFramework(framework).recursive = true;
        }

        public bool isRecursive(string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).recursive;
        }

        public void setAsDefined(string framework)
        {
            if (!existsFramework(framework)) return;
            getFramework(framework).defined = true;
        }

        public bool isDefined(string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).defined;
        }

        public bool addSymbol(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).addSymbol(lex);
        }

        public bool addSymbol(string lex, string type, string framework)
        {
            if (!existsFramework(framework)) return false;
            if (!getFramework(framework).addSymbol(lex)) return false;
            return getFramework(framework).setType(lex, type);
        }

        public bool addSymbol(string lex, QualifiedType inType, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).addSymbol(lex, inType);
        }

        public STEntry getSymbol(string lex, string framework)
        {
            if (!existsFramework(framework)) return null;
            return getFramework(framework).getSymbol(lex);
        }

        public List<STEntry> getSymbols(string framework)
        {
            if (!existsFramework(framework)) return new List<STEntry>();
            return getFramework(framework).getSymbols();
        }

        public bool delSymbol(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).delSymbol(lex);
        }

        public bool existsSymbol(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).existsSymbol(lex);
        }

        public bool setType(string lex, string type, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).setType(lex, type);
        }

        public QualifiedType getType(string lex, string framework)
        {
            if (!existsFramework(framework)) return new QualifiedType("");
            return getFramework(framework).getType(lex);
        }

        public bool setPointerDepth(string lex, string framework, int depth)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).setPointerDepth(lex, depth);
        }

        public int getPointerDepth(string lex, string framework)
        {
            if (!existsFramework(framework)) return -2;
            return getFramework(framework).getPointerDepth(lex);
        }

        public bool isVolatile(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).isVolatile(lex);
        }

        public bool setVolatile(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).setVolatile(lex);
        }

        public bool isConst(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).isConst(lex);
        }

        public bool setConst(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).setConst(lex);
        }

        public bool isLiteral(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).isLiteral(lex);
        }

        public bool setLiteral(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).setLiteral(lex);
        }

        public bool isUnsigned(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).isUnsigned(lex);
        }

        public bool setUnsigned(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).setUnsigned(lex);
        }

        public int getNUses(string lex, string framework)
        {
            if (!existsFramework(framework)) return -1;
            return getFramework(framework).getNUses(lex);
        }

        public bool setNUses(string lex, string framework, int inNUses)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).setNUses(lex, inNUses);
        }

        public bool incNUses(string lex, string framework, int increment)
        {
            if (!existsFramework(framework)) return false;
            int oldValue = getFramework(framework).getNUses(lex);
            if (oldValue < 0) return false;
            return getFramework(framework).setNUses(lex, oldValue + increment);
        }

        public int getAddress(string lex, string framework)
        {
            if (!existsFramework(framework)) return -1;
            return getFramework(framework).getAddress(lex);
        }

        public bool setAddress(string lex, string framework, int address)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).setAddress(lex, address);
        }

        public string toString()
        { 
            int codeSize = frameworks.Sum(_ => _.Value.codeSize);
            string output = string.Empty;
            const string  boxLine = "+-----------------------------------------------------------------------------+";

            output += string.Format("{0}{1}", boxLine, Environment.NewLine);
            output += string.Format("|{0,77}|{1}", " ", Environment.NewLine);
            output += string.Format("| SYMBOLS TABLE {0,61} |{1}", " ", Environment.NewLine);
            output += string.Format("|{0,77}|{1}", " ", Environment.NewLine);
            output += string.Format("| # of frameworks: {0,5}{1,53} |{2}", frameworks.Count, " ", Environment.NewLine);
            output += string.Format("| RAM variables:   {0,5} bytes{1,47} |{2}", memSize, " ", Environment.NewLine);
            output += string.Format("| RAM code size:   {0,5} bytes{1,47} |{2}", codeSize, " ", Environment.NewLine);
            output += string.Format("|{0,77}|{1}", " ", Environment.NewLine);
            output += string.Format("{0}{1}", boxLine, Environment.NewLine);
            foreach(var framework in frameworks)
            {
                framework.Value.calculateSize(this);
                output += framework.Value.toString();
            }
            output += string.Format("{0}{1}", boxLine, Environment.NewLine);
            output += string.Format("|{0,77}|{1}", " ", Environment.NewLine);
            output += string.Format("| \\SYMBOLS TABLE {0,60} |{1}", " ", Environment.NewLine);
            output += string.Format("|{0,77}|{1}", " ", Environment.NewLine);
            output += string.Format("{0}{1}", boxLine, Environment.NewLine);
 
            return output;
        }
    }
}
