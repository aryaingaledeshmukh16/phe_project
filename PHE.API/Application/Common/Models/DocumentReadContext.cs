namespace PHE.API.Application.Common.Models
{
    public class DocumentReadContext
    {
        public string? PhysicalPath { get; set; }
        public string ContentType { get; set; } = "application/octet-stream";
        public string FileName { get; set; } = string.Empty;
        public string? Message { get; set; }
    }
}
