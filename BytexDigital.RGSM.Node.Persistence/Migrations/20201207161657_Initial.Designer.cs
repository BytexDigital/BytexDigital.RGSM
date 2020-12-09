﻿// <auto-generated />
using System;
using BytexDigital.RGSM.Node.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BytexDigital.RGSM.Node.Persistence.Migrations
{
    [DbContext(typeof(NodeDbContext))]
    [Migration("20201207161657_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.Arma3.Arma3Server", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int?>("AllowFilePatching")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BetaBranch")
                        .HasColumnType("TEXT");

                    b.Property<string>("ExecutableFileName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Hostname")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("InstalledVersion")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsInstalled")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MaxPlayers")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MessageOfTheDay")
                        .HasColumnType("TEXT");

                    b.Property<int?>("MessageOfTheDayInterval")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordAdmin")
                        .HasColumnType("TEXT");

                    b.Property<int>("Port")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ServerCommandPassword")
                        .HasColumnType("TEXT");

                    b.Property<string>("ServerId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("TimeCreated")
                        .HasColumnType("TEXT");

                    b.Property<int>("VerifySignatures")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.HasIndex("ServerId")
                        .IsUnique();

                    b.ToTable("Arma3Server");
                });

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.Server", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Directory")
                        .HasColumnType("TEXT");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("NodeId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("TimeCreated")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.Setting", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("TimeCreated")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.ToTable("NodeSettings");
                });

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.Arma3.Arma3Server", b =>
                {
                    b.HasOne("BytexDigital.RGSM.Node.Domain.Entities.Server", "Server")
                        .WithOne("Arma3Server")
                        .HasForeignKey("BytexDigital.RGSM.Node.Domain.Entities.Arma3.Arma3Server", "ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.Server", b =>
                {
                    b.Navigation("Arma3Server");
                });
#pragma warning restore 612, 618
        }
    }
}
