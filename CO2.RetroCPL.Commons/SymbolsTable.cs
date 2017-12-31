using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO2.RetroCPL.Commons
{
    public class Constants
    {
        public static readonly string TYPE_ERR         = "$type_err";
        public static readonly string TYPE_OK          = "$type_ok";
        public static readonly string TYPE_LITERAL     = "$numeric_literal";
        public static readonly string GLOBAL_FRAMEWORK = "$GLOBAL";
    }

    public class QualifiedType
    {
	    public string type         = "void";
	    public int	  arrSize      = 1;
	    public int	  pointerDepth = 0;
	    public bool	  b_unsigned   = false;
	    public bool	  b_const      = false;
	    public bool	  b_volatile   = false;
	    public bool	  b_interrupt  = false;
	
	    public QualifiedType(){  }
	
	    public QualifiedType(string typeName)
        {
		    type = typeName;
	    }

	    public QualifiedType(QualifiedType qualifiedType)
        {
	        type         = qualifiedType.type;
	        arrSize      = qualifiedType.arrSize;
	        pointerDepth = qualifiedType.pointerDepth;
	        b_unsigned   = qualifiedType.b_unsigned;
	        b_const      = qualifiedType.b_const;
	        b_volatile   = qualifiedType.b_volatile;
	        b_interrupt  = qualifiedType.b_interrupt;
	    }

	    public bool sameOutput(QualifiedType operand){
		    //TODO fix this
		    /* FUTURE <strings> */
            if ((operand.type == Constants.TYPE_LITERAL) && ((type == "int") || (type == "char")))
			    ;
		    else
		    {
			    if (type       != operand.type)       return false;
			    if (b_unsigned != operand.b_unsigned) return false;
		    }
	 
		    if (pointerDepth != operand.pointerDepth) return false;
		    if (b_interrupt != operand.b_interrupt) return false;
	 
		    return true;
	    }

	    public bool sameInput(QualifiedType operand)
	    {
		    /* FUTURE <strings> */
            if (type == Constants.TYPE_LITERAL) type = operand.type;
	 
		    if (type         != operand.type)         return false;
		    if (pointerDepth != operand.pointerDepth) return false;
		    if (b_unsigned   != operand.b_unsigned)   return false;
		    if (b_volatile   && !operand.b_volatile)  return false;
		    if (b_const      && !operand.b_const)     return false;
	 
		    return true;
	    }
	 
	    public bool equals(QualifiedType operand)
        {
		    /* FUTURE <strings> */
            if (type == Constants.TYPE_LITERAL) type = operand.type;
	 
		    if (type         != operand.type)         return false;
		    if (pointerDepth != operand.pointerDepth) return false;
		    if (b_unsigned   != operand.b_unsigned)   return false;
		    if (b_volatile   != operand.b_volatile)   return false;
		    if (b_const      != operand.b_const)      return false;
	 
		    return true;
	    }

	    public string toString()
	    {
		    string output = string.Empty;
	 
		    if (b_const) output += "const ";
	 
		    if (b_unsigned) output += "u ";
		    output += type;
	 
		    for (int i=0 ; i < pointerDepth; i++)
			    output += "*";
		 
		    /* FUTURE <arrays> arrSize; */
		 
		    return output;
	    }
    }

    public class STEntry
    {
        public string        lex;
        public QualifiedType qType;
        public string        framework;
        public int           n_uses;
        public bool          literal;
        public bool          overlappable;
        public int           address;

       public STEntry(string inLex, string inFramework)
       {
		    lex          = inLex;
		    qType        = new QualifiedType("void");
		    framework    = inFramework;
		    n_uses       = 0;
		    literal      = false;
		    address      = -1;
       }
	 
	   public STEntry(string inLex, string inFramework, QualifiedType type){
		    lex          = inLex;
		    qType        = new QualifiedType(type);
		    framework    = inFramework;
		    n_uses       = 0;
		    literal      = false;
		    overlappable = false;
		    address      = -1;
       }
        
        public void setType(string type) { qType.type = type; }

        public int getPointerDepth() { return qType.pointerDepth; }

        public void setPointerDepth(int depth) { qType.pointerDepth = depth; }

        public bool isVolatile() { return qType.b_volatile; }

        public void setVolatile() { qType.b_volatile = true; }

        public bool isConst() { return qType.b_const; }

        public void setConst() { qType.b_const = true; }

        public bool isUnsigned() { return qType.b_unsigned; }

        public void setUnsigned() { qType.b_unsigned = true; }

	    public string toString()
        {
            string output = string.Empty;
		    char   c_v;
		    string outputType;
		    string framework_b;
	 
		    c_v = qType.b_volatile? 'Y' : 'N';
		    framework_b = framework == ""? "GLOBAL VAR" : framework;
		    outputType = "";
	 
		    if (qType.b_unsigned)
			    outputType += "u ";
		    outputType += qType.type;
		    if (qType.pointerDepth > 0)
			    for (int i=0 ; i < qType.pointerDepth ; i++) outputType += "*";
	 
            //TODO FIXME
            //output << left;
            //output.fill(' ');
            //output << "| "  << setw(16) << lex.substr(0, 16);
            //output << " | " << setw(12) << outputType.substr(0, 12);
            //output.fill('0');
            //output << right;
            //output << " | " << setw(3) << hex << qType.arrSize;
            //output.fill(' ');
            //output << left;
            //output << " | " << setw(16) << framework_b.substr(0, 16);
            //output << " | " << c_v;
            //output.fill('0');
            //output << right;
            //output << " | " << setw(3)  << n_uses;
            //if (trueConstant)
            //    output << " | Const ";
            //else if (address < 0)
            //    output << " | Undef ";
            //else
            //    output << " | " << "0x" << setw(4) << hex << address;
            //output << " |"  << endl;
	 
		    return output;
	    }
    }

    public class STFramework
    {
        public int id;
        public string name;
        public QualifiedType outputType = new QualifiedType("void");
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
            if (outputType.type == "void")
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
            if(position < inputArguments.Count)
                return inputArguments[position];
            else
                return new List<STEntry>();
        }

        void resetInputType()
        {
            inputType.Clear();
            inputArguments.Clear();
        }

        public int calculateSize(SymbolsTable st){
            HashSet<int> usedLocations = new HashSet<int>();
            symbolsSize = 0;

            foreach(var symbol in symbols)
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
                for(int i = 1 ; i < symSize ; i++){	
                    if (address < 0x0100){
                        address += 0x0001;
                    }else{
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
	 
        private bool addDependency(STFramework newDependency)
        {
            if (dependencies.Contains(newDependency)) return false;	 
            dependencies.Add(newDependency);	 
            return true;
        }
	 
        private bool addSymbol(string lex)
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
            if(symbols.ContainsKey(lex))
                return symbols[lex];
            else
                return null;
        }
	 
        public List <STEntry> getSymbols()
        {
            List <STEntry> output = new List<STEntry>();

            foreach(var symbol in symbols)
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

        //bool existsSymbol(string lex){ return symbols.count(lex) > 0; }
	 
        //bool setType(const string lex, const string type){
        //    unordered_map<string, STEntry*>::const_iterator entry;
        //    entry = symbols.find(lex);
        //    if (entry == symbols.end()) return false;
	 
        //    entry->second->setType(type);
        //    return true;
        //}
	 
        //QualifiedType getType(string lex){
        //    unordered_map<string, STEntry*>::const_iterator entry;
        //    entry = symbols.find(lex);
	 
        //    if (entry != symbols.end()) return entry->second->getType();
	 
        //    return QualifiedType("");
        //}
	 
        //bool STFramework::setPointerDepth(stringlex, const int depth){
        //    unordered_map<string, STEntry*>::const_iterator entry;
        //    entry = symbols.find(lex);
        //    if (entry == symbols.end()) return false;
	 
        //    entry->second->setPointerDepth(depth);
        //    return true;
        //}
	 
        //int STFramework::getPointerDepth(stringlex){
        //    unordered_map<string, STEntry*>::const_iterator entry;
        //    entry = symbols.find(lex);
	 
        //    if (entry != symbols.end()) return entry->second->getPointerDepth();
	 
        //    return -1;
        //}
	 
	 
	 

        //bool STFramework::isVolatile(stringlex){
        //    unordered_map<string, STEntry*>::const_iterator entry;
        //    entry = symbols.find(lex);
	 
        //    if (entry != symbols.end()) return entry->second->isVolatile();
	 
        //    return false;
        //}
	 
	 
	 

        //bool STFramework::setVolatile(stringlex){
        //    unordered_map<string, STEntry*>::const_iterator entry;
        //    entry = symbols.find(lex);
	 
        //    if (entry != symbols.end()) {
        //        entry->second->setVolatile();
        //        return true;
        //    }
	 
        //    return false;
        //}
	 
	 
	 

        //bool STFramework::isConst(stringlex){
        //    unordered_map<string, STEntry*>::const_iterator entry;
        //    entry = symbols.find(lex);
	 
        //    if (entry != symbols.end()) return entry->second->isConst();
	 
        //    return false;
        //}
	 
	 
	 

        //bool STFramework::setConst(stringlex){
        //    unordered_map<string, STEntry*>::const_iterator entry;
        //    entry = symbols.find(lex);
	 
        //    if (entry != symbols.end()) {
        //        entry->second->setConst();
        //        return true;
        //    }
	 
        //    return false;
        //}
	 
	 
	 

        //bool STFramework::isLiteral(stringlex){
        //    unordered_map<string, STEntry*>::const_iterator entry;
        //    entry = symbols.find(lex);
	 
        //    if (entry != symbols.end()) return entry->second->isLiteral();
	 
        //    return false;
        //}
	 
	 
	 

        //bool STFramework::setLiteral(stringlex){
        //    unordered_map<string, STEntry*>::const_iterator entry;
        //    entry = symbols.find(lex);
	 
        //    if (entry != symbols.end()) {
        //        entry->second->setLiteral();
        //        return true;
        //    }
	 
        //    return false;
        //}
	 
	 
	 

        //bool STFramework::isUnsigned(stringlex){
        //    unordered_map<string, STEntry*>::const_iterator entry;
        //    entry = symbols.find(lex);
	 
        //    if (entry != symbols.end()) return entry->second->isUnsigned();
	 
        //    return false;
        //}
	 
	 
	 

        //bool STFramework::setUnsigned(stringlex){
        //    unordered_map<string, STEntry*>::const_iterator entry;
        //    entry = symbols.find(lex);
	 
        //    if (entry != symbols.end()) {
        //        entry->second->setUnsigned();
        //        return true;
        //    }
	 
        //    return false;
        //}
	 
	 
	 

        //int STFramework::getNUses(stringlex){
        //    unordered_map<string, STEntry*>::const_iterator entry;
        //    entry = symbols.find(lex);
	 
        //    if (entry != symbols.end())
        //        return entry->second->getNUses();
	 
        //    return -1;
        //}
	 
	 
	 

        //bool STFramework::setNUses(stringlex, int inNUses){
        //    unordered_map<string, STEntry*>::const_iterator entry;
        //    entry = symbols.find(lex);
	 
        //    if (entry != symbols.end()){
        //        entry->second->setNUses(inNUses);
        //        return true;
        //    }
	 
        //    return false;
        //}
	 
	 
	 

        //int STFramework::getAddress(stringlex){
        //    unordered_map<string, STEntry*>::const_iterator entry;
        //    entry = symbols.find(lex);
	 
        //    if (entry != symbols.end()) return entry->second->getAddress();
	 
        //    return -1;
        //}
	 
	 
	 

        //bool STFramework::setAddress(stringlex, int address){
        //    unordered_map<string, STEntry*>::const_iterator entry;
        //    entry = symbols.find(lex);
	 
        //    if (entry != symbols.end()) {
        //        entry->second->setAddress(address);
        //        return true;
        //    }
	 
        //    return false;
        //}
	 
	 
	 

        //bool STFramework::addLabel(stringnewLabel){
        //    if (existsLabel(newLabel)) return false;
        //    labels.insert(newLabel);
        //    return true;
        //}
	 
	 
	 

        //bool STFramework::existsLabel(stringlabel){
        //    return labels.find(label) != labels.end();
        //}
	 
	 
	 

        //string STFramework::toString() const{
        //    /* <future> print labels */
        //    ostringstream output;
        //    string        inputType_string = "";
	 
        //    auto f_concat = [&output](pair<string, STEntry*> const x){
        //        output << x.second->toString();
        //    };
	 
        //    auto f_printInput = [&output, &inputType_string](const QualifiedType &x){
        //        inputType_string += x.toString() + ", ";
        //    };
	 
	 
        //    if (name == GLOBAL_FRAMEWORK && symbols.size() == 0){
        //        output << endl;
        //        return output.str();
        //    }	

        //    output << "+-------------------------------------------";
        //    output << "----------------------------------+" << endl;
        //    output << "|" << setw(78) << right << "|" << endl;
        //    if(!recursive){
        //        output << "| Framework: " << left << setw(64) << name << " |" << endl;
        //    }else{
        //        output << "| Framework: (R)" << left << setw(61) << name << " |" << endl;
        //    }
        //    output << "|" << setw(78) << right << "|" << endl;
		 
        //    for_each(inputType.begin(), inputType.end(), f_printInput);
        //    if (inputType_string == "")
        //        inputType_string = "void";
        //    else
        //        inputType_string.erase(inputType_string.length() - 2, 2);
	 
        //    output << "| Input:     " << left << setw(65) << inputType_string.substr(0, 64) << right << "|" << endl;
        //    output << "| Output:    " << left << setw(64) << outputType.toString() << " |" << endl;
	 
        //    output << right;
        //    output << "| Var Mem:   " << setw(5) << symbolsSize << " bytes" << right << setw(55) << " |" << endl;
        //    output << "| Code Mem:  " << setw(5) << codeSize    << " bytes" << right << setw(55) << " |" << endl;

        //    output << "|" << setw(78) << right << "|" << endl;
	 
        //    output << "+------------------+--------------+-----+---";
        //    output << "---------------+---+-----+--------+" << endl;
	 
        //    output << "| Lexeme           | Type         | Arr | Fr";
        //    output << "amework        | V | #Us | MemAdd |" << endl;
	 
        //    output << "+------------------+--------------+-----+---";
        //    output << "---------------+---+-----+--------+" << endl;
	 
        //    for_each(symbols.begin(), symbols.end(), f_concat);
	 
        //    output << "+------------------+--------------+-----+---";
        //    output << "---------------+---+-----+--------+" << endl;
        //    output << endl;
	 
        //    return output.str();
        //}
    }

    public class Type
    {
        public string type;
        public int size;

        public Type(string inType, int inSize)
        {
            type = inType;
            size = inSize;
        }
    }

    class SymbolsTable
    {
        Dictionary<string, STFramework>  frameworks      = new Dictionary<string, STFramework>();
        Dictionary<string, string>       constants       = new Dictionary<string, string>();
        List<Type>                       types           = new List<Commons.Type>();
        List<string>                     tempIdentifiers = new List<string>();
        List<string>                     tempTypes       = new List<string>();
        List<string>                     tempLiterals    = new List<string>();
        int                              idCounter       = 0;
        int                              memSize         = 0;

        public SymbolsTable() { }
 
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
 
        private bool existsType(string name)
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

        public string poptempliteral()
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
 
        //public list<string> getInputSymbols(stringframework){
        //    if (!existsFramework(framework)) return list<string>();
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getInputSymbols();
        //}

        //public bool addInputType(stringframework, const QualifiedType &inType){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->addInputType(inType);
        //}

        //public bool addInputType(stringframework, const QualifiedType &inType, stringinSymbol){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->addInputType(inType, inSymbol);
        //}

        //public bool addInputArgument(stringframework, STEntry* argument, int position){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->addInputArgument(argument, position);
        //}

        //public list<STEntry*> getInputArguments(stringframework, int position){
        //    if (!existsFramework(framework)) return list<STEntry*>();
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getInputArguments(position);
        //}

        //public void resetInputType(stringframework){
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    framework_ptr->resetInputType();
        //}

        //public int calculateSize(stringframework){
        //    if (!existsFramework(framework)) return -1;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->calculateSize(this);
        //}

        //public bool addDependency(stringframework, STFramework* dependency){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->addDependency(dependency);
        //}

        //public bool addDependency(stringframework, stringdependency){
        //    if (!existsFramework(framework)) return false;
        //    if (!existsFramework(dependency)) return false;
 
        //    STFramework* framework_ptr = getFramework(framework);
        //    STFramework* dependency_ptr = getFramework(dependency);
 
        //    return framework_ptr->addDependency(dependency_ptr);
        //}

        //public list<STFramework*> getDependencies(stringframework){
        //    if (!existsFramework(framework)) return list<STFramework*>();
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getDependencies();
        //}

        //public bool addLabel(stringframework, stringnewLabel){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->addLabel(newLabel);
        //}

        //public bool existsLabel(stringframework, stringlabel){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->existsLabel(label);
        //}

        //public void setRecursive(stringframework){
        //    if (!existsFramework(framework)) return;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setRecursive();
        //}

        //public bool isRecursive(stringframework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->isRecursive();
        //}

        //public void setAsDefined(stringframework){
        //    if (!existsFramework(framework)) return;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setAsDefined();
        //}

        //public bool isDefined(stringframework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->isDefined();
        //}

        //public bool addSymbol(stringlex, stringframework) {
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->addSymbol(lex);
        //}

        //public bool addSymbol(stringlex, stringtype, stringframework) {
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    if (!framework_ptr->addSymbol(lex)) return false;
 
        //    return framework_ptr->setType(lex, type);
        //}

        //public bool addSymbol(stringlex, const QualifiedType &inType, stringframework) {
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->addSymbol(lex, inType);
        //}

        //public STEntry* getSymbol(stringlex, stringframework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getSymbol(lex);
        //}

        //public list <STEntry*> getSymbols(stringframework){
        //    if (!existsFramework(framework)) return list<STEntry*>();
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getSymbols();
        //}

        //public bool delSymbol(stringlex, stringframework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->delSymbol(lex);
        //}

        //public bool existsSymbol(stringlex, stringframework) {
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->existsSymbol(lex);
        //}

        //public bool setType(stringlex, stringtype, stringframework) {
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setType(lex, type);
        //}

        //public QualifiedType getType(stringlex, stringframework){
        //    if (!existsFramework(framework)) return QualifiedType("");
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getType(lex);
        //}

        //public bool setPointerDepth(stringlex, stringframework, const int depth) {
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setPointerDepth(lex, depth);
        //}

        //public int getPointerDepth(stringlex, stringframework){
        //    if (!existsFramework(framework)) return -2;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getPointerDepth(lex);
        //}

        //public bool isVolatile(stringlex, stringframework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->isVolatile(lex);
        //}

        //public bool setVolatile(stringlex, stringframework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setVolatile(lex);
        //}

        //public bool isConst(stringlex, stringframework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->isConst(lex);
        //}

        //public bool setConst(stringlex, stringframework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setConst(lex);
        //}

        //public bool isLiteral(stringlex, stringframework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->isLiteral(lex);
        //}

        //public bool setLiteral(stringlex, stringframework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setLiteral(lex);
        //}

        //public bool isUnsigned(stringlex, stringframework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->isUnsigned(lex);
        //}

        //public bool setUnsigned(stringlex, stringframework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setUnsigned(lex);
        //}

        //public int getNUses(stringlex, stringframework){
        //    if (!existsFramework(framework)) return -1;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getNUses(lex);
        //}

        //public bool setNUses(stringlex, stringframework, int inNUses){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setNUses(lex, inNUses);
        //}

        //public bool incNUses(stringlex, stringframework, int increment){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    int oldValue = framework_ptr->getNUses(lex);
        //    if (oldValue < 0) return false;
 
        //    return framework_ptr->setNUses(lex, oldValue + increment);
        //}

        //public int getAddress(stringlex, stringframework){
        //    if (!existsFramework(framework)) return -1;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getAddress(lex);
        //}

        //public bool setAddress(stringlex, stringframework, int address){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setAddress(lex, address);
        //}

        //public void setMemSize(int inSize){ memSize = inSize; }

        //public string toString() {
 
        //    int codeSize = 0;
        //    ostringstream output;
        //    const string  boxLine =
        //        "+--------------------------------------"\
        //        "---------------------------------------+";


        //    for_each(frameworks.begin(), frameworks.end(),
        //        [&codeSize](pair<string, STFramework*> const x){
        //        codeSize += x.second->getCodeSize();
        //    });

        //    output << boxLine << endl;
        //    output << "|" << setw(78) << right << "|" << endl;
        //    output << "| SYMBOLS TABLE" << right << setw(64) << " |" << endl;
        //    output << "|" << setw(78) << right << "|" << endl;
        //    output << "| # of frameworks: " << setw(5) << frameworks.size() << right << setw(55) << " |" << endl;
        //    output << "| RAM variables:   " << setw(5) << memSize  << " bytes" << right << setw(49) << " |" << endl;
        //    output << "| RAM code size:   " << setw(5) << codeSize << " bytes" << right << setw(49) << " |" << endl;
        //    output << "|" << setw(78) << right << "|" << endl;
        //    output << boxLine << endl << endl;

        //    for_each(frameworks.begin(), frameworks.end(),
        //        [this, &output](pair<string, STFramework*> const x){
        //        x.second->calculateSize(this);
        //        output << x.second->toString();
        //    });
 
        //    output << boxLine << endl;
        //    output << "|" << setw(78) << right << "|" << endl;
        //    output << "| \\SYMBOLS TABLE" << right << setw(63) << " |" << endl;
        //    output << "|" << setw(78) << right << "|" << endl;
        //    output << boxLine << endl << endl;
 
        //    return output.str();
        //}
    }
}
