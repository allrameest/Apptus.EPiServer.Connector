using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Apptus.ESales.EPiServer.DataAccess;
using Apptus.ESales.EPiServer.Import.Configuration;
using Apptus.ESales.EPiServer.Util;
using Mediachase.Commerce.Marketing.Dto;

namespace Apptus.ESales.EPiServer.Import.Ads
{
    //Todo: Should I extract this to an AdConverter and an AdAttributeConverter?
    internal class AdAttributeHelper
    {
        private static readonly Dictionary<Type, type> TypeMap = new Dictionary<Type, type>
            {
                { typeof( bool ), type.boolean },
                { typeof( byte ), type.@byte },
                { typeof( char ), type.@string },
                { typeof( DateTime ), type.timepoint },
                { typeof( decimal ), type.@double },
                { typeof( double ), type.@double },
                { typeof( short ), type.@short },
                { typeof( int ), type.@int },
                { typeof( long ), type.@long },
                { typeof( SByte ), type.@byte },
                { typeof( float ), type.@float },
                { typeof( string ), type.@string },
                { typeof( TimeSpan ), type.@long },
                { typeof( ushort ), type.@int },
                { typeof( uint ), type.@long },
                { typeof( ulong ), type.@double },
                { typeof( byte[] ), type.@string }
            };

        private static readonly IEnumerable<Attribute> StaticAttributes = new[] {new Attribute( "live_products", "4" ) };


        internal IEnumerable<ConfigurationAttribute> ConvertToAttributes( PromotionDto.PromotionLanguageDataTable promotionLanguageTable, PromotionDto.PromotionDataTable promotionTable,
                                                                    CampaignDto.CampaignDataTable campaignTable )
        {
            var campaignAttributes = ToAttributesInternal( ToMappedColumnInfos( campaignTable ) );
            var promotionAttributes = ToAttributesInternal( ToMappedColumnInfos( promotionTable ) );
            var promotionLanguageAttributes = ToAttributesInternal( ToMappedColumnInfos( promotionLanguageTable ) );

            return campaignAttributes
                .Where( ca => !promotionAttributes.Select( pa => pa.Name ).Contains( ca.Name ) )
                .Where( ca => !promotionLanguageAttributes.Select( pla => pla.Name ).Contains( ca.Name ) )
                .Concat( promotionAttributes.Where( pa => !promotionLanguageAttributes.Select( pla => pla.Name ).Contains( pa.Name ) ) )
                .Concat( promotionLanguageAttributes );
        }

        internal IEnumerable<IEntity> ConvertToAds( PromotionDataTableMapper promotionDataTableMapper, PromotionEntryCodeProvider promotionEntryCodeProvider )
        {
            var promotionLanguageMappedColumns = ToMappedColumnInfos( promotionDataTableMapper.PromotionLanguageDataTable, promotionEntryCodeProvider ).ToArray();
            var promotionMappedColumns = ToMappedColumnInfos( promotionDataTableMapper.PromotionDataTable ).ToArray();
            var campaignMappedColumns = ToMappedColumnInfos( promotionDataTableMapper.CampaignDataTable ).ToArray();

            foreach ( var plpc in promotionDataTableMapper.Promotions )
            {
                var plpc1 = plpc;
                var plAttributes = promotionLanguageMappedColumns
                    .Select( plmc => new Attribute( plmc.SpecificESalesName, plmc.GetValue( plpc1.PromotionLanguageRow ) ) )
                    .Where( a => !string.IsNullOrWhiteSpace( a.Values.FirstOrDefault() ) );
                var pAttributes = promotionMappedColumns
                    .Select( pmc => new Attribute( pmc.SpecificESalesName, pmc.GetValue( plpc1.PromotionRow ) ) )
                    .Where( a => !string.IsNullOrWhiteSpace( a.Values.FirstOrDefault() ) );
                var cAttributes = campaignMappedColumns
                    .Select( cmc => new Attribute( cmc.SpecificESalesName, cmc.GetValue( plpc1.CampaignRow ) ) )
                    .Where( a => !string.IsNullOrWhiteSpace( a.Values.FirstOrDefault() ) );

                //Unique attribute names. PromotionLanguage is most important, then Promotion and last Campaign.
                yield return new Ad(
                    cAttributes.Where( ca => !pAttributes.Select( pa => pa.Name ).Contains( ca.Name ) ).Where( ca => !plAttributes.Select( pla => pla.Name ).Contains( ca.Name ) )
                        .Concat( pAttributes.Where( pa => !plAttributes.Select( pla => pla.Name ).Contains( pa.Name ) ) )
                        .Concat( plAttributes )
                        .Concat( StaticAttributes ) );
            }
        }


