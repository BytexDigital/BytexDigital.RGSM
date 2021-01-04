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
    [Migration("20210104050159_AddedModLoadField")]
    partial class AddedModLoadField
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.ApiKey", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Remarks")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("TimeCreated")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.ToTable("ApiKeys");
                });

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

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.GroupReference", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("GroupId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PermissionId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("TimeCreated")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.HasIndex("PermissionId");

                    b.ToTable("GroupReferences");
                });

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.KeyValue", b =>
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

                    b.ToTable("KeyValues");
                });

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.Permission", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

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

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.Scheduling.ScheduleAction", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("ActionType")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ContinueOnError")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Order")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ScheduleGroupId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("TimeCreated")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleGroupId");

                    b.ToTable("ScheduleActions");
                });

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.Scheduling.ScheduleGroup", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CronExpression")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Priority")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SchedulerPlanId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("TimeCreated")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.HasIndex("SchedulerPlanId");

                    b.ToTable("ScheduleGroups");
                });

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.Scheduling.SchedulerPlan", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsEnabled")
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

                    b.ToTable("SchedulerPlans");
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

                    b.Property<bool>("Load")
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

            modelBuilder.Entity("KeyValueScheduleAction", b =>
                {
                    b.Property<string>("KeyValuesId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ScheduleActionsId")
                        .HasColumnType("TEXT");

                    b.HasKey("KeyValuesId", "ScheduleActionsId");

                    b.HasIndex("ScheduleActionsId");

                    b.ToTable("KeyValueScheduleAction");
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

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.GroupReference", b =>
                {
                    b.HasOne("BytexDigital.RGSM.Node.Domain.Entities.Permission", "Permission")
                        .WithMany("GroupReferences")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Permission");
                });

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.Permission", b =>
                {
                    b.HasOne("BytexDigital.RGSM.Node.Domain.Entities.Server", "Server")
                        .WithMany("Permissions")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.Scheduling.ScheduleAction", b =>
                {
                    b.HasOne("BytexDigital.RGSM.Node.Domain.Entities.Scheduling.ScheduleGroup", "ScheduleGroup")
                        .WithMany("ScheduleActions")
                        .HasForeignKey("ScheduleGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ScheduleGroup");
                });

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.Scheduling.ScheduleGroup", b =>
                {
                    b.HasOne("BytexDigital.RGSM.Node.Domain.Entities.Scheduling.SchedulerPlan", "SchedulerPlan")
                        .WithMany("ScheduleGroups")
                        .HasForeignKey("SchedulerPlanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SchedulerPlan");
                });

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.Scheduling.SchedulerPlan", b =>
                {
                    b.HasOne("BytexDigital.RGSM.Node.Domain.Entities.Server", "Server")
                        .WithOne("SchedulerPlan")
                        .HasForeignKey("BytexDigital.RGSM.Node.Domain.Entities.Scheduling.SchedulerPlan", "ServerId")
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

            modelBuilder.Entity("KeyValueScheduleAction", b =>
                {
                    b.HasOne("BytexDigital.RGSM.Node.Domain.Entities.KeyValue", null)
                        .WithMany()
                        .HasForeignKey("KeyValuesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BytexDigital.RGSM.Node.Domain.Entities.Scheduling.ScheduleAction", null)
                        .WithMany()
                        .HasForeignKey("ScheduleActionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.Permission", b =>
                {
                    b.Navigation("GroupReferences");
                });

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.Scheduling.ScheduleGroup", b =>
                {
                    b.Navigation("ScheduleActions");
                });

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.Scheduling.SchedulerPlan", b =>
                {
                    b.Navigation("ScheduleGroups");
                });

            modelBuilder.Entity("BytexDigital.RGSM.Node.Domain.Entities.Server", b =>
                {
                    b.Navigation("Arma3Server");

                    b.Navigation("Permissions");

                    b.Navigation("SchedulerPlan");

                    b.Navigation("TrackedDepots");

                    b.Navigation("TrackedWorkshopMods");
                });
#pragma warning restore 612, 618
        }
    }
}
