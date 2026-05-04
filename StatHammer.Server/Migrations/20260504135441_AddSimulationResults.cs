using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StatHammer.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddSimulationResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SimulationResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UnitAId = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitBId = table.Column<int>(type: "INTEGER", nullable: false),
                    SimulationCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimulationResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimulationResults_Units_UnitAId",
                        column: x => x.UnitAId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SimulationResults_Units_UnitBId",
                        column: x => x.UnitBId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TurnStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SimulationResultId = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Side = table.Column<string>(type: "TEXT", nullable: false),
                    AvgModelsAlive = table.Column<double>(type: "REAL", nullable: false),
                    AvgWoundsAlive = table.Column<double>(type: "REAL", nullable: false),
                    AvgHits = table.Column<double>(type: "REAL", nullable: false),
                    AvgWounds = table.Column<double>(type: "REAL", nullable: false),
                    AvgSuccessfulSaves = table.Column<double>(type: "REAL", nullable: false),
                    AvgBlockedByFnp = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurnStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TurnStats_SimulationResults_SimulationResultId",
                        column: x => x.SimulationResultId,
                        principalTable: "SimulationResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeaponStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TurnStatId = table.Column<int>(type: "INTEGER", nullable: false),
                    WeaponId = table.Column<int>(type: "INTEGER", nullable: false),
                    AvgHits = table.Column<double>(type: "REAL", nullable: false),
                    AvgWounds = table.Column<double>(type: "REAL", nullable: false),
                    AvgDamage = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeaponStats_TurnStats_TurnStatId",
                        column: x => x.TurnStatId,
                        principalTable: "TurnStats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeaponStats_Weapons_WeaponId",
                        column: x => x.WeaponId,
                        principalTable: "Weapons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SimulationResults_UnitAId",
                table: "SimulationResults",
                column: "UnitAId");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationResults_UnitBId",
                table: "SimulationResults",
                column: "UnitBId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnStats_SimulationResultId_TurnNumber_Side",
                table: "TurnStats",
                columns: new[] { "SimulationResultId", "TurnNumber", "Side" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeaponStats_TurnStatId_WeaponId",
                table: "WeaponStats",
                columns: new[] { "TurnStatId", "WeaponId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeaponStats_WeaponId",
                table: "WeaponStats",
                column: "WeaponId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeaponStats");

            migrationBuilder.DropTable(
                name: "TurnStats");

            migrationBuilder.DropTable(
                name: "SimulationResults");
        }
    }
}
