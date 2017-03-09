using System;
using System.IO;
using System.Security;
using System.Security.AccessControl;

namespace InstaSqlite
{
    public class DbManagerConfiguration
    {
        internal string DbPath { get; private set; }

        internal DbScriptManagerConfiguration DbScriptManagerConfiguration { get; set; }

        public static DbManagerConfiguration Default
        {
            get
            {
                return new DbManagerConfiguration();
            }
        }

        internal DbManagerConfiguration()
        {
            DbPath = Path.Combine(Directory.GetCurrentDirectory(), "database.sqlite");
            DbScriptManagerConfiguration = new DbScriptManagerConfiguration();
        }

        public void SetDatabaseFilePath(DirectoryInfo directory, string fileName = "database.sqlite")
        {
            if (directory == null)
            {
                throw new ArgumentException("Database directory cannot be null");
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("A database file name is required");
            }

            if (!Directory.Exists(directory.FullName))
            {
                throw new ArgumentException("Directory does not exist. Please create the directory to contain the Sqlite database before running");
            }

            if (!HasWriteAccessToFolder(directory.FullName))
            {
                throw new SecurityException("Cannot create SQL database in the specified directory. Write access is denied");
            }

            DbPath = Path.Combine(directory.FullName, fileName);
        }

        public void ConfigureScripts(Action<DbScriptManagerConfiguration> configurationAction)
        {
            configurationAction(DbScriptManagerConfiguration);
        }

        private bool HasWriteAccessToFolder(string folderPath)
        {
            try
            {
                DirectorySecurity ds = Directory.GetAccessControl(folderPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}
