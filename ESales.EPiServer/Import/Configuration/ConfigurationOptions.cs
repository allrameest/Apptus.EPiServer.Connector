using System;
using System.Collections.Generic;
using System.Linq;

namespace Apptus.ESales.EPiServer.Import.Configuration
{
    internal class ConfigurationOptions
    {
        private readonly Dictionary<Format, format> _formats;
        private readonly Dictionary<Tokenization, tokenization> _tokenizations;
        private readonly Dictionary<Normalization, normalization> _normalizations;

        public ConfigurationOptions( configuration baseConfiguration )
        {
            _formats = baseConfiguration.formats.ToDictionary( MapFormat, f => new format { name = f.name } );
            _tokenizations = baseConfiguration.tokenizations.ToDictionary( MapTokenization, t => new tokenization { name = t.name } );
            _normalizations = baseConfiguration.normalizations.ToDictionary( MapNormalization, n => new normalization { name = n.name } );
        }

        public format Format( Format format )
        {
            return _formats[format];
        }

        public tokenization Tokenization( Tokenization tokenization )
        {
            return _tokenizations[tokenization];
        }

        public normalization Normalization( Normalization normalization )
        {
            return _normalizations[normalization];
        }

        private static Format MapFormat( format format )
        {
            switch ( format.name )
            {
                case "(no format)":
                    return Import.Configuration.Format.None;
                case "Comma-separated list (e.g. a,b,c)":
                    return Import.Configuration.Format.CommaSeparated;
                case "Pipe-separated list (e.g. a|b|c)":
                    return Import.Configuration.Format.PipeSeparated;
                case "HTML":
                    return Import.Configuration.Format.Html;
                case "Path (e.g. /a/b/c)":
                    return Import.Configuration.Format.Path;
                case "Pipe-separated list of paths (e.g. /a/b|/e/f)":
                    return Import.Configuration.Format.PipeSeparatedPaths;
                case "Comma-separated list of paths (e.g. /a/b,/e/f)":
                    return Import.Configuration.Format.CommaSeparatedPaths;
            }
            throw new ArgumentOutOfRangeException( "format", format.name, "Unexpected format." );
        }

        private static Tokenization MapTokenization( tokenization tokenization )
        {
            switch ( tokenization.name )
            {
                case "(no refinement)":
                    return Import.Configuration.Tokenization.None;
                case "Case insensitive":
                    return Import.Configuration.Tokenization.CaseInsensitive;
                case "Model designation (e.g. MB12a)":
                    return Import.Configuration.Tokenization.ModelDesignation;
                case "Text (words)":
                    return Import.Configuration.Tokenization.Words;
                case "Text (word stems)":
                    return Import.Configuration.Tokenization.WordStems;
            }
            throw new ArgumentOutOfRangeException( "tokenization", tokenization.name, "Unexpected tokenization." );
        }

        private static Normalization MapNormalization( normalization normalization )
        {
            switch ( normalization.name )
            {
                case "(no refinement)":
                    return Import.Configuration.Normalization.None;
                case "Case insensitive":
                    return Import.Configuration.Normalization.CaseInsensitive;
                case "Keep digits only (e.g. 1234)":
                    return Import.Configuration.Normalization.Digits;
                case "Keep digits and letters (e.g. Text123)":
                    return Import.Configuration.Normalization.DigitsLetters;
                case "Keep digits and letters, case insensitive":
                    return Import.Configuration.Normalization.DigitsLettersCaseInsensitive;
            }
            throw new ArgumentOutOfRangeException( "normalization", normalization.name, "Unexpected normalization." );
        }
    }
}
