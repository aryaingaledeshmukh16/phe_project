using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PHE.API.Data;
using PHE.API.Models;

namespace PHE.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JEScrutinyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public JEScrutinyController(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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
                var applications = await _context.Applicants
                    .AsNoTracking()
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
// VIEW / DOWNLOAD DOCUMENT
// GET: api/JEScrutiny/document?path=Uploads/SatBara/file.pdf
// =========================================================
[HttpGet("document")]
public IActionResult GetDocument(
    [FromQuery] string path,
    [FromQuery] bool download = false)
{
    if (string.IsNullOrWhiteSpace(path))
    {
        return BadRequest(new
        {
            message = "Document path is required."
        });
    }

    // Security: only allow files inside Uploads folder
    path = path.Replace("/", Path.DirectorySeparatorChar.ToString())
               .Replace("\\", Path.DirectorySeparatorChar.ToString());

    if (!path.StartsWith(
            "Uploads" + Path.DirectorySeparatorChar,
            StringComparison.OrdinalIgnoreCase))
    {
        return BadRequest(new
        {
            message = "Invalid document path."
        });
    }

    var uploadsRoot = Path.Combine(
        Directory.GetCurrentDirectory(),
        "Uploads"
    );

    var fullPath = Path.GetFullPath(
        Path.Combine(
            Directory.GetCurrentDirectory(),
            path
        )
    );

    var uploadsFullPath = Path.GetFullPath(uploadsRoot);

    if (!fullPath.StartsWith(
            uploadsFullPath,
            StringComparison.OrdinalIgnoreCase))
    {
        return BadRequest(new
        {
            message = "Invalid document location."
        });
    }

    if (!System.IO.File.Exists(fullPath))
    {
        return NotFound(new
        {
            message = "Document file not found on server.",
            dbPath = path,
            physicalPath = fullPath
        });
    }

    var extension =
        Path.GetExtension(fullPath).ToLowerInvariant();

    var contentType = extension switch
    {
        ".pdf" => "application/pdf",
        ".jpg" => "image/jpeg",
        ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".kml" => "application/vnd.google-earth.kml+xml",
        ".kmz" => "application/vnd.google-earth.kmz",
        _ => "application/octet-stream"
    };

    var fileName = Path.GetFileName(fullPath);

    var stream = new FileStream(
        fullPath,
        FileMode.Open,
        FileAccess.Read,
        FileShare.Read
    );

    if (download)
    {
        return File(
            stream,
            contentType,
            fileName,
            enableRangeProcessing: true
        );
    }

    return File(
        stream,
        contentType,
        enableRangeProcessing: true
    );
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
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return BadRequest(new
                {
                    message = "Application number is required."
                });
            }

            if (string.IsNullOrWhiteSpace(documentType))
            {
                return BadRequest(new
                {
                    message = "Document type is required."
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
                        message = "Application not found."
                    });
                }

                string? dbPath = documentType
                    .Trim()
                    .ToLowerInvariant() switch
                {
                    "satbara" =>
                        application.SatBaraPath,

                    "layoutmap" =>
                        application.ApprovedLayoutMapPath,

                    "geotag" =>
                        application.GeoTagPhotoPath,

                    "kml" =>
                        application.KMLFilePath,

                    "taxnoc" =>
                        application.TaxNocPath,

                    _ => null
                };

                if (string.IsNullOrWhiteSpace(dbPath))
                {
                    return NotFound(new
                    {
                        message = "Document not uploaded.",
                        documentType
                    });
                }

                // -------------------------------------------------
                // Normalize DB path
                // -------------------------------------------------

                var cleanPath = dbPath
                    .Replace("\\", "/")
                    .Trim();

                // Remove leading /
                cleanPath = cleanPath.TrimStart('/');

                // If DB contains wwwroot/Uploads/... remove wwwroot
                if (cleanPath.StartsWith(
                    "wwwroot/",
                    StringComparison.OrdinalIgnoreCase))
                {
                    cleanPath = cleanPath.Substring(
                        "wwwroot/".Length);
                }

                // -------------------------------------------------
                // Physical file path
                // -------------------------------------------------

                var webRoot = _environment.WebRootPath;

                if (string.IsNullOrWhiteSpace(webRoot))
                {
                    webRoot = Path.Combine(
                        _environment.ContentRootPath,
                        "wwwroot");
                }

                var physicalPath = Path.Combine(
                    webRoot,
                    cleanPath.Replace(
                        "/",
                        Path.DirectorySeparatorChar.ToString()
                    )
                );

                // -------------------------------------------------
                // File does not exist
                // -------------------------------------------------

                if (!System.IO.File.Exists(physicalPath))
                {
                    return NotFound(new
                    {
                        message = "Document file not found on server.",
                        dbPath,
                        physicalPath
                    });
                }

                var extension =
                    Path.GetExtension(physicalPath)
                        .ToLowerInvariant();

                var contentType =
                    extension switch
                    {
                        ".pdf" =>
                            "application/pdf",

                        ".jpg" =>
                            "image/jpeg",

                        ".jpeg" =>
                            "image/jpeg",

                        ".png" =>
                            "image/png",

                        ".gif" =>
                            "image/gif",

                        ".kml" =>
                            "application/vnd.google-earth.kml+xml",

                        ".kmz" =>
                            "application/vnd.google-earth.kmz",

                        ".doc" =>
                            "application/msword",

                        ".docx" =>
                            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",

                        _ =>
                            "application/octet-stream"
                    };

                var fileName =
                    Path.GetFileName(physicalPath);

                // -------------------------------------------------
                // INLINE VIEW
                //
                // PDF/Image browser मध्ये उघडेल.
                // -------------------------------------------------

                Response.Headers.Append(
                    "Content-Disposition",
                    $"inline; filename=\"{fileName}\""
                );

                return PhysicalFile(
                    physicalPath,
                    contentType
                );
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
                        message = "Application not found."
                    });
                }

                string? dbPath = documentType
                    .Trim()
                    .ToLowerInvariant() switch
                {
                    "satbara" =>
                        application.SatBaraPath,

                    "layoutmap" =>
                        application.ApprovedLayoutMapPath,

                    "geotag" =>
                        application.GeoTagPhotoPath,

                    "kml" =>
                        application.KMLFilePath,

                    "taxnoc" =>
                        application.TaxNocPath,

                    _ => null
                };

                if (string.IsNullOrWhiteSpace(dbPath))
                {
                    return NotFound(new
                    {
                        message = "Document not uploaded."
                    });
                }

                var cleanPath = dbPath
                    .Replace("\\", "/")
                    .Trim()
                    .TrimStart('/');

                if (cleanPath.StartsWith(
                    "wwwroot/",
                    StringComparison.OrdinalIgnoreCase))
                {
                    cleanPath = cleanPath.Substring(
                        "wwwroot/".Length);
                }

                var webRoot = _environment.WebRootPath;

                if (string.IsNullOrWhiteSpace(webRoot))
                {
                    webRoot = Path.Combine(
                        _environment.ContentRootPath,
                        "wwwroot");
                }

                var physicalPath = Path.Combine(
                    webRoot,
                    cleanPath.Replace(
                        "/",
                        Path.DirectorySeparatorChar.ToString()
                    )
                );

                if (!System.IO.File.Exists(physicalPath))
                {
                    return NotFound(new
                    {
                        message = "Document file not found on server.",
                        dbPath
                    });
                }

                var extension =
                    Path.GetExtension(physicalPath)
                        .ToLowerInvariant();

                var contentType =
                    extension switch
                    {
                        ".pdf" =>
                            "application/pdf",

                        ".jpg" =>
                            "image/jpeg",

                        ".jpeg" =>
                            "image/jpeg",

                        ".png" =>
                            "image/png",

                        ".gif" =>
                            "image/gif",

                        ".kml" =>
                            "application/vnd.google-earth.kml+xml",

                        ".kmz" =>
                            "application/vnd.google-earth.kmz",

                        _ =>
                            "application/octet-stream"
                    };

                var fileName =
                    Path.GetFileName(physicalPath);

                return PhysicalFile(
                    physicalPath,
                    contentType,
                    fileName
                );
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
                var history = await _context.ApplicantsLogs
                    .AsNoTracking()
                    .Where(x =>
                        x.ApplicationNo == applicationNo)
                    .OrderByDescending(x => x.LogId)
                    .Select(x => new
                    {
                        logId = x.LogId,

                        applicationNo =
                            x.ApplicationNo,

                        role = x.role,

                        userCode =
                            x.user_code,

                        userName =
                            x.user_name,

                        scrutinyStatus =
                            x.scrutiny_status,

                        applicationStatus =
                            x.Application_status,

                        remark =
                            x.Remark,

                        entryDate =
                            x.entry_date
                    })
                    .ToListAsync();

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

            var oldApplicationStatus =
                application.Application_status ??
                application.Status ??
                "Pending";

            var action =
                request.Action.Trim();

            string newScrutinyStatus;
            string newApplicationStatus;

            if (action.Equals(
                "Accepted",
                StringComparison.OrdinalIgnoreCase))
            {
                newScrutinyStatus = "Accept";

                newApplicationStatus =
                    GetNextApplicationStatus(
                        oldApplicationStatus
                    );
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
                newApplicationStatus =
                    "Send Back to Jr. Engineer";
            }
            else
            {
                return BadRequest(new
                {
                    message = "Invalid scrutiny action."
                });
            }

            var entryDate = DateTime.Now;

            application.role =
                request.Role;

            application.user_code =
                request.UserCode;

            application.user_name =
                request.UserName;

            application.scrutiny_status =
                newScrutinyStatus;

            application.Application_status =
                newApplicationStatus;

            application.entry_date =
                entryDate;

            application.Status =
                newApplicationStatus;

            application.Remark =
                request.Remark;

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

    ShowAmountAsPerNoOfPlots =
        application.ShowAmountAsPerNoOfPlots,

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
                    message =
                        "Scrutiny updated successfully.",

                    applicationNo =
                        application.ApplicationNo,

                    scrutinyStatus =
                        application.scrutiny_status,

                    applicationStatus =
                        application.Application_status,

                    role =
                        application.role,

                    userCode =
                        application.user_code,

                    userName =
                        application.user_name,

                    entryDate =
                        application.entry_date,

                    remark =
                        application.Remark
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return StatusCode(500, new
                {
                    message =
                        "Scrutiny update failed.",

                    error =
                        ex.Message
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