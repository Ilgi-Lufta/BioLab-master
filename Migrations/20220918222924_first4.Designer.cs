﻿// <auto-generated />
using System;
using BioLab.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BioLab.Migrations
{
    [DbContext(typeof(MyContext))]
    [Migration("20220918222924_first4")]
    partial class first4
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("BioLab.Models.Analiza", b =>
                {
                    b.Property<int>("AnalizaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<float>("Cmimi")
                        .HasColumnType("float");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Emri")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Njesia")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Norma")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<float>("Rezultati")
                        .HasColumnType("float");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("AnalizaId");

                    b.ToTable("Analizat");
                });

            modelBuilder.Entity("BioLab.Models.FleteAnalize", b =>
                {
                    b.Property<int>("FleteAnalizeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Emri")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("PacientId")
                        .HasColumnType("int");

                    b.Property<bool>("Pagesa")
                        .HasColumnType("tinyint(1)");

                    b.Property<float>("Paguar")
                        .HasColumnType("float");

                    b.Property<float>("Totali")
                        .HasColumnType("float");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<float>("Zbritja")
                        .HasColumnType("float");

                    b.Property<bool>("model")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("FleteAnalizeId");

                    b.HasIndex("PacientId");

                    b.ToTable("FleteAnalizes");
                });

            modelBuilder.Entity("BioLab.Models.mtm", b =>
                {
                    b.Property<int>("mtmID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AnalizaId")
                        .HasColumnType("int");

                    b.Property<int>("FleteAnalizeId")
                        .HasColumnType("int");

                    b.HasKey("mtmID");

                    b.HasIndex("AnalizaId");

                    b.HasIndex("FleteAnalizeId");

                    b.ToTable("mtms");
                });

            modelBuilder.Entity("BioLab.Models.Pacient", b =>
                {
                    b.Property<int>("PacientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Emripacientit")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Gjinia")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Mosha")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Tipi")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("PacientId");

                    b.ToTable("Pacients");
                });

            modelBuilder.Entity("BioLab.Models.FleteAnalize", b =>
                {
                    b.HasOne("BioLab.Models.Pacient", "MyPacient")
                        .WithMany("MYfleteanaliz")
                        .HasForeignKey("PacientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MyPacient");
                });

            modelBuilder.Entity("BioLab.Models.mtm", b =>
                {
                    b.HasOne("BioLab.Models.Analiza", "Myanaliz")
                        .WithMany("mtms")
                        .HasForeignKey("AnalizaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BioLab.Models.FleteAnalize", "Myflanaliz")
                        .WithMany("mtms")
                        .HasForeignKey("FleteAnalizeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Myanaliz");

                    b.Navigation("Myflanaliz");
                });

            modelBuilder.Entity("BioLab.Models.Analiza", b =>
                {
                    b.Navigation("mtms");
                });

            modelBuilder.Entity("BioLab.Models.FleteAnalize", b =>
                {
                    b.Navigation("mtms");
                });

            modelBuilder.Entity("BioLab.Models.Pacient", b =>
                {
                    b.Navigation("MYfleteanaliz");
                });
#pragma warning restore 612, 618
        }
    }
}
