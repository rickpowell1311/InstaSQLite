using System;
using Xunit;

namespace InstaSqlite.Tests
{
    public class DbScriptManagerConfigurationTests
    {
        [Fact]
        public void DbScriptManagerConfiguration_WithDuplicateScriptIds_ThrowsInvalidScriptsConfigurationException()
        {
            var dbManager = new DbManager(config =>
            {
                config.ConfigureScripts(s =>
                {
                    s.IncludeScript<ValidTestScript>();
                    s.IncludeScript<DuplicateOfValidTestScript>();
                });
            });

            Assert.Throws<ArgumentException>(() => { using (var database = dbManager.Database()) { } });
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
                    return "CREATE TABLE IF NOT EXISTS Test((id INTERGER PRIMARY KEY)";
                }
            }
        }

        private class DuplicateOfValidTestScript : IScript
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
                    return "CREATE TABLE IF NOT EXISTS Test((id INTERGER PRIMARY KEY)";
                }
            }
        }
    }
}
