namespace PHE.API.DTOs
{
    public class ApplicantDto
    {
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


       public string? TaxNocPath { get; set; }

public string? SatBaraPath { get; set; }

public string? ApprovedLayoutMapPath { get; set; }

public string? GeoTagPhotoPath { get; set; }

    }
}