using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Apptus.ESales.EPiServer.Util;
using Mediachase.Commerce.Catalog.Dto;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class CodeKeyLookup : IKeyLookup
    {
        private readonly Connector.Connector _connector;

        public CodeKeyLookup( ConnectorHelper connectorHelper )
        {
            _connector = connectorHelper.GetConnector();
        }

        public string Value( int id, string language )
        {
            Exception inner = null;
            try
            {
                var session = _connector.Session( "TEST" );
                var content = session
                    .Panel( "/key-lookup" )
                    .RetrieveContent( new Dictionary<string, string>
                        {
                            { "id", id.ToString( CultureInfo.InvariantCulture ) },
                            { "locale_filter", language.ToESalesLocale() },
                            { "notify", "false" }
                        } );
                session.End();
                if ( content.HasResult )
                {
                    var product = content.ResultAsProducts().FirstOrDefault();
                    if ( product != null )
                    {
                        return product.VariantList.Any() ? product.VariantList.First().Key : product.Key;
                    }
                }
            }
            catch ( Exception e )
            {
                inner = e;
            }
            throw new KeyNotFoundException( "Id " + id + " not found. Please do a full rebuild.", inner );
        }

        public string Value( CatalogEntryDto.CatalogEntryRow entry, string language )
        {
            return AttributeHelper.CreateKey( entry.Code, language );
        }
    }
}