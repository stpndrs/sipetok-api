using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sipetok_api.Migrations
{
    /// <inheritdoc />
    public partial class AddAllRelationsModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_customer_id",
                table: "Transactions",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_tenant_id",
                table: "Transactions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionDetails_transaction_id",
                table: "TransactionDetails",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_user_id",
                table: "Tenants",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Operationals_tenant_id",
                table: "Operationals",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_Eggs_category_id",
                table: "Eggs",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Eggs_tenant_id",
                table: "Eggs",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_user_id",
                table: "Customers",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_user_id",
                table: "Customers",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Eggs_EggCategories_category_id",
                table: "Eggs",
                column: "category_id",
                principalTable: "EggCategories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Eggs_Tenants_tenant_id",
                table: "Eggs",
                column: "tenant_id",
                principalTable: "Tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Operationals_Tenants_tenant_id",
                table: "Operationals",
                column: "tenant_id",
                principalTable: "Tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_Users_user_id",
                table: "Tenants",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionDetails_Transactions_transaction_id",
                table: "TransactionDetails",
                column: "transaction_id",
                principalTable: "Transactions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Customers_customer_id",
                table: "Transactions",
                column: "customer_id",
                principalTable: "Customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Tenants_tenant_id",
                table: "Transactions",
                column: "tenant_id",
                principalTable: "Tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Users_user_id",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Eggs_EggCategories_category_id",
                table: "Eggs");

            migrationBuilder.DropForeignKey(
                name: "FK_Eggs_Tenants_tenant_id",
                table: "Eggs");

            migrationBuilder.DropForeignKey(
                name: "FK_Operationals_Tenants_tenant_id",
                table: "Operationals");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_Users_user_id",
                table: "Tenants");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionDetails_Transactions_transaction_id",
                table: "TransactionDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Customers_customer_id",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Tenants_tenant_id",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_customer_id",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_tenant_id",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_TransactionDetails_transaction_id",
                table: "TransactionDetails");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_user_id",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Operationals_tenant_id",
                table: "Operationals");

            migrationBuilder.DropIndex(
                name: "IX_Eggs_category_id",
                table: "Eggs");

            migrationBuilder.DropIndex(
                name: "IX_Eggs_tenant_id",
                table: "Eggs");

            migrationBuilder.DropIndex(
                name: "IX_Customers_user_id",
                table: "Customers");
        }
    }
}
