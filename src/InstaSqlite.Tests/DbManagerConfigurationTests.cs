using System;
using System.IO;
using System.Security;
using Xunit;

namespace InstaSqlite.Tests
{
    public class DbManagerConfigurationTests
    {
        [Fact]
        public void SetDatabaseFilePath_ToDirectoryWithoutWriteAccess_ShouldThrowSecurityException()
        {
            var directory = new DirectoryInfo("c:\\");

            var manager = new DbManager(config =>
            {
                config.SetDatabaseFilePath(directory);
            });

            Assert.Throws<SecurityException>(() => { using (var database = manager.Database()) { } });
        }

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
    }
}
