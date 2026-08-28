using Microsoft.AspNetCore.Http;

namespace PHE.API.DTOs
{
    public class UploadDocumentsDto
    {
        public IFormFile? TaxNoc { get; set; }

        public IFormFile? SatBara { get; set; }

        public IFormFile? LayoutMap { get; set; }

        public IFormFile? GeoTag { get; set; }
    }
}
