using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Luqma.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCartItemOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CartItems_UserId_MenuItemId",
                table: "CartItems");

            migrationBuilder.AddColumn<string>(
                name: "ConfigurationSignature",
                table: "CartItems",
                type: "character varying(600)",
                maxLength: 600,
                nullable: false,
                defaultValue: "none|");

            migrationBuilder.AddColumn<Guid>(
                name: "MenuItemOptionId",
                table: "CartItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CartItemAddons",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CartItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    MenuItemAddonId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_CartItemAddons", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CartItemAddons_CartItems_CartItemId",
                        column: x => x.CartItemId,
                        principalTable: "CartItems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItemAddons_MenuItemAddons_MenuItemAddonId",
                        column: x => x.MenuItemAddonId,
                        principalTable: "MenuItemAddons",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_MenuItemOptionId",
                table: "CartItems",
                column: "MenuItemOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId_MenuItemId_ConfigurationSignature",
                table: "CartItems",
                columns: new[] { "UserId", "MenuItemId", "ConfigurationSignature" },
                unique: true,
                filter: "\"IsArchived\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemAddons_CartItemId_MenuItemAddonId",
                table: "CartItemAddons",
                columns: new[] { "CartItemId", "MenuItemAddonId" },
                unique: true,
                filter: "\"IsArchived\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemAddons_MenuItemAddonId",
                table: "CartItemAddons",
                column: "MenuItemAddonId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_MenuItemOptions_MenuItemOptionId",
                table: "CartItems",
                column: "MenuItemOptionId",
                principalTable: "MenuItemOptions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_MenuItemOptions_MenuItemOptionId",
                table: "CartItems");

            migrationBuilder.DropTable(
                name: "CartItemAddons");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_MenuItemOptionId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_UserId_MenuItemId_ConfigurationSignature",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ConfigurationSignature",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "MenuItemOptionId",
                table: "CartItems");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId_MenuItemId",
                table: "CartItems",
                columns: new[] { "UserId", "MenuItemId" },
                unique: true,
                filter: "\"IsArchived\" = false");
        }
    }
}
