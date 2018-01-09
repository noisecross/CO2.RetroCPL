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
        SyntaxTreeNode _root = null;

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
        public QualifiedType typeCheck(bool recursionEnabled)
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
        public    int                  value       =  0;
        public    bool                 valueIsUsed = false;
        protected bool                 returnDone  = false;
        public    string               lex         = string.Empty;
        protected List<SyntaxTreeNode> children    = new List<SyntaxTreeNode>();
        public    QualifiedType        type        = new QualifiedType();

        /// <summary>
        /// Class costructor.
        /// </summary>
        /// <param name="rule">The reduction rule to apply (if applicable) to their children.</param>
        /// <param name="line">The input file line where this node was read.</param>
        /// <param name="children">The children nodes, if any.</param>
        public SyntaxTreeNode() { }
        public SyntaxTreeNode(int rule, int line)
        {
            this.rule = rule;
            this.line = line;
        }
        public SyntaxTreeNode(int rule, int line, SyntaxTreeNode[] children)
        {
            this.rule = rule;
            this.line = line;

            foreach (SyntaxTreeNode item in children)
                this.children.Add(item);
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
        protected static void addError(string errMessage, string lex, int line)
        {
            string errOutput = string.Format("\t{0} ({1}){2}\t^{3}", lex, line, Environment.NewLine, errMessage);
            ErrManager.Instance.addError(errOutput);
        }
        protected static void addError(string errMessage, int line)
        {
            string errOutput = string.Format("\t({0}){1}\t^{2}", line, Environment.NewLine, errMessage);
            ErrManager.Instance.addError(errOutput);
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
    }
}
