using Microsoft.AspNetCore.Mvc;
using PHE.API.Application.Common.Interfaces;
using PHE.API.Application.Features.Documents.Queries;
using PHE.API.Application.Features.JEScrutiny.Commands;
using PHE.API.Application.Features.JEScrutiny.History;
using PHE.API.Application.Features.JEScrutiny.Queries;
using PHE.API.Models;

namespace PHE.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JEScrutinyController : ControllerBase
    {
        private readonly IGetJEScrutinyApplicationsHandler _getJEScrutinyApplicationsHandler;
        private readonly IGetJEScrutinyApplicationByApplicationNoHandler _getJEScrutinyApplicationByApplicationNoHandler;
        private readonly IGetJEScrutinyHistoryHandler _getJEScrutinyHistoryHandler;
        private readonly IGetJEScrutinyDocumentHandler _getJEScrutinyDocumentHandler;
        private readonly IGetApplicationDocumentHandler _getApplicationDocumentHandler;
        private readonly IGetApplicationDocumentDownloadHandler _getApplicationDocumentDownloadHandler;
        private readonly ISaveJEScrutinyActionHandler _saveJEScrutinyActionHandler;
        private readonly IGetJEScrutinyNocHandler _getJEScrutinyNocHandler;

        public JEScrutinyController(
            IGetJEScrutinyApplicationsHandler getJEScrutinyApplicationsHandler,
            IGetJEScrutinyApplicationByApplicationNoHandler getJEScrutinyApplicationByApplicationNoHandler,
            IGetJEScrutinyHistoryHandler getJEScrutinyHistoryHandler,
            IGetJEScrutinyDocumentHandler getJEScrutinyDocumentHandler,
            IGetApplicationDocumentHandler getApplicationDocumentHandler,
            IGetApplicationDocumentDownloadHandler getApplicationDocumentDownloadHandler,
            ISaveJEScrutinyActionHandler saveJEScrutinyActionHandler,
            IGetJEScrutinyNocHandler getJEScrutinyNocHandler)
        {
            _getJEScrutinyApplicationsHandler = getJEScrutinyApplicationsHandler;
            _getJEScrutinyApplicationByApplicationNoHandler = getJEScrutinyApplicationByApplicationNoHandler;
            _getJEScrutinyHistoryHandler = getJEScrutinyHistoryHandler;
            _getJEScrutinyDocumentHandler = getJEScrutinyDocumentHandler;
            _getApplicationDocumentHandler = getApplicationDocumentHandler;
            _getApplicationDocumentDownloadHandler = getApplicationDocumentDownloadHandler;
            _saveJEScrutinyActionHandler = saveJEScrutinyActionHandler;
            _getJEScrutinyNocHandler = getJEScrutinyNocHandler;
        }

        // =========================================================
        // GET ALL APPLICATIONS
        // GET: api/JEScrutiny
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetApplications()
        {
            try
            {
                var applications = await _getJEScrutinyApplicationsHandler.HandleAsync();

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
        // GET: api/JEScrutiny/{applicationNo}
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
                var application = await _getJEScrutinyApplicationByApplicationNoHandler.HandleAsync(applicationNo);

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
// VIEW / DOWNLOAD DOCUMENT
// GET: api/JEScrutiny/document?path=Uploads/SatBara/file.pdf
// =========================================================
[HttpGet("document")]
public async Task<IActionResult> GetDocument(
    [FromQuery] string path,
    [FromQuery] bool download = false)
{
    try
    {
        return await _getJEScrutinyDocumentHandler.HandleAsync(path, download);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new
        {
            message = "Document opening failed.",
            error = ex.Message
        });
    }
}

// =========================================================
// GENERATE WATER NOC HTML
//
// GET:
// api/JEScrutiny/{applicationNo}/noc
//
// Browser मध्ये NOC format open होईल.
// नंतर याच endpoint ला PDF मध्ये convert करू.
// =========================================================
[HttpGet("{applicationNo}/noc")]
public async Task<IActionResult> GetWaterNoc(
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
        var result = await _getJEScrutinyNocHandler.HandleAsync(applicationNo);

        switch (result.StatusCode)
        {
            case 200:
                return Content(result.HtmlContent ?? string.Empty, "text/html; charset=utf-8");
            case 400:
                return BadRequest(new
                {
                    message = result.Message,
                    applicationNo = result.ApplicationNo
                });
            case 403:
                return StatusCode(403, new
                {
                    message = result.Message,
                    applicationNo = result.ApplicationNo,
                    currentStatus = result.CurrentStatus
                });
            case 404:
                return NotFound(new
                {
                    message = result.Message,
                    applicationNo = result.ApplicationNo
                });
            default:
                return StatusCode(result.StatusCode, new
                {
                    message = result.Message,
                    error = result.Error
                });
        }
    }
    catch (Exception ex)
    {
        return StatusCode(500, new
        {
            message = "NOC generation failed.",
            error = ex.Message
        });
    }
}

        // =========================================================
        // VIEW / DOWNLOAD DOCUMENT
        //
        // GET:
        // api/JEScrutiny/{applicationNo}/documents/{documentType}
        //
        // documentType:
        // satbara
        // layoutmap
        // geotag
        // kml
        // taxnoc
        // =========================================================
        [HttpGet("{applicationNo}/documents/{documentType}")]
        public async Task<IActionResult> GetDocumentFile(
            string applicationNo,
            string documentType)
        {
            try
            {
                var result = await _getApplicationDocumentHandler.HandleAsync(applicationNo, documentType);

                if (result is PhysicalFileResult physicalFileResult)
                {
                    var fileName = Path.GetFileName(physicalFileResult.FileName ?? string.Empty);
                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        Response.Headers["Content-Disposition"] = $"inline; filename=\"{fileName}\"";
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Document opening failed.",
                    error = ex.Message
                });
            }
        }

        // =========================================================
        // DOWNLOAD DOCUMENT
        //
        // GET:
        // api/JEScrutiny/{applicationNo}/documents/{documentType}/download
        // =========================================================
        [HttpGet("{applicationNo}/documents/{documentType}/download")]
        public async Task<IActionResult> DownloadDocumentFile(
            string applicationNo,
            string documentType)
        {
            try
            {
                return await _getApplicationDocumentDownloadHandler.HandleAsync(applicationNo, documentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Document download failed.",
                    error = ex.Message
                });
            }
        }

        // =========================================================
        // GET APPLICATION HISTORY
        //
        // GET:
        // api/JEScrutiny/{applicationNo}/history
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
                var history = await _getJEScrutinyHistoryHandler.HandleAsync(applicationNo);

                return Ok(history);
            }
            catch (Exception ex)
            {
                // IMPORTANT:
                // Frontend ला valid JSON मिळेल.
                return StatusCode(500, new
                {
                    message = "History fetch failed.",
                    error = ex.Message
                });
            }
        }

        // =========================================================
        // SAVE SCRUTINY ACTION
        //
        // PUT:
        // api/JEScrutiny/{applicationNo}/action
        // =========================================================
        [HttpPut("{applicationNo}/action")]
        public async Task<IActionResult> SaveAction(
            string applicationNo,
            [FromBody] ScrutinyActionRequest request)
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

            try
            {
                var application = await _saveJEScrutinyActionHandler.HandleAsync(applicationNo, new JEScrutinyActionCommand
                {
                    Action = request.Action,
                    Remark = request.Remark,
                    Role = request.Role,
                    UserCode = request.UserCode,
                    UserName = request.UserName
                });

                return Ok(new
                {
                    message = "Scrutiny updated successfully.",
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
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Scrutiny update failed.",
                    error = ex.Message
                });
            }
        }

        // =========================================================
        // NEXT APPLICATION STATUS
        // =========================================================
        private static string GetNextApplicationStatus(
            string currentStatus)
        {
            var status =
                (currentStatus ?? "")
                    .Trim()
                    .ToLowerInvariant();

            return status switch
            {
                "" =>
                    "Document Verified",

                "pending" =>
                    "Document Verified",

                "document verified" =>
                    "Site Visit Completed",

                "site visit completed" =>
                    "Deputy Engineer Verification Completed",

                "deputy engineer verification completed" =>
                    "Due for Payment from Citizen",

                _ =>
                    currentStatus
            };
        }
    }

    // =============================================================
    // REQUEST MODEL
    // =============================================================
    public class ScrutinyActionRequest
    {
        public string Action { get; set; }
            = string.Empty;

        public string? Remark { get; set; }

        public string? Role { get; set; }

        public string? UserCode { get; set; }

        public string? UserName { get; set; }
    }
}