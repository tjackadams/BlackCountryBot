using BlackCountryBot.Core.Infrastructure;
using Bot.Domain.AggregatesModel.TranslationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bot.Infrastructure.EntityConfigurations
{
    public class TranslationEntityTypeConfiguration : IEntityTypeConfiguration<Translation>
    {
        public void Configure(EntityTypeBuilder<Translation> builder)
        {
            builder.ToTable("translations", BotContext.DEFAULT_SCHEMA);

            builder.HasKey(p => p.Id);

            builder.Ignore(p => p.DomainEvents);

            builder.Property(p => p.Id)
                .UseHiLo("translationseq",BotContext.DEFAULT_SCHEMA);

            builder.Property(p => p.CreatedTime).IsRequired();
            builder.Property(p => p.OriginalPhrase).HasMaxLength(140).IsRequired();
            builder.Property(p => p.TranslatedPhrase).HasMaxLength(140).IsRequired();
            builder.Property(p => p.TweetCount).IsRequired();
        }
    }
}
