using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaSqlite.ScannedAssembly
{
    public class ScannedScript : IScript
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
