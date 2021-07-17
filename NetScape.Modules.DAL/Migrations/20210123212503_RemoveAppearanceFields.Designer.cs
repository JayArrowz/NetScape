﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetScape.Abstractions.Model.Game;
using NetScape.Modules.DAL;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace NetScape.Modules.DAL.Migrations
{
    [DbContext(typeof(DatabaseContext<Player>))]
    [Migration("20210123212503_RemoveAppearanceFields")]
    partial class RemoveAppearanceFields
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:Collation", "English_United Kingdom.1252")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("NetScape.Abstractions.Model.Game.Appearance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Colors")
                        .HasColumnType("text");

                    b.Property<int>("Gender")
                        .HasColumnType("integer");

                    b.Property<string>("Style")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Appearances");
                });

            modelBuilder.Entity("NetScape.Abstractions.Model.Game.Player", b =>
                {
                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.Property<int>("AppearanceId")
                        .HasColumnType("integer");

                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.HasKey("Username");

                    b.HasIndex("AppearanceId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("NetScape.Abstractions.Model.Game.Player", b =>
                {
                    b.HasOne("NetScape.Abstractions.Model.Game.Appearance", "Appearance")
                        .WithMany()
                        .HasForeignKey("AppearanceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("NetScape.Abstractions.Model.Position", "Position", b1 =>
                        {
                            b1.Property<string>("PlayerUsername")
                                .HasColumnType("text");

                            b1.Property<int>("Height")
                                .HasColumnType("integer");

                            b1.Property<int>("X")
                                .HasColumnType("integer");

                            b1.Property<int>("Y")
                                .HasColumnType("integer");

                            b1.HasKey("PlayerUsername");

                            b1.ToTable("Players");

                            b1.WithOwner()
                                .HasForeignKey("PlayerUsername");
                        });

                    b.Navigation("Appearance");

                    b.Navigation("Position");
                });
#pragma warning restore 612, 618
        }
    }
}
