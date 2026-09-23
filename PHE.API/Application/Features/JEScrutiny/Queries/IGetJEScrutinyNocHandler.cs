namespace PHE.API.Application.Features.JEScrutiny.Queries
{
    public class JEScrutinyNocResult
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public string? ApplicationNo { get; set; }
        public string? CurrentStatus { get; set; }
        public string? HtmlContent { get; set; }
        public string? Error { get; set; }
    }

    public interface IGetJEScrutinyNocHandler
    {
        Task<JEScrutinyNocResult> HandleAsync(string applicationNo, CancellationToken cancellationToken = default);
    }
}
