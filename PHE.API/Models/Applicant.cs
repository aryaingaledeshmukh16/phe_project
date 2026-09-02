using System.ComponentModel.DataAnnotations;


using System.ComponentModel.DataAnnotations.Schema;

namespace PHE.API.Models
{
    public class Applicant
    {
        [Key]
        public int Id { get; set; }

        public string ApplicationNo { get; set; } = string.Empty;

        // This applicant workflow intentionally does not collect applicant info on the first form.
        // FullName and MobileNumber are left blank for new applications created at document submission time.
        public string FullName { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string AadhaarNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string ApplicationType { get; set; } = string.Empty;

        public string Peth { get; set; } = string.Empty;

        public string Zone { get; set; } = string.Empty;

        public string PropertyNumber { get; set; } = string.Empty;

        public string LayoutAddress { get; set; } = string.Empty;

        public string ApprovedLayoutNumber { get; set; } = string.Empty;

        public DateTime? ApprovedLayoutDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pending";

        public string? SatBaraPath { get; set; }

        public string? ApprovedLayoutMapPath { get; set; }

        public string? GeoTagPhotoPath { get; set; }

        public string? KMLFilePath { get; set; }   // ← हा add करा

        public string? TaxNocPath { get; set; }

        public string? OTP { get; set; }

        public DateTime? OTPExpiry { get; set; }

        public bool IsOTPVerified { get; set; } = false;

        [Column(TypeName = "decimal(18,6)")]
public decimal? Latitude { get; set; }

[Column(TypeName = "decimal(18,6)")]
public decimal? Longitude { get; set; }

[Column(TypeName = "decimal(18,2)")]
public decimal? TotalEstimateAmount { get; set; }

[Column(TypeName = "decimal(18,2)")]
public decimal? ShowAmountAsPerNoOfPlots { get; set; }

public string? role { get; set; }

public string? user_code { get; set; }

public string? user_name { get; set; }

public string? scrutiny_status { get; set; }

public string? Application_status { get; set; }

public DateTime? entry_date { get; set; }

public string? Remark { get; set; }

// Site Visit Fields
public string? LayoutYesNo { get; set; }

public int? TotalPlots { get; set; }

public int? PlotsApplicableForThisNoc { get; set; }

[Column(TypeName = "decimal(18,2)")]
public decimal? AmountForPlots { get; set; }

public string? SiteVisitEstimateDocumentPath { get; set; }

public string? SiteVisitGeoTagPhotoPath { get; set; }
    }
}