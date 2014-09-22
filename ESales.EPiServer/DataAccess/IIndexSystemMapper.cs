using System;
using Mediachase.Search;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal interface IIndexSystemMapper
    {
        DateTime LastBuildDate { get; }
        DateTime CurrentBuildDate { get; }
        void SetBuildProperties( int firstCatalogEntryId, int lastCatalogEntryId, string catalogName );
        void SaveBuild( Status status );
        void Log( string message, params object[] args );
        void Log( string message, double percent, params object[] args );
    }
}