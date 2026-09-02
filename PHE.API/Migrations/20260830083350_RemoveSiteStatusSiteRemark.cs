using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PHE.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSiteStatusSiteRemark : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SiteRemark",
                table: "ApplicantsLog");

            migrationBuilder.DropColumn(
                name: "SiteStatus",
                table: "ApplicantsLog");

            migrationBuilder.DropColumn(
                name: "SiteRemark",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "SiteStatus",
                table: "Applicants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SiteRemark",
                table: "ApplicantsLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteStatus",
                table: "ApplicantsLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteRemark",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteStatus",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
