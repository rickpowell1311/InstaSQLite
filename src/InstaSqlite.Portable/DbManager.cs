using System;
using System.Data;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;

namespace InstaSqlite
{
    public class DbManager : IDbManager
    {
        internal DbManagerConfiguration Configuration { get; private set; }
        private readonly Action<DbManagerConfiguration> configurationAction;

        public DbManager(Action<DbManagerConfiguration> configurationAction = null)
        {
            Configuration = DbManagerConfiguration.Default;
            this.configurationAction = configurationAction;
        }

        public Func<IDbConnection> Database
        {
            get
            {
                try
                {
                    configurationAction?.Invoke(Configuration);
                    return Initialize();
                }
                catch (Exception ex)
                {
                    return () => { throw ex; };
                }
            }
        }

        private Func<IDbConnection> Initialize()
        {
            using (var conn = new SqliteConnection(string.Format("Data Source={0};", Configuration.DbPath)))
            {
                conn.Open();

                conn.Execute("CREATE TABLE IF NOT EXISTS DbVersion(id INTERGER PRIMARY KEY)");
                var executedScriptIds = conn.GetAll<ExecutedScript>().Select(scr => scr.Id);

                foreach (var script in Configuration.DbScriptManagerConfiguration.Scripts
                    .Where(scr => !executedScriptIds.Contains(scr.Key))
                    .OrderBy(scr => scr.Key))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            conn.Execute(script.Value.Sql, transaction: transaction);
                            conn.Insert(new ExecutedScript { Id = script.Key });
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            var exception = new Exception(string.Format("Whilst trying to apply script  with Id '{0}'", script.Key), ex);

                            // This ensures any further calls to the database are met with exception until script is amended
                            return () => { throw exception; };
                        }
                    }
                }
            }

            return () =>
            {
                var conn = new SqliteConnection(string.Format("Data Source={0};", Configuration.DbPath));
                conn.Open();
                return conn;
            };
        }
    }
}
