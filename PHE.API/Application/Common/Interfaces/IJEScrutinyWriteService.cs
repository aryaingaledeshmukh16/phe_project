using PHE.API.Models;

namespace PHE.API.Application.Common.Interfaces
{
    public interface IJEScrutinyWriteService
    {
        Task<Applicant> SaveActionAsync(string applicationNo, JEScrutinyActionCommand command, CancellationToken cancellationToken = default);
    }

    public class JEScrutinyActionCommand
    {
        public string Action { get; set; } = string.Empty;
        public string? Remark { get; set; }
        public string? Role { get; set; }
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
    }
}
