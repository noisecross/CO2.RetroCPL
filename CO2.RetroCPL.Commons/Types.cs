using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO2.RetroCPL.Commons
{
    public class QualifiedType
    {
        public string type      = "void";
        public int arrSize      = 1;
        public int pointerDepth = 0;
        public bool b_unsigned  = false;
        public bool b_const     = false;
        public bool b_volatile  = false;
        public bool b_interrupt = false;

        public QualifiedType() { }

        public QualifiedType(string type)
        {
            this.type = type;
        }

        public QualifiedType(QualifiedType qualifiedType)
        {
            type = qualifiedType.type;
            arrSize = qualifiedType.arrSize;
            pointerDepth = qualifiedType.pointerDepth;
            b_unsigned = qualifiedType.b_unsigned;
            b_const = qualifiedType.b_const;
            b_volatile = qualifiedType.b_volatile;
            b_interrupt = qualifiedType.b_interrupt;
        }

        public bool sameOutput(QualifiedType operand)
        {
            /* FUTURE <strings> */
            if (!((operand.type == Constants.TYPE_LITERAL) && ((type == "word") || (type == "byte"))))
            {
                if (type != operand.type) return false;
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

            if (type != operand.type) return false;
            if (pointerDepth != operand.pointerDepth) return false;
            if (b_unsigned != operand.b_unsigned) return false;
            if (b_volatile && !operand.b_volatile) return false;
            if (b_const && !operand.b_const) return false;

            return true;
        }

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
}
