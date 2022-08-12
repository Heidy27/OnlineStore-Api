﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OnlineStore.models;

#nullable disable

namespace OnlineStore.Migrations
{
    [DbContext(typeof(OnlineStorageContext))]
    [Migration("20220811022012_Payment4")]
    partial class Payment4
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("OnlineStore.models.Item", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("amount")
                        .HasColumnType("bit");

                    b.Property<bool>("description")
                        .HasColumnType("bit");

                    b.HasKey("ID");

                    b.ToTable("items");
                });

            modelBuilder.Entity("OnlineStore.models.Payment", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("expirationDate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("holderName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("number")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("userID")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ID");

                    b.HasIndex("userID");

                    b.ToTable("payment");
                });

            modelBuilder.Entity("OnlineStore.models.User", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("enabled")
                        .HasColumnType("bit");

                    b.Property<string>("pwd")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("role")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("userName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ID");

                    b.HasIndex("userName")
                        .IsUnique()
                        .HasFilter("[userName] IS NOT NULL");

                    b.ToTable("users");
                });

            modelBuilder.Entity("OnlineStore.models.Payment", b =>
                {
                    b.HasOne("OnlineStore.models.User", null)
                        .WithMany("payments")
                        .HasForeignKey("userID");
                });

            modelBuilder.Entity("OnlineStore.models.User", b =>
                {
                    b.Navigation("payments");
                });
#pragma warning restore 612, 618
        }
    }
}
