using Microsoft.AspNetCore.Http;

namespace PHE.API.DTOs
{
    public class SiteVisitDto
    {
        public string? LayoutYesNo { get; set; }

        public decimal? TotalEstimateAmount { get; set; }

        public int? TotalPlots { get; set; }

        public int? PlotsApplicableForThisNoc { get; set; }

        public decimal? AmountForPlots { get; set; }

        public IFormFile? SiteVisitEstimateDocument { get; set; }

        public IFormFile? SiteVisitGeoTagPhoto { get; set; }

        public string? Status { get; set; }

        public string? Remark { get; set; }
    }
}
