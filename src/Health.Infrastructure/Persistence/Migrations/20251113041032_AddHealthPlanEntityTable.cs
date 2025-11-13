using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHealthPlanEntityTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "health_plans",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", maxLength: 255, nullable: false),
                    ans_registration_code = table.Column<string>(type: "text", maxLength: 6, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_health_plans_id", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ans_registration_code",
                table: "health_plans",
                column: "ans_registration_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "health_plans");
        }
    }
}
