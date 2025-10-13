namespace DataBase;
public static class DB
{
    #region Declaration
    private static string dataProvider = AppSettingFile.Get("DataProvider");
    private static readonly DbProviderFactory factory = DbProviderFactories.GetFactory(dataProvider);
    private static readonly string connectionString = GetConnectionString();
    #endregion

    #region Database Common Function

    /// <summary>
    /// GetDataTable
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    public static DataTable GetDataTable(DbCommand cmd)
    {
        DataTable dt = new DataTable();
        if (cmd != null)
        {
            DbDataAdapter da = factory.CreateDataAdapter()!;
            cmd.CommandTimeout = 10000000;
            da.SelectCommand = cmd;
            da.Fill(dt);
        }
        GC.Collect();
        return dt;
    }

    /// <summary>
    /// GetDataSet
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    public static DataSet GetDataSet(DbCommand cmd)
    {
        DataSet ds = new DataSet();
        if (cmd != null)
        {
            DbDataAdapter da = factory.CreateDataAdapter()!;
            cmd.CommandTimeout = 10000000;
            da.SelectCommand = cmd;
            da.Fill(ds);
        }
        GC.Collect();
        return ds;
    }

    /// <summary>
    /// GetDataRow
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    public static DataRow? GetDataRow(DbCommand cmd)
    {
        DataTable dt = new DataTable();
        if (cmd != null)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            DbDataAdapter da = factory.CreateDataAdapter()!;

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            cmd.Connection = Connection;
            cmd.CommandTimeout = 10000000;
            da.SelectCommand = cmd;
            da.Fill(dt);
        }
        GC.Collect();
        if (dt.Rows.Count > 0)
        {
            return dt.Rows[0];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// GetDataTableByProc
    /// </summary>
    /// <param name="p_sProcName"></param>
    /// <param name="Parameters"></param>
    /// <returns></returns>
    public static DataTable GetDataTableByProc(string p_sProcName, params IDataParameter[] Parameters)
    {
        using (DbCommand cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            cmd.Connection = Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10000000;
            cmd.CommandText = p_sProcName;
            if (Parameters != null)
            {
                for (int iParamCount = 0; iParamCount < Parameters.Length; iParamCount++)
                {
                    cmd.Parameters.Add(Parameters[iParamCount]);
                }
            }
            GC.Collect();
            return GetDataTable(cmd);
        }
    }


    /// <summary>
    /// GetDataSetByProc
    /// </summary>
    /// <param name="p_sProcName"></param>
    /// <param name="Parameters"></param>
    /// <returns></returns>
    public static DataSet GetDataSetByProc(string p_sProcName, params IDataParameter[] Parameters)
    {
        using (DbCommand cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            cmd.Connection = Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10000000;
            cmd.CommandText = p_sProcName;
            if (Parameters != null)
            {
                for (int iParamCount = 0; iParamCount < Parameters.Length; iParamCount++)
                {
                    cmd.Parameters.Add(Parameters[iParamCount]);
                }
            }
            GC.Collect();
            return GetDataSet(cmd);
        }
    }

    /// <summary>
    /// GetDataTableByQuery
    /// </summary>
    /// <param name="sQuery"></param>
    /// <param name="Parameters"></param>
    /// <returns></returns>
    public static DataTable GetDataTableByQuery(string sQuery, params IDataParameter[] Parameters)
    {
        using (DbCommand cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            cmd.Connection = Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 10000000;
            cmd.CommandText = sQuery;
            if (Parameters != null)
            {
                for (int iParamCount = 0; iParamCount < Parameters.Length; iParamCount++)
                {
                    cmd.Parameters.Add(Parameters[iParamCount]);
                }
            }
            GC.Collect();
            return GetDataTable(cmd);
        }
    }

    /// <summary>
    /// GetDataTableByProc
    /// </summary>
    /// <param name="p_sProcName"></param>
    /// <returns></returns>
    public static DataTable GetDataTableByProc(string p_sProcName)
    {
        using (DbCommand cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            cmd.Connection = Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = p_sProcName;
            cmd.CommandTimeout = 10000000;
            GC.Collect();
            return GetDataTable(cmd);
        }
    }

    /// <summary>
    /// ExecProc
    /// </summary>
    /// <param name="p_sProcName"></param>
    public static void ExecProc(string p_sProcName)
    {
        using (DbCommand cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            cmd.Connection = Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10000000;
            cmd.CommandText = p_sProcName;
            cmd.ExecuteNonQuery();
        }
        GC.Collect();
    }

    /// <summary>
    /// GetDataRow
    /// </summary>
    /// <param name="p_sProcName"></param>
    /// <param name="Parameters"></param>
    /// <returns></returns>
    public static DataRow? GetDataRow(string p_sProcName, params IDataParameter[] Parameters)
    {
        using (DbCommand cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            cmd.Connection = Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10000000;
            cmd.CommandText = p_sProcName;
            if (Parameters != null)
            {
                for (int iParamCount = 0; iParamCount < Parameters.Length; iParamCount++)
                {
                    cmd.Parameters.Add(Parameters[iParamCount]);
                }
            }
            return GetDataRow(cmd);
        }
    }

    /// <summary>
    /// ExecNonQueryByProc
    /// </summary>
    /// <param name="p_sProcName"></param>
    /// <param name="Parameters"></param>
    /// <returns></returns>
    public static DbCommand ExecNonQueryByProc(string p_sProcName, params IDataParameter[] Parameters)

    {
        using (DbCommand cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            cmd.Connection = Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10000000;
            cmd.CommandText = p_sProcName;
            if (Parameters != null)
            {
                for (int iParamCount = 0; iParamCount < Parameters.Length; iParamCount++)
                {
                    cmd.Parameters.Add(Parameters[iParamCount]);
                }

                cmd.ExecuteNonQuery();
            }
            GC.Collect();
            return cmd;
        }
    }

    /// <summary>
    /// ExecNonQueryByQuery
    /// </summary>
    /// <param name="p_sQuery"></param>
    /// <param name="Parameters"></param>
    /// <returns></returns>
    public static DbCommand ExecNonQueryByQuery(string p_sQuery, params IDataParameter[] Parameters)
    {
        using (DbCommand cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            cmd.Connection = Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 10000000;
            cmd.CommandText = p_sQuery;
            if (Parameters != null)
            {
                for (int iParamCount = 0; iParamCount < Parameters.Length; iParamCount++)
                {
                    cmd.Parameters.Add(Parameters[iParamCount]);
                }

                cmd.ExecuteNonQuery();
            }
            GC.Collect();
            return cmd;
        }
    }

    public static DbCommand ExecAllNonQuery(List<string> p_sQuery)
    {
        using (DbCommand cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            cmd.Connection = Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 10000000;
            foreach (string sqry in p_sQuery)
            {
                cmd.CommandText = sqry;
                cmd.ExecuteNonQuery();
            }
            GC.Collect();
            return cmd;
        }
    }

    /// <summary>
    /// GetColumns
    /// </summary>
    /// <param name="sTableName"></param>
    /// <param name="sRowFilter"></param>
    /// <returns></returns>
    public static DataTable? GetColumns(string sTableName, string sRowFilter)
    {
        DataTable dt = new DataTable();

        DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
        if (Connection.State == ConnectionState.Closed)
        {
            Connection.Open();
        }
        if (sTableName != string.Empty)
            dt = Connection.GetSchema("Columns", new string[] { string.Empty, string.Empty, sTableName, string.Empty });

        if (sRowFilter == "COLUMN_NAME NOT LIKE '%ID%'")
        {
            dt.DefaultView.RowFilter = sRowFilter;
        }
        else
        {
            dt.DefaultView.RowFilter = "COLUMN_NAME NOT LIKE '%ID%' AND " + sRowFilter;
        }
        return dt;
    }

    /// <summary>
    /// GetColumns
    /// </summary>
    /// <param name="sTableName"></param>
    /// <returns></returns>
    public static DataTable? GetColumns(string sTableName)
    {
        return GetColumns(sTableName, "COLUMN_NAME NOT LIKE '%ID%'");
    }


    /// <summary>
    /// FillDatasetByQuery
    /// </summary>
    /// <param name="p_sQuery"></param>
    /// <param name="p_dDataset"></param>
    /// <param name="p_sTableName"></param>
    /// <returns></returns>
    public static DataSet FillDatasetByQuery(string p_sQuery, DataSet p_dDataset, string p_sTableName)
    {
        DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
        DbCommand cmd = factory.CreateCommand()!;
        DbDataAdapter da = factory.CreateDataAdapter()!;
        if (Connection.State == ConnectionState.Closed)
        {
            Connection.Open();
        }
        cmd.Connection = Connection;
        cmd.CommandText = p_sQuery;
        cmd.CommandTimeout = 10000000;
        da.SelectCommand = cmd;
        da.SelectCommand = cmd;
        da.Fill(p_dDataset, p_sTableName);
        GC.Collect();
        return p_dDataset;
    }

    /// <summary>
    /// FillReportDataSetByProc
    /// This is used for filling the table in the dataset used for crystal report
    /// </summary>
    /// <param name="p_sProcName"></param>
    /// <param name="p_dDataset"></param>
    /// <param name="p_sTableName"></param>
    /// <param name="Parameters"></param>
    /// <returns></returns>
    public static DataSet FillReportDataSetByProc(string p_sProcName, DataSet p_dDataset, string p_sTableName,
                                            params IDataParameter[] Parameters)
    {
        using (DbCommand cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);

            DbDataAdapter da = factory.CreateDataAdapter()!;
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            cmd.Connection = Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10000000;
            cmd.CommandText = p_sProcName;
            if (Parameters != null)
            {
                for (int iParamCount = 0; iParamCount < Parameters.Length; iParamCount++)
                {
                    cmd.Parameters.Add(Parameters[iParamCount]);
                }
            }
            da.SelectCommand = cmd;

            da.Fill(p_dDataset, p_sTableName);

            GC.Collect();
            return p_dDataset;
        }
    }

