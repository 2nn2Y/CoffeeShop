using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.EntityFramework;
using MySql.Data.MySqlClient;
using YT.EntityFramework;

namespace YT
{
   public static class MySqlHelper
    {
        //从配置文件读取连接字符串
        private static readonly string Connstr = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

        //创建连接
        public static MySqlConnection CreateConnection()
        {
            MySqlConnection conn = new MySqlConnection(Connstr);
            conn.Open();
            return conn;
        }

        public static int ExecuteNonQuery(MySqlConnection conn, string sql, params MySqlParameter[] parameters)
        {
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }
        }

        public static int ExecuteNonQuery(string sql, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = CreateConnection())
            {
                return ExecuteNonQuery(conn, sql, parameters);
            }
        }

        public static object ExecuteScalar(MySqlConnection conn, string sql, params MySqlParameter[] parameters)
        {
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteScalar();
            }
        }

        public static object ExecuteScalar(string sql, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = CreateConnection())
            {
                return ExecuteScalar(conn, sql, parameters);
            }
        }

        public static DataTable ExecuteQuery(MySqlConnection conn, string sql, MySqlParameter[] parameters)
        {
            DataTable table = new DataTable();
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(parameters);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    table.Load(reader);
                }
            }
            return table;
        }

        public static DataTable ExecuteQuery(string sql, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = CreateConnection())
            {
                return ExecuteQuery(conn, sql, parameters);
            }
        }
    }
}
