using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Luqma.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "Stores",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryFee",
                table: "Stores",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryTimeMinutes",
                table: "Stores",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "Stores",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "Stores",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Stores",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Stores",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "PriceLevel",
                table: "Stores",
                type: "integer",
                nullable: false,
                // Existing stores default to Moderate (2), matching the domain default
                defaultValue: 2);

            migrationBuilder.AddColumn<int>(
                name: "RatingsCount",
                table: "Stores",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StoreImages",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_StoreImages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_StoreImages_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Stores_AverageRating",
                table: "Stores",
                sql: "\"AverageRating\" >= 0 AND \"AverageRating\" <= 5");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Stores_DeliveryFee",
                table: "Stores",
                sql: "\"DeliveryFee\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Stores_DeliveryTimeMinutes",
                table: "Stores",
                sql: "\"DeliveryTimeMinutes\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Stores_RatingsCount",
                table: "Stores",
                sql: "\"RatingsCount\" >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_StoreImages_StoreId_IsArchived",
                table: "StoreImages",
                columns: new[] { "StoreId", "IsArchived" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreImages");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Stores_AverageRating",
                table: "Stores");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Stores_DeliveryFee",
                table: "Stores");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Stores_DeliveryTimeMinutes",
                table: "Stores");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Stores_RatingsCount",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "DeliveryFee",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "DeliveryTimeMinutes",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "PriceLevel",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "RatingsCount",
                table: "Stores");
        }
    }
}
