using System.Globalization;
using System.Linq;
using System.Threading;
using Xunit;

namespace UpsCoolWeb.Components.Mvc.Tests
{
    public class LanguagesTests
    {
        private Languages languages;

        public LanguagesTests()
        {
            languages = new Languages("en", new []
            {
                new Language
                {
                    Name = "English",
                    Abbreviation = "en",
                    Culture = new CultureInfo("en-GB")
                },
                new Language
                {
                    Name = "Lietuvių",
                    Abbreviation = "lt",
                    Culture = new CultureInfo("lt-LT")
                }
            });
        }

        #region Default

        [Fact]
        public void Default_Language()
        {
            Thread.CurrentThread.CurrentUICulture = languages["lt"].Culture;

            Language actual = languages.Default;
            Language expected = languages["en"];

            Assert.Same(expected, actual);
        }

        #endregion

        #region Current

        [Fact]
        public void Current_GetsLanguage()
        {
            Thread.CurrentThread.CurrentUICulture = languages["en"].Culture;

            Language actual = languages.Current;
            Language expected = languages["en"];

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Current_SetsLanguage()
        {
            languages.Current = languages.Supported.Last();

            CultureInfo expectedCulture = languages.Supported.Last().Culture;
            CultureInfo actualUICulture = CultureInfo.CurrentUICulture;
            CultureInfo actualCulture = CultureInfo.CurrentCulture;

            Assert.Same(expectedCulture, actualUICulture);
            Assert.Same(expectedCulture, actualCulture);
        }

        #endregion

        #region Supported

        [Fact]
        public void Supported_Languages()
        {
            Language[] actual = languages.Supported;

            Assert.Equal(new CultureInfo("lt-LT"), actual[1].Culture);
            Assert.Equal(new CultureInfo("en-GB"), actual[0].Culture);
            Assert.Equal("lt", actual[1].Abbreviation);
            Assert.Equal("en", actual[0].Abbreviation);
            Assert.Equal("Lietuvių", actual[1].Name);
            Assert.Equal("English", actual[0].Name);
        }

        #endregion

        #region this[String abbreviation]

        [Fact]
        public void Indexer_ReturnsLanguage()
        {
            Language actual = languages["en"];

            Assert.Equal(new CultureInfo("en-GB"), actual.Culture);
            Assert.Equal("en", actual.Abbreviation);
            Assert.Equal("English", actual.Name);
        }

        #endregion
    }
}
