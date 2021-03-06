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
    [Migration("20201209050915_TrackedModOfAppId")]
    partial class TrackedModOfAppId
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

                    b.Property<string>("AdditionalArguments")
                        .HasColumnType("TEXT");

                    b.Property<int?>("AppId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BattlEyePath")
                        .HasColumnType("TEXT");

                    b.Property<string>("Branch")
                        .HasColumnType("TEXT");

                    b.Property<string>("ExecutableFileName")
                        .HasColumnType("TEXT");

                    b.Property<string>("InstalledVersion")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsInstalled")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Port")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ProfilesPath")
                        .HasColumnType("TEXT");

                    b.Property<string>("RconIp")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("RconPassword")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("RconPort")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ServerId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("TimeCreated")
                        .HasColumnType("TEXT");

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
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("DisplayName")
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

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.TrackedDepot", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<long>("DepotId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ServerId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("TimeCreated")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.ToTable("TrackedDepots");
                });

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.TrackedWorkshopMod", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Directory")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsLoaded")
                        .HasColumnType("INTEGER");

                    b.Property<uint?>("OfAppId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("PublishedFileId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ServerId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("TimeCreated")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.ToTable("TrackedWorkshopMods");
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

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.TrackedDepot", b =>
                {
                    b.HasOne("BytexDigital.RGSM.Node.Domain.Entities.Server", "Server")
                        .WithMany("TrackedDepots")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.TrackedWorkshopMod", b =>
                {
                    b.HasOne("BytexDigital.RGSM.Node.Domain.Entities.Server", "Server")
                        .WithMany("TrackedWorkshopMods")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.Server", b =>
                {
                    b.Navigation("Arma3Server");

                    b.Navigation("TrackedDepots");

                    b.Navigation("TrackedWorkshopMods");
                });
#pragma warning restore 612, 618
        }
    }
}
