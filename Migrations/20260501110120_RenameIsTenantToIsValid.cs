using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sipetok_api.Migrations
{
    /// <inheritdoc />
    public partial class RenameIsTenantToIsValid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isTenant",
                table: "Tenants",
                newName: "isValid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isValid",
                table: "Tenants",
                newName: "isTenant");
        }
    }
}
