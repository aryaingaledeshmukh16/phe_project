using Microsoft.EntityFrameworkCore;
using PHE.API.Application.Common.Interfaces;
using PHE.API.Data;
using PHE.API.DTOs;
using PHE.API.Models;

namespace PHE.API.Infrastructure.Services
{
    public class ApplicantWriteService : IApplicantWriteService
    {
        private readonly ApplicationDbContext _context;

        public ApplicantWriteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Applicant> CreateAsync(Applicant applicant, CancellationToken cancellationToken = default)
        {
            if (applicant == null)
            {
                throw new ArgumentNullException(nameof(applicant));
            }

            if (string.IsNullOrWhiteSpace(applicant.ApplicationNo))
            {
                applicant.ApplicationNo = DateTime.Now.ToString("yyyyMMddHHmmss");
            }

            applicant.role = "JE";
            applicant.user_code = null;
            applicant.user_name = null;
            applicant.scrutiny_status = "Pending";
            applicant.Application_status = "Pending";
            applicant.entry_date = DateTime.Now;

            if (applicant.CreatedDate == default)
            {
                applicant.CreatedDate = DateTime.Now;
            }

            if (string.IsNullOrWhiteSpace(applicant.Status))
            {
                applicant.Status = "Pending";
            }

            _context.Applicants.Add(applicant);
            await _context.SaveChangesAsync(cancellationToken);

            var log = CreateApplicantLog(applicant, "Application Created");
            _context.ApplicantsLogs.Add(log);
            await _context.SaveChangesAsync(cancellationToken);

            return applicant;
        }

        public async Task<Applicant> UpdateLayoutAsync(string applicationNo, LayoutUpdateCommand command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                throw new ArgumentException("Application Number is required.", nameof(applicationNo));
            }

            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var applicant = await _context.Applicants
                .FirstOrDefaultAsync(x => x.ApplicationNo == applicationNo, cancellationToken);

            if (applicant == null)
            {
                throw new KeyNotFoundException(applicationNo);
            }

            applicant.ApplicationType = command.ApplicationType?.Trim() ?? "";
            applicant.Peth = command.Peth?.Trim() ?? "";
            applicant.Zone = command.Zone?.Trim() ?? "";
            applicant.PropertyNumber = command.PropertyNumber?.Trim() ?? "";
            applicant.LayoutAddress = command.LayoutAddress?.Trim() ?? "";
            applicant.ApprovedLayoutNumber = command.ApprovedLayoutNumber?.Trim() ?? "";

            if (!string.IsNullOrWhiteSpace(command.ApprovedLayoutDate))
            {
                if (DateTime.TryParse(command.ApprovedLayoutDate, out DateTime layoutDate))
                {
                    applicant.ApprovedLayoutDate = layoutDate;
                }
                else
                {
                    throw new FormatException("Invalid approved layout date.");
                }
            }

            if (string.IsNullOrWhiteSpace(applicant.Application_status))
            {
                applicant.Application_status = "Pending";
            }

            if (string.IsNullOrWhiteSpace(applicant.scrutiny_status))
            {
                applicant.scrutiny_status = "Pending";
            }

            if (string.IsNullOrWhiteSpace(applicant.Status))
            {
                applicant.Status = "Pending";
            }

            applicant.entry_date = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);

            var log = CreateApplicantLog(applicant, "Layout information updated");
            _context.ApplicantsLogs.Add(log);
            await _context.SaveChangesAsync(cancellationToken);

