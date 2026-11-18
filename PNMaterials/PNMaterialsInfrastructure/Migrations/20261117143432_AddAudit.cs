using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PNMaterialsInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Audit_CreatedAt",
                table: "PurchaseRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Audit_CreatedByUserId",
                table: "PurchaseRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Audit_EditedAt",
                table: "PurchaseRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Audit_EditedByUserId",
                table: "PurchaseRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Audit_CreatedAt",
                table: "Materials",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Audit_CreatedByUserId",
                table: "Materials",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Audit_EditedAt",
                table: "Materials",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Audit_EditedByUserId",
                table: "Materials",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Audit_CreatedAt",
                table: "MaterialGroups",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Audit_CreatedByUserId",
                table: "MaterialGroups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Audit_EditedAt",
                table: "MaterialGroups",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Audit_EditedByUserId",
                table: "MaterialGroups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Audit_CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Audit_CreatedByUserId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Audit_EditedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Audit_EditedByUserId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Audit_CreatedAt",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "Audit_CreatedByUserId",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "Audit_EditedAt",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "Audit_EditedByUserId",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "Audit_CreatedAt",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "Audit_CreatedByUserId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "Audit_EditedAt",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "Audit_EditedByUserId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "Audit_CreatedAt",
                table: "MaterialGroups");

            migrationBuilder.DropColumn(
                name: "Audit_CreatedByUserId",
                table: "MaterialGroups");

            migrationBuilder.DropColumn(
                name: "Audit_EditedAt",
                table: "MaterialGroups");

            migrationBuilder.DropColumn(
                name: "Audit_EditedByUserId",
                table: "MaterialGroups");

            migrationBuilder.DropColumn(
                name: "Audit_CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Audit_CreatedByUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Audit_EditedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Audit_EditedByUserId",
                table: "AspNetUsers");
        }
    }
}
