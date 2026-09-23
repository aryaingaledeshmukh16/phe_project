using Microsoft.EntityFrameworkCore;
using PHE.API.Application.Common.Interfaces;
using PHE.API.Data;
using PHE.API.Models;

namespace PHE.API.Infrastructure.Services
{
    public class DeScrutinyWriteService : IDeScrutinyWriteService
    {
        private readonly ApplicationDbContext _context;

        public DeScrutinyWriteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Applicant> SaveActionAsync(string applicationNo, DeScrutinyActionCommand command, CancellationToken cancellationToken = default)
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
                throw new InvalidOperationException("Verification action is required.");
            }

            if (string.IsNullOrWhiteSpace(command.Remark))
            {
                throw new InvalidOperationException("Remark is required before saving DE verification.");
            }

            var application = await _context.Applicants
                .FirstOrDefaultAsync(x => x.ApplicationNo == applicationNo, cancellationToken);

            if (application == null)
            {
                throw new KeyNotFoundException(applicationNo);
            }

            if (application.Application_status != "Development Charge Fixed")
            {
                throw new InvalidOperationException("Application is not in 'Development Charge Fixed' status.");
            }

            var action = command.Action.Trim();
            string newScrutinyStatus;
            string newApplicationStatus;

            if (action.Equals("Select", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Please select a valid scrutiny status.");
            }
            else if (action.Equals("Accepted", StringComparison.OrdinalIgnoreCase))
            {
                newScrutinyStatus = "Accepted";
                newApplicationStatus = "Deputy Engineer Verification Completed";
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
            else if (action.Equals("Pending", StringComparison.OrdinalIgnoreCase))
            {
                newScrutinyStatus = "Pending";
                newApplicationStatus = "Development Charge Fixed";
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
            application.Remark = command.Remark;
            application.Status = newApplicationStatus;

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
                    Remark = application.Remark,
                    LayoutYesNo = application.LayoutYesNo,
                    TotalPlots = application.TotalPlots,
                    PlotsApplicableForThisNoc = application.PlotsApplicableForThisNoc,
                    AmountForPlots = application.AmountForPlots,
                    SiteVisitEstimateDocumentPath = application.SiteVisitEstimateDocumentPath,
                    SiteVisitGeoTagPhotoPath = application.SiteVisitGeoTagPhotoPath
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
    }
}
