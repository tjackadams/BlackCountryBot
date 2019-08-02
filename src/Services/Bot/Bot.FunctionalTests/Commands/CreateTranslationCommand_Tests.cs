using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Bot.API.Application.Commands;
using Bot.Domain.AggregatesModel.TranslationAggregate;
using Bot.Domain.Exceptions;
using Bot.FunctionalTests.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using static Bot.FunctionalTests.SliceFixture;

namespace Bot.FunctionalTests.Commands
{
    [ExcludeFromCodeCoverage]
    public class CreateTranslationCommand_Tests : IntegrationTestBase
    {
        [Fact]
        public async Task CreateTranslationCommand_Should_InsertIntoDatabase()
        {
            // arrange
            var message = new CreateTranslationCommand("original phrase", "translated phrase");

            // act
            bool commandResult = await SendAsync(message);

            // assert
            commandResult.ShouldBeTrue();

            Translation sut = await ExecuteDbContextAsync(context =>
            {
                return context.Translations
                    .Where(t => t.OriginalPhrase == message.OriginalPhrase)
                    .FirstOrDefaultAsync();
            });

            sut.ShouldNotBeNull();
            sut.OriginalPhrase.ShouldBe(message.OriginalPhrase);
            sut.TranslatedPhrase.ShouldBe(message.TranslatedPhrase);
        }

        [Fact]
        public async Task CreateTranslationCommand_Should_ThrowDomainException()
        {
            // arrange
            var message = new CreateTranslationCommand(ContentHelpers.GetWords(), ContentHelpers.GetWords());

            // act
            Should.Throw<BotDomainException>(SendAsync(message));

            // assert
            Translation sut = await ExecuteDbContextAsync(context =>
            {
                return context.Translations
                    .Where(t => t.OriginalPhrase == message.OriginalPhrase)
                    .FirstOrDefaultAsync();
            });

            sut.ShouldBeNull();
        }
    }
}
