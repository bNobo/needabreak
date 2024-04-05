using NeedABreak.Extensions;
using Xunit;

namespace NeedABreakTests
{
    public class TimeSpanExtensionsTests
    {
        [Theory]
        [InlineData("fr", "00:00:59", "0 minutes")]
        [InlineData("fr", "00:01:00", "1 minute")]
        [InlineData("fr", "00:12:00", "12 minutes")]
        [InlineData("fr", "01:00:00", "1 heure")]
        [InlineData("fr", "01:34:00", "1 heure, 34 minutes")]
        [InlineData("fr", "02:00:00", "2 heures")]
        [InlineData("fr", "02:56:00", "2 heures, 56 minutes")]
        [InlineData("en", "00:00:59", "0 minutes")]
        [InlineData("en", "00:01:00", "1 minute")]
        [InlineData("en", "00:12:00", "12 minutes")]
        [InlineData("en", "01:00:00", "1 hour")]
        [InlineData("en", "01:34:00", "1 hour, 34 minutes")]
        [InlineData("en", "02:00:00", "2 hours")]
        [InlineData("en", "02:56:00", "2 hours, 56 minutes")]
        [InlineData("it", "00:00:59", "0 minuti")]
        [InlineData("it", "00:01:00", "1 minuto")]
        [InlineData("it", "00:12:00", "12 minuti")]
        [InlineData("it", "01:00:00", "1 ora")]
        [InlineData("it", "01:34:00", "1 ora, 34 minuti")]
        [InlineData("it", "02:00:00", "2 ore")]
        [InlineData("it", "02:56:00", "2 ore, 56 minuti")]
        public void ToHumanFriendlyStringTest(string culture, string durationString, string expectedResult)
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);

            TimeSpan duration = TimeSpan.Parse(durationString);
            string res = TimeSpanExtensions.ToHumanFriendlyString(duration);

            Assert.Equal(expectedResult, res);
        }
    }
}
