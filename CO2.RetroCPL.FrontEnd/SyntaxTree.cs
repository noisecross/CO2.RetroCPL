/**
* |------------------------------------------|
* | CO2.RetroCPL COMPILER OPTIMIZER 2 RETROC |
* | File: SyntaxTree.cs                      |
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
    /// <summary>
    /// Syntax Tree
    /// </summary>
    public class SyntaxTree
    {
		public Dictionary<string, KeyValuePair<int,int>> labelsExpected = new Dictionary<string, KeyValuePair<int, int>>();
		public int continuable = 0;
		public int breakable   = 0;

        private SyntaxTreeNode _root = null;

        private static SyntaxTree _uniqueSyntaxTree = null;
        public  static SyntaxTree Instance
        {
            get
            {
                if (_uniqueSyntaxTree == null)
                    _uniqueSyntaxTree = new SyntaxTree();
                return _uniqueSyntaxTree;
            }
        }

        /// <summary>
        /// Set the root node of the Syntax Tree
        /// </summary>
        /// <param name="root">The node to set as root.</param>
        public void setRoot(SyntaxTreeNode root) { _root = root; }

        /// <summary>
        /// The core of the semantic analysis. Decorate the node and all the children nodes.
        /// Perform the type check and store the labels, symbols and frameworks into the symbols table.
        /// </summary>
        /// <param name="recursionEnabled">Flag which informs if recursion is allowed or not.</param>
        /// <returns>QualifiedType with the type of the node or null if the process failed.</returns>
        public QualifiedType typeCheck(bool recursionEnabled = false)
        {
            if (_root == null) return null;

		    QualifiedType result = _root.typeCheck();
		    Helper.validateSemanticTree(recursionEnabled);
            Helper.killUnusedSymbols();
		    return result;
        }

        /// <summary>
        /// Returns a string containing the text formated SyntaxTree.
        /// </summary>
        /// <returns>String containing the text formated SyntaxTree.</returns>
        public static string toString()
        {
            if (_uniqueSyntaxTree == null || Instance._root == null)
                return string.Empty;

            return Instance._root.toString();
        }
    }

    /// <summary>
    /// Syntax Tree Node
    /// </summary>
    public class SyntaxTreeNode
    {
        public    int                  rule        = -1;
        protected int                  line        = -1;
        protected int                  column      = -1;
        public    int                  value       =  0;
        public    bool                 valueIsUsed = false;
        public    bool                 returnDone  = false;
        public    string               lex         = string.Empty;
        public    List<SyntaxTreeNode> children    = new List<SyntaxTreeNode>();
        public    QualifiedType        type        = new QualifiedType();

        protected static SymbolsTable symbolsTable { get { return SymbolsTable.Instance; } }

        protected static SyntaxTree syntaxTree { get { return SyntaxTree.Instance; } }



        /// <summary>
        /// Class costructor.
        /// </summary>
        /// <param name="rule">The reduction rule to apply (if applicable) to their children.</param>
        /// <param name="line">The input file line where this node was read.</param>
        /// <param name="children">The children nodes, if any.</param>
        public SyntaxTreeNode() { }
        public SyntaxTreeNode(int rule, int line, int column)
        {
            this.rule   = rule;
            this.line   = line;
            this.column = column;
        }
        public SyntaxTreeNode(int rule, int line, int column, SyntaxTreeNode[] children)
        {
            this.rule = rule;
            this.line = line;
            this.column = column;

            if (children.Length > 0)
            {
                this.column = children[0].column;
                foreach (SyntaxTreeNode item in children)
                    this.children.Add(item);
            }
        }

        /// <summary>
        /// Getters
        /// </summary>
        /// <returns></returns>
        public int getLine()
        {
            return line;
        }
        public int getColumn()
        {
            return column;
        }

        /// <summary>
        /// Add new children to the node.
        /// </summary>
        /// <param name="children">The given child to add</param>
        public void addChildren(SyntaxTreeNode[] children)
        {
            foreach (SyntaxTreeNode item in children)
                this.children.Add(item);
        }

        /// <summary>
        /// Return the inner type of this node as a string.
        /// </summary>
        /// <returns>The inner type of this node as a string</returns>
        public string getNodeType() { return this.GetType().Name; }

        /// <summary>
        /// Perform the type check and store the labels, symbols and frameworks into the symbols table.
        /// </summary>
        /// <returns>QualifiedType with the type of the node.</returns>
        public virtual QualifiedType typeCheck() { return new QualifiedType(); }

        /// <summary>
        /// Auxiliar method which takes an error message, a lex token and a line argument and perform a call to
        /// the error manager with that data.
        /// </summary>
        /// <param name="errMessage">The generic error message to this error.</param>
        /// <param name="lex">The token which have produced the error.</param>
        /// <param name="line">Number line where the error has been produced.</param>
        protected static void addError(string errMessage, string lex, int line, int column)
        {
            string errOutput = string.Format("\t{0} ({1}:{2}){3}\t^{4}", lex, line, column, Environment.NewLine, errMessage);
            ErrManager.Instance.addError(errOutput);
        }
        protected static void addError(string errMessage, int line, int column)
        {
            string errOutput = string.Format("\t({0}:{1}){2}\t^{3}", line, column, Environment.NewLine, errMessage);
            ErrManager.Instance.addError(errOutput);
        }
        protected static void addWarning(string warMessage, string lex, int line, int column)
        {
            string warOutput;
            if (!string.IsNullOrEmpty(lex))
                warOutput = string.Format("\t{0} ({1}:{2}){3}\t^{4}", lex, line, column, Environment.NewLine, warMessage);
            else
                warOutput = string.Format("\t({0}:{1}) {2}", line, column, warMessage);
            ErrManager.Instance.addWarning(warOutput);
        }

        /// <summary>
        /// Read the type of every children and set the type of the node to TYPE_ERR if one of them have a TYPE_ERR type.
        /// </summary>
        protected void inheritTypeFromChildren()
        {
            foreach (SyntaxTreeNode child in children)
            {
                child.typeCheck();
                if (child.type.type == Constants.TYPE_ERR)
                    type.type = Constants.TYPE_ERR;
            }
        }

        /// <summary>
        /// Inherit the main properties from a given node.
        /// </summary>
        protected void inheritPropertiesFromNode(SyntaxTreeNode node)
        {
            type = node.type.Clone();

            if (node.valueIsUsed)
            {
                valueIsUsed = true;
                value = node.value;
            }

            if (!string.IsNullOrEmpty(node.lex))
                lex = node.lex;
        }


        /// <summary>
        /// Returns a string with a tree text formatted vision of the node and all its children.
        /// </summary>
        /// <returns>String with a tree text formatted vision of the node and all its children.</returns>
        public string toString(){ return toString("   "); }
        private string toString(string prefix)
        {
            string output = string.Format("{0}+-{1} : ({2})", Helper.substr(prefix, (prefix.Length - 3)), getNodeType(), type.toString());
            if (lex != "")   output += string.Format(" ({0})", lex);
            if (valueIsUsed) output += string.Format(" ({0})", value);
            output += Environment.NewLine;

            int size = children.Count;
            foreach (var item in children)
            {
                size--;
                string newPrefix = prefix + ((size > 0) ? "|  " : "   ");

                if (item == null)
                    output += string.Format("{0}<ERROR>", newPrefix);
                else
                    output += string.Format("{0}", item.toString(newPrefix));
            }

            return output;
        }

        /// <summary>
        /// Performs the promotion of the type of an expression given its input parameters.
        /// </summary>
        /// <param name="typeL"></param>
        /// <param name="typeR"></param>
        /// <returns>the promoted type</returns>
        protected static QualifiedType promoteType(QualifiedType typeL, QualifiedType typeR, int line, int column, SyntaxTreeNode assignmentNodeR = null, string lex = "")
        {
            QualifiedType output;

            // FUTURE <strings> STRING_LITERAL
            //FIXME cuidar de los arrays
            if (typeL.isLiteral())
                output = typeR.Clone();
            else if (typeR.isLiteral())
            {
                output = typeL.Clone();
            }
            else
            {
                if (typeL.b_unsigned != typeR.b_unsigned)
                    addWarning(ErrorMessages.WAR_SEM_MSG_01, "( " + typeL.toString() + " / " + typeR.toString() + " )", line, column);

                int pointerDepth1 = typeL.pointerDepth;
                int pointerDepth2 = typeR.pointerDepth;

                if (pointerDepth1 < 0) pointerDepth1 = 0;
                if (pointerDepth2 < 0) pointerDepth2 = 0;

                if ((pointerDepth1 | pointerDepth2) == 0)
                {
                    // Both types are not pointers return bigger type
                    // If both types are equally sized unsigned is preferred
                    if (SymbolsTable.Instance.getTypeSize(typeL.type) > SymbolsTable.Instance.getTypeSize(typeR.type))
                        output = typeL.Clone();
                    else if (SymbolsTable.Instance.getTypeSize(typeL.type) < SymbolsTable.Instance.getTypeSize(typeR.type))
                        output = typeR.Clone();
                    else
                    {
                        output = typeL.Clone();
                        output.b_unsigned = typeL.b_unsigned || typeR.b_unsigned;
                    }
                }
                else
                    output = promoteExpressionPointerType(typeL, typeR, line, column, pointerDepth1, pointerDepth2);
            }


            // Check sizes only for assignment promotions
            if (assignmentNodeR != null)
            {
                if (typeR.isLiteral())
                {
                    if (assignmentNodeR.valueIsUsed)
                    {
                        if (symbolsTable.getSizeToFitValue(assignmentNodeR.value, !typeL.b_unsigned) > symbolsTable.getTypeSize(typeL))
                            addWarning(ErrorMessages.WAR_SEM_MSG_06, "( " + typeL.toString() + " ) " + lex, line, column);

                        if (typeL.b_unsigned && assignmentNodeR.value < 0)
                            addWarning(ErrorMessages.WAR_SEM_MSG_01, "( " + typeL.toString() + " / " + symbolsTable.getTypeToFitValue(assignmentNodeR.value, !typeL.b_unsigned) + " )", line, column);
                    }
                }
                else
                {
                    if (symbolsTable.getTypeSize(output) > symbolsTable.getTypeSize(typeL))
                        addWarning(ErrorMessages.WAR_SEM_MSG_06, lex, line, column);

                    if(typeL.pointerDepth > 0 && typeR.pointerDepth <= 0)
                        addError(ErrorMessages.ERR_SEM_MSG_04, "( " + typeL.toString() + " / " + typeR.toString() + " ) " + lex, line, column);
                }

                // In an assignment, typeL is always promoted
                if (typeL.type != Constants.TYPE_LITERAL)
                    output = typeL.Clone();
            }

            return output;
        }

        /// <summary>
        /// Performs a forced promotion in the type of one given node.
        /// </summary>
        /// <param name="node"></param>
        public void tryPromoteLiteral()
        {
            if (type.type != Constants.TYPE_LITERAL || !valueIsUsed)
                return;

            type.type = Constants.TYPE_BYTE;

            // Choose 'signed' property
            if (value < 0)
                type.b_unsigned = false;
            else
                type.b_unsigned = true;

            if (value < -128)
                type.type = "word";

            if (value > 0xFF)
                type.type = "word";

            if (value > 0x7F && !(type.b_unsigned))
                type.type = "word";

            // Define boundaries
            string auxMsg = "{ " + type.toString() + " (" + value + ") }";
            if (value > 0xFFFF || value < -0x7FFF)
                addError(ErrorMessages.ERR_SEM_MSG_14, auxMsg, line, column);
            if (value > 0x7FFF && !(type.b_unsigned))
                addError(ErrorMessages.ERR_SEM_MSG_14, auxMsg, line, column);
        }

        /// <summary>
        /// Performs a forced promotion in the type of one given node.
        /// </summary>
        /// <param name="node"></param>
        public void tryPromoteLiteral(QualifiedType typeToPromote)
        {
            if (type.type != Constants.TYPE_LITERAL || !valueIsUsed)
                return;

            if((typeToPromote.b_unsigned || typeToPromote.pointerDepth > 0) && value < 0)
                addWarning(ErrorMessages.WAR_SEM_MSG_01, "( " + typeToPromote.toString() + " / " + symbolsTable.getTypeToFitValue(value, false) + " )", line, column);

            if (symbolsTable.getSizeToFitValue(value, typeToPromote.b_unsigned) > symbolsTable.getTypeSize(typeToPromote))
                addWarning(ErrorMessages.WAR_SEM_MSG_06, "( " + typeToPromote.toString() + " ) " + value, line, column);

            type = typeToPromote.Clone();

            // Define boundaries
            string auxMsg = "{ " + type.toString() + " (" + value + ") }";
            if (value > 0xFFFF || value < -0x7FFF)
                addError(ErrorMessages.ERR_SEM_MSG_14, auxMsg, line, column);
            if (value > 0x7FFF && !(type.b_unsigned))
                addError(ErrorMessages.ERR_SEM_MSG_14, auxMsg, line, column);
        }

        /// <summary>
        /// Performs the promotion of the type of an expression using pointers given its input parameters.
        /// </summary>
        /// <param name="typeL"></param>
        /// <param name="typeR"></param>
        /// <param name="line"></param>
        /// <param name="pointerDepth1"></param>
        /// <param name="pointerDepth2"></param>
        /// <returns></returns>
        private static QualifiedType promoteExpressionPointerType(QualifiedType typeL, QualifiedType typeR, int line, int column, int pointerDepth1, int pointerDepth2)
        {
            QualifiedType output = new QualifiedType(Constants.TYPE_ERR);

            if (pointerDepth1 == 0)
            {
                // First operand type is not a pointer
                output = typeR.Clone();
            }
            else if (pointerDepth2 == 0)
            {
                // Second operand type is not a pointer
                output = typeL.Clone();
            }
            else
            {
                // Both types are pointers
                if (typeL.type == "void")
                    // If one of them is void* is ok 
                    output = typeR.Clone();
                else if (typeR.type == "void")
                    // If one of them is void* is ok 
                    output = typeL.Clone();
                else if (pointerDepth1 == pointerDepth2 && typeL.type == typeR.type)
                    // If both are the same type and depth is ok
                    output = typeL.Clone();
                else
                    // Else error
                    addError(ErrorMessages.ERR_SEM_MSG_06, string.Format("{0} <- {1}", typeL.toString(), typeR.toString()), line, column);
            }

            return output;
        }

        /// <summary>
        /// Takes an argument list and a framework name and check if the types in the list are the expected by a
        /// call to the function represents that framework.
        /// </summary>
        /// <param name="name">Function which operates over the children nodes values.</param>
        /// <param name="nodeArgumentList">The argument list.</param>
        /// <returns>True if the call is properly performed.</returns>
        protected bool checkInputTypeCorrectness(string name, SyntaxTreeNode nodeArgumentList)
        {
            List<SyntaxTreeNode> children = nodeArgumentList.children;
            List<QualifiedType> typesExpected = symbolsTable.getInputType(name);
            int nArguments = children.Count;

            if (typesExpected.Count != nArguments) return false;

            for (int i = 0; i < nArguments; i++)
            {
                if (children[i].type.isLiteral())
                    children[i].tryPromoteLiteral(typesExpected[i]);
                else if (!typesExpected[i].sameInput(children[i].type))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Auxiliar method. Before an assignment operation check if is been doing over a const variable
        /// defined. Notify the error manager every undefined label.
        /// Also performs a check to ensure that an argument is not beign accessed in a way that would spoil the overlap
        /// That means, only can be l-value if the operation is MVA or STP
        /// </summary>
        /// <param name="l_value">The memory place where the assignation will go.</param>
        /// <param name="line">The line where the operation is taken place.</param>
        /// <param name="bypass">The overlappable value to store in a modified argument.</param>
        protected static void checkConstantProperty(string l_value, int line, int column, bool bypass)
        {
	        if (string.IsNullOrEmpty(l_value)) return;

            if (SymbolsTable.Instance.existsSymbol(l_value, SymbolsTable.Instance.currentFramework))
            {
                STEntry l_Symbol = SymbolsTable.Instance.getSymbol(l_value);

			    if (l_Symbol.qType.b_const)
				    addError(ErrorMessages.ERR_SEM_MSG_13, l_value, line, column);
                else if(l_Symbol.qType.pointerDepth > 0 && l_Symbol.isOverlappable)
                    l_Symbol.isOverlappable = bypass;
		    }
            else if (SymbolsTable.Instance.existsSymbol(l_value, Constants.GLOBAL_FRAMEWORK))
                if (SymbolsTable.Instance.isConst(l_value, Constants.GLOBAL_FRAMEWORK))
                    addError(ErrorMessages.ERR_SEM_MSG_13, l_value, line, column);
        }

        /// <summary>
        /// Auxiliar method. Performs a check which try to calculate an expression in compilation time.
        /// Only uses the children nodes 0 and 1 to perform the said calculus.
        /// </summary>
        /// <returns></returns>
        protected QualifiedType expressionCheck(Action calculateExpression, bool promoteResult = true)
        {
            children[0].typeCheck();
            if (rule == 1)
            {
                type = children[0].type.Clone();
                valueIsUsed = children[0].valueIsUsed;
                value = children[0].value;
                lex = children[0].lex;
            }
            else
            {
                //Apply Expression
                children[1].typeCheck();
                if(promoteResult)
                    type = promoteType(children[0].type, children[1].type, line, column, null, children[0].lex);
                if (children[0].valueIsUsed && children[1].valueIsUsed)
                    calculateExpression();
            }

            return type;
        }

        /// <summary>
        /// Performs the typeCheck of a pointer node and combines the result with a given qualified type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pointerNode"></param>
        /// <returns></returns>
        protected QualifiedType CheckPointerAndCombineWithQualifiedType(QualifiedType type, SyntaxTreeNode pointerNode)
        {
            pointerNode.typeCheck();

            QualifiedType output = type.Clone();
            output.pointerDepth = pointerNode.value;

            return output;
        }
    }
}
