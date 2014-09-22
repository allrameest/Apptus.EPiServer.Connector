using System;
using Apptus.ESales.EPiServer.Config;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class FileHelper
    {
        private readonly IAppConfig _appConfig;
        private readonly DateTime _updateId;

        public FileHelper( IAppConfig appConfig )
        {
            _appConfig = appConfig;
            _updateId = DateTime.Now;
        }

        public string AdsFile
        {
            get { return string.Format( @"{0}\Export\ads_{1}.xml", _appConfig.AppDataPath, _updateId.ToString( "yyyyMMdd_HHmmss" ) ); }
        }

        public string ProductsFile
        {
            get { return string.Format( @"{0}\Export\products_{1}.xml", _appConfig.AppDataPath, _updateId.ToString( "yyyyMMdd_HHmmss" ) ); }
        }

        public string ProductsAddFile
        {
            get { return string.Format( @"{0}\Export\products_{1}_add.xml", _appConfig.AppDataPath, _updateId.ToString( "yyyyMMdd_HHmmss" ) ); }
        }

        public string ProductsDelFile
        {
            get { return string.Format( @"{0}\Export\products_{1}_del.xml", _appConfig.AppDataPath, _updateId.ToString( "yyyyMMdd_HHmmss" ) ); }
        }

        public string ConfigurationFile
        {
            get { return string.Format( @"{0}\Export\configuration_{1}.xml", _appConfig.AppDataPath, _updateId.ToString( "yyyyMMdd_HHmmss" ) ); }
        }
    }
}