        private static IEnumerable<MappedColumnInfo> ToMappedColumnInfos( CampaignDto.CampaignDataTable table )
        {
            var mappedColumns = new[]
                {
                    new MappedColumnInfo( "Name", "campaign_name", type.@string, new FilterOptions( Format.PipeSeparated, Tokenization.Words ) ),
                    new MappedColumnInfo( "Created", "campaign_created", type.@long, r => ToUtcEpochTime( r, "Created" ) ),
                    new MappedColumnInfo( "Exported", "campaign_exported", type.@long, r => ToUtcEpochTime( r, "Exported" ) ),
                    new MappedColumnInfo( "Modified", "campaign_modified", type.@long, r => ToUtcEpochTime( r, "Modified" ) ),
                    new MappedColumnInfo( "ModifiedBy", "campaign_modified_by", type.@string ),
                    new MappedColumnInfo( "IsActive", "campaign_is_active", type.@string ),
                    new MappedColumnInfo( "IsArchived", "campaign_is_archived", type.@string ),
                    new MappedColumnInfo( "Comments", "campaign_comments", type.@string, new FilterOptions( Format.PipeSeparated, Tokenization.Words ) )
                }.ToLookup( mc => mc.ColumnName, mc => mc );
            var ignoredColumns = new HashSet<string> { "CampaignId", "ApplicationId", "StartDate", "EndDate" };
            return ToMappedColumnInfosInternal( table.Columns.Cast<DataColumn>(), mappedColumns, ignoredColumns );
        }

        private static IEnumerable<MappedColumnInfo> ToMappedColumnInfos( PromotionDto.PromotionLanguageDataTable table, PromotionEntryCodeProvider promotionEntryCodeProvider = null )
        {
            var mappedColumns = new[]
                {
                    new MappedColumnInfo( "PromotionId", "ad_key", type.@string, r => AttributeHelper.CreateKey( r["PromotionId"].ToString(), r["LanguageCode"].ToString() ) ),
                    new MappedColumnInfo( "PromotionId", "included", type.@string,
                                          r =>
                                              {
                                                  var language = r["LanguageCode"].ToString();
                                                  var includedProductFilter =
                                                      GetIncludedProductFilter( AttributeHelper.CreateKey( r["PromotionId"].ToString(), language ),
                                                                                promotionEntryCodeProvider );
                                                  var includedLocaleFilter = string.Format( "locale_filter:'{0}'", language.ToESalesLocale() );
                                                  return !string.IsNullOrWhiteSpace( includedProductFilter )
                                                             ? string.Format( "{0} AND {1}", includedLocaleFilter, includedProductFilter )
                                                             : includedLocaleFilter;
                                              },
                                          false ),
                    new MappedColumnInfo( "DisplayName", "name", type.@string, new FilterOptions( Format.PipeSeparated, Tokenization.Words ) ),
                    new MappedColumnInfo( "LanguageCode", "locale", type.@string, r => r["LanguageCode"].ToString().ToESalesLocale(), false ),
                    new MappedColumnInfo( "LanguageCode", "locale_filter", type.@string, r => r["LanguageCode"].ToString().ToESalesLocale() )
                }.ToLookup( mc => mc.ColumnName, mc => mc );
            var ignoredColumnNames = new HashSet<string> { "PromotionLanguageId" };
            return ToMappedColumnInfosInternal( table.Columns.Cast<DataColumn>(), mappedColumns, ignoredColumnNames );
        }

