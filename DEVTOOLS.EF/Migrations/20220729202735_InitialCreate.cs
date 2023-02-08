using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEVTOOLS.EF.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.ProjectId);
                });

            migrationBuilder.CreateTable(
                name: "DatabaseSource",
                columns: table => new
                {
                    DatabaseSourceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    DatabaseSourceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatabaseSourceType = table.Column<int>(type: "int", nullable: false),
                    DatabaseSourceDNS = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatabaseSource", x => x.DatabaseSourceId);
                    table.ForeignKey(
                        name: "FK_DatabaseSource_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTemplate",
                columns: table => new
                {
                    ProjectTemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    ProjectTemplateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectTemplateObservations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectTemplateSourceCodeType = table.Column<int>(type: "int", nullable: false),
                    ProjectTemplateContent = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTemplate", x => x.ProjectTemplateId);
                    table.ForeignKey(
                        name: "FK_ProjectTemplate_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Entity",
                columns: table => new
                {
                    EntityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityTableName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityProjectDatabaseSourceId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entity", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_Entity_DatabaseSource_EntityProjectDatabaseSourceId",
                        column: x => x.EntityProjectDatabaseSourceId,
                        principalTable: "DatabaseSource",
                        principalColumn: "DatabaseSourceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Entity_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Column",
                columns: table => new
                {
                    ColumnId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColumnIndex = table.Column<int>(type: "int", nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    ColumnPrimaryKey = table.Column<bool>(type: "bit", nullable: false),
                    ColumnForeignKey = table.Column<bool>(type: "bit", nullable: false),
                    ColumnParentForeignKey = table.Column<int>(type: "int", nullable: false),
                    ColumnName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ColumnDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ColumnType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Column", x => x.ColumnId);
                    table.ForeignKey(
                        name: "FK_Column_Entity_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entity",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityTemplate",
                columns: table => new
                {
                    EntityTemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    EntityTemplateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityTemplateObservations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityTemplateSourceCodeType = table.Column<int>(type: "int", nullable: false),
                    EntityTemplateContent = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTemplate", x => x.EntityTemplateId);
                    table.ForeignKey(
                        name: "FK_EntityTemplate_Entity_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entity",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ColumnTemplate",
                columns: table => new
                {
                    ColumnTemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColumnId = table.Column<int>(type: "int", nullable: false),
                    ColumnTemplateSourceCodeType = table.Column<int>(type: "int", nullable: false),
                    ColumnTemplateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ColumnTemplateObservations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ColumnTemplateContent = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnTemplate", x => x.ColumnTemplateId);
                    table.ForeignKey(
                        name: "FK_ColumnTemplate_Column_ColumnId",
                        column: x => x.ColumnId,
                        principalTable: "Column",
                        principalColumn: "ColumnId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Column_EntityId",
                table: "Column",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ColumnTemplate_ColumnId",
                table: "ColumnTemplate",
                column: "ColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_DatabaseSource_ProjectId",
                table: "DatabaseSource",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Entity_EntityProjectDatabaseSourceId",
                table: "Entity",
                column: "EntityProjectDatabaseSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Entity_ProjectId",
                table: "Entity",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityTemplate_EntityId",
                table: "EntityTemplate",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTemplate_ProjectId",
                table: "ProjectTemplate",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColumnTemplate");

            migrationBuilder.DropTable(
                name: "EntityTemplate");

            migrationBuilder.DropTable(
                name: "ProjectTemplate");

            migrationBuilder.DropTable(
                name: "Column");

            migrationBuilder.DropTable(
                name: "Entity");

            migrationBuilder.DropTable(
                name: "DatabaseSource");

            migrationBuilder.DropTable(
                name: "Project");
        }
    }
}
