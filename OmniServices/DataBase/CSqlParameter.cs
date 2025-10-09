namespace DataBase;
public sealed class CSqlParameter
{
    public static SqlParameter CreateParameter(string p_sName, SqlDbType p_Type, object p_oValue)
    {
        SqlParameter param = new SqlParameter();

        param.ParameterName = p_sName;
        param.SqlDbType = p_Type;
        param.Value = p_oValue;
        return param;
    }

    public static SqlParameter CreateParameter(string p_sName, SqlDbType p_Type)
    {
        SqlParameter param = new SqlParameter();
        param.ParameterName = p_sName;
        param.SqlDbType = p_Type;

        return param;
    }

    public static SqlParameter CreateParameter(string p_sName, object p_oValue)
    {
        SqlParameter param = new SqlParameter();
        param.ParameterName = p_sName;
        param.Value = p_oValue;
        return param;
    }

    public static SqlParameter CreateParameter(string p_sName, SqlDbType p_Type, ParameterDirection p_Direction,
                                               object p_oValue)
    {
        SqlParameter param = new SqlParameter();
        param.ParameterName = p_sName;
        param.SqlDbType = p_Type;
        param.Direction = p_Direction;
        param.Value = p_oValue;
        return param;
    }

    public static SqlParameter CreateDecimalParameter(string p_sName, SqlDbType p_Type,
                                                      ParameterDirection p_Direction)
    {
        SqlParameter param = new SqlParameter();
        param.ParameterName = p_sName;
        param.SqlDbType = p_Type;
        param.Precision = 16;
        param.Scale = 2;
        param.Size = 16;
        param.Direction = p_Direction;
        return param;
    }

    public static SqlParameter CreateParameter(string p_sName, SqlDbType p_Type, ParameterDirection p_Direction)
    {
        SqlParameter param = new SqlParameter();
        param.ParameterName = p_sName;
        param.SqlDbType = p_Type;
        if (p_Type == SqlDbType.Int)
        {
            param.Size = 10;
        }
        if (p_Type == SqlDbType.VarChar)
        {
            param.Size = 3000;
        }
        if (p_Type == SqlDbType.Decimal)
        {
            param.Size = 20;
            param.Scale = 2;
        }

        param.Direction = p_Direction;
        return param;
    }

    public static SqlParameter CreateParameter(string p_sName, ParameterDirection p_Direction)
    {
        SqlParameter param = new SqlParameter();
        param.ParameterName = p_sName;
        param.Direction = p_Direction;
        return param;
    }
}
