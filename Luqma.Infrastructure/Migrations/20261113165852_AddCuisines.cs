using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Luqma.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCuisines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cuisines",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    NameAr = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_Cuisines", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "StoreCuisines",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    CuisineId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_StoreCuisines", x => x.ID);
                    table.ForeignKey(
                        name: "FK_StoreCuisines_Cuisines_CuisineId",
                        column: x => x.CuisineId,
                        principalTable: "Cuisines",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreCuisines_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Cuisines",
                columns: new[] { "ID", "ArchivedById", "ArchivedByName", "ArchivedDateTime", "CreatedById", "CreatedByName", "CreatedDateTime", "DisplayOrder", "ImageUrl", "IsActive", "IsArchived", "ModifiedById", "ModifiedByName", "ModifiedDateTime", "NameAr", "NameEn" },
                values: new object[,]
                {
                    { new Guid("7a2d4c31-5b6e-4f80-9c11-000000000001"), null, null, null, null, null, null, 1, null, true, false, null, null, null, "مشويات", "Grill" },
                    { new Guid("7a2d4c31-5b6e-4f80-9c11-000000000002"), null, null, null, null, null, null, 2, null, true, false, null, null, null, "أسماك", "Seafood" },
                    { new Guid("7a2d4c31-5b6e-4f80-9c11-000000000003"), null, null, null, null, null, null, 3, null, true, false, null, null, null, "شرقي", "Eastern" },
                    { new Guid("7a2d4c31-5b6e-4f80-9c11-000000000004"), null, null, null, null, null, null, 4, null, true, false, null, null, null, "غربي", "Western" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cuisines_IsArchived_IsActive_DisplayOrder",
                table: "Cuisines",
                columns: new[] { "IsArchived", "IsActive", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Cuisines_NameAr",
                table: "Cuisines",
                column: "NameAr",
                unique: true,
                filter: "\"IsArchived\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_StoreCuisines_CuisineId_IsArchived",
                table: "StoreCuisines",
                columns: new[] { "CuisineId", "IsArchived" });

            migrationBuilder.CreateIndex(
                name: "IX_StoreCuisines_StoreId_CuisineId",
                table: "StoreCuisines",
                columns: new[] { "StoreId", "CuisineId" },
                unique: true,
                filter: "\"IsArchived\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreCuisines");

            migrationBuilder.DropTable(
                name: "Cuisines");
        }
    }
}
