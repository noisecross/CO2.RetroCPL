using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO2.RetroCPL.Commons
{
    public class Constants
    {
        public static readonly string VERSION = "v0.01";

        public static readonly string TYPE_ERR         = "$type_err";
        public static readonly string TYPE_OK          = "$type_ok";
        public static readonly string TYPE_LITERAL     = "$numeric_literal";
        public static readonly string GLOBAL_FRAMEWORK = "$GLOBAL";

        public const int MAIN_RET_OK       =  0;
        public const int MAIN_RET_HELP     = -1;
        public const int MAIN_RET_VER      = -2;
        public const int MAIN_RET_BAD_ARGS = -3;
        public const int MAIN_RET_FILE_ERR = -4;
        public const int MAIN_RET_CODE_ERR = -5;
    }

    public class ErrorMessages
    {
        public static readonly string ERR_LEX_MSG_00 = "Illegal symbol in language";
        public static readonly string ERR_LEX_MSG_01 = "Identifier name is too long";
        public static readonly string ERR_LEX_MSG_02 = "End of file found inside a comment";

        //public static readonly string ERR_SIN_MSG_00 = "Functionallity still not operational";
        //public static readonly string ERR_SIN_MSG_01 = "Unsupported operation by the destination machine";
        public static readonly string ERR_SIN_MSG_02 = "Type specifier expected";

        //public static readonly string ERR_SEM_MSG_00 = "Undeclared identifier";
        //public static readonly string ERR_SEM_MSG_01 = "Input arguments imcompatible with function";
        //public static readonly string ERR_SEM_MSG_02 = "Undeclared function";
        //public static readonly string ERR_SEM_MSG_03 = "Undeclared type";
        //public static readonly string ERR_SEM_MSG_04 = "Type cannot be promoted";
        public static readonly string ERR_SEM_MSG_05 = "Operation only valid to 1 byte sized types";
        //public static readonly string ERR_SEM_MSG_06 = "Incompatible types";
        public static readonly string ERR_SEM_MSG_07 = "Type error in declaration";
        //public static readonly string ERR_SEM_MSG_08 = "Variables don\'t allow the \'interrupt\' property";
        //public static readonly string ERR_SEM_MSG_09 = "Variable already declared";
        public static readonly string ERR_SEM_MSG_0A = "Initial function \'main\' not found";
        //public static readonly string ERR_SEM_MSG_0B = "Redefinition of an already declared function";
        //public static readonly string ERR_SEM_MSG_0C = "Redefinition of a label in the same namespace";
        //public static readonly string ERR_SEM_MSG_0D = "The sentence is illegal in that part of the code";
        public static readonly string ERR_SEM_MSG_0E = "The \'main\' function don\'t allow input arguments";
        //public static readonly string ERR_SEM_MSG_0F = "The literal cannot be transformed into a number";
        //public static readonly string ERR_SEM_MSG_10 = "Bad operation performing semantic analisys. This is a compiler error.";
        //public static readonly string ERR_SEM_MSG_11 = "Invalid operation for unsigned types";
        //public static readonly string ERR_SEM_MSG_12 = "Label undefined in this function";
        //public static readonly string ERR_SEM_MSG_13 = "Assign to  a \'const\' variable";
        //public static readonly string ERR_SEM_MSG_14 = "Assign overflow";
        public static readonly string ERR_SEM_MSG_15 = "Function declared but not defined";
        public static readonly string ERR_SEM_MSG_16 = "This compiler don\'t allow close graphs in function calls.";
        public static readonly string ERR_SEM_MSG_17 = ". Avoid recursion and call cycles.";

        //public static readonly string WAR_SEM_MSG_00 = "Value will be always 0";
        //public static readonly string WAR_SEM_MSG_01 = "Operation between signed and unsigned types";
        //public static readonly string WAR_SEM_MSG_02 = "Useless code after a mandatory \'return\'";
        //public static readonly string WAR_SEM_MSG_03 = "Not every path return a value";
        //public static readonly string WAR_SEM_MSG_04 = "Wrong loop. Initial condition will never pass";
        //public static readonly string WAR_SEM_MSG_05 = "Diferent pointers. Did you forget a cast?";
        //public static readonly string WAR_SEM_MSG_06 = "Information lost promoting type";
        public static readonly string WAR_SEM_MSG_07 = "Variable declared but not used";
    }
}
