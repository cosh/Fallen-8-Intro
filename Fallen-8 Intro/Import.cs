using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Fallen8.API.Index;
using Fallen8.API.Helper;
using System.Diagnostics;
using Fallen8.API.Model;
using System.Text;

namespace Intro
{
    public class Import
    {
        public static String ImportFromMySql(Fallen8.API.Fallen8 myFallen8, SingleValueIndex nodeIndex)
        {            
            #region Connect to MySql

            var connectionString = String.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3}", 
                Server.Default.WortSchatzDBHOST,
                Server.Default.WortschatzDatabase,
                Server.Default.User, 
                Server.Default.Password);

            var connection = new MySqlConnection(connectionString);
            connection.Open();

            #endregion

            #region Import

            var sb = new StringBuilder();

            // import words
            var sw = Stopwatch.StartNew();
            sb.AppendLine(ReadWords(myFallen8, connection, nodeIndex, Config.TABLE_WORDS));
            sb.AppendLine(String.Format("took {0} ms", sw.Elapsed.TotalMilliseconds));
            sw.Restart();
            sb.AppendLine(ReadCooccurrences(myFallen8, connection, nodeIndex, Config.TABLE_CO_N, Config.CO_N_EDGE_PROPERTY_ID));
            sb.AppendLine(String.Format("took {0} ms", sw.Elapsed.TotalMilliseconds));
            sw.Restart();
            sb.AppendLine(ReadCooccurrences(myFallen8, connection, nodeIndex, Config.TABLE_CO_S, Config.CO_S_EDGE_PROPERTY_ID));
            sb.AppendLine(String.Format("took {0} ms", sw.Elapsed.TotalMilliseconds));
            sw.Stop();

            #endregion

            #region Disconnect MySql

            connection.Close();

            #endregion

            GC.Collect();
            GC.Collect();
            GC.WaitForFullGCApproach();

            return sb.ToString();
        }

        private static string ReadWords(Fallen8.API.Fallen8 myFallen8, MySqlConnection mySql, IIndex nodeIndex, String tableName)
        {            
            // query
            var query = mySql.CreateCommand();
            query.CommandText = "SELECT w_id, word FROM " + tableName;

            var sb = new StringBuilder();

            String word;
            Int32 w_id;
            var creationDate = DateTime.Now.ToBinary();
            VertexModel vertex;

            sb.AppendLine(String.Format("importing {0} words from {1}", GetMySqlRowCount(mySql, tableName), tableName));

            var reader = query.ExecuteReader();

            while (reader.Read())
            {
                w_id = reader.GetInt32(0);
                word = reader.GetString(1);

                vertex = myFallen8.CreateVertex(creationDate, new List<PropertyContainer> 
                { 
                    new PropertyContainer { PropertyId = Config.W_ID_PROPERTY_ID, Value = w_id},
                    new PropertyContainer { PropertyId = Config.WORD_PROPERTY_ID, Value = word}
                });

                nodeIndex.AddOrUpdate(w_id, vertex);
            }

            reader.Close();

            return sb.ToString();
        }

        private static string ReadCooccurrences(Fallen8.API.Fallen8 myFallen8, MySqlConnection mySql, SingleValueIndex nodeIdx, String tableName, Int32 edgePropertyID)
        {
            // query
            var query = mySql.CreateCommand();
            query.CommandText = "SELECT w1_id, w2_id, freq, sig FROM " + tableName;

            var sb = new StringBuilder();

            var creationDate = DateTime.Now.ToBinary();
            Int32 w1_id;
            Int32 w2_id;
            Int32 freq;
            Double sig;

            AGraphElement sources;
            AGraphElement targets;

            VertexModel source = null;
            VertexModel target = null;

            sb.AppendLine(String.Format("importing {0} co-occurrences from {1}", GetMySqlRowCount(mySql, tableName), tableName));

            var reader = query.ExecuteReader();
        
            while (reader.Read())
            {
                w1_id = reader.GetInt32(0);
                w2_id = reader.GetInt32(1);
                freq = reader.GetInt32(2);
                sig = reader.GetDouble(3);

                if (nodeIdx.TryGetValue(out sources, w1_id))
                {
                    source = (VertexModel) sources;
                }

                if (nodeIdx.TryGetValue(out targets, w2_id))
                {
                    target = (VertexModel) targets;
                }

                // create edge

                myFallen8.CreateEdge(source.Id, edgePropertyID, new EdgeModelDefinition(target.Id, creationDate, new List<PropertyContainer> 
                { 
                    new PropertyContainer { PropertyId = Config.FREQ_PROPERTY_ID, Value = freq},
                    new PropertyContainer { PropertyId = Config.SIG_PROPERTY_ID, Value = sig},
                    }));              
            }
            reader.Close();

            return sb.ToString();
        }

        private static int GetMySqlRowCount(MySqlConnection mySql, String tableName = "")
        {            
            var count = 0;

            var query = mySql.CreateCommand();
            query.CommandText = "SELECT COUNT(*) FROM " + tableName;

            var reader = query.ExecuteReader();

            while (reader.Read())
            {
                count = reader.GetInt32(0);
            }

            reader.Close();

            return count;
        }
    }
}
