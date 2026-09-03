using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PHE.API.Data;
using PHE.API.Models;

namespace PHE.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PHEScrutinyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PHEScrutinyController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetApplications()
        {
            try
            {
                var applications = await _context.Applicants
                    .AsNoTracking()
                    .Where(x => x.Application_status == "Deputy Engineer Verification Completed")
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


        [HttpGet("{applicationNo}")]
        public async Task<IActionResult> GetApplication(string applicationNo)
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
                    .FirstOrDefaultAsync(x => x.ApplicationNo == applicationNo);

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


        [HttpGet("{applicationNo}/history")]
        public async Task<IActionResult> GetHistory(string applicationNo)
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
                    .Where(x => x.ApplicationNo == applicationNo)
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

        [HttpPut("{applicationNo}/action")]
        public async Task<IActionResult> SaveAction(string applicationNo, [FromBody] PHEScrutinyActionRequest request)
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
                    message = "Scrutiny action is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Remark))
            {
                return BadRequest(new
                {
                    message = "Remark is required before saving PHE verification."
                });
            }

            var application = await _context.Applicants
                .FirstOrDefaultAsync(x => x.ApplicationNo == applicationNo);

            if (application == null)
            {
                return NotFound(new
                {
                    message = "Application not found.",
                    applicationNo
                });
            }

            if (application.Application_status != "Deputy Engineer Verification Completed")
            {
                return BadRequest(new
                {
                    message = "Application is not ready for PHE verification.",
                    currentStatus = application.Application_status
                });
            }

            var action = request.Action.Trim();
            string newScrutinyStatus;
            string newApplicationStatus;

            if (action.Equals("Accepted", StringComparison.OrdinalIgnoreCase))
            {
                newScrutinyStatus = "Accepted";
                newApplicationStatus = "Due for Payment from Citizen";
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
            application.Status = newApplicationStatus;
            application.entry_date = entryDate;
            application.Remark = request.Remark;

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.SaveChangesAsync();

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
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "PHE scrutiny updated successfully.",
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
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return StatusCode(500, new
                {
                    message = "PHE scrutiny update failed.",
                    error = ex.Message
                });
            }
        }
    }

    public class PHEScrutinyActionRequest
    {
        public string Action { get; set; } = string.Empty;
        public string? Remark { get; set; }
        public string? Role { get; set; }
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
    }
}
