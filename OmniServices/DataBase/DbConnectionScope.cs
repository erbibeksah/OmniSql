namespace DataBase;

/// <summary>
/// Options for modifying how DbConnectionScope.Current is affected while constructing a new scope.
/// </summary>
public enum DbConnectionScopeOption
{
    Required, // Set self as currentScope if there isn't one already on the thread, otherwise don't do anything.
    RequiresNew, // Push self as currentScope (track prior scope and restore it on dispose).
    Suppress, // Push null reference as currentScope (track prior scope and restore it on dispose).
}

/// <summary>
/// Class to assist in managing connection lifetimes inside scopes on a particular thread.
/// </summary>
public sealed class DbConnectionScope : IDisposable
{
    #region class fields

    [ThreadStatic()] private static DbConnectionScope __currentScope = null;
    // Scope that is currently active on this thread

    private static Object __nullKey = new Object(); // used to allow null as a key

    #endregion

    #region instance fields

    private DbConnectionScope _priorScope; // previous scope in stack of scopes on this thread
    private Dictionary<object, DbConnection> _connections; // set of connections contained by this scope.
    private bool _isDisposed; // 

    #endregion

    #region public class methods and properties

    /// <summary>
    /// Obtain the currently active connection scope
    /// </summary>
    public static DbConnectionScope Current
    {
        get { return __currentScope; }
    }

    #endregion

    #region public instance methods and properties

    /// <summary>
    /// Default Constructor
    /// </summary>
    public DbConnectionScope() : this(DbConnectionScopeOption.Required)
    {
    }

    /// <summary>
    /// Constructor with options
    /// </summary>
    /// <param name="option">Option for how to modify Current during constructor</param>
    public DbConnectionScope(DbConnectionScopeOption option)
    {
        _isDisposed = true; // short circuit Dispose until we're properly set up

        bool mustPush; // do we need to change __currentScope?
        bool pushNull; // should we set __currentScope to NULL or this?
        switch (option)
        {
            case DbConnectionScopeOption.Required:
                if (null == __currentScope)
                {
                    mustPush = true;
                    pushNull = false;
                }
                else
                {
                    mustPush = false;
                    pushNull = false;
                }
                break;
            case DbConnectionScopeOption.RequiresNew:
                mustPush = true;
                pushNull = false;
                break;

            case DbConnectionScopeOption.Suppress:
                mustPush = true;
                pushNull = true;
                break;

            default:
                throw new ArgumentOutOfRangeException("option");
        }

        if (mustPush)
        {
            // only bother allocating dictionary if we're going to push
            _connections = new Dictionary<object, DbConnection>();
            _priorScope = __currentScope;
            _isDisposed = false;
            if (pushNull)
            {
                __currentScope = null;
            }
            else
            {
                __currentScope = this;
            }
        }
    }

    /// <summary>
    /// Convenience constructor to add an initial connection
    /// </summary>
    /// <param name="key">Key to associate with connection</param>
    /// <param name="connection">Connection to add</param>
    public DbConnectionScope(object key, DbConnection connection) : this()
    {
        AddConnection(key, connection);
    }

    /// <summary>
    /// Add a connection and associate it with the given key
    /// </summary>
    /// <param name="key">Key to associate with the connection</param>
    /// <param name="connection">Connection to add</param>
    public void AddConnection(object key, DbConnection connection)
    {
        CheckDisposed();
        if (null == key)
        {
            key = __nullKey;
        }
        _connections[key] = connection;
    }

    /// <summary>
    /// Check to see if there is a connection associated with this key
    /// </summary>
    /// <param name="key">Key to use for lookup</param>
    /// <returns>true if there is a connection, false otherwise</returns>
    public bool ContainsKey(object key)
    {
        CheckDisposed();
        return _connections.ContainsKey(key);
    }

