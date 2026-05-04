using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StatHammer.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Move = table.Column<int>(type: "INTEGER", nullable: false),
                    Toughness = table.Column<int>(type: "INTEGER", nullable: false),
                    Save = table.Column<int>(type: "INTEGER", nullable: false),
                    InvulnerableSave = table.Column<int>(type: "INTEGER", nullable: true),
                    Wounds = table.Column<int>(type: "INTEGER", nullable: false),
                    Leadership = table.Column<int>(type: "INTEGER", nullable: false),
                    OC = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModelAbilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ModelId = table.Column<int>(type: "INTEGER", nullable: false),
                    AbilityId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelAbilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelAbilities_Abilities_AbilityId",
                        column: x => x.AbilityId,
                        principalTable: "Abilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModelAbilities_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModelWargears",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ModelId = table.Column<int>(type: "INTEGER", nullable: false),
                    WargearId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelWargears", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelWargears_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModelWargears_Wargears_WargearId",
                        column: x => x.WargearId,
                        principalTable: "Wargears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModelWeapons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ModelId = table.Column<int>(type: "INTEGER", nullable: false),
                    WeaponId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelWeapons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelWeapons_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModelWeapons_Weapons_WeaponId",
                        column: x => x.WeaponId,
                        principalTable: "Weapons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModelAbilities_AbilityId",
                table: "ModelAbilities",
                column: "AbilityId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelAbilities_ModelId_AbilityId",
                table: "ModelAbilities",
                columns: new[] { "ModelId", "AbilityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModelWargears_ModelId_WargearId",
                table: "ModelWargears",
                columns: new[] { "ModelId", "WargearId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModelWargears_WargearId",
                table: "ModelWargears",
                column: "WargearId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelWeapons_ModelId_WeaponId",
                table: "ModelWeapons",
                columns: new[] { "ModelId", "WeaponId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModelWeapons_WeaponId",
                table: "ModelWeapons",
                column: "WeaponId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModelAbilities");

            migrationBuilder.DropTable(
                name: "ModelWargears");

            migrationBuilder.DropTable(
                name: "ModelWeapons");

            migrationBuilder.DropTable(
                name: "Models");
        }
    }
}
