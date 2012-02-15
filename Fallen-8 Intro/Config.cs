using System;

namespace Intro
{
    public class Config
    {
        public const String TABLE_WORDS = "words";
        public const String TABLE_CO_N = "co_n";
        public const String TABLE_CO_S = "co_s";


        #region Node properties

        public const UInt16 W_ID_PROPERTY_ID = 0;
        public const UInt16 WORD_PROPERTY_ID = 1;

        public const UInt16 CO_N_EDGE_PROPERTY_ID = 23;
        public const UInt16 CO_S_EDGE_PROPERTY_ID = 42;

        #endregion

        #region Edge properties

        public const UInt16 SIG_PROPERTY_ID = 10;
        public const UInt16 FREQ_PROPERTY_ID = 11;

        #endregion
    }
}
