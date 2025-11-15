using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCollationInHealthPlanAndBeneficiaryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE COLLATION IF NOT EXISTS case_insensitive (
                    provider = icu,
                    locale = 'und-u-ks-level2',
                    deterministic = false
                );
            ");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "health_plans",
                type: "text",
                maxLength: 255,
                nullable: false,
                collation: "case_insensitive",
                oldClrType: typeof(string),
                oldType: "text",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "full_name",
                table: "beneficiaries",
                type: "text",
                maxLength: 255,
                nullable: false,
                collation: "case_insensitive",
                oldClrType: typeof(string),
                oldType: "text",
                oldMaxLength: 255);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "health_plans",
                type: "text",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldMaxLength: 255,
                oldCollation: "case_insensitive");

            migrationBuilder.AlterColumn<string>(
                name: "full_name",
                table: "beneficiaries",
                type: "text",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldMaxLength: 255,
                oldCollation: "case_insensitive");
        }
    }
}
