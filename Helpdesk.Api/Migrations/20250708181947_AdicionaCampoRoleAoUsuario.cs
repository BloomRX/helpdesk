using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.Api.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaCampoRoleAoUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Usuarios",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Usuarios");
        }
    }
}
