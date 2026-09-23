using PHE.API.Models;

namespace PHE.API.Application.Common.Interfaces
{
    public interface IDeScrutinyWriteService
    {
        Task<Applicant> SaveActionAsync(string applicationNo, DeScrutinyActionCommand command, CancellationToken cancellationToken = default);
    }

    public class DeScrutinyActionCommand
    {
        public string? Action { get; set; }
        public string? Remark { get; set; }
        public string? Role { get; set; }
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
    }
}
