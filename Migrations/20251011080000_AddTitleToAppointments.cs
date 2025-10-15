using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace citas.Migrations
{
    /// <inheritdoc />
    public partial class AddTitleToAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE Appointments ADD COLUMN Title VARCHAR(255) NOT NULL DEFAULT '';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE Appointments DROP COLUMN Title;");
        }
    }
}
