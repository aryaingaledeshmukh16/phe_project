using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PHE.API.Data;
using PHE.API.DTOs;
using PHE.API.Models;

namespace PHE.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApplicantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET: api/Applicants
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetApplicants()
        {
            try
            {
                var applications = await _context.Applicants
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();

                return Ok(applications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Failed to fetch applicants.",
                    error = ex.Message
                });
            }
        }



        

        // =========================================================
        // GET: api/Applicants/{applicationNo}
        // =========================================================
        [HttpGet("{applicationNo}")]
        public async Task<IActionResult> GetApplicant(string applicationNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(applicationNo))
                {
                    return BadRequest(new
                    {
                        message = "Application Number is required."
                    });
                }

                var applicant = await _context.Applicants
                    .FirstOrDefaultAsync(x =>
                        x.ApplicationNo == applicationNo);

                if (applicant == null)
                {
                    return NotFound(new
                    {
                        message = "Application not found.",
                        applicationNo = applicationNo
                    });
                }

                return Ok(applicant);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Failed to fetch applicant.",
                    error = ex.Message
                });
            }
        }

        // =========================================================
        // POST: api/Applicants
        // =========================================================
        [HttpPost]
        public async Task<IActionResult> CreateApplicant(
            [FromBody] Applicant applicant)
        {
            try
            {
                if (applicant == null)
                {
                    return BadRequest(new
                    {
                        message = "Application data is required."
                    });
                }

                // -------------------------------------------------
                // Generate Application Number
                // -------------------------------------------------
                if (string.IsNullOrWhiteSpace(applicant.ApplicationNo))
                {
                    applicant.ApplicationNo =
                        DateTime.Now.ToString("yyyyMMddHHmmss");
                }

                // -------------------------------------------------
                // Initial Workflow Information
                // -------------------------------------------------
                applicant.role = "JE";
                applicant.user_code = null;
                applicant.user_name = null;

                applicant.scrutiny_status = "Pending";
                applicant.Application_status = "Pending";

                applicant.entry_date = DateTime.Now;

                // -------------------------------------------------
                // Created Date
                // -------------------------------------------------
                if (applicant.CreatedDate == default)
                {
                    applicant.CreatedDate = DateTime.Now;
                }

                // -------------------------------------------------
                // Status
                // -------------------------------------------------
                if (string.IsNullOrWhiteSpace(applicant.Status))
                {
                    applicant.Status = "Pending";
                }

                // -------------------------------------------------
                // Save Applicant
                // -------------------------------------------------
                _context.Applicants.Add(applicant);

                await _context.SaveChangesAsync();

                // -------------------------------------------------
                // First History Log
                // -------------------------------------------------
                var log = CreateApplicantLog(
                    applicant,
                    "Application Created"
                );

                _context.ApplicantsLogs.Add(log);

                await _context.SaveChangesAsync();

                // -------------------------------------------------
                // Return Application Number
                // -------------------------------------------------
                return Ok(new
                {
                    message = "Application saved successfully.",
                    applicationNo = applicant.ApplicationNo,
                    applicantId = applicant.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Application save failed.",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // =========================================================
        // POST: api/Applicants/Documents/{applicationNo}
        // =========================================================
        [HttpPost("Documents/{applicationNo}")]
        [RequestSizeLimit(50_000_000)]
        public async Task<IActionResult> UploadDocuments(
            string applicationNo,
            [FromForm] UploadDocumentsDto documents)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return BadRequest(new
                {
                    message = "Application Number is required."
                });
            }

            if (documents == null ||
                documents.TaxNoc == null ||
                documents.SatBara == null ||
                documents.LayoutMap == null ||
                documents.GeoTag == null)
            {
                return BadRequest(new
                {
                    message = "All four documents are required."
                });
            }

            try
            {
                var applicant = await _context.Applicants
                    .FirstOrDefaultAsync(x =>
                        x.ApplicationNo == applicationNo);

                if (applicant == null)
                {
                    return NotFound(new
                    {
                        message = "Application not found.",
                        applicationNo
                    });
                }

                var uploadRoot = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Uploads");

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

                    var fileName =
                        $"{Guid.NewGuid()}_{Path.GetFileName(item.File.FileName)}";
                    var fullPath = Path.Combine(folder, fileName);

                    await using var stream = new FileStream(
                        fullPath,
                        FileMode.CreateNew);
                    await item.File.CopyToAsync(stream);

                    savedPaths[item.Folder] =
                        $"Uploads/{item.Folder}/{fileName}";
                }

                applicant.TaxNocPath = savedPaths["TaxNOC"];
                applicant.SatBaraPath = savedPaths["SatBara"];
                applicant.ApprovedLayoutMapPath = savedPaths["LayoutMap"];
                applicant.GeoTagPhotoPath = savedPaths["Geotag"];

                await _context.SaveChangesAsync();

                var log = CreateApplicantLog(
                    applicant,
                    "Documents uploaded");

                _context.ApplicantsLogs.Add(log);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Documents uploaded successfully.",
                    applicationNo,
                    taxNocPath = applicant.TaxNocPath,
                    satBaraPath = applicant.SatBaraPath,
                    approvedLayoutMapPath = applicant.ApprovedLayoutMapPath,
                    geoTagPhotoPath = applicant.GeoTagPhotoPath
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Documents upload failed.",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // =========================================================
        // PUT: api/Applicants/Layout/{applicationNo}
        // =========================================================
        [HttpPut("Layout/{applicationNo}")]
        public async Task<IActionResult> UpdateLayout(
            string applicationNo,
            [FromBody] LayoutUpdateRequest request)
        {
            try
            {
                // -------------------------------------------------
                // Validate Application Number
                // -------------------------------------------------
                if (string.IsNullOrWhiteSpace(applicationNo))
                {
                    return BadRequest(new
                    {
                        message = "Application Number is required."
                    });
                }

                // -------------------------------------------------
                // Validate Request
                // -------------------------------------------------
                if (request == null)
                {
                    return BadRequest(new
                    {
                        message = "Layout information is required."
                    });
                }

                // -------------------------------------------------
                // Find Applicant
                // -------------------------------------------------
                var applicant = await _context.Applicants
                    .FirstOrDefaultAsync(x =>
                        x.ApplicationNo == applicationNo);

                if (applicant == null)
                {
                    return NotFound(new
                    {
                        message = "Application not found.",
                        applicationNo = applicationNo
                    });
                }

                // -------------------------------------------------
                // Update Layout Information
                // -------------------------------------------------
                applicant.ApplicationType =
                    request.ApplicationType?.Trim() ?? "";

                applicant.Peth =
                    request.Peth?.Trim() ?? "";

                applicant.Zone =
                    request.Zone?.Trim() ?? "";

                applicant.PropertyNumber =
                    request.PropertyNumber?.Trim() ?? "";

                applicant.LayoutAddress =
                    request.LayoutAddress?.Trim() ?? "";

                applicant.ApprovedLayoutNumber =
                    request.ApprovedLayoutNumber?.Trim() ?? "";

                // -------------------------------------------------
                // Approved Layout Date
                // -------------------------------------------------
                if (!string.IsNullOrWhiteSpace(
                    request.ApprovedLayoutDate))
                {
                    if (DateTime.TryParse(
                        request.ApprovedLayoutDate,
                        out DateTime layoutDate))
                    {
                        applicant.ApprovedLayoutDate =
                            layoutDate;
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            message =
                                "Invalid approved layout date.",
                            approvedLayoutDate =
                                request.ApprovedLayoutDate
                        });
                    }
                }

                // -------------------------------------------------
                // Keep Workflow Information
                // -------------------------------------------------
                if (string.IsNullOrWhiteSpace(
                    applicant.Application_status))
                {
                    applicant.Application_status = "Pending";
                }

                if (string.IsNullOrWhiteSpace(
                    applicant.scrutiny_status))
                {
                    applicant.scrutiny_status = "Pending";
                }

                if (string.IsNullOrWhiteSpace(
                    applicant.Status))
                {
                    applicant.Status = "Pending";
                }

                applicant.entry_date = DateTime.Now;

                // -------------------------------------------------
                // Save Applicant
                // -------------------------------------------------
                await _context.SaveChangesAsync();

                // -------------------------------------------------
                // Create History Log
                // -------------------------------------------------
                var log = CreateApplicantLog(
                    applicant,
                    "Layout information updated"
                );

                _context.ApplicantsLogs.Add(log);

                await _context.SaveChangesAsync();

                // -------------------------------------------------
                // Response
                // -------------------------------------------------
                return Ok(new
                {
                    message =
                        "Layout information saved successfully.",
                    applicationNo =
                        applicant.ApplicationNo,
                    applicantId =
                        applicant.Id
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new
                {
                    message = "Layout information save failed.",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Layout information save failed.",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // =========================================================
        // Create History Log
        // =========================================================
        private ApplicantsLog CreateApplicantLog(
            Applicant applicant,
            string remark)
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
                ApprovedLayoutNumber =
                    applicant.ApprovedLayoutNumber,

                ApprovedLayoutDate =
                    applicant.ApprovedLayoutDate,

                CreatedDate =
                    applicant.CreatedDate,

                Status =
                    applicant.Status,

                SatBaraPath =
                    applicant.SatBaraPath,

                ApprovedLayoutMapPath =
                    applicant.ApprovedLayoutMapPath,

                GeoTagPhotoPath =
                    applicant.GeoTagPhotoPath,

                KMLFilePath =
                    applicant.KMLFilePath,

                TaxNocPath =
                    applicant.TaxNocPath,

                Latitude =
                    applicant.Latitude,

                Longitude =
                    applicant.Longitude,

                TotalEstimateAmount =
                    applicant.TotalEstimateAmount,

                ShowAmountAsPerNoOfPlots =
                    applicant.ShowAmountAsPerNoOfPlots,

                // Workflow
                role =
                    applicant.role,

                user_code =
                    applicant.user_code,

                user_name =
                    applicant.user_name,

                scrutiny_status =
                    applicant.scrutiny_status,

                Application_status =
                    applicant.Application_status,

                entry_date =
                    DateTime.Now,

                Remark =
                    remark
            };
        }
    }

    

    // =============================================================
    // Layout Request Model
    // =============================================================
    public class LayoutUpdateRequest
    {
        public string? ApplicationType { get; set; }

        public string? Peth { get; set; }

        public string? Zone { get; set; }

        public string? PropertyNumber { get; set; }

        public string? LayoutAddress { get; set; }

        public string? ApprovedLayoutNumber { get; set; }

        public string? ApprovedLayoutDate { get; set; }
    }


    
}