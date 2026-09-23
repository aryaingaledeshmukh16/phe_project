using Microsoft.EntityFrameworkCore;
using PHE.API.Application.Common.Interfaces;
using PHE.API.Data;
using PHE.API.Models;

namespace PHE.API.Infrastructure.Services
{
    public class PHEScrutinyWriteService : IPHEScrutinyWriteService
    {
        private readonly ApplicationDbContext _context;

        public PHEScrutinyWriteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Applicant> SaveActionAsync(string applicationNo, PHEScrutinyActionCommand command, CancellationToken cancellationToken = default)
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

            if (string.IsNullOrWhiteSpace(command.LayoutYesNo) || command.LayoutYesNo == "Select")
            {
                throw new InvalidOperationException("Please select whether the pipeline exists under approved layout.");
            }

            if (string.IsNullOrWhiteSpace(command.Remark))
            {
                throw new InvalidOperationException("Remark is required before saving PHE verification.");
            }

            var application = await _context.Applicants
                .FirstOrDefaultAsync(x => x.ApplicationNo == applicationNo, cancellationToken);

            if (application == null)
            {
                throw new KeyNotFoundException(applicationNo);
            }

            if (application.Application_status != "Deputy Engineer Verification Completed")
            {
                throw new InvalidOperationException("Application is not ready for PHE verification.");
            }

            var action = command.Action.Trim();
            var layoutYesNo = command.LayoutYesNo.Trim();
            string newScrutinyStatus;
            string newApplicationStatus;

            if (action.Equals("Accepted", StringComparison.OrdinalIgnoreCase))
            {
                newScrutinyStatus = "Accepted";

                if (layoutYesNo == "अस्तित्वात आहे")
                {
                    newApplicationStatus = "Application Approved by PHE";
                }
                else if (layoutYesNo == "अस्तित्वात नाही")
                {
                    newApplicationStatus = "Due for Payment from Citizen";
                }
                else
                {
                    newApplicationStatus = "Application Approved by PHE";
                }
            }
            else if (action.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
            {
                newScrutinyStatus = "Rejected";
                newApplicationStatus = "Rejected";
            }
            else if (action.Equals("Pending", StringComparison.OrdinalIgnoreCase))
            {
                newScrutinyStatus = "Pending";
                newApplicationStatus = "Deputy Engineer Verification Completed";
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
            application.LayoutYesNo = layoutYesNo;
            application.Application_status = newApplicationStatus;
            application.Status = newApplicationStatus;
            application.entry_date = entryDate;
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
                    LayoutYesNo = application.LayoutYesNo,
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
    }
}
