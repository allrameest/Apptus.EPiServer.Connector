using System;
using System.Globalization;
using Apptus.ESales.EPiServer.Import;
using Mediachase.Search;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class IndexSystemMapper : IIndexSystemMapper
    {
        private readonly IndexBuilder _indexer;
        private readonly IIndexLogger _logger;
        private double _percent;

        public IndexSystemMapper( IndexBuilder indexer, IIndexLogger logger )
        {
            _indexer = indexer;
            _logger = logger;
        }

        public DateTime CurrentBuildDate
        {
            get { return _indexer.BuildDate; }
        }

        public void SetBuildProperties( int firstCatalogEntryId, int lastCatalogEntryId, string catalogName )
        {
            _indexer.GetBuildConfig().Properties = new[]
                {
                    new BuildProperty { name = "StartingRecord", value = firstCatalogEntryId.ToString( CultureInfo.InvariantCulture ) },
                    new BuildProperty { name = "LastRecordKey", value = lastCatalogEntryId.ToString( CultureInfo.InvariantCulture ) },
                    new BuildProperty { name = "CatalogName", value = catalogName }
                };
        }

        public void SaveBuild( Status status )
        {
            _indexer.SaveBuild( status );
        }

        public void Log( string message, params object[] args )
        {
            _logger.Log( message, _percent, args );
        }

        public void Log( string message, double percent, params object[] args )
        {
            _percent = percent;
            _logger.Log( message, percent, args );
        }

        public DateTime LastBuildDate
        {
            get
            {
                var lastBuildDate = _indexer.GetBuildConfig().LastBuildDate;
                return lastBuildDate;
            }
        }
    }
}