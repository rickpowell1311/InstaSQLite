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

        public void IncludeScript<T>(T script) where T : IScript
        {
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

        public void IncludeScript<T>() where T : class, IScript, new()
        {
            var script = Activator.CreateInstance<T>();

            this.IncludeScript(script);
        }
    }
}
