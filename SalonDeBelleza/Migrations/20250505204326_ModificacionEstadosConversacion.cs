using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalonDeBelleza.Migrations
{
    /// <inheritdoc />
    public partial class ModificacionEstadosConversacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Servicio",
                table: "EstadosConversacion",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Hora",
                table: "EstadosConversacion",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EstadosConversacion",
                keyColumn: "Servicio",
                keyValue: null,
                column: "Servicio",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Servicio",
                table: "EstadosConversacion",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "EstadosConversacion",
                keyColumn: "Hora",
                keyValue: null,
                column: "Hora",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Hora",
                table: "EstadosConversacion",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
