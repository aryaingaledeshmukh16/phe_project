using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PHE.API.Data;
using PHE.API.Models;

namespace PHE.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeScrutinyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DeScrutinyController(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // =========================================================
        // GET ALL APPLICATIONS FOR DEPUTY ENGINEER
        // GET: api/DeScrutiny
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetApplications()
        {
            try
            {
                var applications = await _context.Applicants
                    .AsNoTracking()
                    .Where(x => 
                        x.Application_status == "Development Charge Fixed")
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();

                return Ok(applications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Applications fetch failed.",
                    error = ex.Message
                });
            }
        }

        // =========================================================
        // GET SINGLE APPLICATION
        // GET: api/DeScrutiny/{applicationNo}
        // =========================================================
        [HttpGet("{applicationNo}")]
        public async Task<IActionResult> GetApplication(
            string applicationNo)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return BadRequest(new
                {
                    message = "Application number is required."
                });
            }

            try
            {
                var application = await _context.Applicants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.ApplicationNo == applicationNo
                    );

                if (application == null)
                {
                    return NotFound(new
                    {
                        message = "Application not found.",
                        applicationNo
                    });
                }

                return Ok(application);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Application fetch failed.",
                    error = ex.Message
                });
            }
        }

        // =========================================================
        // GET APPLICATION HISTORY
        // GET: api/DeScrutiny/{applicationNo}/history
        // =========================================================
        [HttpGet("{applicationNo}/history")]
        public async Task<IActionResult> GetHistory(
            string applicationNo)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return BadRequest(new
                {
                    message = "Application number is required."
                });
            }

            try
            {
                var history = await _context.ApplicantsLogs
                    .AsNoTracking()
                    .Where(x =>
                        x.ApplicationNo == applicationNo)
                    .OrderByDescending(x => x.LogId)
                    .Select(x => new
                    {
                        logId = x.LogId,
                        applicationNo = x.ApplicationNo,
                        role = x.role,
                        userCode = x.user_code,
                        userName = x.user_name,
                        scrutinyStatus = x.scrutiny_status,
                        applicationStatus = x.Application_status,
                        remark = x.Remark,
                        entryDate = x.entry_date
                    })
                    .ToListAsync();

                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "History fetch failed.",
                    error = ex.Message
                });
            }
        }

        // =========================================================
        // SAVE DE VERIFICATION ACTION
        // PUT: api/DeScrutiny/{applicationNo}/action
        // =========================================================
        [HttpPut("{applicationNo}/action")]
        public async Task<IActionResult> SaveAction(
            string applicationNo,
            [FromBody] DeVerificationRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    message = "Request data is required."
                });
            }

            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return BadRequest(new
                {
                    message = "Application number is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Action))
            {
                return BadRequest(new
                {
                    message = "Verification action is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Remark))
            {
                return BadRequest(new
                {
                    message = "Remark is required before saving DE verification."
                });
            }

            try
            {
                var application = await _context.Applicants
                    .FirstOrDefaultAsync(
                        x => x.ApplicationNo == applicationNo
                    );

                if (application == null)
                {
                    return NotFound(new
                    {
                        message = "Application not found.",
                        applicationNo
                    });
                }

                // Verify the application is ready for DE verification
                if (application.Application_status != "Development Charge Fixed")
                {
                    return BadRequest(new
                    {
                        message = "Application is not in 'Development Charge Fixed' status.",
                        currentStatus = application.Application_status
                    });
                }

                var action = request.Action.Trim();
                string newScrutinyStatus;
                string newApplicationStatus;

                if (action.Equals(
                    "Select",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new
                    {
                        message = "Please select a valid scrutiny status."
                    });
                }
                else if (action.Equals(
                    "Accepted",
                    StringComparison.OrdinalIgnoreCase))
                {
                    newScrutinyStatus = "Accepted";
                    newApplicationStatus = "Deputy Engineer Verification Completed";
                }
                else if (action.Equals(
                    "Rejected",
                    StringComparison.OrdinalIgnoreCase))
                {
                    newScrutinyStatus = "Rejected";
                    newApplicationStatus = "Rejected";
                }
                else if (action.Equals(
                    "Send Back to Jr. Engineer",
                    StringComparison.OrdinalIgnoreCase))
                {
                    newScrutinyStatus = "Send Back";
                    newApplicationStatus = "Send Back to Jr. Engineer";
                }
                else if (action.Equals(
                    "Pending",
                    StringComparison.OrdinalIgnoreCase))
                {
                    newScrutinyStatus = "Pending";
                    newApplicationStatus = "Development Charge Fixed";
                }
                else
                {
                    return BadRequest(new
                    {
                        message = "Invalid scrutiny action."
                    });
                }

                var entryDate = DateTime.Now;

                application.role = request.Role;
                application.user_code = request.UserCode;
                application.user_name = request.UserName;
                application.scrutiny_status = newScrutinyStatus;
                application.Application_status = newApplicationStatus;
                application.entry_date = entryDate;
                application.Remark = request.Remark;
                application.Status = newApplicationStatus;

                await using var transaction =
                    await _context.Database
                        .BeginTransactionAsync();

                try
                {
                    // Save main table
                    await _context.SaveChangesAsync();

                    // Save history
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
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(new
                    {
                        message = "DE verification completed successfully.",
                        applicationNo = application.ApplicationNo,
                        scrutinyStatus = application.scrutiny_status,
                        applicationStatus = application.Application_status,
                        role = application.role,
                        userCode = application.user_code,
                        userName = application.user_name,
                        entryDate = application.entry_date,
                        remark = application.Remark
                    });
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "DE verification save failed.",
                    error = ex.Message
                });
            }
        }

        // =========================================================
        // REQUEST CLASS FOR DE VERIFICATION
        // =========================================================
        public class DeVerificationRequest
        {
            public string? Action { get; set; }
            public string? Remark { get; set; }
            public string? Role { get; set; }
            public string? UserCode { get; set; }
            public string? UserName { get; set; }
        }
    }
}
