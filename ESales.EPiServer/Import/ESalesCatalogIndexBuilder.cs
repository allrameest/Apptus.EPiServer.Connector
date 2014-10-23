using System;
using System.Linq;
using Apptus.ESales.EPiServer.Config;
using Apptus.ESales.EPiServer.DataAccess;
using Apptus.ESales.EPiServer.Extensions;
using Apptus.ESales.EPiServer.Import.Ads;
using Apptus.ESales.EPiServer.Import.Configuration;
using Apptus.ESales.EPiServer.Import.Formatting;
using Apptus.ESales.EPiServer.Import.Products;
using Apptus.ESales.EPiServer.Resources;
using Apptus.ESales.EPiServer.Util;
using Autofac;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Search;

namespace Apptus.ESales.EPiServer.Import
{
    /// <summary>
    /// A catalog index builder that can be used for eSales.
    /// </summary>
    public class ESalesCatalogIndexBuilder : ISearchIndexBuilder, IIndexLogger
    {
        private static readonly object BuildKey = new object();
        private static DateTime _lastRun = DateTime.MinValue;
        private IContainer _container;

        /// <summary>
        /// Default constructor, used by the provider system.
        /// </summary>
        public ESalesCatalogIndexBuilder()
        {
            BuildResources();
        }

        /// <summary>
        /// Internal manager.
        /// </summary>
        public SearchManager Manager { get; set; }

        /// <summary>
        /// Internal indexer.
        /// </summary>
        public IndexBuilder Indexer { get; set; }

        /// <summary>
        /// Build product data, in an eSales-friendly format, and send it to eSales.
        /// </summary>
        /// <param name="rebuild">True if a full rebuild should be performed.</param>
        public void BuildIndex( bool rebuild )
        {
            var currentTime = DateTime.Now;
            lock ( BuildKey )
            {
                if ( currentTime <= _lastRun )
                {
                    return;
                }

                using ( var scope = _container.BeginLifetimeScope() )
                {
                    var indexSystem = scope.Resolve<IIndexSystemMapper>();

                    var incremental = !rebuild && indexSystem.LastBuildDate != DateTime.MinValue;

                    scope.Resolve<ESalesIndexBuilder>( new NamedParameter( "incremental", incremental ) ).Run();
                }

                _lastRun = DateTime.Now;
            }
        }

        public event SearchIndexHandler SearchIndexMessage;

        public bool UpdateIndex(System.Collections.Generic.IEnumerable<int> itemIds)
        {
            return true;
        }

        public void Log( string message, double percent, params object[] args )
        {
            if (SearchIndexMessage != null)
            {
                SearchIndexMessage( this, new SearchIndexEventArgs( string.Format( message, args ), percent ) );
            }
        }

        private void BuildResources()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<CatalogSystemMapper>().As<ICatalogSystemMapper>().InstancePerLifetimeScope();
            builder.Register( c => Indexer ).InstancePerLifetimeScope();
            builder.Register( c => this ).As<IIndexLogger>().InstancePerLifetimeScope();
            builder.RegisterType<IndexSystemMapper>().As<IIndexSystemMapper>().InstancePerLifetimeScope();
            builder.RegisterType<MetaDataMapper>().As<IMetaDataMapper>().InstancePerLifetimeScope();
            builder.RegisterType<PriceServiceMapper>().As<IPriceServiceMapper>().InstancePerLifetimeScope();
            builder.RegisterType<AdAttributeHelper>().InstancePerLifetimeScope();
            builder.RegisterType<Configuration.Configuration>().As<IConfiguration>().InstancePerLifetimeScope();
            builder.Register( c => new ConfigurationOptions( Loader.LoadBaseConfiguration() ) ).InstancePerLifetimeScope();
            builder.RegisterType<AttributeConverter>().InstancePerLifetimeScope();
            builder.RegisterType<ConfigurationWriter>().InstancePerLifetimeScope();
            builder.RegisterType<OperationsWriter>().As<IOperationsWriter>();
            builder.RegisterType<AppConfig>().As<IAppConfig>().InstancePerLifetimeScope();
            builder.RegisterType<EntryConverter>().InstancePerLifetimeScope();
            builder.Register( c =>
                {
                    var conf = c.Resolve<IAppConfig>();
                    return conf.UseCodeAsKey ? (IKeyLookup) new CodeKeyLookup( c.Resolve<ConnectorHelper>() ) : new IdKeyLookup();
                } ).InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes( typeof( IIndexBuilder ).Assembly ).As<IIndexBuilder>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes( typeof( IIndexImporter ).Assembly ).As<IIndexImporter>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes( typeof( IIndexDeleter ).Assembly ).As<IIndexDeleter>().InstancePerLifetimeScope();
            builder.RegisterType<FileSystem>().As<IFileSystem>().InstancePerLifetimeScope();
            builder.RegisterType<FileHelper>().InstancePerLifetimeScope();
            builder.RegisterType<ConnectorHelper>().InstancePerLifetimeScope();
            builder.RegisterType<ConnectorMapper>().As<IConnectorMapper>().InstancePerLifetimeScope();
            builder.Register(
                c => new PromotionDataTableMapper(
                         PromotionManager.GetPromotionDto().PromotionLanguage,
                         PromotionManager.GetPromotionDto().Promotion,
                         CampaignManager.GetCampaignDto().Campaign ) ).InstancePerLifetimeScope();
            builder.RegisterType<PromotionEntryCodeProvider>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes( typeof( IFormatRule ).Assembly ).As<IFormatRule>().InstancePerLifetimeScope();
            builder.RegisterType<Formatter>().InstancePerLifetimeScope();
            builder.RegisterType<ESalesIndexBuilder>().InstancePerLifetimeScope();
            builder.RegisterType<UrlResolver>().As<IUrlResolver>();
            builder.RegisterType<EntryAdditionalData>().As<IEntryAdditionalData>();

            RegisterPlugins( builder );

            try
            {
                _container = builder.Build();
            }
            catch ( Exception e )
            {
                throw e.Unwrap();
            }
        }

        private static void RegisterPlugins(ContainerBuilder builder)
        {
            var assemblies = AssemblyProvider.GetAssemblies().ToArray();
            builder.RegisterAssemblyTypes(assemblies).As<IPromotionEntryCodes>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(assemblies).As<IProductConverter>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(assemblies).As<IBatchedProductConverter>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(assemblies).As<IAdConverter>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(assemblies).As<IProductsAppender>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(assemblies).As<IAdsAppender>().InstancePerLifetimeScope();
        }
    }
}
