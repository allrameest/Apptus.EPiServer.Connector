using System.IO;
using System.Xml.Serialization;

namespace Apptus.ESales.EPiServer.Resources
{
    internal class Loader
    {
        public static configuration LoadBaseConfiguration()
        {
            return new configuration
                {
                    formats = Load<formats>().format,
                    normalizations = Load<normalizations>().normalization,
                    search_refinements = Load<search_refinements>(),
                    tokenizations = Load<tokenizations>().tokenization
                };
        }

        public static T Load<T>()
        {
            var resourceStream = typeof( Loader ).Assembly.GetManifestResourceStream( typeof( Loader ), typeof( T ).Name + ".xml" );
            return Deserialize<T>( resourceStream );
        }

        public static T Deserialize<T>( Stream resource )
        {
            var serializer = new XmlSerializer( typeof( T ) );
            return (T) serializer.Deserialize( resource );
        }
    }
}