﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RSAAPI.Database;

#nullable disable

namespace RSAAPI.Migrations
{
    [DbContext(typeof(RSAContext))]
    [Migration("20240828042345_RemovedSubId")]
    partial class RemovedSubId
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("RSAAPI.Models.DbUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("ApiToken")
                        .HasMaxLength(1025)
                        .HasColumnType("nvarchar(1025)");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValue(new DateTime(2024, 8, 27, 21, 23, 45, 571, DateTimeKind.Local).AddTicks(5818));

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("LastLogin")
                        .HasColumnType("datetime2");

                    b.Property<string>("LicenseKey")
                        .HasMaxLength(1025)
                        .HasColumnType("nvarchar(1025)");

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValue(new DateTime(2024, 8, 27, 21, 23, 45, 571, DateTimeKind.Local).AddTicks(5960));

                    b.Property<string>("SandBoxToken")
                        .HasMaxLength(1025)
                        .HasColumnType("nvarchar(1025)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasFilter("[Email] IS NOT NULL");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