    /// <summary>
    /// FillReportDataSetByProc
    /// This is used for filling the table in the dataset used for crystal report
    /// </summary>
    /// <param name="p_sProcName"></param>
    /// <param name="p_dDataset"></param>
    /// <param name="p_sTableName"></param>
    /// <param name="Parameters"></param>
    /// <returns></returns>
    public static DataSet FillReportDataSetByProc(string p_sProcName, DataSet p_dDataset, string p_sTableName,
                                            out DbCommand dbcmd, params IDataParameter[] Parameters)
    {
        using (DbCommand cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);

            DbDataAdapter da = factory.CreateDataAdapter()!;
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            cmd.Connection = Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10000000;
            cmd.CommandText = p_sProcName;
            if (Parameters != null)
            {
                for (int iParamCount = 0; iParamCount < Parameters.Length; iParamCount++)
                {
                    cmd.Parameters.Add(Parameters[iParamCount]);
                }
                cmd.ExecuteNonQuery();
            }
            da.SelectCommand = cmd;

            da.Fill(p_dDataset, p_sTableName);

            GC.Collect();
            dbcmd = cmd;
            return p_dDataset;
        }
    }

    /// <summary>
    /// ExeScalarByProc
    /// </summary>
    /// <param name="p_sProcName"></param>
    /// <param name="Parameters"></param>
    /// <returns></returns>
    public static int ExeScalarByProc(string p_sProcName, params IDataParameter[] Parameters)
    {
        using (DbCommand cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            cmd.Connection = Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10000000;
            cmd.CommandText = p_sProcName;
            if (Parameters != null)
            {
                for (int iParamCount = 0; iParamCount < Parameters.Length; iParamCount++)
                {
                    cmd.Parameters.Add(Parameters[iParamCount]);
                }
                GC.Collect();
                return int.Parse(cmd.ExecuteScalar()!.ToString()!);
            }
        }
        GC.Collect();
        return 0;
    }

    /// <summary>
    /// ExecuteScalarWithQuery
    /// </summary>
    /// <param name="p_sQuery"></param>
    /// <param name="p_sParameters"></param>
    /// <returns></returns>
    /// // BS: 18092024 (IS5-T55) - Application VAPT: Cycle 1 (July 24 to Dec 24)
    public static int ExecScalarByQuery(string p_sQuery, params IDataParameter[] p_sParameters)
    {
        using (DbCommand cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            cmd.Connection = Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 10000000;
            cmd.CommandText = p_sQuery;
            if (p_sParameters != null)
            {
                for (int iParamCount = 0; iParamCount < p_sParameters.Length; iParamCount++)
                {
                    cmd.Parameters.Add(p_sParameters[iParamCount]);
                }
                GC.Collect();
                return int.Parse(cmd.ExecuteScalar()!.ToString()!);
            }
        }
        GC.Collect();
        return 0;
    }

    /// <summary>
    /// ExecProcedure
    /// </summary>
    /// <param name="p_sProcName"></param>
    /// <param name="Parameters"></param>
    /// <returns></returns>
    public static DbCommand ExecProcedure(string p_sProcName, params IDataParameter[] Parameters)
    {
        using (DbCommand Cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            Cmd.Connection = Connection;
            Cmd.CommandText = p_sProcName;
            Cmd.CommandTimeout = 10000000;

            Cmd.CommandType = CommandType.StoredProcedure;
            if (Parameters != null)
            {
                for (int iParamCount = 0; iParamCount < Parameters.Length; iParamCount++)
                {
                    Cmd.Parameters.Add(Parameters[iParamCount]);
                }
                Cmd.ExecuteNonQuery();
            }
            GC.Collect();
            return Cmd;
        }
    }

    public static DbCommand ExecProcedure(string p_sProcName, out DataTable dtbl,
                                                          params IDataParameter[] Parameters)
    {
        using (DbCommand Cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            Cmd.Connection = Connection;
            Cmd.CommandText = p_sProcName;
            Cmd.CommandTimeout = 10000000;

            Cmd.CommandType = CommandType.StoredProcedure;
            if (Parameters != null)
            {
                for (int iParamCount = 0; iParamCount < Parameters.Length; iParamCount++)
                {
                    Cmd.Parameters.Add(Parameters[iParamCount]);
                }
            }
            dtbl = GetDataTable(Cmd);
            GC.Collect();
            return Cmd;
        }
    }

    public static DbCommand ExecProcedure(string p_sProcName, out DataSet ds,
                                                          params IDataParameter[] Parameters)
    {
        using (DbCommand Cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            Cmd.Connection = Connection;
            Cmd.CommandText = p_sProcName;
            Cmd.CommandTimeout = 10000000;

            Cmd.CommandType = CommandType.StoredProcedure;
            if (Parameters != null)
            {
                for (int iParamCount = 0; iParamCount < Parameters.Length; iParamCount++)
                {
                    Cmd.Parameters.Add(Parameters[iParamCount]);
                }
            }
            ds = GetDataSet(Cmd);
            GC.Collect();
            return Cmd;
        }
    }

    #endregion

    #region Table Operation Function

    /// <summary>
    /// InsertByProc
    /// </summary>
    /// <param name="p_sProcName"></param>
    /// <param name="Parameters"></param>
    /// <returns></returns>
    public static DbCommand InsertByProc(string p_sProcName, params IDataParameter[] Parameters)
    {
        using (DbCommand Cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            Cmd.Connection = Connection;
            Cmd.CommandText = p_sProcName;
            Cmd.CommandTimeout = 10000000;

            Cmd.CommandType = CommandType.StoredProcedure;
            if (Parameters != null)
            {
                for (int iParamCount = 0; iParamCount < Parameters.Length; iParamCount++)
                {
                    Cmd.Parameters.Add(Parameters[iParamCount]);
                }
                Cmd.ExecuteNonQuery();
            }
            return Cmd;
        }
    }

    /// <summary>
    /// InsertByProc
    /// </summary>
    /// <param name="p_sProcName"></param>
    /// <param name="dtbl"></param>
    /// <param name="Parameters"></param>
    /// <returns></returns>
    public static DbCommand InsertByProc(string p_sProcName, out DataTable dtbl, params IDataParameter[] Parameters)
    {
        using (DbCommand Cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            Cmd.Connection = Connection;
            Cmd.CommandTimeout = 10000000;
            Cmd.CommandText = p_sProcName;
            Cmd.CommandType = CommandType.StoredProcedure;
            if (Parameters != null)
            {
                for (int iParamCount = 0; iParamCount < Parameters.Length; iParamCount++)
                {
                    Cmd.Parameters.Add(Parameters[iParamCount]);
                }
            }

            dtbl = GetDataTable(Cmd);
            return Cmd;
        }
    }

    /// <summary>
    /// UpdateByProc
    /// </summary>
    /// <param name="p_sProcName"></param>
    /// <param name="dtbl"></param>
    /// <param name="Parameters"></param>
    public static void UpdateByProc(string p_sProcName, out DataTable dtbl, params IDataParameter[] Parameters)
    {
        using (DbCommand Cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            Cmd.Connection = Connection;
            Cmd.CommandTimeout = 10000000;
            Cmd.CommandText = p_sProcName;
            Cmd.CommandType = CommandType.StoredProcedure;
            if (Parameters != null)
            {
                for (int iParamCount = 0; iParamCount < Parameters.Length; iParamCount++)
                {
                    Cmd.Parameters.Add(Parameters[iParamCount]);
                }
            }

            dtbl = GetDataTable(Cmd);
        }
    }

    /// <summary>
    /// UpdateByProc
    /// </summary>
    /// <param name="p_sProcName"></param>
    /// <param name="Parameters"></param>
    public static void UpdateByProc(string p_sProcName, params IDataParameter[] Parameters)
    {
        using (DbCommand Cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            Cmd.Connection = Connection;
            Cmd.CommandText = p_sProcName;
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.CommandTimeout = 10000000;
            if (Parameters != null)
            {
                for (int iParamCount = 0; iParamCount < Parameters.Length; iParamCount++)
                {
                    Cmd.Parameters.Add(Parameters[iParamCount]);
                }

                Cmd.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// UpdateByQuery
    /// </summary>
    /// <param name="p_sSqlQuery"></param>
    /// <param name="Parameters"></param>
    public static void UpdateByQuery(string p_sSqlQuery, params IDataParameter[] Parameters)
    {
        using (DbCommand Cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            Cmd.Connection = Connection;
            Cmd.CommandText = p_sSqlQuery;
            Cmd.CommandType = CommandType.Text;
            Cmd.CommandTimeout = 10000000;
            if (Parameters != null)
            {
                for (int iParamCount = 0; iParamCount < Parameters.Length; iParamCount++)
                {
                    Cmd.Parameters.Add(Parameters[iParamCount]);
                }

                Cmd.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// DeleteByProc
    /// </summary>
    /// <param name="p_sProcName"></param>
    /// <param name="Parameters"></param>
    public static void DeleteByProc(string p_sProcName, params IDataParameter[] Parameters)
    {
        using (DbCommand cmd = factory.CreateCommand()!)
        {
            DbConnection Connection = DbConnectionScope.Current.GetOpenConnection(factory, connectionString, false);
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            cmd.Connection = Connection;
            cmd.CommandText = p_sProcName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10000000;
            if (Parameters != null)
            {
                for (int iParamCount = 0; iParamCount < Parameters.Length; iParamCount++)
                {
                    cmd.Parameters.Add(Parameters[iParamCount]);
                }
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static TransactionScope GetNewTransactionScope(TransactionScopeOption tso)
    {
        return new TransactionScope(tso, new TimeSpan(2, 0, 0));
    }

    #endregion

    #region DataField Convert Function

    /// <summary>
    /// GetString
    /// </summary>
    /// <param name="p_obj"></param>
    /// <returns></returns>
    public static string GetString(object p_obj)
    {
        return p_obj == DBNull.Value ? string.Empty : (string)p_obj;
    }

    /// <summary>
    /// GetDateStringMM
    /// </summary>
    /// After fetching row from database, to set entity, this function is used. 
    /// <param name="p_obj"></param>
    /// <returns></returns>
    public static string GetDateStringMM(object p_obj)
    {
        if (p_obj.Equals(DBNull.Value))
        {
            return string.Empty;
        }
        else
        {
            return ((DateTime)p_obj).ToString("dd-MM-yyyy");
        }
    }

    /// <summary>
    /// GetDateString
    /// </summary>
    /// <param name="p_obj"></param>
    /// <returns></returns>
    public static string GetDateString(object p_obj)
    {
        if (p_obj.Equals(DBNull.Value))
        {
            return string.Empty;
        }
        else
        {
            return ((DateTime)p_obj).ToString("dd-MMM-yyyy, ddd, hh:mm tt");
        }
    }

    /// <summary>
    /// GetDateTimeByFormat by DateTime format
    /// </summary>
    /// <param name="p_obj">DateTime object</param>
    /// <param name="p_format">DateTime format string</param>
    /// <returns></returns>
    public static string GetDateTimeByFormat(object p_obj, string p_format)
    {
        if (p_obj.Equals(DBNull.Value))
        {
            return string.Empty;
        }
        else
        {
            return ((DateTime)p_obj).ToString(p_format, CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// GetInt
    /// </summary>
    /// <param name="p_obj"></param>
    /// <returns></returns>
    public static int GetInt(object p_obj)
    {
        int iReasult;
        return int.TryParse(Convert.ToString(p_obj), out iReasult) ? iReasult : 0;
    }

    /// <summary>
    /// GetBoolean
    /// </summary>
    /// <param name="p_obj"></param>
    /// <returns></returns>
    public static bool GetBoolean(object p_obj)
    {
        if (p_obj is null) return false;
        var str = p_obj.ToString();
        if (string.IsNullOrEmpty(str)) return false;
        return (str.Equals("1") || str.Equals("T", StringComparison.OrdinalIgnoreCase) || str.Equals("TRUE", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// GetDouble
    /// </summary>
    /// <param name="p_obj"></param>
    /// <returns></returns>
    public static double GetDouble(object p_obj)
    {
        double dReasult;
        return double.TryParse(Convert.ToString(p_obj), out dReasult) ? dReasult : 0.00;
    }

    /// <summary>
    /// GetDecimal
    /// </summary>
    /// <param name="p_obj"></param>
    /// <returns></returns>
    public static decimal GetDecimal(object p_obj)
    {
        decimal dReasult;
        return decimal.TryParse(Convert.ToString(p_obj), out dReasult) ? dReasult : decimal.Zero;
    }

    /// <summary>
    /// GetFloat
    /// </summary>
    /// <param name="p_obj"></param>
    /// <returns></returns>
    public static float GetFloat(object p_obj)
    {
        float iReasult;
        return float.TryParse(Convert.ToString(p_obj), out iReasult) ? iReasult : 0;
    }

    /// <summary>
    /// Replace
    /// </summary>
    /// <param name="p_sString"></param>
    /// <returns></returns>
    public static string Replace(string p_sString)
    {
        return p_sString.Replace("'", "''").Trim();
    }

    #endregion
    public static string GetConnectionString()
    {
        if (AppSettingFile.Get("IsEncryptedConnString").Equals("Y", StringComparison.OrdinalIgnoreCase))
        {
            return Security.AcDec(AppSettingFile.GetConnection("ConnectionString") ?? "");
        }
        else
        {
            return AppSettingFile.GetConnection("ConnectionString") ?? "";
        }
    }
}
