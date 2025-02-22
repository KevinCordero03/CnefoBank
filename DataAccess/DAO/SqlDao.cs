﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccess.DAO
{
    public class SqlDao
    {
        private string ConnectionString = "";

        private static SqlDao Instance;

        private SqlDao()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["Arbank"].ConnectionString;
        }

        public static SqlDao GetInstance()
        {
            if (Instance == null)
                Instance = new SqlDao();
            return Instance;
        }

        public void ExecuteProcedure(SqlOperation operation)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(operation.ProcedureName, conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                foreach (var param in operation.Parameters)
                {
                    command.Parameters.Add(param);
                }
                conn.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<Dictionary<string, object>> ExecuteQueryProcedure(SqlOperation operation)
        {
            var lstResult = new List<Dictionary<string, object>>();

            using (var conn = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(operation.ProcedureName, conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                foreach (var param in operation.Parameters)
                {
                    command.Parameters.Add(param);
                }

                conn.Open();
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var dict = new Dictionary<string, object>();
                        for (var lp = 0; lp < reader.FieldCount; lp++)
                        {
                            dict.Add(reader.GetName(lp), reader.GetValue(lp));
                        }
                        lstResult.Add(dict);
                    }
                }
            }

            return lstResult;
        }
    }
}