        private static string GetIncludedProductFilter( string adKey, PromotionEntryCodeProvider promotionEntryCodeProvider )
        {
            if ( promotionEntryCodeProvider == null )
            {
                return "";
            }

            var includedFilterText = string.Format( "ad_key_included:'{0}'", adKey );
            var excludedFilterText = string.Format( "NOT ad_key_excluded:'{0}'", adKey );
            var hasIncluded = promotionEntryCodeProvider.AdKeyHasIncluded( adKey );
            var hasExcluded = promotionEntryCodeProvider.AdKeyHasExcluded( adKey );
            if ( hasIncluded && hasExcluded )
            {
                return string.Format( "{0} AND {1}", includedFilterText, excludedFilterText );
            }
            if ( hasIncluded )
            {
                return includedFilterText;
            }
            if ( hasExcluded )
            {
                return excludedFilterText;
            }
            return "";
        }

        private static IEnumerable<MappedColumnInfo> ToMappedColumnInfos( PromotionDto.PromotionDataTable table )
        {
            var mappedColumns = new[]
                {
                    new MappedColumnInfo( "Name", "name", type.@string, new FilterOptions( Format.PipeSeparated, Tokenization.Words ) ),
                    new MappedColumnInfo( "ApplicationId", type.@string ),
                    new MappedColumnInfo( "Status", type.@string ),
                    new MappedColumnInfo( "StartDate", "start_time", type.timepoint, r => ToStandardDateTime( r, "StartDate" ), false ),
                    new MappedColumnInfo( "EndDate", "end_time", type.timepoint, r => ToStandardDateTime( r, "EndDate" ), false ),
                    new MappedColumnInfo( "CouponCode", type.@string ),
                    new MappedColumnInfo( "OfferAmount", type.@double, r => double.Parse( r["OfferAmount"].ToString() ).ToString( "0.00", CultureInfo.InvariantCulture ) ),
                    new MappedColumnInfo( "OfferType", type.@string ),
                    new MappedColumnInfo( "PromotionGroup", type.@string ),
                    new MappedColumnInfo( "CampaignId", "campaign_key", type.@string ),
                    new MappedColumnInfo( "ExclusivityType", type.@string ),
                    new MappedColumnInfo( "Priority", type.@string ),
                    new MappedColumnInfo( "Created", type.@long, r => ToUtcEpochTime( r, "Created" ) ),
                    new MappedColumnInfo( "Modified", type.@long, r => ToUtcEpochTime( r, "Modified" ) ),
                    new MappedColumnInfo( "ModifiedBy", type.@string ),
                    new MappedColumnInfo( "PromotionType", type.@string, new FilterOptions( Format.PipeSeparated, Tokenization.Words ) ),
                    new MappedColumnInfo( "PerOrderLimit", type.@int ),
                    new MappedColumnInfo( "ApplicationLimit", type.@int ),
                    new MappedColumnInfo( "CustomerLimit", type.@int )
                }.ToLookup( mc => mc.ColumnName, mc => mc );
            var ignoredColumnNames = new HashSet<string> {"PromotionId", "Params"};
            return ToMappedColumnInfosInternal( table.Columns.Cast<DataColumn>(), mappedColumns, ignoredColumnNames );
        }

        private static IEnumerable<MappedColumnInfo> ToMappedColumnInfosInternal( IEnumerable<DataColumn> columns, ILookup<string, MappedColumnInfo> mappedColumns,
                                                                                 HashSet<string> ignoredColumnNames )
        {
            return from column in columns
                   let columnName = column.ColumnName
                   where !ignoredColumnNames.Contains( columnName )
                   from columnInfo in mappedColumns.Contains( columnName )
                                          ? mappedColumns[columnName]
                                          : new[] {new MappedColumnInfo( columnName, GetType( column.DataType ) )}
                   select columnInfo;
        }

        private static IEnumerable<ConfigurationAttribute> ToAttributesInternal( IEnumerable<MappedColumnInfo> mappedColumnInfos )
        {
            return mappedColumnInfos
                .Where( mc => mc.Configure )
                .Select( mappedColumnInfo =>
                         new ConfigurationAttribute(
                             mappedColumnInfo.SpecificESalesName,
                             mappedColumnInfo.Type,
                             Present.Yes,
                             new SearchOptions( null, Format.PipeSeparated, true, true ),
                             mappedColumnInfo.FilterOptions ?? new FilterOptions( Format.PipeSeparated, Tokenization.None ),
                             new SortOptions( Normalization.CaseInsensitive ) ) );
        }



