// <auto-generated />
using HazelcastDemo.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.EntityFrameworkCore.Metadata;

#nullable disable

namespace HazelcastDemo.Migrations
{
    [DbContext(typeof(HazelCastContext))]
    [Migration("20220826042708_hazelcastTe")]
    partial class hazelcastTe
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            OracleModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("HazelcastDemo.Models.HazelcastModel", b =>
                {
                    b.Property<string>("HKey")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("HValue")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.HasKey("HKey");

                    b.ToTable("hazelcasts");
                });
#pragma warning restore 612, 618
        }
    }
}
