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

        public static readonly string ERR_SEM_MSG_05 = "Operation only valid to 1 byte sized types";
    }
}
