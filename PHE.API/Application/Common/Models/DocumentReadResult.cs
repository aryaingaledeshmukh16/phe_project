using Microsoft.AspNetCore.Mvc;

namespace PHE.API.Application.Common.Models
{
    public class DocumentReadResult
    {
        public string? PhysicalPath { get; set; }
        public string? ContentType { get; set; }
        public string? FileName { get; set; }
        public bool IsDownload { get; set; }
        public IActionResult? Result { get; set; }
        public string? Message { get; set; }
        public object? Error { get; set; }
        public bool IsSuccess => Result != null && string.IsNullOrWhiteSpace(Message);
    }
}
