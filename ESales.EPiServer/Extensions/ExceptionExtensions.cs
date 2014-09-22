using System;
using System.Linq;
using System.Reflection;

namespace Apptus.ESales.EPiServer.Extensions
{
    internal static class ExceptionExtensions
    {
        public static Exception Unwrap( this Exception e )
        {
            var rtle = e as ReflectionTypeLoadException;
            return ( rtle != null ? ( rtle.LoaderExceptions.FirstOrDefault() ?? rtle ) : e ).GetBaseException();
        }
    }
}
