using System;
using System.IO;
using Xunit;

namespace InstaSqlite.Tests
{
    public class DbManagerTests
    {
        public void DbManager_WithDefaultConfiguration_DoesNotThrowExceptions()
        {
            var dbManager = new DbManager();
            using (var conn = dbManager.Database())
            {
            }
        }

        [Fact]
        public void DbManager_WithOneValidScript_DoesNotThrowExceptions()
        {
            var dbManager = new DbManager(config =>
            {
                config.ConfigureScripts(s =>
                {
                    s.IncludeScript<ValidTestScript>();
                });
            });

            if (File.Exists(dbManager.Configuration.DbPath))
            {
                File.Delete(dbManager.Configuration.DbPath);
            }

            using (var conn = dbManager.Database())
            {
            }
        }

        [Fact]
        public void DbManager_WithOneValidScript_CalledTwice_DoesNotThrowExceptions()
        {
            var dbManager = new DbManager(config =>
            {
                config.ConfigureScripts(s =>
                {
                    s.IncludeScript<ValidTestScript>();
                });
            });

            using (var conn = dbManager.Database()) { };
            using (var conn = dbManager.Database()) { };
        }

        [Fact]
        public void DbManager_WithOneInvalidScript_ThrowsException()
        {
            var dbManager = new DbManager(config =>
            {
                config.ConfigureScripts(s =>
                {
                    s.IncludeScript<InvalidTestScript>();
                });
            });

            if (File.Exists(dbManager.Configuration.DbPath))
            {
                File.Delete(dbManager.Configuration.DbPath);
            }

            Assert.Throws<Exception>(() => { using (var database = dbManager.Database()) { } });
        }

        private class InvalidTestScript : IScript
        {
            public int Id
            {
                get
                {
                    return 1;
                }
            }

            public string Sql
            {
                get
                {
                    return "NOT EVEN SQL!";
                }
            }
        }

        private class ValidTestScript : IScript
        {
            public int Id
            {
                get
                {
                    return 1;
                }
            }

            public string Sql
            {
                get
                {
                    return "CREATE TABLE IF NOT EXISTS Test(id INTERGER PRIMARY KEY)";
                }
            }
        }
    }
}
