using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductImportJobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductImportJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginalFileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    StoredFilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    ImportedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImportedByName = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    FinishedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DurationMs = table.Column<long>(type: "bigint", nullable: true),
                    TotalRows = table.Column<int>(type: "int", nullable: false),
                    SkuCount = table.Column<int>(type: "int", nullable: false),
                    CreatedCount = table.Column<int>(type: "int", nullable: false),
                    UpdatedCount = table.Column<int>(type: "int", nullable: false),
                    ErrorCount = table.Column<int>(type: "int", nullable: false),
                    SummaryMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImportJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductImportJobItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    RowNumber = table.Column<int>(type: "int", nullable: false),
                    Sku = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    DownloadedImages = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ImportedAttributes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImportJobItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImportJobItems_ProductImportJobs_JobId",
                        column: x => x.JobId,
                        principalTable: "ProductImportJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductImportJobItems_JobId_RowNumber",
                table: "ProductImportJobItems",
                columns: new[] { "JobId", "RowNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductImportJobItems_Sku",
                table: "ProductImportJobItems",
                column: "Sku");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImportJobItems_Status",
                table: "ProductImportJobItems",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImportJobs_CreatedAt",
                table: "ProductImportJobs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImportJobs_Status",
                table: "ProductImportJobs",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductImportJobItems");

            migrationBuilder.DropTable(
                name: "ProductImportJobs");
        }
    }
}
