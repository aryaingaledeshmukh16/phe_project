using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PHE.API.Migrations
{
    /// <inheritdoc />
    public partial class AddJEApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApplicationDate",
                table: "Applicants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedLayoutMapPath",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EntryDate",
                table: "Applicants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstimateFilePath",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeoTagPhotoPath",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOTPVerified",
                table: "Applicants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Applicants",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Applicants",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoOfPlots",
                table: "Applicants",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NocMatter",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OTP",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OTPExpiry",
                table: "Applicants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PendingAtEmployee",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PipelineExists",
                table: "Applicants",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResolveDays",
                table: "Applicants",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SatBaraPath",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScrutinyStatus",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShowAmountAsPerNoOfPlots",
                table: "Applicants",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxNocPath",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalEstimateAmount",
                table: "Applicants",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalPlots",
                table: "Applicants",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCode",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JEApplications",
                columns: table => new
                {
                    SrNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApplicantName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Peth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Zone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JagechaAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManjurLayoutNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManjurLayoutDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MilakataKramanka = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrutinyStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxNoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SatBaraUtara = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedLayoutMap = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SitePhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JEApplications", x => x.SrNo);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JEApplications");

            migrationBuilder.DropColumn(
                name: "ApplicationDate",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "ApprovedLayoutMapPath",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "EntryDate",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "EstimateFilePath",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "GeoTagPhotoPath",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "IsOTPVerified",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "NoOfPlots",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "NocMatter",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "OTP",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "OTPExpiry",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "PendingAtEmployee",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "PipelineExists",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "ResolveDays",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "SatBaraPath",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "ScrutinyStatus",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "ShowAmountAsPerNoOfPlots",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "TaxNocPath",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "TotalEstimateAmount",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "TotalPlots",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "UserCode",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Applicants");
        }
    }
}
