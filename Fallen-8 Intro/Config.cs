using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intro
{
    public class Config
    {
        public const String HOST = "localhost";
        public const String USER = "root";
        public const String PASSWORD = "password";

        public const String DATABASE = "wortschatz";
        public const String TABLE_WORDS = "words";
        public const String TABLE_CO_N = "co_n";
        public const String TABLE_CO_S = "co_s";


        #region Node properties

        public const Int32 W_ID_PROPERTY_ID = 0;
        public const Int32 WORD_PROPERTY_ID = 1;

        public const Int32 CO_N_EDGE_PROPERTY_ID = 23;
        public const Int32 CO_S_EDGE_PROPERTY_ID = 42;

        #endregion

        #region Edge properties

        public const Int32 SIG_PROPERTY_ID = 10;
        public const Int32 FREQ_PROPERTY_ID = 11;

        #endregion
    }
}
