using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NeedABreakTests
{
    public class LocalizationTests
    {
        [Theory]
        [InlineData("fr", "Texte en français")]
        [InlineData("fr-FR", "Texte en français")]
        [InlineData("fr-BE", "Texte en français")]
        [InlineData("fr-MA", "Texte en français")]
        [InlineData("fr-PF", "Texte en français")]
        [InlineData("it", "Testo in italiano")]
        [InlineData("en", "Text in english")]
        [InlineData("en-US", "Text in english")]
        [InlineData("sv", "Text in english")]       // Language not supported, fallback to english
        public void TextShouldBeTranslated(string culture, string expectedResult)
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);

            string translated = NeedABreak.Properties.Resources.translation_test;

            Assert.Equal(expectedResult, translated);
        }
    }
}
