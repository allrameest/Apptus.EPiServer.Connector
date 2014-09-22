using System.IO;
using Apptus.ESales.EPiServer.Config;
using Apptus.ESales.EPiServer.DataAccess;

namespace Apptus.ESales.EPiServer.Import.Products
{
    internal class ProductIndexDeleter : IIndexDeleter
    {
        private readonly IIndexSystemMapper _indexSystem;
        private readonly IAppConfig _configuration;
        private readonly FileHelper _fileHelper;

        public ProductIndexDeleter( IIndexSystemMapper indexSystem, IAppConfig configuration, FileHelper fileHelper )
        {
            _indexSystem = indexSystem;
            _configuration = configuration;
            _fileHelper = fileHelper;
        }

        public void Delete( bool incremental )
        {
            if ( !_configuration.KeepDataFiles )
            {
                File.Delete( _fileHelper.ProductsFile );
                _indexSystem.Log( "Deleted temporary products file." );
            }
        }
    }
}