using System;
using System.IO;
using System.Security;
using Xunit;

namespace InstaSqlite.Tests
{
    public class DbManagerConfigurationTests
    {
        [Fact]
        public void SetDatabaseDirectory_AsNull_ShouldThrowArgumentException()
        {
            var manager = new DbManager(config =>
            {
                config.SetDatabaseFilePath(null);
            });

            Assert.Throws<ArgumentException>(() => { using (var database = manager.Database()) { } });
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void SetDatabaseFileName_AsNullOrEmpty_ShouldThrowArgumentException(string fileName)
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());

            var manager = new DbManager(config =>
            {
                config.SetDatabaseFilePath(directory, fileName);
            });

            Assert.Throws<ArgumentException>(() => { using (var database = manager.Database()) { } });
        }

        [Fact]
        public void SetValidDatabaseFileName_AndValidDirectoryWithWriteAccess_DoesNotThrowException()
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());

            var manager = new DbManager(config =>
            {
                config.SetDatabaseFilePath(directory, "testdatabase.sqlite");
            });

            using (var database = manager.Database()) { } ;
        }
    }
}
