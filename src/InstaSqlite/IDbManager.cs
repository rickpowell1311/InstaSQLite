using System;
using System.Data;

namespace InstaSqlite
{
    public interface IDbManager
    {
        Func<IDbConnection> Database { get; }
    }
}
