using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Web;


namespace WebOppointmentApi.Common
{
    /// <summary>
    ///SqlCommonDataAccess 通用数据库访问类，请不要修改此类代码。
    /// </summary>
    public static class SqlCommon
    {
        public static DataSet SqlQuery(SqlConnection sqlConnection, string sql)
        {
            SqlDataAdapter adapter = null;
            DataSet set = null;
            using (SqlCommand command = new SqlCommand(sql, sqlConnection))
            {
                adapter = new SqlDataAdapter(command);
                set = new DataSet();
                adapter.Fill(set);
                adapter.SelectCommand.Parameters.Clear();
                adapter.Dispose();
                command.Parameters.Clear();
                command.Dispose();

                return set;
            }
        }

    }
}

