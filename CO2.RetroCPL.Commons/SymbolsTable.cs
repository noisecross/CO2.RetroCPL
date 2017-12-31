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
	 
	    void setType(string type) { qType.type = type; }
	 
	    int getPointerDepth() { return qType.pointerDepth; }
	 
	    void setPointerDepth(int depth) { qType.pointerDepth = depth; }
	 
	    bool isVolatile() { return qType.b_volatile; }
	 
	    void setVolatile() { qType.b_volatile = true; }
	 
	    bool isConst() { return qType.b_const; }
	 
	    void setConst() { qType.b_const = true; }
	 
        bool isUnsigned() { return qType.b_unsigned; }
	 
	    void setUnsigned() { qType.b_unsigned = true; }

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

        bool setOutputType(QualifiedType inOutType)
        {
            if (outputType.type == "void")
            {
                outputType = inOutType;
                return true;
            }
            return false;
        }

        bool addInputType(QualifiedType inType){
		    inputType.Add(inType);
            inputArguments.Add(new List<STEntry>());
		    return true;
	    }

        //bool addInputType(const QualifiedType &inType, const string &inSymbol){
        //    inputType.push_back(inType);
        //    inputSymbols.push_back(inSymbol);
        //    inputArguments.push_back(list<STEntry*>());
        //    return true;
        //}

    //bool addInputArgument(STEntry* argument, int position){

    //    list<list<STEntry*>>::iterator it = inputArguments.begin();

    //    for(int i = 0 ; i < position ; i++){
    //        if (it == inputArguments.end()) return false;
    //        it++;
    //    }

    //    (*it).push_back(argument);
    //    return true;
    //}

    //list<STEntry*> getInputArguments(int position){

    //    list<list<STEntry*>>::iterator it = inputArguments.begin();

    //    for(int i = 0 ; i < position ; i++){
    //        if (it == inputArguments.end()) return list<STEntry*>();
    //        it++;
    //    }

    //    if(it != inputArguments.end())
    //        return *it;
    //    else
    //        return list<STEntry*>();
    //}

    //void resetInputType(){
    //    inputType.clear();
    //    inputArguments.clear();
    //}

    //int  calculateSize(SymbolsTable *st){
    //    unordered_set<int> usedLocations = unordered_set<int>();

    //    symbolsSize = 0;	

    //    for_each(symbols.begin(), symbols.end(), [this, &st, &usedLocations](pair<string, STEntry*> const x){
    //        int symSize = st->getTypeSize(x.second->getType());
    //        int address = x.second->getAddress();

    //        if (address < 0)
    //            return;

    //        /* Add 1 byte to 1 byte symbols */
    //        if (!usedLocations.count(address)){
    //            usedLocations.insert(address);
    //            symbolsSize++;
    //        }

    //        /* Add more bytes to biger symbols */
    //        for(int i = 1 ; i < symSize ; i++){	
    //            if (address < 0x0100){
    //                address += 0x0001;
    //            }else{
    //                address += 0x0100;
    //            }

    //            if (!usedLocations.count(address)){
    //                usedLocations.insert(address);
    //                symbolsSize++;
    //            }
    //        }
    //    });

    //    return symbolsSize;
    //}
	 
    //bool addDependency(STFramework* newDependency){
    //    bool exists = false;
    //    list<STFramework*>::iterator it = dependencies.begin();
	 
    //    while(it != dependencies.end() && !exists)
    //        if ((*it++)->getName() == newDependency->getName()) exists = true;
	 
    //    if (exists) return false;
	 
    //    dependencies.push_back(newDependency);
	 
    //    return true;
    //}
	 
    //bool addSymbol(string lex){
    //    if (symbols.count(lex) > 0) return false;
	 
    //    STEntry* newSymbol = new STEntry(lex, name);
    //    pair <string, STEntry*> newEntry = make_pair (lex, newSymbol);
	 
    //    symbols.insert(newEntry);
	 
    //    return true;
    //}
	 
    //bool addSymbol(const string &lex, const QualifiedType &inType){
    //    if (symbols.count(lex) > 0) return false;
	 
    //    STEntry* newSymbol = new STEntry(lex, name, inType);
    //    pair <string, STEntry*> newEntry = make_pair (lex, newSymbol);
	 
    //    symbols.insert(newEntry);
	 
    //    return true;
    //}
	 
    //STEntry getSymbol(const string &lex){
    //    unordered_map<string, STEntry*>::const_iterator entry;
    //    entry = symbols.find(lex);
	 
    //    if (entry != symbols.end()) return entry->second;
	 
    //    return NULL;
    //}
	 
    //List <STEntry> getSymbols(){
    //    list <STEntry> output;

    //    auto f_addToList = [&output](pair<string, STEntry*> const x){
    //        output.push_back(x.second);
    //    };

    //    for_each(symbols.begin(), symbols.end(), f_addToList);

    //    return output;
    //}

    //bool delSymbol(string lex){
    //unordered_map<string, STEntry>::const_iterator entry;
    //    entry = symbols.find(lex);
	 
    //    if (entry == symbols.end()) return false;

    //    symbols.erase(entry);

    //    return true;
    //}

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
	 
    //bool STFramework::setPointerDepth(const string &lex, const int depth){
    //    unordered_map<string, STEntry*>::const_iterator entry;
    //    entry = symbols.find(lex);
    //    if (entry == symbols.end()) return false;
	 
    //    entry->second->setPointerDepth(depth);
    //    return true;
    //}
	 
    //int STFramework::getPointerDepth(const string &lex){
    //    unordered_map<string, STEntry*>::const_iterator entry;
    //    entry = symbols.find(lex);
	 
    //    if (entry != symbols.end()) return entry->second->getPointerDepth();
	 
    //    return -1;
    //}
	 
	 
	 

    //bool STFramework::isVolatile(const string &lex){
    //    unordered_map<string, STEntry*>::const_iterator entry;
    //    entry = symbols.find(lex);
	 
    //    if (entry != symbols.end()) return entry->second->isVolatile();
	 
    //    return false;
    //}
	 
	 
	 

    //bool STFramework::setVolatile(const string &lex){
    //    unordered_map<string, STEntry*>::const_iterator entry;
    //    entry = symbols.find(lex);
	 
    //    if (entry != symbols.end()) {
    //        entry->second->setVolatile();
    //        return true;
    //    }
	 
    //    return false;
    //}
	 
	 
	 

    //bool STFramework::isConst(const string &lex){
    //    unordered_map<string, STEntry*>::const_iterator entry;
    //    entry = symbols.find(lex);
	 
    //    if (entry != symbols.end()) return entry->second->isConst();
	 
    //    return false;
    //}
	 
	 
	 

    //bool STFramework::setConst(const string &lex){
    //    unordered_map<string, STEntry*>::const_iterator entry;
    //    entry = symbols.find(lex);
	 
    //    if (entry != symbols.end()) {
    //        entry->second->setConst();
    //        return true;
    //    }
	 
    //    return false;
    //}
	 
	 
	 

    //bool STFramework::isLiteral(const string &lex){
    //    unordered_map<string, STEntry*>::const_iterator entry;
    //    entry = symbols.find(lex);
	 
    //    if (entry != symbols.end()) return entry->second->isLiteral();
	 
    //    return false;
    //}
	 
	 
	 

    //bool STFramework::setLiteral(const string &lex){
    //    unordered_map<string, STEntry*>::const_iterator entry;
    //    entry = symbols.find(lex);
	 
    //    if (entry != symbols.end()) {
    //        entry->second->setLiteral();
    //        return true;
    //    }
	 
    //    return false;
    //}
	 
	 
	 

    //bool STFramework::isUnsigned(const string &lex){
    //    unordered_map<string, STEntry*>::const_iterator entry;
    //    entry = symbols.find(lex);
	 
    //    if (entry != symbols.end()) return entry->second->isUnsigned();
	 
    //    return false;
    //}
	 
	 
	 

    //bool STFramework::setUnsigned(const string &lex){
    //    unordered_map<string, STEntry*>::const_iterator entry;
    //    entry = symbols.find(lex);
	 
    //    if (entry != symbols.end()) {
    //        entry->second->setUnsigned();
    //        return true;
    //    }
	 
    //    return false;
    //}
	 
	 
	 

    //int STFramework::getNUses(const string &lex){
    //    unordered_map<string, STEntry*>::const_iterator entry;
    //    entry = symbols.find(lex);
	 
    //    if (entry != symbols.end())
    //        return entry->second->getNUses();
	 
    //    return -1;
    //}
	 
	 
	 

    //bool STFramework::setNUses(const string &lex, int inNUses){
    //    unordered_map<string, STEntry*>::const_iterator entry;
    //    entry = symbols.find(lex);
	 
    //    if (entry != symbols.end()){
    //        entry->second->setNUses(inNUses);
    //        return true;
    //    }
	 
    //    return false;
    //}
	 
	 
	 

    //int STFramework::getAddress(const string &lex){
    //    unordered_map<string, STEntry*>::const_iterator entry;
    //    entry = symbols.find(lex);
	 
    //    if (entry != symbols.end()) return entry->second->getAddress();
	 
    //    return -1;
    //}
	 
	 
	 

    //bool STFramework::setAddress(const string &lex, int address){
    //    unordered_map<string, STEntry*>::const_iterator entry;
    //    entry = symbols.find(lex);
	 
    //    if (entry != symbols.end()) {
    //        entry->second->setAddress(address);
    //        return true;
    //    }
	 
    //    return false;
    //}
	 
	 
	 

    //bool STFramework::addLabel(const string &newLabel){
    //    if (existsLabel(newLabel)) return false;
    //    labels.insert(newLabel);
    //    return true;
    //}
	 
	 
	 

    //bool STFramework::existsLabel(const string &label){
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
        string type;
        int size;

        public Type(string inType, int inSize)
        {
            type = inType;
            size = inSize;
        }
    }

    class SymbolsTable
    {
	    Dictionary <string, STFramework> frameworks;
        Dictionary<string, string>       constants;
	    List <Type>                      types;
	    List <string>                    tempIdentifiers;
	    List <string>                    tempTypes;
	    List <string>                    tempLiterals;
	    int                              idCounter;
	    int                              memSize;

        //SymbolsTable::SymbolsTable() {
        //    frameworks      = unordered_map <string, STFramework*>();
        //    constants       = unordered_map <string, string>();
        //    types           = list <Type>();
        //    tempIdentifiers = list <string>();
        //    tempTypes       = list <string>();
        //    tempLiterals    = list <string>();
        //    idCounter       = 0;
        //}
 
 
 

        //SymbolsTable::~SymbolsTable() {
        //    auto f_delSymbolPointers = [](pair<string, STFramework*> const &x){ delete x.second; };
        //    for_each(frameworks.begin(), frameworks.end(), f_delSymbolPointers);
        //    frameworks.clear();
        //    constants.clear();
        //    types.clear();
        //    tempIdentifiers.clear();
        //    tempTypes.clear();
        //    tempLiterals.clear();
        //}
 
 
 

        //bool SymbolsTable::addFramework(const string &framework) {
        //    if (frameworks.count(framework) > 0) return false;
 
        //    STFramework* newSymbol = new STFramework(framework, idCounter++);
        //    pair <string, STFramework*> newEntry = make_pair (framework, newSymbol);
 
        //    frameworks.insert(newEntry);
 
        //    return true;
        //}
 
 
 

        //bool SymbolsTable::existsFramework(const string& framework) {
        //    return frameworks.count(framework) > 0;
        //}
 
 
 

        //STFramework* SymbolsTable::getFramework(const string &framework) {
        //    unordered_map<string, STFramework*>::const_iterator entry;
        //    entry = frameworks.find(framework);
 
        //    return entry->second;
        //}
 
 
 

        //list <STFramework*> SymbolsTable::getFrameworks(){
        //    list <STFramework*> output;

        //    auto f_addToList = [&output](pair<string, STFramework*> const x){
        //        output.push_back(x.second);
        //    };

        //    for_each(frameworks.begin(), frameworks.end(), f_addToList);
 
        //    return output;
        //}




        //bool SymbolsTable::addConstant(const string &name, const string &constant){
        //    if (constants.count(name) > 0) return false;
 
        //    pair <string, string> newEntry = make_pair (name, constant);
 
        //    constants.insert(newEntry);
 
        //    return true;
        //}
 
 
 

        //bool SymbolsTable::existsConstant(const string &name){
        //    return constants.count(name) > 0;
        //}
 
 
 

        //string SymbolsTable::getConstant(const string &name){
        //    unordered_map<string, string>::const_iterator entry;
        //    entry = constants.find(name);
 
        //    return entry->second;
        //}
 
 
 

        //bool SymbolsTable::addType(const string &name, int size){
        //    types.push_back(Type(name, size));
        //    return true;
        //}
 
 
 

        //bool SymbolsTable::existsType(const string &name){
        //    bool exists = false;
        //    list<Type>::iterator it = types.begin();
 
        //    while(it != types.end() && !exists)
        //        if ((*it++).type == name) exists = true;
 
        //    return exists;
        //}
 
 
 

        //int SymbolsTable::getTypeSize(const string &name){
        //    int output = -255;
        //    list<Type>::iterator it = types.begin();
 
        //    while(it != types.end() && output < -254)
        //        if ((*it++).type == name) output = (*--it).size;
 
        //    return output;
        //}
 
 
 

        //int SymbolsTable::getTypeSize(const QualifiedType &inType){
        //    int output = -255;
        //    list<Type>::iterator it = types.begin();
 
        //    if (inType.pointerDepth == 0){
        //        while(it != types.end() && output <= -255)
        //            if ((*it++).type == inType.type) output = (*--it).size;
        //    }else{
        //        /* The size of a pointer */
        //        output = 2;
        //    }
 
        //    output *= inType.arrSize;
 
        //    return output;
        //}
 
 
 

        //void SymbolsTable::pushTempIdentifier(const string &name){
        //    tempIdentifiers.push_back(name);
        //}
 
 
 

        //string SymbolsTable::popTempIdentifier(){
        //    string output = tempIdentifiers.back();
        //    tempIdentifiers.pop_back();
        //    return output;
        //}
 
 
 

        //void SymbolsTable::pushTempLiteral(const string &literalValue){
        //    tempLiterals.push_back(literalValue);
        //}
 
 
 

        //string SymbolsTable::popTempLiteral(){
        //    string output = tempLiterals.back();
        //    tempLiterals.pop_back();
        //    return output;
        //}
 
 
 

        //void SymbolsTable::pushTempType(const string &name){
        //    tempIdentifiers.push_back(name);
        //}
 
 
 

        //string SymbolsTable::popTempType(){
        //    string output = tempTypes.back();
        //    tempTypes.pop_back();
        //    return output;
        //}
 
 
 

        //void SymbolsTable::resetNUses(){

        //    auto f_resetNUses = [](pair<string, STFramework*> x){
        //        list <STEntry*> symbolsList = x.second->getSymbols();
        //        for_each(symbolsList.begin(), symbolsList.end(),
        //            [](STEntry* x){ x->setNUses(0); }
        //        );
        //    };


        //    for_each(frameworks.begin(), frameworks.end(), f_resetNUses);
        //}




        //int SymbolsTable::getId(const string &framework){
        //    if (!existsFramework(framework)) return -1;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getId();
        //}




        //QualifiedType SymbolsTable::getOutputType(const string &framework){
        //    if (!existsFramework(framework)) return QualifiedType("");
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getOutputType();
        //}
 
 
 

        //bool SymbolsTable::setOutputType(const string &framework, const QualifiedType &type){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setOutputType(type);
        //}
 
 
 

        //list<QualifiedType> SymbolsTable::getInputType(const string &framework){
        //    if (!existsFramework(framework)) return list<QualifiedType>();
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getInputType();
        //}
 
 
 

        //list<string> SymbolsTable::getInputSymbols(const string &framework){
        //    if (!existsFramework(framework)) return list<string>();
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getInputSymbols();
        //}




        //bool SymbolsTable::addInputType(const string &framework, const QualifiedType &inType){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->addInputType(inType);
        //}
 
 
 

        //bool SymbolsTable::addInputType(const string &framework, const QualifiedType &inType, const string &inSymbol){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->addInputType(inType, inSymbol);
        //}




        //bool SymbolsTable::addInputArgument(const string &framework, STEntry* argument, int position){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->addInputArgument(argument, position);
        //}




        //list<STEntry*> SymbolsTable::getInputArguments(const string &framework, int position){
        //    if (!existsFramework(framework)) return list<STEntry*>();
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getInputArguments(position);
        //}




        //void SymbolsTable::resetInputType(const string &framework){
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    framework_ptr->resetInputType();
        //}
 
 
 

        //int SymbolsTable::calculateSize(const string &framework){
        //    if (!existsFramework(framework)) return -1;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->calculateSize(this);
        //}
 
 
 

        //bool SymbolsTable::addDependency(const string &framework, STFramework* dependency){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->addDependency(dependency);
        //}
 
 
 

        //bool SymbolsTable::addDependency(const string &framework, const string &dependency){
        //    if (!existsFramework(framework)) return false;
        //    if (!existsFramework(dependency)) return false;
 
        //    STFramework* framework_ptr = getFramework(framework);
        //    STFramework* dependency_ptr = getFramework(dependency);
 
        //    return framework_ptr->addDependency(dependency_ptr);
        //}
 
 
 

        //list<STFramework*> SymbolsTable::getDependencies(const string &framework){
        //    if (!existsFramework(framework)) return list<STFramework*>();
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getDependencies();
        //}




        //bool SymbolsTable::addLabel(const string &framework, const string &newLabel){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->addLabel(newLabel);
        //}
 
 
 

        //bool SymbolsTable::existsLabel(const string &framework, const string &label){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->existsLabel(label);
        //}
 
 
 

        //void SymbolsTable::setRecursive(const string &framework){
        //    if (!existsFramework(framework)) return;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setRecursive();
        //}




        //bool SymbolsTable::isRecursive(const string &framework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->isRecursive();
        //}




        //void SymbolsTable::setAsDefined(const string &framework){
        //    if (!existsFramework(framework)) return;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setAsDefined();
        //}




        //bool SymbolsTable::isDefined(const string &framework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->isDefined();
        //}




        //bool SymbolsTable::addSymbol(const string &lex, const string &framework) {
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->addSymbol(lex);
        //}
 
 
 

        //bool SymbolsTable::addSymbol(const string &lex, const string &type, const string &framework) {
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    if (!framework_ptr->addSymbol(lex)) return false;
 
        //    return framework_ptr->setType(lex, type);
        //}
 
 
 

        //bool SymbolsTable::addSymbol(const string &lex, const QualifiedType &inType, const string &framework) {
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->addSymbol(lex, inType);
        //}
 
 
 

        //STEntry* SymbolsTable::getSymbol(const string &lex, const string &framework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getSymbol(lex);
        //}




        //list <STEntry*> SymbolsTable::getSymbols(const string &framework){
        //    if (!existsFramework(framework)) return list<STEntry*>();
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getSymbols();
        //}




        //bool SymbolsTable::delSymbol(const string &lex, const string &framework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->delSymbol(lex);
        //}




        //bool SymbolsTable::existsSymbol(const string &lex, const string &framework) {
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->existsSymbol(lex);
        //}
 
 
 

        //bool SymbolsTable::setType(const string &lex, const string &type, const string &framework) {
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setType(lex, type);
        //}
 
 
 

        //QualifiedType SymbolsTable::getType(const string &lex, const string &framework){
        //    if (!existsFramework(framework)) return QualifiedType("");
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getType(lex);
        //}
 
 
 

        //bool SymbolsTable::setPointerDepth(const string &lex, const string &framework, const int depth) {
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setPointerDepth(lex, depth);
        //}
 
 
 

        //int SymbolsTable::getPointerDepth(const string &lex, const string &framework){
        //    if (!existsFramework(framework)) return -2;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getPointerDepth(lex);
        //}
 
 
 

        //bool SymbolsTable::isVolatile(const string &lex, const string &framework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->isVolatile(lex);
        //}
 
 
 

        //bool SymbolsTable::setVolatile(const string &lex, const string &framework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setVolatile(lex);
        //}
 
 
 

        //bool SymbolsTable::isConst(const string &lex, const string &framework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->isConst(lex);
        //}
 
 
 

        //bool SymbolsTable::setConst(const string &lex, const string &framework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setConst(lex);
        //}
 
 
 

        //bool SymbolsTable::isLiteral(const string &lex, const string &framework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->isLiteral(lex);
        //}
 
 
 

        //bool SymbolsTable::setLiteral(const string &lex, const string &framework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setLiteral(lex);
        //}
 
 
 

        //bool SymbolsTable::isUnsigned(const string &lex, const string &framework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->isUnsigned(lex);
        //}
 
 
 

        //bool SymbolsTable::setUnsigned(const string &lex, const string &framework){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setUnsigned(lex);
        //}
 
 
 

        //int SymbolsTable::getNUses(const string &lex, const string &framework){
        //    if (!existsFramework(framework)) return -1;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getNUses(lex);
        //}
 
 
 

        //bool SymbolsTable::setNUses(const string &lex, const string &framework, int inNUses){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setNUses(lex, inNUses);
        //}
 
 
 

        //bool SymbolsTable::incNUses(const string &lex, const string &framework, int increment){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    int oldValue = framework_ptr->getNUses(lex);
        //    if (oldValue < 0) return false;
 
        //    return framework_ptr->setNUses(lex, oldValue + increment);
        //}
 
 
 

        //int SymbolsTable::getAddress(const string &lex, const string &framework){
        //    if (!existsFramework(framework)) return -1;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->getAddress(lex);
        //}
 
 
 

        //bool SymbolsTable::setAddress(const string &lex, const string &framework, int address){
        //    if (!existsFramework(framework)) return false;
        //    STFramework* framework_ptr = getFramework(framework);
 
        //    return framework_ptr->setAddress(lex, address);
        //}




        //void SymbolsTable::setMemSize(int inSize){ memSize = inSize; }




        //string SymbolsTable::toString() {
 
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
