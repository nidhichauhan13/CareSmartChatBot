﻿// <auto-generated />
using CareSmartChatBot.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CareSmartChatBot.Data.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    partial class ApplicationDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("CareSmartChatBot.Models.Conversation", b =>
                {
                    b.Property<string>("ConversationId")
                        .HasColumnType("text");

                    b.Property<string>("MetaDataValue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("OriginalJsonResponse")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ConversationId");

                    b.ToTable("conversations");
                });
#pragma warning restore 612, 618
        }
    }
}