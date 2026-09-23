using PHE.API.Models;

namespace PHE.API.Application.Common.Interfaces
{
    public interface IPHEScrutinyWriteService
    {
        Task<Applicant> SaveActionAsync(string applicationNo, PHEScrutinyActionCommand command, CancellationToken cancellationToken = default);
    }

    public class PHEScrutinyActionCommand
    {
        public string Action { get; set; } = string.Empty;
        public string? LayoutYesNo { get; set; }
        public string? Remark { get; set; }
        public string? Role { get; set; }
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
    }
}
