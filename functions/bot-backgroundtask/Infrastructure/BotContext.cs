using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FunctionHandler.Infrastructure
{
    public partial class BotContext : DbContext
    {
        public BotContext()
        {
        }

        public BotContext(DbContextOptions<BotContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Translations> Translations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<Translations>(entity =>
            {
                entity.ToTable("translations", "bot");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.OriginalPhrase)
                    .IsRequired()
                    .HasMaxLength(140);

                entity.Property(e => e.TranslatedPhrase)
                    .IsRequired()
                    .HasMaxLength(140);
            });

            modelBuilder.HasSequence("translationseq").IncrementsBy(10);

            modelBuilder.HasSequence("EntityFrameworkHiLoSequence").IncrementsBy(10);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}