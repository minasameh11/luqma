using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Luqma.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMainCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MainCategories",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    NameAr = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IconUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedByName = table.Column<string>(type: "text", nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArchivedById = table.Column<string>(type: "text", nullable: true),
                    ArchivedByName = table.Column<string>(type: "text", nullable: true),
                    ArchivedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainCategories", x => x.ID);
                });

            migrationBuilder.InsertData(
                table: "MainCategories",
                columns: new[] { "ID", "ArchivedById", "ArchivedByName", "ArchivedDateTime", "CreatedById", "CreatedByName", "CreatedDateTime", "DisplayOrder", "IconUrl", "IsActive", "IsArchived", "ModifiedById", "ModifiedByName", "ModifiedDateTime", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { new Guid("6f1c7b8e-1a2b-4c3d-8e4f-000000000001"), null, null, null, null, null, null, 1, "/icons/categories/restaurants.png", true, false, null, null, null, "مطاعم", "Restaurants" },
                    { new Guid("6f1c7b8e-1a2b-4c3d-8e4f-000000000002"), null, null, null, null, null, null, 2, "/icons/categories/pharmacy.png", true, false, null, null, null, "صيدلية", "Pharmacy" },
                    { new Guid("6f1c7b8e-1a2b-4c3d-8e4f-000000000003"), null, null, null, null, null, null, 3, "/icons/categories/sweets.png", true, false, null, null, null, "حلويات", "Sweets" },
                    { new Guid("6f1c7b8e-1a2b-4c3d-8e4f-000000000004"), null, null, null, null, null, null, 4, "/icons/categories/beverages.png", true, false, null, null, null, "مشروبات", "Beverages" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MainCategories_IsArchived_DisplayOrder",
                table: "MainCategories",
                columns: new[] { "IsArchived", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_MainCategories_NameAr",
                table: "MainCategories",
                column: "NameAr",
                unique: true,
                filter: "\"IsArchived\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MainCategories");
        }
    }
}
