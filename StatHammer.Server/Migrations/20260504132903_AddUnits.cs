using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StatHammer.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UnitAbilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UnitId = table.Column<int>(type: "INTEGER", nullable: false),
                    AbilityId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitAbilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitAbilities_Abilities_AbilityId",
                        column: x => x.AbilityId,
                        principalTable: "Abilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitAbilities_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnitKeywords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UnitId = table.Column<int>(type: "INTEGER", nullable: false),
                    KeywordId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitKeywords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitKeywords_Keywords_KeywordId",
                        column: x => x.KeywordId,
                        principalTable: "Keywords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitKeywords_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnitModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UnitId = table.Column<int>(type: "INTEGER", nullable: false),
                    ModelId = table.Column<int>(type: "INTEGER", nullable: false),
                    MinCount = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitModels_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitModels_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnitOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UnitId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    MaxSelections = table.Column<int>(type: "INTEGER", nullable: false),
                    GroupKey = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitOptions_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OptionItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UnitOptionId = table.Column<int>(type: "INTEGER", nullable: false),
                    WeaponId = table.Column<int>(type: "INTEGER", nullable: true),
                    WargearId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptionItems_UnitOptions_UnitOptionId",
                        column: x => x.UnitOptionId,
                        principalTable: "UnitOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OptionItems_Wargears_WargearId",
                        column: x => x.WargearId,
                        principalTable: "Wargears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OptionItems_Weapons_WeaponId",
                        column: x => x.WeaponId,
                        principalTable: "Weapons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OptionItems_UnitOptionId",
                table: "OptionItems",
                column: "UnitOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionItems_WargearId",
                table: "OptionItems",
                column: "WargearId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionItems_WeaponId",
                table: "OptionItems",
                column: "WeaponId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitAbilities_AbilityId",
                table: "UnitAbilities",
                column: "AbilityId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitAbilities_UnitId_AbilityId",
                table: "UnitAbilities",
                columns: new[] { "UnitId", "AbilityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitKeywords_KeywordId",
                table: "UnitKeywords",
                column: "KeywordId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitKeywords_UnitId_KeywordId",
                table: "UnitKeywords",
                columns: new[] { "UnitId", "KeywordId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitModels_ModelId",
                table: "UnitModels",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitModels_UnitId_ModelId",
                table: "UnitModels",
                columns: new[] { "UnitId", "ModelId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitOptions_UnitId",
                table: "UnitOptions",
                column: "UnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OptionItems");

            migrationBuilder.DropTable(
                name: "UnitAbilities");

            migrationBuilder.DropTable(
                name: "UnitKeywords");

            migrationBuilder.DropTable(
                name: "UnitModels");

            migrationBuilder.DropTable(
                name: "UnitOptions");
        }
    }
}
