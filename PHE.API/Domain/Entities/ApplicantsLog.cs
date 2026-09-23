using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PHE.API.Domain.Entities
{
    [Table("ApplicantsLog")]
    public class ApplicantsLog
    {
        [Key]
        public int LogId { get; set; }

        public int? ApplicantId { get; set; }

        public string? ApplicationNo { get; set; }

        public string? FullName { get; set; }

        public string? MobileNumber { get; set; }

        public string? Email { get; set; }

        public string? AadhaarNumber { get; set; }

        public string? Address { get; set; }

        public string? ApplicationType { get; set; }

        public string? Peth { get; set; }

        public string? Zone { get; set; }

        public string? PropertyNumber { get; set; }

        public string? LayoutAddress { get; set; }

        public string? ApprovedLayoutNumber { get; set; }

        public DateTime? ApprovedLayoutDate { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? Status { get; set; }

        public string? SatBaraPath { get; set; }

        public string? ApprovedLayoutMapPath { get; set; }

        public string? GeoTagPhotoPath { get; set; }

        public string? KMLFilePath { get; set; }

        public string? TaxNocPath { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public decimal? TotalEstimateAmount { get; set; }

        public decimal? ShowAmountAsPerNoOfPlots { get; set; }

        public string? role { get; set; }

        public string? user_code { get; set; }

        public string? user_name { get; set; }

        public string? scrutiny_status { get; set; }

        public string? Application_status { get; set; }

        public DateTime? entry_date { get; set; }

        public string? Remark { get; set; }

        public string? LayoutYesNo { get; set; }

        public int? TotalPlots { get; set; }

        public int? PlotsApplicableForThisNoc { get; set; }

        public decimal? AmountForPlots { get; set; }

        public string? SiteVisitEstimateDocumentPath { get; set; }

        public string? SiteVisitGeoTagPhotoPath { get; set; }
    }
}