            return applicant;
        }

        public async Task<Applicant> SaveSiteVisitAsync(string applicationNo, SiteVisitDto siteVisit, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                throw new ArgumentException("Application Number is required.", nameof(applicationNo));
            }

            if (siteVisit == null)
            {
                throw new ArgumentNullException(nameof(siteVisit));
            }

            if (string.IsNullOrWhiteSpace(siteVisit.LayoutYesNo) || siteVisit.LayoutYesNo == "Select")
            {
                throw new InvalidOperationException("Layout Pipeline Status must be selected.");
            }

            if (string.IsNullOrWhiteSpace(siteVisit.Status) || siteVisit.Status == "Status")
            {
                throw new InvalidOperationException("Site Status must be selected (Accept, Reject, or Hold).");
            }

            if (siteVisit.SiteVisitGeoTagPhoto == null)
            {
                throw new InvalidOperationException("Site Visit Geo Tag Photo is required.");
            }

            if (siteVisit.LayoutYesNo == "अस्तित्वात नाही")
            {
                if (siteVisit.TotalEstimateAmount == null || siteVisit.TotalEstimateAmount <= 0)
                {
                    throw new InvalidOperationException("Total Estimate Amount is required and must be greater than 0.");
                }

                if (siteVisit.TotalPlots == null || siteVisit.TotalPlots <= 0)
                {
                    throw new InvalidOperationException("Total Plots is required and must be greater than 0.");
                }

                if (siteVisit.PlotsApplicableForThisNoc == null || siteVisit.PlotsApplicableForThisNoc <= 0)
                {
                    throw new InvalidOperationException("Plots applicable for this NOC is required and must be greater than 0.");
                }

                if (siteVisit.PlotsApplicableForThisNoc > siteVisit.TotalPlots)
                {
                    throw new InvalidOperationException("Plots applicable cannot exceed Total Plots.");
                }

                if (siteVisit.SiteVisitEstimateDocument == null)
                {
                    throw new InvalidOperationException("Estimate Document is required when pipeline does not exist.");
                }
            }

            var applicant = await _context.Applicants
                .FirstOrDefaultAsync(x => x.ApplicationNo == applicationNo, cancellationToken);

            if (applicant == null)
            {
                throw new KeyNotFoundException(applicationNo);
            }

            var uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            var savedPaths = new Dictionary<string, string>();

            if (siteVisit.SiteVisitEstimateDocument != null)
            {
                var estimateFolder = Path.Combine(uploadRoot, "SiteVisit");
                Directory.CreateDirectory(estimateFolder);
                var estimateFileName = $"{Guid.NewGuid()}_{Path.GetFileName(siteVisit.SiteVisitEstimateDocument.FileName)}";
                var estimateFullPath = Path.Combine(estimateFolder, estimateFileName);
                await using (var estimateStream = new FileStream(estimateFullPath, FileMode.CreateNew))
                {
                    await siteVisit.SiteVisitEstimateDocument.CopyToAsync(estimateStream, cancellationToken);
                }
                savedPaths["SiteVisitEstimate"] = $"Uploads/SiteVisit/{estimateFileName}";
            }

            var geoTagFolder = Path.Combine(uploadRoot, "SiteVisit");
            Directory.CreateDirectory(geoTagFolder);
            var geoTagFileName = $"{Guid.NewGuid()}_{Path.GetFileName(siteVisit.SiteVisitGeoTagPhoto.FileName)}";
            var geoTagFullPath = Path.Combine(geoTagFolder, geoTagFileName);
            await using (var geoTagStream = new FileStream(geoTagFullPath, FileMode.CreateNew))
            {
                await siteVisit.SiteVisitGeoTagPhoto.CopyToAsync(geoTagStream, cancellationToken);
            }
            savedPaths["SiteVisitGeoTag"] = $"Uploads/SiteVisit/{geoTagFileName}";

            decimal? calculatedAmount = null;
            if (siteVisit.LayoutYesNo == "अस्तित्वात नाही" && siteVisit.TotalEstimateAmount.HasValue && siteVisit.TotalPlots.HasValue && siteVisit.PlotsApplicableForThisNoc.HasValue)
            {
                double estimatePerPlot = (double)siteVisit.TotalEstimateAmount / siteVisit.TotalPlots.Value;
                double estimateForThisNoc = estimatePerPlot * siteVisit.PlotsApplicableForThisNoc.Value;
                double roundedAmount = Math.Round(estimateForThisNoc, 0);
                calculatedAmount = Convert.ToDecimal(roundedAmount);
            }

            applicant.LayoutYesNo = siteVisit.LayoutYesNo;
            applicant.TotalPlots = siteVisit.TotalPlots;
            applicant.PlotsApplicableForThisNoc = siteVisit.PlotsApplicableForThisNoc;
            applicant.AmountForPlots = calculatedAmount;

            if (savedPaths.ContainsKey("SiteVisitEstimate"))
            {
                applicant.SiteVisitEstimateDocumentPath = savedPaths["SiteVisitEstimate"];
            }

            if (savedPaths.ContainsKey("SiteVisitGeoTag"))
            {
                applicant.SiteVisitGeoTagPhotoPath = savedPaths["SiteVisitGeoTag"];
            }

            applicant.Remark = siteVisit.Remark;
            applicant.scrutiny_status = siteVisit.Status == "Reject" ? "Rejected" : siteVisit.Status;

            if (siteVisit.Status == "Accept")
            {
                applicant.Application_status = "Development Charge Fixed";
            }
            else if (siteVisit.Status == "Reject")
            {
                applicant.Application_status = "Rejected";
            }

            applicant.entry_date = DateTime.Now;
            await _context.SaveChangesAsync(cancellationToken);

            var log = CreateApplicantLogForSiteVisit(applicant, "Site Visit Updated");
            _context.ApplicantsLogs.Add(log);
            await _context.SaveChangesAsync(cancellationToken);

            return applicant;
        }

        public async Task<Applicant> UploadDocumentsAsync(string applicationNo, UploadDocumentsDto documents, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                throw new ArgumentException("Application Number is required.", nameof(applicationNo));
            }

            if (documents == null || documents.TaxNoc == null || documents.SatBara == null || documents.LayoutMap == null || documents.GeoTag == null)
            {
                throw new InvalidOperationException("All four documents are required.");
            }

            var applicant = await _context.Applicants
                .FirstOrDefaultAsync(x => x.ApplicationNo == applicationNo, cancellationToken);

            if (applicant == null)
            {
                throw new KeyNotFoundException(applicationNo);
            }

            var uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            var files = new[]
            {
                (File: documents.TaxNoc, Folder: "TaxNOC"),
                (File: documents.SatBara, Folder: "SatBara"),
                (File: documents.LayoutMap, Folder: "LayoutMap"),
                (File: documents.GeoTag, Folder: "Geotag")
            };

            var savedPaths = new Dictionary<string, string>();

            foreach (var item in files)
            {
                var folder = Path.Combine(uploadRoot, item.Folder);
                Directory.CreateDirectory(folder);

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(item.File.FileName)}";
                var fullPath = Path.Combine(folder, fileName);

                await using var stream = new FileStream(fullPath, FileMode.CreateNew);
                await item.File.CopyToAsync(stream, cancellationToken);

                savedPaths[item.Folder] = $"Uploads/{item.Folder}/{fileName}";
            }

            applicant.TaxNocPath = savedPaths["TaxNOC"];
            applicant.SatBaraPath = savedPaths["SatBara"];
            applicant.ApprovedLayoutMapPath = savedPaths["LayoutMap"];
            applicant.GeoTagPhotoPath = savedPaths["Geotag"];

            await _context.SaveChangesAsync(cancellationToken);

            var log = CreateApplicantLog(applicant, "Documents uploaded");
            _context.ApplicantsLogs.Add(log);
            await _context.SaveChangesAsync(cancellationToken);

            return applicant;
        }

        public async Task<Applicant> SendOtpAsync(string applicationNo, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                throw new ArgumentException("Application Number is required.", nameof(applicationNo));
            }

            var applicant = await _context.Applicants
                .FirstOrDefaultAsync(x => x.ApplicationNo == applicationNo, cancellationToken);

            if (applicant == null)
            {
                throw new KeyNotFoundException(applicationNo);
            }

            var otp = new Random().Next(100000, 999999).ToString();
            applicant.OTP = otp;
            applicant.OTPExpiry = DateTime.Now.AddMinutes(5);
            applicant.IsOTPVerified = false;

            await _context.SaveChangesAsync(cancellationToken);
            return applicant;
        }

        public async Task<Applicant> VerifyOtpAsync(string applicationNo, string otp, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                throw new ArgumentException("Application Number is required.", nameof(applicationNo));
            }

            if (string.IsNullOrWhiteSpace(otp))
            {
                throw new InvalidOperationException("OTP is required.");
            }

            var applicant = await _context.Applicants
                .FirstOrDefaultAsync(x => x.ApplicationNo == applicationNo, cancellationToken);

            if (applicant == null)
            {
                throw new KeyNotFoundException(applicationNo);
            }

            if (applicant.OTPExpiry == null || applicant.OTPExpiry < DateTime.Now)
            {
                throw new InvalidOperationException("OTP expired.");
            }

            if (applicant.OTP != otp.Trim())
            {
                throw new InvalidOperationException("Invalid OTP.");
            }

            applicant.IsOTPVerified = true;
            applicant.OTP = null;
            applicant.OTPExpiry = null;

            await _context.SaveChangesAsync(cancellationToken);
            return applicant;
        }

        private ApplicantsLog CreateApplicantLog(Applicant applicant, string remark)
        {
            return new ApplicantsLog
            {
                ApplicantId = applicant.Id,
                ApplicationNo = applicant.ApplicationNo,
                FullName = applicant.FullName,
                MobileNumber = applicant.MobileNumber,
                Email = applicant.Email,
                AadhaarNumber = applicant.AadhaarNumber,
                Address = applicant.Address,
                ApplicationType = applicant.ApplicationType,
                Peth = applicant.Peth,
                Zone = applicant.Zone,
                PropertyNumber = applicant.PropertyNumber,
                LayoutAddress = applicant.LayoutAddress,
                ApprovedLayoutNumber = applicant.ApprovedLayoutNumber,
                ApprovedLayoutDate = applicant.ApprovedLayoutDate,
                CreatedDate = applicant.CreatedDate,
                Status = applicant.Status,
                SatBaraPath = applicant.SatBaraPath,
                ApprovedLayoutMapPath = applicant.ApprovedLayoutMapPath,
                GeoTagPhotoPath = applicant.GeoTagPhotoPath,
                KMLFilePath = applicant.KMLFilePath,
                TaxNocPath = applicant.TaxNocPath,
                Latitude = applicant.Latitude,
                Longitude = applicant.Longitude,
                TotalEstimateAmount = applicant.TotalEstimateAmount,
                ShowAmountAsPerNoOfPlots = applicant.ShowAmountAsPerNoOfPlots,
                role = applicant.role,
                user_code = applicant.user_code,
                user_name = applicant.user_name,
                scrutiny_status = applicant.scrutiny_status,
                Application_status = applicant.Application_status,
                entry_date = DateTime.Now,
                Remark = remark
            };
        }

        private ApplicantsLog CreateApplicantLogForSiteVisit(Applicant applicant, string remark)
        {
            var log = CreateApplicantLog(applicant, remark);
            log.LayoutYesNo = applicant.LayoutYesNo;
            log.TotalPlots = applicant.TotalPlots;
            log.PlotsApplicableForThisNoc = applicant.PlotsApplicableForThisNoc;
            log.AmountForPlots = applicant.AmountForPlots;
            log.SiteVisitEstimateDocumentPath = applicant.SiteVisitEstimateDocumentPath;
            log.SiteVisitGeoTagPhotoPath = applicant.SiteVisitGeoTagPhotoPath;
            return log;
        }
    }
}