        private static string ToStandardDateTime( DataRow row, string columnName )
        {
            const string minUtc = "1900-01-01T00:00Z";
            if ( row == null )
            {
                return minUtc;
            }

            var o = row[columnName];

            if ( o == null || o == DBNull.Value )
            {
                return minUtc;
            }

            var utcDateTime = TimeZoneInfo.ConvertTimeToUtc( (DateTime) o );
            var minUtcDateTime = new DateTime( 1900, 1, 1, 0, 0, 0, DateTimeKind.Utc );
            var maxUtcDateTime = new DateTime( 3000, 1, 1, 0, 0, 0, DateTimeKind.Utc );
            
            if ( utcDateTime < minUtcDateTime )
            {
                return minUtcDateTime.ToString( "s" ) + "Z";
            }
            if ( utcDateTime > maxUtcDateTime )
            {
                return maxUtcDateTime.ToString( "s" ) + "Z";
            }
            return utcDateTime.ToString( "s" ) + "Z";
        }

        private static string ToUtcEpochTime( DataRow row, string columnName )
        {
            if ( row == null )
            {
                return "0";
            }
            var o = row[columnName];
            return o == null || o == DBNull.Value
                       ? "0"
                       : ( (long) TimeZoneInfo.ConvertTimeToUtc( (DateTime) o )
                                      .Subtract( new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc ) )
                                      .TotalMilliseconds ).ToString( CultureInfo.InvariantCulture );
        }

        private static type GetType( Type systemType )
        {
            type importType;
            if ( TypeMap.TryGetValue( systemType, out importType ) )
            {
                return importType;
            }
            throw new ArgumentOutOfRangeException( "systemType" );
        }

        private class MappedColumnInfo
        {
            internal MappedColumnInfo( string columnName, type type, FilterOptions filterOptions = null )
                : this( columnName, ConvertAdAttributeNameToUnderscore( columnName ), type, filterOptions )
            {
            }

            internal MappedColumnInfo( string columnName, type type, Func<DataRow, string> getValue )
                : this( columnName, ConvertAdAttributeNameToUnderscore( columnName ), type, getValue )
            {
            }

            internal MappedColumnInfo( string columnName, string specificESalesName, type type, FilterOptions filterOptions = null )
                : this( columnName, specificESalesName, type, r => r[columnName].ToString(), filterOptions: filterOptions )
            {
            }

            internal MappedColumnInfo( string columnName, string specificESalesName, type type, Func<DataRow, string> getValue, bool configure = true, FilterOptions filterOptions = null )
            {
                ColumnName = columnName;
                SpecificESalesName = specificESalesName;
                Type = type;
                GetValue = getValue;
                Configure = configure;
                FilterOptions = filterOptions;
            }

            internal bool Configure { get; private set; }
            internal FilterOptions FilterOptions { get; private set; }
            internal string ColumnName { get; private set; }
            internal string SpecificESalesName { get; private set; }
            internal type Type { get; private set; }
            internal Func<DataRow, string> GetValue { get; private set; }

            private static string ConvertAdAttributeNameToUnderscore( string externalName )
            {
                if ( externalName == null ) throw new ArgumentNullException( "externalName" );

                var eSalesname = new StringBuilder( externalName.Length * 2 - 1 ); //Worst case size = all characters are uppercase.
                bool firstLetter = true;
                foreach ( char character in externalName )
                {
                    char lower = char.ToLowerInvariant( character );
                    if ( !char.IsWhiteSpace( character ) )
                    {
                        if ( firstLetter && character != lower )
                        {
                            eSalesname.Append( lower );
                        }
                        else if ( character != lower )
                        {
                            eSalesname.Append( "_" + lower );
                        }
                        else
                        {
                            eSalesname.Append( character );
                        }
                        firstLetter = false;
                    }
                }

                return eSalesname.ToString();
            }
        }
    }
}