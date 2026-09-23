using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PHE.API.Application.Common.Interfaces;
using PHE.API.Application.Features.Applications.Commands;
using PHE.API.Application.Features.Applications.Queries;
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
        private readonly IGetApplicantsHandler _getApplicantsHandler;
        private readonly IGetApplicantByApplicationNoHandler _getApplicantByApplicationNoHandler;
        private readonly ICreateApplicantHandler _createApplicantHandler;
        private readonly IUpdateApplicantLayoutHandler _updateApplicantLayoutHandler;
        private readonly ISaveApplicantSiteVisitHandler _saveApplicantSiteVisitHandler;
        private readonly IUploadApplicantDocumentsHandler _uploadApplicantDocumentsHandler;

        public ApplicantsController(
            ApplicationDbContext context,
            IGetApplicantsHandler getApplicantsHandler,
            IGetApplicantByApplicationNoHandler getApplicantByApplicationNoHandler,
            ICreateApplicantHandler createApplicantHandler,
            IUpdateApplicantLayoutHandler updateApplicantLayoutHandler,
            ISaveApplicantSiteVisitHandler saveApplicantSiteVisitHandler,
            IUploadApplicantDocumentsHandler uploadApplicantDocumentsHandler)
        {
            _context = context;
            _getApplicantsHandler = getApplicantsHandler;
            _getApplicantByApplicationNoHandler = getApplicantByApplicationNoHandler;
            _createApplicantHandler = createApplicantHandler;
            _updateApplicantLayoutHandler = updateApplicantLayoutHandler;
            _saveApplicantSiteVisitHandler = saveApplicantSiteVisitHandler;
            _uploadApplicantDocumentsHandler = uploadApplicantDocumentsHandler;
        }

        // =========================================================
        // GET: api/Applicants
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetApplicants()
        {
            try
            {
                var applications = await _getApplicantsHandler.HandleAsync();

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

                var applicant = await _getApplicantByApplicationNoHandler.HandleAsync(applicationNo);

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

                var createdApplicant = await _createApplicantHandler.HandleAsync(applicant);

                return Ok(new
                {
                    message = "Application saved successfully.",
                    applicationNo = createdApplicant.ApplicationNo,
                    applicantId = createdApplicant.Id
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
                var applicant = await _uploadApplicantDocumentsHandler.HandleAsync(applicationNo, documents);

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
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    message = "Application not found.",
                    applicationNo
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
                if (string.IsNullOrWhiteSpace(applicationNo))
                {
                    return BadRequest(new
                    {
                        message = "Application Number is required."
                    });
                }

                if (request == null)
                {
                    return BadRequest(new
                    {
                        message = "Layout information is required."
                    });
                }

                try
                {
                    var applicant = await _updateApplicantLayoutHandler.HandleAsync(applicationNo, new LayoutUpdateCommand
                    {
                        ApplicationType = request.ApplicationType,
                        Peth = request.Peth,
                        Zone = request.Zone,
                        PropertyNumber = request.PropertyNumber,
                        LayoutAddress = request.LayoutAddress,
                        ApprovedLayoutNumber = request.ApprovedLayoutNumber,
                        ApprovedLayoutDate = request.ApprovedLayoutDate
                    });

                    return Ok(new
                    {
                        message = "Layout information saved successfully.",
                        applicationNo = applicant.ApplicationNo,
                        applicantId = applicant.Id
                    });
                }
                catch (KeyNotFoundException)
                {
                    return NotFound(new
                    {
                        message = "Application not found.",
                        applicationNo = applicationNo
                    });
                }
                catch (FormatException ex)
                {
                    return BadRequest(new
                    {
                        message = "Invalid approved layout date.",
                        approvedLayoutDate = request.ApprovedLayoutDate,
                        error = ex.Message
                    });
                }
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

        // =========================================================
        // POST: api/Applicants/SiteVisit/{applicationNo}
        // =========================================================
        [HttpPost("SiteVisit/{applicationNo}")]
        [RequestSizeLimit(50_000_000)]
        public async Task<IActionResult> SaveSiteVisit(
            string applicationNo,
            [FromForm] SiteVisitDto siteVisit)
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
            // Validate DTO
            // -------------------------------------------------
            if (siteVisit == null)
            {
                return BadRequest(new
                {
                    message = "Site Visit data is required."
                });
            }



            // -------------------------------------------------
            // Validate LayoutYesNo
            // -------------------------------------------------
            if (string.IsNullOrWhiteSpace(siteVisit.LayoutYesNo) ||
                siteVisit.LayoutYesNo == "Select")
            {
                return BadRequest(new
                {
                    message = "Layout Pipeline Status must be selected."
                });
            }

            // -------------------------------------------------
            // Validate status
            // -------------------------------------------------
            if (string.IsNullOrWhiteSpace(siteVisit.Status) ||
                siteVisit.Status == "Status")
            {
                return BadRequest(new
                {
                    message = "Site Status must be selected (Accept, Reject, or Hold)."
                });
            }

            // -------------------------------------------------
            // Validate GeoTagPhoto - always required
            // -------------------------------------------------
            if (siteVisit.SiteVisitGeoTagPhoto == null)
            {
                return BadRequest(new
                {
                    message = "Site Visit Geo Tag Photo is required."
                });
            }

            // -------------------------------------------------
            // Conditional validation for no pipeline case
            // -------------------------------------------------
            if (siteVisit.LayoutYesNo == "अस्तित्वात नाही")
            {
                if (siteVisit.TotalEstimateAmount == null ||
                    siteVisit.TotalEstimateAmount <= 0)
                {
                    return BadRequest(new
                    {
                        message = "Total Estimate Amount is required and must be greater than 0."
                    });
                }

                if (siteVisit.TotalPlots == null ||
                    siteVisit.TotalPlots <= 0)
                {
                    return BadRequest(new
                    {
                        message = "Total Plots is required and must be greater than 0."
                    });
                }

                if (siteVisit.PlotsApplicableForThisNoc == null ||
                    siteVisit.PlotsApplicableForThisNoc <= 0)
                {
                    return BadRequest(new
                    {
                        message = "Plots applicable for this NOC is required and must be greater than 0."
                    });
                }

                if (siteVisit.PlotsApplicableForThisNoc >
                    siteVisit.TotalPlots)
                {
                    return BadRequest(new
                    {
                        message = "Plots applicable cannot exceed Total Plots."
                    });
                }

                if (siteVisit.SiteVisitEstimateDocument == null)
                {
                    return BadRequest(new
                    {
                        message = "Estimate Document is required when pipeline does not exist."
                    });
                }
            }

            try
            {
                try
                {
                    var applicant = await _saveApplicantSiteVisitHandler.HandleAsync(applicationNo, siteVisit);

                    return Ok(new
                    {
                        message = "Site Visit saved successfully.",
                        applicationNo,
                        layoutYesNo = applicant.LayoutYesNo,
                        totalPlots = applicant.TotalPlots,
                        plotsApplicableForThisNoc = applicant.PlotsApplicableForThisNoc,
                        amountForPlots = applicant.AmountForPlots,
                        siteVisitEstimateDocumentPath = applicant.SiteVisitEstimateDocumentPath,
                        siteVisitGeoTagPhotoPath = applicant.SiteVisitGeoTagPhotoPath,
                        status = applicant.scrutiny_status,
                        remark = applicant.Remark,
                        applicationStatus = applicant.Application_status
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
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Site Visit save failed.",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // =========================================================
        // Helper: Create Applicant Log for Site Visit
        // =========================================================
        private ApplicantsLog CreateApplicantLogForSiteVisit(
            Applicant applicant,
            string remark)
        {
            var log = CreateApplicantLog(applicant, remark);

            // Add Site Visit fields to the log
            log.LayoutYesNo = applicant.LayoutYesNo;
            log.TotalPlots = applicant.TotalPlots;
            log.PlotsApplicableForThisNoc =
                applicant.PlotsApplicableForThisNoc;
            log.AmountForPlots = applicant.AmountForPlots;
            log.SiteVisitEstimateDocumentPath =
                applicant.SiteVisitEstimateDocumentPath;
            log.SiteVisitGeoTagPhotoPath =
                applicant.SiteVisitGeoTagPhotoPath;
            return log;
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