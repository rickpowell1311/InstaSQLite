using Dapper.Contrib.Extensions;

namespace InstaSqlite
{
    [Table("DbVersion")]
    public class ExecutedScript
    {
        [ExplicitKey]
        public int Id { get; set; }
    }
}
