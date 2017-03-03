using System;
using System.Collections.Generic;
using System.Linq;

namespace InstaSqlite
{
    public class DbScriptManagerConfiguration
    {
        internal List<IScript> Scripts { get; private set; }

        internal DbScriptManagerConfiguration()
        {
            Scripts = new List<IScript>();
        }

        public void IncludeScript<T>() where T : IScript, new()
        {
            var script = Activator.CreateInstance<T>();

            if (Scripts.Select(scr => scr.Id).Contains(script.Id))
            {
                throw new ArgumentException(string.Format("Multiple scripts detected with Id of '{0}'", script.Id));
            }

            Scripts.Add(script);
        }
    }
}
