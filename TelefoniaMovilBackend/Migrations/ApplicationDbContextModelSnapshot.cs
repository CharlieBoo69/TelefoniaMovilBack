﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TelefoniaMovilBackend.Data;

#nullable disable

namespace TelefoniaMovilBackend.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("TelefoniaMovilBackend.Models.Plan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("BeneficiosAdicionales")
                        .HasColumnType("longtext");

                    b.Property<decimal>("Costo")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Datos")
                        .HasColumnType("int");

                    b.Property<int>("Minutos")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Operadora")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("SMS")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("planes", (string)null);
                });

            modelBuilder.Entity("TelefoniaMovilBackend.Models.Suscripcion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("FechaSuscripcion")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("NumeroTelefono")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("PlanId")
                        .HasColumnType("int");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PlanId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("suscripciones", (string)null);
                });

            modelBuilder.Entity("TelefoniaMovilBackend.Models.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Telefono")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("usuarios", (string)null);
                });

            modelBuilder.Entity("TelefoniaMovilBackend.Models.Suscripcion", b =>
                {
                    b.HasOne("TelefoniaMovilBackend.Models.Plan", "Plan")
                        .WithMany()
                        .HasForeignKey("PlanId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TelefoniaMovilBackend.Models.Usuario", "Usuario")
                        .WithMany("Suscripciones")
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Plan");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("TelefoniaMovilBackend.Models.Usuario", b =>
                {
                    b.Navigation("Suscripciones");
                });
#pragma warning restore 612, 618
        }
    }
}
