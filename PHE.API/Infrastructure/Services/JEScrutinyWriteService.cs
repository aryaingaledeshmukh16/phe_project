using Microsoft.EntityFrameworkCore;
using PHE.API.Application.Common.Interfaces;
using PHE.API.Data;
using PHE.API.Models;

namespace PHE.API.Infrastructure.Services
{
    public class JEScrutinyWriteService : IJEScrutinyWriteService
    {
        private readonly ApplicationDbContext _context;

        public JEScrutinyWriteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Applicant> SaveActionAsync(string applicationNo, JEScrutinyActionCommand command, CancellationToken cancellationToken = default)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                throw new ArgumentException("Application number is required.", nameof(applicationNo));
            }

            if (string.IsNullOrWhiteSpace(command.Action))
            {
                throw new InvalidOperationException("Scrutiny action is required.");
            }

            var application = await _context.Applicants
                .FirstOrDefaultAsync(x => x.ApplicationNo == applicationNo, cancellationToken);

            if (application == null)
            {
                throw new KeyNotFoundException(applicationNo);
            }

            var oldApplicationStatus = application.Application_status ?? application.Status ?? "Pending";
            var action = command.Action.Trim();
            string newScrutinyStatus;
            string newApplicationStatus;

            if (action.Equals("Accepted", StringComparison.OrdinalIgnoreCase))
            {
                newScrutinyStatus = "Accept";
                newApplicationStatus = GetNextApplicationStatus(oldApplicationStatus);
            }
            else if (action.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
            {
                newScrutinyStatus = "Rejected";
                newApplicationStatus = "Rejected";
            }
            else if (action.Equals("Send Back to Jr. Engineer", StringComparison.OrdinalIgnoreCase))
            {
                newScrutinyStatus = "Send Back";
                newApplicationStatus = "Send Back to Jr. Engineer";
            }
            else
            {
                throw new InvalidOperationException("Invalid scrutiny action.");
            }

            var entryDate = DateTime.Now;
            application.role = command.Role;
            application.user_code = command.UserCode;
            application.user_name = command.UserName;
            application.scrutiny_status = newScrutinyStatus;
            application.Application_status = newApplicationStatus;
            application.entry_date = entryDate;
            application.Status = newApplicationStatus;
            application.Remark = command.Remark;

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);

                var history = new ApplicantsLog
                {
                    ApplicantId = application.Id,
                    ApplicationNo = application.ApplicationNo,
                    FullName = application.FullName,
                    MobileNumber = application.MobileNumber,
                    Email = application.Email,
                    AadhaarNumber = application.AadhaarNumber,
                    Address = application.Address,
                    ApplicationType = application.ApplicationType,
                    Peth = application.Peth,
                    Zone = application.Zone,
                    PropertyNumber = application.PropertyNumber,
                    LayoutAddress = application.LayoutAddress,
                    ApprovedLayoutNumber = application.ApprovedLayoutNumber,
                    ApprovedLayoutDate = application.ApprovedLayoutDate,
                    CreatedDate = application.CreatedDate,
                    Status = application.Status,
                    SatBaraPath = application.SatBaraPath,
                    ApprovedLayoutMapPath = application.ApprovedLayoutMapPath,
                    GeoTagPhotoPath = application.GeoTagPhotoPath,
                    KMLFilePath = application.KMLFilePath,
                    TaxNocPath = application.TaxNocPath,
                    Latitude = application.Latitude,
                    Longitude = application.Longitude,
                    TotalEstimateAmount = application.TotalEstimateAmount,
                    ShowAmountAsPerNoOfPlots = application.ShowAmountAsPerNoOfPlots,
                    role = application.role,
                    user_code = application.user_code,
                    user_name = application.user_name,
                    scrutiny_status = application.scrutiny_status,
                    Application_status = application.Application_status,
                    entry_date = application.entry_date,
                    Remark = application.Remark
                };

                _context.ApplicantsLogs.Add(history);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync();
                return application;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static string GetNextApplicationStatus(string currentStatus)
        {
            var status = (currentStatus ?? "").Trim().ToLowerInvariant();

            return status switch
            {
                "" => "Document Verified",
                "pending" => "Document Verified",
                "document verified" => "Site Visit Completed",
                "site visit completed" => "Deputy Engineer Verification Completed",
                "deputy engineer verification completed" => "Due for Payment from Citizen",
                _ => currentStatus
            };
        }
    }
}
