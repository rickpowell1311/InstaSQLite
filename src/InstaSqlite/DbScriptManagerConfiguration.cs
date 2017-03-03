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

            foreach (var existingScript in Scripts)
            {
                if (existingScript.Id == script.Id)
                {
                    if (script.GetType().GUID != existingScript.GetType().GUID)
                    {
                        throw new ArgumentException(string.Format("Multiple scripts detected with Id of '{0}'", script.Id));
                    }

                    return;
                }
            }

            Scripts.Add(script);
        }
    }
}
