/**
* |------------------------------------------|
* | CO2.RetroCPL COMPILER OPTIMIZER 2 RETROC |
* | File: Types.cs                           |
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
    public class QualifiedType
    {
        public string type         = "void";
        public int    arrSize      = 1;
        public int    pointerDepth = 0;
        public bool   b_unsigned   = false;
        public bool   b_const      = false;
        public bool   b_volatile   = false;
        public bool   b_interrupt  = false;

        /// <summary>
        /// Class constructor.
        /// </summary>
        public QualifiedType() { }

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="type">Initializer type.</param>
        public QualifiedType(string type)
        {
            this.type = type;
        }

        /// <summary>
        /// Class constructor. Clone.
        /// </summary>
        /// <param name="qualifiedType">Input type to clone.</param>
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
        public QualifiedType Clone()
        {
            return new QualifiedType(this);
        }

        /// <summary>
        /// Return a true value if the given qualified type is valid as output data.
        /// </summary>
        /// <param name="operand">The given qualified type.</param>
        /// <returns>A true value if the qualified type is valid as output data.</returns>
        public bool sameOutput(QualifiedType operand)
        {
            /* FUTURE <strings> */
            if (!((operand.isLiteral()) && ((type == "word") || (type == "byte"))))
            {
                if (type != operand.type) return false;
                if (b_unsigned != operand.b_unsigned) return false;
            }

            if (pointerDepth != operand.pointerDepth) return false;
            if (b_interrupt != operand.b_interrupt) return false;

            return true;
        }

        /// <summary>
        /// Return a true value if this type is undefined and is refered to a literal.
        /// </summary>
        /// <returns></returns>
        public bool isLiteral()
        {
            return type == Constants.TYPE_LITERAL;
        }

        /// <summary>
        /// Return a true value if the given qualified type is valid as input data.
        /// </summary>
        /// <param name="operand">The given qualified type.</param>
        /// <returns>A true value if the qualified type is valid as input data.</returns>
        public bool sameInput(QualifiedType operand)
        {
            /* FUTURE <strings> */
            if (type == Constants.TYPE_LITERAL) type = operand.type;

            if (type != operand.type) return false;
            if (pointerDepth != operand.pointerDepth) return false;
            if (b_unsigned != operand.b_unsigned) return false;
            //if (b_volatile && !operand.b_volatile) return false;
            //if (b_const && !operand.b_const) return false;

            return true;
        }

        /// <summary>
        /// Returns a true value if both qualified type are equivalent.
        /// </summary>
        /// <param name="operand">The given qualified type.</param>
        /// <returns>A true value if both qualified type are equivalent.</returns>
        public bool equals(QualifiedType operand)
        {
            /* FUTURE <strings> */
            if (type == Constants.TYPE_LITERAL) type = operand.type;

            if (type != operand.type) return false;
            if (pointerDepth != operand.pointerDepth) return false;
            if (b_unsigned != operand.b_unsigned) return false;
            if (b_volatile != operand.b_volatile) return false;
            if (b_const != operand.b_const) return false;

            return true;
        }

        /// <summary>
        /// Returns a string containing the text formated type.
        /// </summary>
        /// <returns>String containing the text formated type.</returns>
        public string toString()
        {
            string output = string.Empty;

            if (b_const) output += "const ";

            if (b_unsigned) output += "u ";
            output += type;

            for (int i = 0; i < pointerDepth; i++)
                output += "*";

            /* FUTURE <arrays> arrSize; */

            return output;
        }
    }

    public class STType
    {
        public string type;
        public int size;

        public STType(string inType, int inSize)
        {
            type = inType;
            size = inSize;
        }
    }
}
