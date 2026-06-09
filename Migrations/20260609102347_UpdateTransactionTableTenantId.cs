using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sipetok_api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransactionTableTenantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TenantId",
                table: "Transactions",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Tenants_TenantId",
                table: "Transactions",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Tenants_TenantId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TenantId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Transactions");
        }
    }
}
