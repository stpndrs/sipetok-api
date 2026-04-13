using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sipetok_api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchemaToNativeEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "Users",
                type: "enum('ACTIVE', 'INACTIVE')",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "Users",
                type: "enum('ADMIN', 'TENANT', 'CUSTOMER')",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "enum('ACTIVE', 'INACTIVE')");

            migrationBuilder.AlterColumn<int>(
                name: "role",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "enum('ADMIN', 'TENANT', 'CUSTOMER')");
        }
    }
}
