using CodeKoenig.SyndicationToolbox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SyndicationToolbox.Tests
{
    public class ParserTests
    {
        [Theory]
        [InlineData("WordPressHubbubNotification.xml", "Announcing Dexterity Training at GPUG Summit 2016 in Tampa, FL, USA", 1, null)]
        public void ShouldParseFeedFromGivenFile(string fileName, string expectedTitle, int expectedItemCount, string expectedCategory)
        {
            string content = File.ReadAllText($"TestFeeds/{fileName}");

            FeedParser parser = FeedParser.Create(content);
            Feed parsedFeed = parser.Parse();

            Assert.NotNull(parsedFeed);
            Assert.Equal(expectedTitle, parsedFeed.Name);

            if (expectedCategory != null)
            {
                Assert.Contains(parsedFeed.Categories, c => c.Term == expectedCategory);
            }
            else
            {
                Assert.Null(parsedFeed.Categories);
            }

            Assert.Equal(expectedItemCount, parsedFeed.Articles.Count);
        }
    }
}
