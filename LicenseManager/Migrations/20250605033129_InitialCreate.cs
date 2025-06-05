using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicenseManager.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Applicat__3214EC07BAD0BE80", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Clients__3214EC0776FDC569", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeatureGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FeatureG__3214EC07AF331C17", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientApplicationMapping",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    LicenseId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FK_ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FK_ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ActivationsCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ClientAp__3214EC074B5C0135", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientApplicationMapping_Applications",
                        column: x => x.FK_ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientApplicationMapping_Clients",
                        column: x => x.FK_ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FK_FeatureGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Features_FeatureGroups",
                        column: x => x.FK_FeatureGroupId,
                        principalTable: "FeatureGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClientApplicationLicensedFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    FK_ClientApplicationMappingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Features = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientApplicationLicensedFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientApplicationLicensedFeatures_ClientApplicationMapping",
                        column: x => x.FK_ClientApplicationMappingId,
                        principalTable: "ClientApplicationMapping",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientApplicationLicensedFeatures_FK_ClientApplicationMappingId",
                table: "ClientApplicationLicensedFeatures",
                column: "FK_ClientApplicationMappingId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientApplicationMapping_FK_ApplicationId",
                table: "ClientApplicationMapping",
                column: "FK_ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientApplicationMapping_FK_ClientId",
                table: "ClientApplicationMapping",
                column: "FK_ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Features_FK_FeatureGroupId",
                table: "Features",
                column: "FK_FeatureGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientApplicationLicensedFeatures");

            migrationBuilder.DropTable(
                name: "Features");

            migrationBuilder.DropTable(
                name: "ClientApplicationMapping");

            migrationBuilder.DropTable(
                name: "FeatureGroups");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
