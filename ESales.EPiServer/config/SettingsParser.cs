using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using Apptus.ESales.EPiServer.Util;

namespace Apptus.ESales.EPiServer.Config
{
    internal class SettingsParser
    {
        private readonly NameValueCollection _settings;

        public SettingsParser(NameValueCollection settings)
        {
            _settings = settings;
        }

        public IEnumerable<string> OptionalEnumerable( string key, string separator )
        {
            var value = _settings[key] ?? "";
            return value.Split( separator, StringSplitOptions.RemoveEmptyEntries );
        }

        public bool OptionalBool( string key, bool defaultValue )
        {
            bool value;
            return !bool.TryParse( _settings[key], out value ) ? defaultValue : value;
        }

        public bool RequiredBool( string key )
        {
            bool value;
            if ( bool.TryParse( _settings[key], out value ) )
            {
                return value;
            }
            throw new ProviderException( ConfError( key ) );
        }

        public string RequiredString( string key, Func<string, bool> validator )
        {
            var value = _settings[key];
            if ( !validator( value ) )
            {
                throw new ProviderException( ConfError( key ) );
            }
            return value;
        }

        public string RequiredString( string key )
        {
            return RequiredString( key, v => !string.IsNullOrWhiteSpace( v ) );
        }

        public string OptionalString( string key, string defaultValue, Func<string, bool> validator )
        {
            var value = _settings[key];
            return validator( value ) ? value : defaultValue;
        }

        public string OptionalString( string key, string defaultValue )
        {
            return OptionalString( key, defaultValue, v => !string.IsNullOrWhiteSpace( v ) );
        }

        public int OptionalInt( string key, int defaultValue )
        {
            var value = _settings[key];
            int typedValue;
            return !int.TryParse( value, out typedValue ) ? defaultValue : typedValue;
        }

        private static string ConfError( string key )
        {
            return string.Format( "Missing valid configuration value for \"{0}\"", key );
        }
    }
}