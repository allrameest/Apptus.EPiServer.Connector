using System.IO;
using Apptus.ESales.EPiServer.DataAccess;

namespace Apptus.ESales.EPiServer.Import.Products
{
    internal class ProductIndexImporter : IIndexImporter
    {
        private readonly IIndexSystemMapper _indexSystem;
        private readonly FileHelper _fileHelper;
        private readonly IConnectorMapper _connector;
        private readonly IFileSystem _fileSystem;

        public ProductIndexImporter( IIndexSystemMapper indexSystem, FileHelper fileHelper, IConnectorMapper connector, IFileSystem fileSystem )
        {
            _indexSystem = indexSystem;
            _fileHelper = fileHelper;
            _connector = connector;
            _fileSystem = fileSystem;
        }

        public void Import( bool incremental )
        {
            _indexSystem.Log( "Importing products." );
            using ( var stream = _fileSystem.Open( _fileHelper.ProductsFile, FileMode.Open, FileAccess.Read ) )
            {
                _connector.ImportProducts( stream );
            }
            _indexSystem.Log( "Done importing products." );
        }
    }
}