### Insta SQLite - SQLite database management in an instance ###

If you're using a SQLite database in your project, manage your database updates in code without ever touching the database anywhere else.

NOTE: Because SQLite interop dependencies are managed separately, you need to add the nuget package System.Data.SQLite.Core to any startup projects that reference Insta SQLite

Create a database customization script:

```
public MyScript : IScript
{
    public int Id { get { return 1; } }

    public string Sql { get { "CREATE TABLE IF NOT EXISTS MyTable(id INTERGER PRIMARY KEY)"; } }
}
```

Configure the Database manager to initialize the database with database customization scripts (and a custom file path if you wish)

```
var dbManager = new DbManager(config =>
{
    config.ConfigureScripts(s =>
    {
        s.IncludeScript<MyScript>();
    });
    config.SetDatabaseFilePath(new DirectoryInfo(@"C:\MyDirectory"), "mydatabase.sqlite");
});
```

Get the standard ADO.NET IDbConnection, and use it (probably with Dapper or another nice micro ORM). Updates will be applied automatically if they have not been already.

```
using (var conn = dbManager.Database())
{
    // Database connection is established and open. Do some queries or something idc.
}
```
