﻿// <auto-generated />
using System;
using BlackCountryBot.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BlackCountryBot.Infrastructure.Migrations
{
    [DbContext(typeof(BlackCountryContext))]
    partial class BlackCountryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("Relational:Sequence:.EntityFrameworkHiLoSequence", "'EntityFrameworkHiLoSequence', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BlackCountryBot.Shared.Translations.Models.TranslatedPhrase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:HiLoSequenceName", "EntityFrameworkHiLoSequence")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<DateTimeOffset>("CreatedTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset?>("LastTweetTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("OriginalPhrase")
                        .IsRequired()
                        .HasColumnType("nvarchar(240)")
                        .HasMaxLength(240);

                    b.Property<string>("TranslatedPhrase")
                        .IsRequired()
                        .HasColumnType("nvarchar(240)")
                        .HasMaxLength(240);

                    b.Property<int>("TweetCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Translations");
                });
#pragma warning restore 612, 618
        }
    }
}
