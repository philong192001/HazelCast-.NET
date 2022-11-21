using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HazelcastDemo.Migrations
{
    public partial class hazelcastTe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "hazelcasts",
                columns: table => new
                {
                    HKey = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    HValue = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hazelcasts", x => x.HKey);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hazelcasts");
        }
    }
}
