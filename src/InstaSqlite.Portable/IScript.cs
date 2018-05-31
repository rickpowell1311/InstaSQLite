namespace InstaSqlite
{
    public interface IScript
    {
        string Sql { get; }

        int Id { get; }
    }
}
