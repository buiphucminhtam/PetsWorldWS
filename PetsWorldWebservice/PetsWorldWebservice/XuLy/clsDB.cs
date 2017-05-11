using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlTypes;


namespace PetsWorldWebservice.XuLy
{
    public class clsDB
    {

        private SqlConnection m_Connection;
        SqlCommand sqlCmd;
        SqlTransaction sqlTran;

        public static string strConnSalesUp = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
       

        private string connectionStr = "";

        //Properties.Settings.Default.OPV;
        public clsDB()
        {
            this.pConnectionStr = strConnSalesUp;
            m_Connection = new SqlConnection(this.pConnectionStr);
            if (m_Connection.State == ConnectionState.Closed)
                m_Connection.Open();
            sqlCmd = new SqlCommand("", m_Connection);
        }

        public clsDB(string strConLocal)
        {
            this.pConnectionStr = strConLocal;
            m_Connection = new SqlConnection(pConnectionStr);
            if (m_Connection.State == ConnectionState.Closed)
                m_Connection.Open();
            sqlCmd = new SqlCommand("", m_Connection);
        }

        public void BeginTran()
        {
            sqlTran = m_Connection.BeginTransaction(IsolationLevel.Snapshot);

            if (sqlCmd == null)
                sqlCmd = new SqlCommand("", m_Connection, sqlTran);
            else
                sqlCmd.Transaction = sqlTran;
        }

        public void BeginTran_Not_Snapshot()
        {
            sqlTran = m_Connection.BeginTransaction();

            if (sqlCmd == null)
                sqlCmd = new SqlCommand("", m_Connection, sqlTran);
            else
                sqlCmd.Transaction = sqlTran;
        }

        public string pConnectionStr
        {
            set { this.connectionStr = value; }
            get
            {
                return connectionStr;
            }
        }


        public SqlConnection pConnection
        {
            set { this.m_Connection = value; }
            get
            {
                return m_Connection;
            }
        }

        public SqlTransaction pSqlTran
        {
            set { this.sqlTran = value; }
            get
            {
                return sqlTran;
            }
        }

        public SqlCommand pSqlCmd
        {
            set { this.sqlCmd = value; }
            get
            {
                return sqlCmd;
            }
        }


        public void CLose_Connection()
        {
            if (sqlTran != null)
                sqlTran.Dispose();
            sqlCmd.Dispose();
            SqlConnection.ClearPool(m_Connection);
            m_Connection.Dispose();
        }

        public void CommitAndDispose()
        {
            if(sqlTran != null )
            sqlTran.Commit();
            CLose_Connection();
        }

        public void RollbackAndDispose()
        {
            try
            {
                if (sqlTran != null)
                    sqlTran.Rollback();
            }
            catch (Exception e) { }
            CLose_Connection();
        }

        public  bool updateQuery(SqlCommand lenh, string query)
        {
            try
            {
                lenh.CommandText = query;
                lenh.ExecuteNonQuery();
                return true;
            }
            catch (Exception e) { return false; }
        }

        public bool updateQuery( string query)
        {
            try
            {
                sqlCmd.CommandText = query;
                sqlCmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e) { return false; }
        }

        public  int updateQueryReturnInt(SqlCommand lenh, string query)
        {
            try
            {
                lenh.CommandText = query;
                return lenh.ExecuteNonQuery();
            }
            catch (Exception e) { return -1; }
        }
        public int updateQueryReturnInt(string query)
        {
            try
            {
                sqlCmd.CommandText = query;
                return sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e) { string a = e.Message; return -1; }
        }
        public string updateQueryReturnMsg(string query)
        {
            try
            {
                sqlCmd.CommandText = query;
                sqlCmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception e) { return "Error:" + e.Message; }
        }

        public DataTable getDataTable(string query)
        {
            DataTable dt = new DataTable("Table");
            sqlCmd.CommandText = query;
            dt.Load(sqlCmd.ExecuteReader());
            return dt;
        }

        public string getFirstValueSql(SqlCommand lenh, string query)
        {
            lenh.CommandText = query;
            return lenh.ExecuteScalar().ToString();
        }

        public string getFirstStringValueSqlCatchException(SqlCommand lenh, string query)
        {
            try
            {
                lenh.CommandText = query;
                return lenh.ExecuteScalar().ToString();
            }
            catch (Exception e) { return ""; }
        }
        public string getFirstStringValueSqlCatchException(string query)
        {
            try
            {
                sqlCmd.CommandText = query;
                return sqlCmd.ExecuteScalar().ToString();
            }
            catch (Exception e) { return ""; }
        }

        public double getFirstDoubleValueSqlCatchException( string query)
        {
            try
            {
                sqlCmd.CommandText = query;
                return double.Parse(sqlCmd.ExecuteScalar().ToString());
            }
            catch (Exception e) { return 0; }
        }

        public int getIntDoubleValueSqlCatchException(SqlCommand lenh, string query)
        {
            try
            {
                lenh.CommandText = query;
                return int.Parse(lenh.ExecuteScalar().ToString());
            }
            catch (Exception e) { return 0; }
        }

        /// <summary>
        /// ///////////// static Database function
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>


        public static  DataTable getDataTable(string connectionStrLocal,string query)
        {
            DataTable dt = new DataTable("Table");
            SqlDataAdapter da = new SqlDataAdapter(query, connectionStrLocal);
            da.Fill(dt);
            da.Dispose();
            return dt;
        }
        public static DataTable getDataTable(SqlDataAdapter da, string query)
        {
            DataTable dt = new DataTable("Table");
            da.SelectCommand.CommandText = query;
            da.SelectCommand.CommandTimeout = 120;
            da.Fill(dt);
            da.Dispose();
            return dt;
        }
        public static string getFirstStringValueSql(string connectionStrLocal, string query)
        {
            DataTable dt = new DataTable("Table");
            SqlDataAdapter da = new SqlDataAdapter(query, connectionStrLocal);
            da.Fill(dt);
            da.Dispose();
            string value = "";
            if (dt.Rows.Count > 0)
                value = dt.Rows[0][0].ToString();
            dt.Dispose();
            return value;
        }

        public static int getFirstIntValueSql(string connectionStrLocal, string query)
        {
            try
            {
                DataTable dt = new DataTable("Table");
                SqlDataAdapter da = new SqlDataAdapter(query, connectionStrLocal);
                da.Fill(dt);
                da.Dispose();
                string value = "0";
                if (dt.Rows.Count > 0)
                    value = dt.Rows[0][0].ToString();
                dt.Dispose();
                return int.Parse(value);
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }
}
