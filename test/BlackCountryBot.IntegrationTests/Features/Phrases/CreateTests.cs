using System.Threading.Tasks;
using BlackCountryBot.Core.Models.Phrases;
using BlackCountryBot.Web.Features.Phrases;
using Shouldly;
using Xunit;
using static BlackCountryBot.IntegrationTests.SliceFixture;

namespace BlackCountryBot.IntegrationTests.Features.Phrases
{
    public class CreateTests : IntegrationTestBase
    {
        [Fact]
        public async Task CreateCommand_Should_CreatePhrase()
        {
            // arrange
            var cmd = new Create.Command
            {
                Original = GetString(10),
                Translation = GetString(11)
            };

            // act
            int phraseId = await SendAsync(cmd);

            // assert
            Phrase phrase = await FindAsync<Phrase>(phraseId);

            phrase.ShouldNotBeNull();
            phrase.Original.ShouldBe(cmd.Original);
            phrase.Translation.ShouldBe(cmd.Translation);
        }
    }
}
