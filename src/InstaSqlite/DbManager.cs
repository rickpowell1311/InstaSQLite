using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;

namespace InstaSqlite
{
    public class DbManager : IDbManager
    {
        public static bool initialized;

        private Func<IDbConnection> database;
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
                if (!initialized)
                {
                    try
                    {
                        configurationAction?.Invoke(Configuration);
                        database = Initialize(Configuration);
                    }
                    catch (Exception ex)
                    {
                        database = () => { throw ex; };
                    }
                }

                return database;
            }
        }

        private Func<IDbConnection> Initialize(DbManagerConfiguration configurationAction)
        {
            using (var conn = new SQLiteConnection(string.Format("Data Source={0};", Configuration.DbPath)))
            {
                conn.Open();

                conn.Execute("CREATE TABLE IF NOT EXISTS DbVersion(id INTERGER PRIMARY KEY)");
                var executedScriptIds = conn.GetAll<ExecutedScript>().Select(scr => scr.Id);

                foreach (var script in Configuration.DbScriptManagerConfiguration.Scripts
                    .Where(scr => !executedScriptIds.Contains(scr.Id))
                    .OrderBy(scr => scr.Id))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            conn.Execute(script.Sql, transaction: transaction);
                            conn.Insert(new ExecutedScript { Id = script.Id });
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            var exception = new Exception(string.Format("Whilst trying to apply script  with Id '{0}'", script.Id), ex);

                            // This ensures any further calls to the database are met with exception until script is amended
                            return () => { throw exception; };
                        }
                    }
                }
            }

            return () =>
            {
                var conn = new SQLiteConnection(string.Format("Data Source={0};", Configuration.DbPath));
                conn.Open();
                return conn;
            };
        }
    }
}
