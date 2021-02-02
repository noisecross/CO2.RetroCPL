/**
* |------------------------------------------|
* | CO2.RetroCPL COMPILER OPTIMIZER 2 RETROC |
* | File: Quad.cs                            |
* | v1.00, February 2021                     |
* | Author: Emilio Arango Delgado de Mendoza |
* |------------------------------------------|
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO2.RetroCPL.Commons
{
    public class Quad
    {
        public enum Opcode
        {
            ERR = -1,
            NOP , ADD , SUB , MUL , DIV , MOD , INC , DEC ,
            LES , RIS ,
            NEG , BR  , BEQ , BNE , BGR , BLS ,
            EQ  , NE  , GR  , LS  , LAND, LOR ,
            AND , OR  , XOR , NOT ,
            MOVE, MVP , MVA , STP , STA ,
            LABL,
            PUSH, POP , CALL, RET , HALT
        };

        // NOP  ---,
        // ADD  xyz,  SUB  xyz,  MUL  xyz,  DIV  xyz,  MOD  xyz,  INC  xy-,  DEC  xy-,
        // LES  xyz,  RIS  xyz,
        // NEG  xy-,  BR   --L,  BEQ  xyL,  BNE  xyL,  BGR  xyL,  BLS  xyL,  EQ   xyz,  NE   xyz, GR   xyz,  LS   xyz,  LAND xyz,  LOR  xyz,
        // AND  xyz,  OR   xyz,  XOR  xyz,  NOT  xy-,
        // MOVE xy-,  MVP  xy-,  MVA  xy-,  STP  xy-,  STA  xy-,
        // LABL --L,
        // PUSH x--,  POP  x--,  CALL --L,  RET  x--,
        // HALT ---

        public static readonly string[] opcodeTxt = new string[] {
            "NOP ", "ADD ", "SUB ", "MUL ", "DIV ", "MOD ", "INC ", "DEC ",
            "LES ", "RIS ",
            "NEG ", "BR  ", "BEQ ", "BNE ", "BGR ", "BLS ",
            "EQ  ", "NE  ", "GR  ", "LS  ", "LAND", "LOR ",
            "AND ", "OR  ", "XOR ", "NOT ",
            "MOVE", "MVP ", "MVA ", "STP ", "STA ",
            "LABL",
            "PUSH", "POP ", "CALL", "RET ", "HALT" };


        public enum OpType { UNDEF = -1, VAR, ADDRESS, CONST, LABEL };

        public Opcode  opcode;
        public Operand opX;
        public Operand opY;
        public Operand opZ;
        public int     size;

        /// <summary>
        /// Class constructor.
        /// </summary>
        public Quad()
        {
            opcode = Opcode.ERR;
            opX = new Operand();
            opY = new Operand();
            opZ = new Operand();
            size = 0;
        }

        /// <summary>
        /// Class constructor.
        /// </summary>
        public Quad(Opcode inOpcode, Operand inX, Operand inY, Operand inZ)
        {
            opcode = inOpcode;
            opX = inX;
            opY = inY;
            opZ = inZ;
            size = 0;
        }


        public string toString()
        {
            //Label if exists 
            string sLabel = (opZ.opType == OpType.LABEL && opcode == Opcode.LABL) ? opZ.label : "";

            //Opcode
            string sOpcode = (opcode < 0) ? "ERR" : opcodeTxt[(int)opcode];

            //Full Quad
            return string.Format("{0,-" + Config.MAX_ID_LEN + 5 + "}{1,-5}{2,-16}{3,-16}{4,-16}", sLabel, sOpcode, opX.toString(), opY.toString(), opZ.toString());
        }


        /// <summary>
        /// Quad operand
        /// </summary>
        public class Operand
        {
            public OpType opType;
            public string label;
            public OpSymbol opSymbol;

            public Operand()
            {
                opSymbol.constant = 0;
                opType = OpType.CONST;
            }

            public string toString()
            {
                switch (opType)
                {
                    case OpType.VAR:
                        return opSymbol.var.lex.Substring(0, 15);
                    case OpType.CONST:
                        return opSymbol.constant.ToString("x4");
                    case OpType.LABEL:
                        return label;
                    default:
                        return "-";
                }
            }

            /// <summary>
            /// Operand symbol
            /// </summary>
            public class OpSymbol
            {
                public STEntry var;
                public int constant;
                public int label;
            }
        }
    }
}
