using System.ComponentModel.DataAnnotations;

namespace PHE.API.Models

{
    public class JEApplication
    {
         [Key]
        public int SrNo { get; set; }
        

        public string? ApplicationNo { get; set; }

        public DateTime? ApplicationDate { get; set; }


        public string? ApplicantName { get; set; }

        public string? Address { get; set; }

        public string? MobileNumber { get; set; }

        public string? Peth { get; set; }

        public string? Zone { get; set; }

        public string? JagechaAddress { get; set; }

        public string? ManjurLayoutNo { get; set; }

        public DateTime? ManjurLayoutDate { get; set; }

        public string? MilakataKramanka { get; set; }

        public string? ScrutinyStatus { get; set; }

        public string? ApplicationStatus { get; set; }

        public string? TaxNoc { get; set; }

        public string? SatBaraUtara { get; set; }

        public string? ApprovedLayoutMap { get; set; }

        public string? SitePhotoPath { get; set; }
    }
}