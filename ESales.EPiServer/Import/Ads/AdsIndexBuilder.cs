using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Apptus.ESales.EPiServer.Config;
using Apptus.ESales.EPiServer.DataAccess;

namespace Apptus.ESales.EPiServer.Import.Ads
{
    internal class AdsIndexBuilder : IIndexBuilder
    {
        private readonly IAppConfig _appConfig;
        private readonly IFileSystem _fileSystem;
        private readonly FileHelper _fileHelper;
        private readonly PromotionDataTableMapper _dataTableMapper;
        private readonly PromotionEntryCodeProvider _promotionEntryCodeProvider;
        private readonly IIndexSystemMapper _indexSystem;
        private readonly IAdConverter _converterPlugin;
        private readonly IReadOnlyCollection<IAdsAppender> _appenderPlugins;

        public AdsIndexBuilder(
            IAppConfig appConfig, IFileSystem fileSystem, FileHelper fileHelper, PromotionDataTableMapper dataTableMapper,
            PromotionEntryCodeProvider promotionEntryCodeProvider, IIndexSystemMapper indexSystem,
            IEnumerable<IAdsAppender> appenderPlugins, IAdConverter converterPlugin = null)
        {
            _appConfig = appConfig;
            _fileSystem = fileSystem;
            _fileHelper = fileHelper;
            _dataTableMapper = dataTableMapper;
            _promotionEntryCodeProvider = promotionEntryCodeProvider;
            _indexSystem = indexSystem;
            _converterPlugin = converterPlugin;
            _appenderPlugins = appenderPlugins.ToArray();
        }

        public void Build(bool incremental)
        {
            if (_appConfig.EnableAds)
            {
                _indexSystem.Log("Extracting ads.");

                var ads = Enumerable.Empty<IEntity>();
                if (!incremental)
                {
                    ads = new AdAttributeHelper().ConvertToAds(_dataTableMapper, _promotionEntryCodeProvider);
                }
                ads = UsePlugins(ads, incremental);

                var allAds = ads.ToArray();
                if (allAds.Any() || !incremental)
                {
                    var adsXml = new XElement("operations",
                                           new XElement("clear",
                                                         new XElement("ad")),
                                           new XElement("add",
                                                         ConvertAdsToXml(allAds)));
                    using (var file = _fileSystem.Open(_fileHelper.AdsFile, FileMode.CreateNew, FileAccess.Write))
                    {
                        adsXml.Save(file);
                    }
                }
                _indexSystem.Log("Done extracting ads.");
            }
        }

        private IEnumerable<IEntity> UsePlugins(IEnumerable<IEntity> ads, bool incremental)
        {
            if (_converterPlugin != null)
            {
                ads = ads.Select(ad => _converterPlugin.Convert(ad));
            }

            var adsToAppend = _appenderPlugins.SelectMany(a => a.Append(incremental));
            return ads.Concat(adsToAppend);
        }

        private static IEnumerable<XElement> ConvertAdsToXml( IEnumerable<IEntity> ads )
        {
            return ads.Select( ad => new XElement( "ad", ad.Select( attr => new XElement( attr.Name, attr.Value ) ) ) );
        }
    }
}