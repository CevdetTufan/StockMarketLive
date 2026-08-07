using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMarketLive.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStockSignalSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiReason",
                table: "StockSignals");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "StockSignals");

            migrationBuilder.DropColumn(
                name: "Signal",
                table: "StockSignals");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "StockSignals",
                newName: "PublishedAt");

            migrationBuilder.RenameIndex(
                name: "IX_StockSignals_Timestamp",
                table: "StockSignals",
                newName: "IX_StockSignals_PublishedAt");

            migrationBuilder.AddColumn<string>(
                name: "Recommendation",
                table: "StockSignals",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "StockSignals",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Recommendation",
                table: "StockSignals");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "StockSignals");

            migrationBuilder.RenameColumn(
                name: "PublishedAt",
                table: "StockSignals",
                newName: "Timestamp");

            migrationBuilder.RenameIndex(
                name: "IX_StockSignals_PublishedAt",
                table: "StockSignals",
                newName: "IX_StockSignals_Timestamp");

            migrationBuilder.AddColumn<string>(
                name: "AiReason",
                table: "StockSignals",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "StockSignals",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Signal",
                table: "StockSignals",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
