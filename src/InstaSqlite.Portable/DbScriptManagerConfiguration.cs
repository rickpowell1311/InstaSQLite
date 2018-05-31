using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InstaSqlite
{
    public class DbScriptManagerConfiguration
    {
        internal Dictionary<int, IScript> Scripts { get; private set; }

        internal DbScriptManagerConfiguration()
        {
            Scripts = new Dictionary<int, IScript>();
        }

        public void IncludeScript<T>(T script) where T : IScript
        {
            IncludeScript((IScript)script);
        }

        public void IncludeScript(IScript script)
        {
            foreach (var existingScript in Scripts.Values)
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

            Scripts[script.Id] = script;
        }

        public void IncludeScript<T>() where T : class, IScript, new()
        {
            var script = Activator.CreateInstance<T>();

            this.IncludeScript(script);
        }

        public void ScanForScripts(params Assembly[] assemblies)
        {
            var scriptTypes = assemblies
                .SelectMany(a => a.GetTypes().Where(
                    t => t.GetInterfaces().Any(i => i == typeof(IScript))
                    && t.IsClass
                    && t.GetConstructors().Any(c => !c.GetParameters().Any())));

            foreach (var scriptType in scriptTypes)
            {
                var script = (IScript)Activator.CreateInstance(scriptType);
                this.IncludeScript(script);
            }
        }
    }
}
