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

        public const String DATABASE = "deu_news_2009_300K";
        public const String TABLE_WORDS = "words";
        public const String TABLE_CO_N = "co_n";
        public const String TABLE_CO_S = "co_s";


        #region Node properties

        public const long W_ID_PROPERTY_ID = 0L;
        public const long WORD_PROPERTY_ID = 1L;
        
        public const long CO_N_EDGE_PROPERTY_ID = 23L;
        public const long CO_S_EDGE_PROPERTY_ID = 42L;

        #endregion

        #region Edge properties

        public const long SIG_PROPERTY_ID = 10L;
        public const long FREQ_PROPERTY_ID = 11L;

        #endregion
    }
}
