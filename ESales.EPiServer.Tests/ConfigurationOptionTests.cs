using Apptus.ESales.EPiServer.Import.Configuration;
using Apptus.ESales.EPiServer.Resources;
using Autofac;
using NUnit.Framework;

namespace ESales.EPiServer.Tests
{
    [TestFixture]
    class ConfigurationOptionTests
    {
        private IContainer _container;

        [SetUp]
        public void Init()
        {
            var builder = new ContainerBuilder();
            builder.Register( c => new configuration
                {
                    formats = Loader.Load<formats>().format,
                    tokenizations = Loader.Load<tokenizations>().tokenization,
                    normalizations = Loader.Load<normalizations>().normalization
                } );
            builder.RegisterType<ConfigurationOptions>();
            _container = builder.Build();
        }

        [Test]
        public void NoFormat()
        {
            var format = new format { name = "(no format)" };
            var options = _container.Resolve<ConfigurationOptions>();
            var actual = options.Format( Format.None );
            AssertEquivalentFormat( actual, format );
        }

        [Test]
        public void HtmlFormat()
        {
            var format = new format { name = "HTML" };
            var options = _container.Resolve<ConfigurationOptions>();
            var actual = options.Format( Format.Html );
            AssertEquivalentFormat( actual, format );
        }

        [Test]
        public void PathFormat()
        {
            var format = new format { name = "Path (e.g. /a/b/c)" };
            var options = _container.Resolve<ConfigurationOptions>();
            var actual = options.Format( Format.Path );
            AssertEquivalentFormat( actual, format );
        }

        [Test]
        public void CommaSeparatedFormat()
        {
            var format = new format { name = "Comma-separated list (e.g. a,b,c)" };
            var options = _container.Resolve<ConfigurationOptions>();
            var actual = options.Format( Format.CommaSeparated );
            AssertEquivalentFormat( actual, format );
        }

        [Test]
        public void CommaSeparatedPathsFormat()
        {
            var format = new format { name = "Comma-separated list of paths (e.g. /a/b,/e/f)" };
            var options = _container.Resolve<ConfigurationOptions>();
            var actual = options.Format( Format.CommaSeparatedPaths );
            AssertEquivalentFormat( actual, format );
        }

        [Test]
        public void PipeSeparatedFormat()
        {
            var format = new format { name = "Pipe-separated list (e.g. a|b|c)" };
            var options = _container.Resolve<ConfigurationOptions>();
            var actual = options.Format( Format.PipeSeparated );
            AssertEquivalentFormat( actual, format );
        }

        [Test]
        public void PipeSeparatedPaths()
        {
            var format = new format { name = "Pipe-separated list of paths (e.g. /a/b|/e/f)" };
            var options = _container.Resolve<ConfigurationOptions>();
            var actual = options.Format( Format.PipeSeparatedPaths );
            AssertEquivalentFormat( actual, format );
        }

        [Test]
        public void NoTokenization()
        {
            var tokenization = new tokenization { name = "(no refinement)" };
            var options = _container.Resolve<ConfigurationOptions>();
            var actual = options.Tokenization( Tokenization.None );
            AssertEquivalentTokenization( actual, tokenization );
        }

        [Test]
        public void CaseInsensitiveTokenization()
        {
            var tokenization = new tokenization { name = "Case insensitive" };
            var options = _container.Resolve<ConfigurationOptions>();
            var actual = options.Tokenization( Tokenization.CaseInsensitive );
            AssertEquivalentTokenization( actual, tokenization );
        }

        [Test]
        public void ModelDesignationTokenization()
        {
            var tokenization = new tokenization { name = "Model designation (e.g. MB12a)" };
            var options = _container.Resolve<ConfigurationOptions>();
            var actual = options.Tokenization( Tokenization.ModelDesignation );
            AssertEquivalentTokenization( actual, tokenization );
        }

        [Test]
        public void WordsTokenization()
        {
            var tokenization = new tokenization { name = "Text (words)" };
            var options = _container.Resolve<ConfigurationOptions>();
            var actual = options.Tokenization( Tokenization.Words );
            AssertEquivalentTokenization( actual, tokenization );
        }

        [Test]
        public void WordStemsTokenization()
        {
            var tokenization = new tokenization { name = "Text (word stems)" };
            var options = _container.Resolve<ConfigurationOptions>();
            var actual = options.Tokenization( Tokenization.WordStems );
            AssertEquivalentTokenization( actual, tokenization );
        }

        [Test]
        public void NoNormalization()
        {
            var normalization = new normalization { name = "(no refinement)" };
            var options = _container.Resolve<ConfigurationOptions>();
            var actual = options.Normalization( Normalization.None );
            AssertEquivalentNormalization( actual, normalization );
        }

        [Test]
        public void CaseInsensitiveNormalization()
        {
            var normalization = new normalization { name = "Case insensitive" };
            var options = _container.Resolve<ConfigurationOptions>();
            var actual = options.Normalization( Normalization.CaseInsensitive );
            AssertEquivalentNormalization( actual, normalization );
        }

        [Test]
        public void DigitsNormalization()
        {
            var normalization = new normalization { name = "Keep digits only (e.g. 1234)" };
            var options = _container.Resolve<ConfigurationOptions>();
            var actual = options.Normalization( Normalization.Digits );
            AssertEquivalentNormalization( actual, normalization );
        }

        [Test]
        public void DigitsLettersNormalization()
        {
            var normalization = new normalization { name = "Keep digits and letters (e.g. Text123)" };
            var options = _container.Resolve<ConfigurationOptions>();
            var actual = options.Normalization( Normalization.DigitsLetters );
            AssertEquivalentNormalization( actual, normalization );
        }

        [Test]
        public void DigitsLettersCaseInsensitiveNormalization()
        {
            var normalization = new normalization { name = "Keep digits and letters, case insensitive" };
            var options = _container.Resolve<ConfigurationOptions>();
            var actual = options.Normalization( Normalization.DigitsLettersCaseInsensitive );
            AssertEquivalentNormalization( actual, normalization );
        }

        private static void AssertEquivalentFormat( format actual, format expected )
        {
            Assert.That( actual, Is.Not.Null );
            Assert.That( actual.name, Is.EqualTo( expected.name ) );
            Assert.That( actual.rules, Is.Null );
        }

        private static void AssertEquivalentTokenization( tokenization actual, tokenization expected )
        {
            Assert.That( actual, Is.Not.Null );
            Assert.That( actual.name, Is.EqualTo( expected.name ) );
            Assert.That( actual.product, Is.Null );
            Assert.That( actual.query, Is.Null );
        }

        private static void AssertEquivalentNormalization( normalization actual, normalization expected )
        {
            Assert.That( actual, Is.Not.Null );
            Assert.That( actual.name, Is.EqualTo( expected.name ) );
            Assert.That( actual.@default, Is.Null );
            Assert.That( actual.localized, Is.Null );
        }
    }
}
