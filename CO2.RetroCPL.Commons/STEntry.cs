/**
* |------------------------------------------|
* | CO2.RetroCPL COMPILER OPTIMIZER 2 RETROC |
* | File: STEntry.cs                         |
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
    public class STEntry
    {
        public string        lex;
        public QualifiedType qType;
        public string        framework;
        public int           n_uses;
        public bool          isLiteral;
        public bool          isOverlappable;
        public int           address;

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="inLex">New symbol lexeme.</param>
        /// <param name="inFramework">Framework where this symbol is operational.</param>
        public STEntry(string inLex, string inFramework)
        {
            lex = inLex;
            qType = new QualifiedType("void");
            framework = inFramework;
            n_uses = 0;
            isLiteral = false;
            address = -1;
        }

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="inLex">New symbol lexeme.</param>
        /// <param name="inFramework">Framework where this symbol is operational.</param>
        /// <param name="type">Type of the symbol.</param>
        public STEntry(string inLex, string inFramework, QualifiedType type)
        {
            lex = inLex;
            qType = new QualifiedType(type);
            framework = inFramework;
            n_uses = 0;
            isLiteral = false;
            isOverlappable = false;
            address = -1;
        }

        /// <summary>
        /// Returns a string containing the text formated symbol.
        /// </summary>
        /// <returns>String containing the text formated symbol.</returns>
        public string toString()
        {
            string output = string.Empty;
            char c_v;
            string outputType;
            string framework_b;

            c_v = qType.b_volatile ? 'Y' : 'N';
            framework_b = framework == "" ? "GLOBAL VAR" : framework;
            outputType = "";

            if (qType.b_unsigned)
                outputType += "u ";
            outputType += qType.type;
            if (qType.pointerDepth > 0)
                for (int i = 0; i < qType.pointerDepth; i++) outputType += "*";
            
            output += string.Format("| {0,-16} | {1,-12} | {2,3} | {3,-16} | {4} | {5,3} | {6,-6} |{7}",
                substr(lex, 16), substr(outputType, 12), qType.arrSize.ToString("x3"), substr(framework_b, 16), c_v, n_uses.ToString("d3"),
                /*(isLiteral) ? "Const" : "Undef",*/ (address) < 0 ? "  --" : "0x" + address.ToString("x4"), Environment.NewLine);

            return output;
        }

        /// <summary>
        /// Auxiliar method. Truncates a string.
        /// </summary>
        /// <param name="input">The input string to truncate.</param>
        /// <param name="size">The max size of the output.</param>
        /// <returns>The truncated string.</returns>
        private string substr(string input, int size)
        {
            return input.Substring(0, Math.Min(size, input.Length));
        }
    }
}
