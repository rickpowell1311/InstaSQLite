using System.IO;

namespace InstaSqlite
{
    public static class DbManagerDefaults
    {
        public static string DefaultDbPath
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "database.sqlite");
            }
        }
    }
}
