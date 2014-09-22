using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;

namespace Apptus.ESales.EPiServer.Util
{
    internal static class AssemblyProvider
    {
        public static IEnumerable<Assembly> GetAssemblies()
        {
            try
            {
                return BuildManager.GetReferencedAssemblies().Cast<Assembly>();
            }
            catch ( Exception )
            {
                return Directory.GetFiles( AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.AllDirectories ).Select( Assembly.LoadFrom );
            }
        }
    }
}
