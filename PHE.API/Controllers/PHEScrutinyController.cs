using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PHE.API.Application.Common.Interfaces;
using PHE.API.Application.Features.PHEScrutiny.Commands;
using PHE.API.Application.Features.PHEScrutiny.History;
using PHE.API.Application.Features.PHEScrutiny.Queries;
using PHE.API.Data;
using PHE.API.Models;

namespace PHE.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PHEScrutinyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IGetPHEScrutinyApplicationsHandler _getPHEScrutinyApplicationsHandler;
        private readonly IGetPHEScrutinyApplicationByApplicationNoHandler _getPHEScrutinyApplicationByApplicationNoHandler;
        private readonly IGetPHEScrutinyHistoryHandler _getPHEScrutinyHistoryHandler;
        private readonly ISavePHEScrutinyActionHandler _savePHEScrutinyActionHandler;

        public PHEScrutinyController(
            ApplicationDbContext context,
            IGetPHEScrutinyApplicationsHandler getPHEScrutinyApplicationsHandler,
            IGetPHEScrutinyApplicationByApplicationNoHandler getPHEScrutinyApplicationByApplicationNoHandler,
            IGetPHEScrutinyHistoryHandler getPHEScrutinyHistoryHandler,
            ISavePHEScrutinyActionHandler savePHEScrutinyActionHandler)
        {
            _context = context;
            _getPHEScrutinyApplicationsHandler = getPHEScrutinyApplicationsHandler;
            _getPHEScrutinyApplicationByApplicationNoHandler = getPHEScrutinyApplicationByApplicationNoHandler;
            _getPHEScrutinyHistoryHandler = getPHEScrutinyHistoryHandler;
            _savePHEScrutinyActionHandler = savePHEScrutinyActionHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetApplications()
        {
            try
            {
                var applications = await _getPHEScrutinyApplicationsHandler.HandleAsync();

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
                var application = await _getPHEScrutinyApplicationByApplicationNoHandler.HandleAsync(applicationNo);

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
                var history = await _getPHEScrutinyHistoryHandler.HandleAsync(applicationNo);

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

            if (string.IsNullOrWhiteSpace(request.LayoutYesNo) || request.LayoutYesNo == "Select")
            {
                return BadRequest(new
                {
                    message = "Please select whether the pipeline exists under approved layout."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Remark))
            {
                return BadRequest(new
                {
                    message = "Remark is required before saving PHE verification."
                });
            }

            try
            {
                var application = await _savePHEScrutinyActionHandler.HandleAsync(applicationNo, new PHEScrutinyActionCommand
                {
                    Action = request.Action,
                    LayoutYesNo = request.LayoutYesNo,
                    Remark = request.Remark,
                    Role = request.Role,
                    UserCode = request.UserCode,
                    UserName = request.UserName
                });

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
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    message = "Application not found.",
                    applicationNo
                });
            }
            catch (InvalidOperationException ex)
            {
                var currentStatus = _context.Applicants
                    .AsNoTracking()
                    .Where(x => x.ApplicationNo == applicationNo)
                    .Select(x => x.Application_status)
                    .FirstOrDefault();

                if (ex.Message == "Application is not ready for PHE verification.")
                {
                    return BadRequest(new
                    {
                        message = "Application is not ready for PHE verification.",
                        currentStatus = currentStatus
                    });
                }

                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
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
        public string? LayoutYesNo { get; set; }
        public string? Remark { get; set; }
        public string? Role { get; set; }
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
    }
}
