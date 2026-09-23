using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PHE.API.Application.Common.Interfaces;
using PHE.API.Application.Features.DeScrutiny.Commands;
using PHE.API.Application.Features.DeScrutiny.History;
using PHE.API.Application.Features.DeScrutiny.Queries;
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
        private readonly IGetDeScrutinyApplicationsHandler _getDeScrutinyApplicationsHandler;
        private readonly IGetDeScrutinyApplicationByApplicationNoHandler _getDeScrutinyApplicationByApplicationNoHandler;
        private readonly IGetDeScrutinyHistoryHandler _getDeScrutinyHistoryHandler;
        private readonly ISaveDeScrutinyActionHandler _saveDeScrutinyActionHandler;

        public DeScrutinyController(
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            IGetDeScrutinyApplicationsHandler getDeScrutinyApplicationsHandler,
            IGetDeScrutinyApplicationByApplicationNoHandler getDeScrutinyApplicationByApplicationNoHandler,
            IGetDeScrutinyHistoryHandler getDeScrutinyHistoryHandler,
            ISaveDeScrutinyActionHandler saveDeScrutinyActionHandler)
        {
            _context = context;
            _environment = environment;
            _getDeScrutinyApplicationsHandler = getDeScrutinyApplicationsHandler;
            _getDeScrutinyApplicationByApplicationNoHandler = getDeScrutinyApplicationByApplicationNoHandler;
            _getDeScrutinyHistoryHandler = getDeScrutinyHistoryHandler;
            _saveDeScrutinyActionHandler = saveDeScrutinyActionHandler;
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
                var applications = await _getDeScrutinyApplicationsHandler.HandleAsync();

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
                var application = await _getDeScrutinyApplicationByApplicationNoHandler.HandleAsync(applicationNo);

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
                var history = await _getDeScrutinyHistoryHandler.HandleAsync(applicationNo);

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
                var application = await _saveDeScrutinyActionHandler.HandleAsync(applicationNo, new DeScrutinyActionCommand
                {
                    Action = request.Action,
                    Remark = request.Remark,
                    Role = request.Role,
                    UserCode = request.UserCode,
                    UserName = request.UserName
                });

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
                var message = ex.Message;
                if (message == "Application is not in 'Development Charge Fixed' status.")
                {
                    var currentStatus = _context.Applicants
                        .AsNoTracking()
                        .Where(x => x.ApplicationNo == applicationNo)
                        .Select(x => x.Application_status)
                        .FirstOrDefault();

                    return BadRequest(new
                    {
                        message = "Application is not in 'Development Charge Fixed' status.",
                        currentStatus = currentStatus
                    });
                }

                return BadRequest(new
                {
                    message = message
                });
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
