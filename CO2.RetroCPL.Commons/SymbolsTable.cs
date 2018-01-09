/**
* |------------------------------------------|
* | CO2.RetroCPL COMPILER OPTIMIZER 2 RETROC |
* | File: SymbolsTable.cs                    |
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
    public class SymbolsTable
    {
        Dictionary<string, STFramework>  frameworks      = new Dictionary<string, STFramework>();
        Dictionary<string, string>       constants       = new Dictionary<string, string>();
        List<Type>                       types           = new List<Commons.Type>();
        List<string>                     tempIdentifiers = new List<string>();
        List<QualifiedType>              tempSpecifiers  = new List<QualifiedType>();
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

        /// <summary>
        /// Class constructor.
        /// </summary>
        public SymbolsTable() {
            _uniqueSymbolsTable = this;
        }

        #region SymbolsTable methods

        /// <summary>
        /// Add a new framework to the symbols table.
        /// </summary>
        /// <param name="framework">The name of the framework to add.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool addFramework(string framework)
        {
            if (frameworks.ContainsKey(framework)) return false;
 
            STFramework newSymbol = new STFramework(framework, idCounter++);
            frameworks.Add(framework, newSymbol);

            return true;
        }
 
        /// <summary>
        /// Determines if exists a framework with a given name.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>True if the framework exists.</returns>
        public bool existsFramework(string framework)
        {
            return frameworks.ContainsKey(framework);
        }
 
        /// <summary>
        /// Return a STFramework object with a given name.
        /// </summary>
        /// <param name="framework">The name of the given framework.</param>
        /// <returns>The pointer to the STFramework object.</returns>
        public STFramework getFramework(string framework)
        {
            return frameworks[framework];
        }
 
        /// <summary>
        /// Returns the list of all STFrameworks.
        /// </summary>
        /// <returns>The list of all STFrameworks.</returns>
        public List<STFramework> getFrameworks()
        {
            return frameworks.Values.ToList();
        }

        /// <summary>
        /// Adds a constant to the symbols table.
        /// </summary>
        /// <param name="name">The name of the new constant.</param>
        /// <param name="constant">The literal string related to the constant.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool addConstant(string name, string constant)
        {
            if (constants.ContainsKey(name)) return false;
            constants.Add(name, constant);

            return true;
        }
        
        /// <summary>
        /// Check the existence of a certain constant.
        /// </summary>
        /// <param name="name">The name of a constant.</param>
        /// <returns>True if the constant does exist.</returns>
        public bool existsConstant(string name)
        {
            return constants.ContainsKey(name);
        }
 
        /// <summary>
        /// Return the literal string related to a given constant.
        /// </summary>
        /// <param name="name">The name of a constant.</param>
        /// <returns>The literal string related to a given constant.</returns>
        public string getConstant(string name)
        {
            return constants[name];
        }
 
        /// <summary>
        /// Adds a new type to the symbols table.
        /// </summary>
        /// <param name="name">The name of a type.</param>
        /// <param name="size">The size of the type.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool addType(string name, int size)
        {
            types.Add(new Type(name, size));
            return true;
        }

        /// <summary>
        /// Check if a given type exists.
        /// </summary>
        /// <param name="name">The name of a type.</param>
        /// <returns>True if the type exists.</returns>
        public bool existsType(string name)
        {
            return types.Where(_ => _.type == name).Count() > 0;
        }

        /// <summary>
        /// Return the size value in bytes of a certain type or -1 if the type doesn't exist.
        /// </summary>
        /// <param name="inType">The name type.</param>
        /// <returns>The size value in bytes of a certain type or -1 if the type doesn't exist.</returns>
        public int getTypeSize(string name)
        {
            var type = types.Where(_ => _.type == name);
            if (type == null || type.Count() <= 0) return -255;
            return type.First().size;
        }
 
        /// <summary>
        /// Return the size value in bytes of a certain type or -1 if the type doesn't exist.
        /// </summary>
        /// <param name="inType">The type.</param>
        /// <returns>The size value in bytes of a certain type or -1 if the type doesn't exist.</returns>
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
 
        /// <summary>
        /// Store a identifier name on the stack to be given later to the syntax analyzer.
        /// </summary>
        /// <param name="name">The name of the new identifier.</param>
        public void pushTempIdentifier(string name)
        {
            tempIdentifiers.Add(name);
        }
 
        /// <summary>
        /// Get the last identifier name set on the temporal stack.
        /// </summary>
        /// <returns>The given name.</returns>
        public string popTempIdentifier()
        {
            //string output = tempIdentifiers.back();
            //tempIdentifiers.pop_back();
            string output = tempIdentifiers.Last();
            tempIdentifiers.RemoveAt(tempIdentifiers.Count - 1);
            return output;
        }

        /// <summary>
        /// Store a specifier on the stack to be given later to the semantic analyzer.
        /// </summary>
        /// <param name="name">The name of the new identifier.</param>
        public void pushTempSpecifier(QualifiedType type)
        {
            tempSpecifiers.Add(type.Clone());
        }

        /// <summary>
        /// Get the last specifier object set on the temporal stack.
        /// </summary>
        /// <returns>The given specifier.</returns>
        public QualifiedType popTempSpecifier()
        {
            QualifiedType output = tempSpecifiers.Last().Clone();
            tempSpecifiers.RemoveAt(tempIdentifiers.Count - 1);
            return output;
        }

        /// <summary>
        /// Let view the last specifier object set on the temporal stack.
        /// </summary>
        /// <returns>The given specifier.</returns>
        public QualifiedType peekTempSpecifier()
        {
            QualifiedType output = tempSpecifiers.Last().Clone();
            return output;
        }

        /// <summary>
        /// Store a literal on the stack to be given later to the syntax analyzer
        /// </summary>
        /// <param name="literalValue">The literal.</param>
        public void pushTempLiteral(string literalValue)
        {
            tempLiterals.Add(literalValue);
        }

        /// <summary>
        /// Get the last literal value set on the temporal stack.
        /// </summary>
        /// <returns>The given literal.</returns>
        public string popTempLiteral()
        {
            string output = tempLiterals.Last();
            tempLiterals.RemoveAt(tempLiterals.Count - 1);
            return output;
        }
 
        /// <summary>
        /// Store a type name on the stack to be given later to the syntax analyzer.
        /// </summary>
        /// <param name="name">The name of the new type.</param>
        public void pushTempType(string name)
        {
            tempIdentifiers.Add(name);
        }
 
        /// <summary>
        /// Get the last type name set on the temporal stack.
        /// </summary>
        /// <returns>The given name.</returns>
        public string popTempType(){
            string output = tempTypes.Last();
            tempTypes.RemoveAt(tempTypes.Count - 1);
            return output;
        }
 

        /// <summary>
        /// Set to 0 the nUses of every symbol.
        /// </summary>
        public void resetNUses()
        {
            foreach(var framework in frameworks)
                foreach (var symbol in framework.Value.getSymbols())
                    symbol.n_uses = 0;
        }

        #endregion

        #region SymbolsTable and frameworks methods

        /// <summary>
        /// Get the ID from an existing framework.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>The ID if the operation was performed successfully. Otherwise return -1.</returns>
        public int getId(string framework)
        {
            if (!existsFramework(framework)) return -1;
            return getFramework(framework).id;
        }

        /// <summary>
        /// Get the output type from an existing framework.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>The type if the operation was performed successfully. Otherwise return ""</returns>
        public QualifiedType getOutputType(string framework)
        {
            if (!existsFramework(framework)) return null;
            return getFramework(framework).outputType;
        }
 
        /// <summary>
        /// Set the output type on an existing framework.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        /// <param name="type">The type to set.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool setOutputType(string framework, QualifiedType type)
        {
            if (!existsFramework(framework)) return false;
            getFramework(framework).outputType = type;

            return true;
        }

        /// <summary>
        /// Get the input type from an existing framework.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>The type list if the operation was performed successfully. Otherwise return an empty list.</returns>
        public List<QualifiedType> getInputType(string framework)
        {
            if (!existsFramework(framework)) return new List<QualifiedType>();
            return getFramework(framework).inputType;
        }

        /// <summary>
        /// Get the input symbol from an existing framework.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>The symbol list if the operation was performed successfully. Otherwise return an empty list.</returns>
        public List<string> getInputSymbols(string framework)
        {
            if (!existsFramework(framework)) return new List<string>();
            return getFramework(framework).inputSymbols;
        }

        /// <summary>
        /// Add an input type on an existing framework.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        /// <param name="inType">The type string to set.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool addInputType(string framework, QualifiedType inType)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).addInputType(inType);
        }

        /// <summary>
        /// Add an input type and its symbol lex on an existing framework.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        /// <param name="inType">The type string to set.</param>
        /// <param name="inSymbol">The symbol string to set.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool addInputType(string framework, QualifiedType inType, string inSymbol)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).addInputType(inType, inSymbol);
        }

        /// <summary>
        /// Add an argument in a given position on an existing framework to perform future optimizations.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        /// <param name="argument">The STEntry argument.</param>
        /// <param name="position">The place in the argument list where the symbol is allocated.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool addInputArgument(string framework, STEntry argument, int position)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).addInputArgument(argument, position);
        }

        /// <summary>
        /// Get the symbol list from the argument in a given position on an existing framework to perform future optimizations.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        /// <param name="position">The place in the argument list where the symbol is allocated.</param>
        /// <returns>The STEntry list of Symbols related with that argument.</returns>
        public List<STEntry> getInputArguments(string framework, int position)
        {
            if (!existsFramework(framework)) return new List<STEntry>();
            if (getFramework(framework).inputArguments.Count <= position) return new List<STEntry>();
            return getFramework(framework).inputArguments[position];
        }

        /// <summary>
        /// Reset the input types on an existing framework.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        public void resetInputType(string framework)
        {
            getFramework(framework).resetInputType();
        }

        /// <summary>
        /// Perform an operation to calculate the memory size required to store the variables
        /// of a given framework. Store that value on the framework and returns it.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>The memory size given after the operation.</returns>
        public int calculateSize(string framework)
        {
            if (!existsFramework(framework)) return -1;
            return getFramework(framework).calculateSize(this);
        }

        /// <summary>
        /// Add a new dependency to a given framework.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        /// <param name="dependency">The dependency.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool addDependency(string framework, STFramework dependency)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).addDependency(dependency);
        }

        /// <summary>
        /// Add a new dependency to a given framework.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        /// <param name="dependency">The name of the dependency.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool addDependency(string framework, string dependency)
        {
            if (!existsFramework(framework)) return false;
            if (!existsFramework(dependency)) return false;
            return getFramework(framework).addDependency(getFramework(dependency));
        }

        /// <summary>
        /// Get the dependency list from a given framework.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>The framework dependency list.</returns>
        public List<STFramework> getDependencies(string framework)
        {
            if (!existsFramework(framework)) return new List<STFramework>();
            return getFramework(framework).dependencies;
        }

        /// <summary>
        /// Add a new label to the framework namespace.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        /// <param name="newLabel">The name of the new label.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool addLabel(string framework, string newLabel)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).addLabel(newLabel);
        }
        
        /// <summary>
        /// Check if a label exists in the framework namespace.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        /// <param name="label">The name of the label to check.</param>
        /// <returns>True if the label exists.</returns>
        public bool existsLabel(string framework, string label)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).existsLabel(label);
        }

        /// <summary>
        /// Set a given framework as recursive.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        public void setRecursive(string framework)
        {
            if (!existsFramework(framework)) return;
            getFramework(framework).recursive = true;
        }

        /// <summary>
        /// Check if a given framework is recursive.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>True if the framework is recursive.</returns>
        public bool isRecursive(string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).recursive;
        }

        /// <summary>
        /// Set a given framework as defined.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        public void setAsDefined(string framework)
        {
            if (!existsFramework(framework)) return;
            getFramework(framework).defined = true;
        }

        /// <summary>
        /// Return true if the given framework is defined.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>True if the framework is defined.</returns>
        public bool isDefined(string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).defined;
        }

        #endregion

        #region SymbolsTable, frameworks and symbols methods

        /// <summary>
        /// Adds a new symbol to a framework with given names.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool addSymbol(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).addSymbol(lex);
        }

        /// <summary>
        /// Adds a new symbol with a type to a framework with given names.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="type">The name of the type.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool addSymbol(string lex, string type, string framework)
        {
            if (!existsFramework(framework)) return false;
            if (!getFramework(framework).addSymbol(lex)) return false;
            return getFramework(framework).setType(lex, type);
        }

        /// <summary>
        /// Adds a new symbol with a type to a framework with given names.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="inType">The type of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool addSymbol(string lex, QualifiedType inType, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).addSymbol(lex, inType);
        }

        /// <summary>
        /// Gets the symbol with given framework and name.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>The symbol if the operation was performed successfully.</returns>
        public STEntry getSymbol(string lex, string framework)
        {
            if (!existsFramework(framework)) return null;
            return getFramework(framework).getSymbol(lex);
        }
        
        /// <summary>
        /// Gets a list with the symbols of a given framework.
        /// </summary>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>Gets a list with the symbols of a given framework.</returns>
        public List<STEntry> getSymbols(string framework)
        {
            if (!existsFramework(framework)) return new List<STEntry>();
            return getFramework(framework).getSymbols();
        }

        /// <summary>
        /// Deletes a symbol in a given framework.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool delSymbol(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).delSymbol(lex);
        }

        /// <summary>
        /// Determines if exists a symbol in a framework with given names.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>True if exists a symbol in the framework with given names.</returns>
        public bool existsSymbol(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).existsSymbol(lex);
        }
        
        /// <summary>
        /// Set a type to an existing symbol in a framework with given names.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="type">The name of the type.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool setType(string lex, string type, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).setType(lex, type);
        }

        /// <summary>
        /// Get a type from an existing symbol in a framework with given names.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>The type if the operation was performed successfully. Otherwise return ""</returns>
        public QualifiedType getType(string lex, string framework)
        {
            if (!existsFramework(framework)) return new QualifiedType("");
            return getFramework(framework).getType(lex);
        }

        /// <summary>
        /// Set the pointer depth to an existing symbol in a framework with given names.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <param name="depth">The depth of the pointer.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool setPointerDepth(string lex, string framework, int depth)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).setPointerDepth(lex, depth);
        }

        /// <summary>
        /// Get the pointer depth from an existing symbol in a framework with given names.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>The pointer depth if the operation was performed successfully. Otherwise return -2.</returns>
        public int getPointerDepth(string lex, string framework)
        {
            if (!existsFramework(framework)) return -2;
            return getFramework(framework).getPointerDepth(lex);
        }

        /// <summary>
        /// Check if a certain symbol in a certain framework is volatile.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>True if the symbol in the framework is volatile.</returns>
        public bool isVolatile(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).isVolatile(lex);
        }

        /// <summary>
        /// Set a certain symbol in a certain framework as volatile.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool setVolatile(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).setVolatile(lex);
        }

        /// <summary>
        /// Check if a certain symbol in a certain framework is const.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>true if the symbol in the framework is const.</returns>
        public bool isConst(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).isConst(lex);
        }

        /// <summary>
        /// Set a certain symbol in a certain framework as const.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool setConst(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).setConst(lex);
        }

        /// <summary>
        /// Check if a certain symbol in a certain framework is literal.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool isLiteral(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).isLiteral(lex);
        }

        /// <summary>
        /// Set a certain symbol in a certain framework as literal.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool setLiteral(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).setLiteral(lex);
        }

        /// <summary>
        /// Check if a certain symbol in a certain framework is unsigned.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>True if the symbol in the framework is unsigned.</returns>
        public bool isUnsigned(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).isUnsigned(lex);
        }

        /// <summary>
        /// Set a certain symbol in a certain framework as unsigned.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool setUnsigned(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).setUnsigned(lex);
        }

        /// <summary>
        /// Set a certain symbol in a certain framework as signed.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool clearUnsigned(string lex, string framework)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).clearUnsigned(lex);
        }

        /// <summary>
        /// Get the number of uses of a certain symbol in a certain framework.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>The current uses if the operation was performed successfully or -1 otherwise.</returns>
        public int getNUses(string lex, string framework)
        {
            if (!existsFramework(framework)) return -1;
            return getFramework(framework).getNUses(lex);
        }

        /// <summary>
        /// Set the number of uses of a certain symbol in a certain framework.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <param name="inNUses">The number of uses to set.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool setNUses(string lex, string framework, int inNUses)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).setNUses(lex, inNUses);
        }

        /// <summary>
        /// Increment the number of uses of a certain symbol in a certain framework with a certain value.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <param name="increment">The value of the increment.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool incNUses(string lex, string framework, int increment)
        {
            if (!existsFramework(framework)) return false;
            int oldValue = getFramework(framework).getNUses(lex);
            if (oldValue < 0) return false;
            return getFramework(framework).setNUses(lex, oldValue + increment);
        }

        /// <summary>
        /// Get the current proposed memory address of a certain symbol in a certain
        /// framework.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <returns>The current proposed memory address value if the operation was performed
        /// successfully or -1 otherwise.</returns>
        public int getAddress(string lex, string framework)
        {
            if (!existsFramework(framework)) return -1;
            return getFramework(framework).getAddress(lex);
        }

        /// <summary>
        /// Set the current proposed memory address of a certain symbol in a certain
        /// framework.
        /// </summary>
        /// <param name="lex">The name of the symbol.</param>
        /// <param name="framework">The name of a framework.</param>
        /// <param name="address">The address to set on.</param>
        /// <returns>True if the operation was performed successfully.</returns>
        public bool setAddress(string lex, string framework, int address)
        {
            if (!existsFramework(framework)) return false;
            return getFramework(framework).setAddress(lex, address);
        }
        
        #endregion

        /// <summary>
        /// Pretty printer.
        /// </summary>
        /// <returns>String containing the text formated symbols table.</returns>
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
