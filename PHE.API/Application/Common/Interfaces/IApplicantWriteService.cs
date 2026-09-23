using PHE.API.DTOs;
using PHE.API.Models;

namespace PHE.API.Application.Common.Interfaces
{
    public interface IApplicantWriteService
    {
        Task<Applicant> CreateAsync(Applicant applicant, CancellationToken cancellationToken = default);
        Task<Applicant> UpdateLayoutAsync(string applicationNo, LayoutUpdateCommand command, CancellationToken cancellationToken = default);
        Task<Applicant> SaveSiteVisitAsync(string applicationNo, SiteVisitDto siteVisit, CancellationToken cancellationToken = default);
        Task<Applicant> UploadDocumentsAsync(string applicationNo, UploadDocumentsDto documents, CancellationToken cancellationToken = default);
        Task<Applicant> SendOtpAsync(string applicationNo, CancellationToken cancellationToken = default);
        Task<Applicant> VerifyOtpAsync(string applicationNo, string otp, CancellationToken cancellationToken = default);
    }

    public class LayoutUpdateCommand
    {
        public string? ApplicationType { get; set; }
        public string? Peth { get; set; }
        public string? Zone { get; set; }
        public string? PropertyNumber { get; set; }
        public string? LayoutAddress { get; set; }
        public string? ApprovedLayoutNumber { get; set; }
        public string? ApprovedLayoutDate { get; set; }
    }
}