    /// <summary>
    /// Shut down this instance.  Disposes all connections it holds and restores the prior scope.
    /// </summary>
    public void Dispose()
    {
        if (!IsDisposed)
        {
            if (__currentScope == this)
            {
                DbConnectionScope prior = _priorScope;
                while (null != prior && prior.IsDisposed)
                {
                    prior = prior._priorScope;
                }
                __currentScope = prior;
            }
            IDictionary<object, DbConnection> connections = _connections;
            _connections = null;
            if (null != connections)
            {
                foreach (DbConnection connection in connections.Values)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    connection.Dispose();
                    GC.Collect();
                }
            }

            _isDisposed = true;
        }
    }

    /// <summary>
    /// Get the connection associated with this key. Throws if there is no entry for the key.
    /// </summary>
    /// <param name="key">Key to use for lookup</param>
    /// <param name="searchPriorScopes">Search for connection throughout the current scope stack?</param>
    /// <returns>Associated connection</returns>
    public DbConnection GetConnection(object key, bool searchPriorScopes)
    {
        CheckDisposed();
        DbConnection returnValue = null;

        // allow null-ref as key
        if (null == key)
        {
            key = __nullKey;
        }

        if (!_connections.TryGetValue(key, out returnValue))
        {
            if (searchPriorScopes && null != _priorScope)
            {
                returnValue = _priorScope.GetConnection(key, searchPriorScopes);
            }
            else
            {
                returnValue = _connections[key]; // we alread know it's not there so force the exception
            }
        }

        return returnValue;
    }

    /// <summary>
    /// Get the connection associated with this key in this scope. Throws if there is no entry for the key.
    /// </summary>
    /// <param name="key">Key to use for lookup</param>
    /// <returns>Associated connection</returns>
    public DbConnection GetConnection(object key)
    {
        return GetConnection(key, false);
    }

    /// <summary>
    /// This method gets the connection using the connection string as a key.  If no connection is
    /// associated with the string, the connection factory is used to create the connection.
    /// Finally, if the resulting connection is in the closed state, it is opened.
    /// </summary>
    /// <param name="factory">Factory to use to create connection if it is not already present</param>
    /// <param name="connectionString">Connection string to use</param>
    /// <returns>Connection in open state</returns>
    public DbConnection GetOpenConnection(DbProviderFactory factory, string connectionString, bool searchPriorScopes)
    {
        CheckDisposed();
        object key;

        // allow null-ref as key
        if (null == connectionString)
        {
            key = __nullKey;
        }
        else
        {
            key = connectionString;
        }

        // go get the connection
        DbConnection result;
        if (!TryGetConnection(key, searchPriorScopes, out result))
        {
            // didn't find it, so create it.
            result = factory.CreateConnection()!;
            result.ConnectionString = connectionString;
            _connections[key] = result;
        }

        // however we got it, open it if it's closed.
        //  note: don't open unless state is unambiguous that it's ok to open
        if (ConnectionState.Closed == result.State)
        {
            result.Open();
        }

        return result;
    }

    /// <summary>
    /// Get the connection associated with this key.
    /// </summary>
    /// <param name="key">Key to use for lookup</param>
    /// <param name="searchPriorScopes">Search for connection throughout the current scope stack?</param>
    /// <param name="connection">Associated connection</param>
    /// <returns>True if connection found, false otherwise</returns>
    public bool TryGetConnection(object key, bool searchPriorScopes, out DbConnection connection)
    {
        CheckDisposed();
        connection = null;

        // allow null-ref as key
        if (null == key)
        {
            key = __nullKey;
        }

        bool found = _connections.TryGetValue(key, out connection);
        if (!found)
        {
            if (searchPriorScopes && null != _priorScope)
            {
                found = _priorScope.TryGetConnection(key, searchPriorScopes, out connection);
            }
        }

        return found;
    }

    /// <summary>
    /// Get the connection associated with this key without searching current scope stack.
    /// </summary>
    /// <param name="key">Key to use for lookup</param>
    /// <param name="connection">Associated connection</param>
    /// <returns>True if connection found, false otherwise</returns>
    public bool TryGetConnection(object key, out DbConnection connection)
    {
        return TryGetConnection(key, false, out connection);
    }

    #endregion

    #region private methods and properties

    /// <summary>
    /// Was this instance previously disposed?
    /// </summary>
    private bool IsDisposed
    {
        get { return _isDisposed; }
    }

    /// <summary>
    /// Handle calling API function after instance has been disposed
    /// </summary>
    private void CheckDisposed()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException("DbConnectionScope");
        }
    }

    #endregion
}
