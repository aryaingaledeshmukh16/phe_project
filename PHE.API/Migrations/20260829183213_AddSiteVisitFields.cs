using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PHE.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteVisitFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Applicants]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Applicants', 'LayoutYesNo') IS NULL
        ALTER TABLE [Applicants] ADD [LayoutYesNo] nvarchar(max) NULL;

    IF COL_LENGTH('dbo.Applicants', 'TotalPlots') IS NULL
        ALTER TABLE [Applicants] ADD [TotalPlots] int NULL;

    IF COL_LENGTH('dbo.Applicants', 'PlotsApplicableForThisNoc') IS NULL
        ALTER TABLE [Applicants] ADD [PlotsApplicableForThisNoc] int NULL;

    IF COL_LENGTH('dbo.Applicants', 'AmountForPlots') IS NULL
        ALTER TABLE [Applicants] ADD [AmountForPlots] decimal(18,2) NULL;

    IF COL_LENGTH('dbo.Applicants', 'SiteVisitEstimateDocumentPath') IS NULL
        ALTER TABLE [Applicants] ADD [SiteVisitEstimateDocumentPath] nvarchar(max) NULL;

    IF COL_LENGTH('dbo.Applicants', 'SiteVisitGeoTagPhotoPath') IS NULL
        ALTER TABLE [Applicants] ADD [SiteVisitGeoTagPhotoPath] nvarchar(max) NULL;

    IF COL_LENGTH('dbo.Applicants', 'SiteStatus') IS NULL
        ALTER TABLE [Applicants] ADD [SiteStatus] nvarchar(max) NULL;

    IF COL_LENGTH('dbo.Applicants', 'SiteRemark') IS NULL
        ALTER TABLE [Applicants] ADD [SiteRemark] nvarchar(max) NULL;
END;
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[ApplicantsLog]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.ApplicantsLog', 'LayoutYesNo') IS NULL
        ALTER TABLE [ApplicantsLog] ADD [LayoutYesNo] nvarchar(max) NULL;

    IF COL_LENGTH('dbo.ApplicantsLog', 'TotalPlots') IS NULL
        ALTER TABLE [ApplicantsLog] ADD [TotalPlots] int NULL;

    IF COL_LENGTH('dbo.ApplicantsLog', 'PlotsApplicableForThisNoc') IS NULL
        ALTER TABLE [ApplicantsLog] ADD [PlotsApplicableForThisNoc] int NULL;

    IF COL_LENGTH('dbo.ApplicantsLog', 'AmountForPlots') IS NULL
        ALTER TABLE [ApplicantsLog] ADD [AmountForPlots] decimal(18,2) NULL;

    IF COL_LENGTH('dbo.ApplicantsLog', 'SiteVisitEstimateDocumentPath') IS NULL
        ALTER TABLE [ApplicantsLog] ADD [SiteVisitEstimateDocumentPath] nvarchar(max) NULL;

    IF COL_LENGTH('dbo.ApplicantsLog', 'SiteVisitGeoTagPhotoPath') IS NULL
        ALTER TABLE [ApplicantsLog] ADD [SiteVisitGeoTagPhotoPath] nvarchar(max) NULL;

    IF COL_LENGTH('dbo.ApplicantsLog', 'SiteStatus') IS NULL
        ALTER TABLE [ApplicantsLog] ADD [SiteStatus] nvarchar(max) NULL;

    IF COL_LENGTH('dbo.ApplicantsLog', 'SiteRemark') IS NULL
        ALTER TABLE [ApplicantsLog] ADD [SiteRemark] nvarchar(max) NULL;
END;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Applicants]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Applicants', 'LayoutYesNo') IS NOT NULL
        ALTER TABLE [Applicants] DROP COLUMN [LayoutYesNo];

    IF COL_LENGTH('dbo.Applicants', 'TotalPlots') IS NOT NULL
        ALTER TABLE [Applicants] DROP COLUMN [TotalPlots];

    IF COL_LENGTH('dbo.Applicants', 'PlotsApplicableForThisNoc') IS NOT NULL
        ALTER TABLE [Applicants] DROP COLUMN [PlotsApplicableForThisNoc];

    IF COL_LENGTH('dbo.Applicants', 'AmountForPlots') IS NOT NULL
        ALTER TABLE [Applicants] DROP COLUMN [AmountForPlots];

    IF COL_LENGTH('dbo.Applicants', 'SiteVisitEstimateDocumentPath') IS NOT NULL
        ALTER TABLE [Applicants] DROP COLUMN [SiteVisitEstimateDocumentPath];

    IF COL_LENGTH('dbo.Applicants', 'SiteVisitGeoTagPhotoPath') IS NOT NULL
        ALTER TABLE [Applicants] DROP COLUMN [SiteVisitGeoTagPhotoPath];

    IF COL_LENGTH('dbo.Applicants', 'SiteStatus') IS NOT NULL
        ALTER TABLE [Applicants] DROP COLUMN [SiteStatus];

    IF COL_LENGTH('dbo.Applicants', 'SiteRemark') IS NOT NULL
        ALTER TABLE [Applicants] DROP COLUMN [SiteRemark];
END;
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[ApplicantsLog]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.ApplicantsLog', 'LayoutYesNo') IS NOT NULL
        ALTER TABLE [ApplicantsLog] DROP COLUMN [LayoutYesNo];

    IF COL_LENGTH('dbo.ApplicantsLog', 'TotalPlots') IS NOT NULL
        ALTER TABLE [ApplicantsLog] DROP COLUMN [TotalPlots];

    IF COL_LENGTH('dbo.ApplicantsLog', 'PlotsApplicableForThisNoc') IS NOT NULL
        ALTER TABLE [ApplicantsLog] DROP COLUMN [PlotsApplicableForThisNoc];

    IF COL_LENGTH('dbo.ApplicantsLog', 'AmountForPlots') IS NOT NULL
        ALTER TABLE [ApplicantsLog] DROP COLUMN [AmountForPlots];

    IF COL_LENGTH('dbo.ApplicantsLog', 'SiteVisitEstimateDocumentPath') IS NOT NULL
        ALTER TABLE [ApplicantsLog] DROP COLUMN [SiteVisitEstimateDocumentPath];

    IF COL_LENGTH('dbo.ApplicantsLog', 'SiteVisitGeoTagPhotoPath') IS NOT NULL
        ALTER TABLE [ApplicantsLog] DROP COLUMN [SiteVisitGeoTagPhotoPath];

    IF COL_LENGTH('dbo.ApplicantsLog', 'SiteStatus') IS NOT NULL
        ALTER TABLE [ApplicantsLog] DROP COLUMN [SiteStatus];

    IF COL_LENGTH('dbo.ApplicantsLog', 'SiteRemark') IS NOT NULL
        ALTER TABLE [ApplicantsLog] DROP COLUMN [SiteRemark];
END;
");
        }
    }
}
