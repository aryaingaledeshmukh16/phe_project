using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PHE.API.Data;
using PHE.API.Models;
using ZXing;
using ZXing.Common;

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

        var currentStatus =
            (application.Application_status ?? application.Status ?? "")
            .Trim();

        if (!currentStatus.Equals(
            "Application Approved by PHE",
            StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(403, new
            {
                message = "NOC is available only after Application Approved by PHE.",
                applicationNo,
                currentStatus
            });
        }

        // -------------------------------------------------
        // DATA MAPPING
        // -------------------------------------------------

        var applicantName =
            application.FullName ?? "";

        var applicantAddress =
            application.Address ?? "";

        var applicationNoValue =
            application.ApplicationNo ?? "";

        // Old NOC:
        // lblnocnumber = application_no
        var nocNumber =
            applicationNoValue;

        // Old NOC:
        // lblnocdate = entry_date
        var nocDate =
            application.entry_date?
                .ToString("dd-MM-yyyy")
            ?? "";

       var applicationDate = application.CreatedDate.ToString("dd-MM-yyyy");

        var layoutAddress =
            application.LayoutAddress ?? "";

        var layoutNo =
            application.ApprovedLayoutNumber ?? "";

        var layoutDate =
            application.ApprovedLayoutDate?
                .ToString("dd-MM-yyyy")
            ?? "";

        // -------------------------------------------------
        // QR TEXT
        //
        // Old code:
        //
        // " App. No : " + Lblapplicationno.Text
        // + " /  Name : " + LblName.Text
        // -------------------------------------------------

        var qrText =
            $"App. No : {applicationNoValue} / Name : {applicantName}";

        // HTML encode all DB values
        var nameHtml =
            System.Net.WebUtility.HtmlEncode(applicantName);

        var addressHtml =
            System.Net.WebUtility.HtmlEncode(applicantAddress);

        var applicationNoHtml =
            System.Net.WebUtility.HtmlEncode(applicationNoValue);

        var nocNumberHtml =
            System.Net.WebUtility.HtmlEncode(nocNumber);

        var nocDateHtml =
            System.Net.WebUtility.HtmlEncode(nocDate);

        var applicationDateHtml =
            System.Net.WebUtility.HtmlEncode(applicationDate);

        var layoutAddressHtml =
            System.Net.WebUtility.HtmlEncode(layoutAddress);

        var layoutNoHtml =
            System.Net.WebUtility.HtmlEncode(layoutNo);

        var layoutDateHtml =
            System.Net.WebUtility.HtmlEncode(layoutDate);

        var qrTextHtml =
            System.Net.WebUtility.HtmlEncode(qrText);

        var frontendBaseUrl =
            Environment.GetEnvironmentVariable("FRONTEND_BASE_URL")
            ?? "http://localhost:3000";

        var logoUrl =
            $"{frontendBaseUrl.TrimEnd('/')}/smc-logo.png.jpg";

        var signatureUrl =
            $"{frontendBaseUrl.TrimEnd('/')}/choubesirsin.jpeg";

        var qrBase64 = GenerateQrBase64(qrText);

        // -------------------------------------------------
        // NOC HTML
        // -------------------------------------------------

        var html = $@"
<!DOCTYPE html>

<html lang='mr'>

<head>

<meta charset='UTF-8' />

<title>
Water NOC - {applicationNoHtml}
</title>

<style>

@page
{{
    size: A4;
    margin: 15mm;
}}

* {{
    box-sizing: border-box;
}}

body
{{
    margin: 0;
    padding: 0;
    background: #ffffff;
    color: #000000;

    font-family:
        'Noto Sans Devanagari',
        'Mangal',
        Arial,
        sans-serif;

    font-size: 16px;
}}

.noc-page
{{
    width: 100%;
    max-width: 800px;
    margin: 0 auto;

    border: 2px solid #000000;

    padding: 25px 35px;

    min-height: 1100px;
}}

.header
{{
    text-align: center;
    border-bottom: 2px solid #000000;

    padding-bottom: 12px;

    margin-bottom: 20px;
}}

.logo
{{
    width: 130px;
    height: 100px;

    border: 1px solid #000;

    margin-right: 25px;

    display: inline-flex;

    align-items: center;
    justify-content: center;

    vertical-align: middle;

    overflow: hidden;
    background: #fff;
    font-size: 13px;
}}

.logo-image,
.signature-image
{{
    display: block;
    width: 100%;
    height: 100%;
    object-fit: contain;
}}

.header-content
{{
    display: inline-block;

    width: 65%;

    vertical-align: middle;

    text-align: center;
}}

.header-title
{{
    font-size: 24px;
    font-weight: bold;

    margin-bottom: 5px;
}}

.header-line
{{
    font-size: 15px;

    line-height: 1.6;
}}

.subject
{{
    text-align: center;

    font-size: 18px;

    font-weight: 500;

    margin-top: 22px;

    margin-bottom: 15px;
}}

.reference
{{
    text-align: center;

    font-size: 17px;

    margin-bottom: 10px;
}}

.dashes
{{
    text-align: center;

    margin: 15px 0;
}}

.content
{{
    font-size: 18px;

    line-height: 1.8;
}}

.field
{{
    margin: 15px 0;

    font-size: 18px;
}}

.field-label
{{
    font-weight: bold;
}}

.intro
{{
    margin-top: 20px;

    margin-bottom: 20px;
}}

.note
{{
    margin-top: 25px;

    font-size: 18px;

    font-weight: 500;
}}

.signature-area
{{
    text-align: right;

    margin-top: 70px;

    min-height: 140px;
}}

.signature-box
{{
    width: 200px;

    height: 100px;

    border: 1px solid #000;

    margin-left: auto;

    display: flex;

    align-items: center;

    justify-content: center;

    overflow: hidden;
    background: #fff;
    font-size: 13px;
}}

.engineer
{{
    margin-top: 10px;

    font-size: 17px;

    font-weight: bold;
}}

.qr-area
{{
    margin-top: 35px;

    text-align: left;
}}

.qr-box
{{
    width: 170px;

    height: 170px;

    border: 1px solid #000;

    display: flex;

    align-items: center;

    justify-content: center;

    text-align: center;

    font-size: 12px;

    padding: 10px;
}}

.print-button
{{
    text-align: center;

    margin: 20px;
}}

button
{{
    padding: 10px 25px;

    font-size: 16px;

    cursor: pointer;
}}

@media print
{{
    .print-button
    {{
        display: none;
    }}

    .noc-page
    {{
        border: 2px solid #000;

        max-width: none;

        min-height: auto;
    }}
}}

</style>

</head>

<body>

<div class='print-button'>

<button onclick='window.print()'>
    Print / Save as PDF
</button>

</div>

<div class='noc-page'>

    <!-- HEADER -->

    <div class='header'>

        <div class='logo'>
            <img class='logo-image' src='{logoUrl}' alt='SMC Logo' />
        </div>

        <div class='header-content'>

            <div class='header-title'>
                सोलापूर महानगरपालिका सोलापूर
            </div>

            <div class='header-line'>
                इंद्रभुवन, डॉ.बाबासाहेब आंबेडकर चौक,
                रेल्वेलाईन सोलापूर-413001
            </div>

            <div class='header-line'>
                सार्वजनिक आरोग्य अभियंता यांचे कार्यालय
                (जलवितरण विभाग)
            </div>

            <div class='header-line'>
                ई मेल : smcphewtr@gmail.com
                टेलिफोन नं :-०२१७-२७४०३५०
            </div>

        </div>

    </div>


    <!-- APPLICANT -->

    <div class='content'>

        <div style='margin-top:25px;'>
            प्रति,
        </div>

        <div class='field'>
            श्री/श्रीमती
            <strong>{nameHtml}</strong>
        </div>

        <div class='field'>
            {addressHtml}
        </div>


        <!-- SUBJECT -->

        <div class='subject'>
            विषय - पिण्याचे पाण्याचे पाईपलाईन चे
            ना-हरकत प्रमाणपत्राबाबत
        </div>


        <!-- REFERENCE -->

        <div class='reference'>

            संदर्भ - आपले दि.
            <strong>{applicationDateHtml}</strong>

            रोजीचा अर्ज क्रमांक.
            <strong>{applicationNoHtml}</strong>

        </div>


        <div class='dashes'>
            - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        </div>


        <div class='intro'>

            खालीलप्रमाणे पिण्याचे पाण्याचे
            पाईपलाईनबाबत ना-हरकत प्रमाणपत्र देणेत येत आहे -

        </div>


        <!-- NOC NUMBER -->

        <div class='field'>

            <span class='field-label'>
                ना-हरकत प्रमाणपत्र क्रमांक :
            </span>

            {nocNumberHtml}

        </div>


        <!-- NOC DATE -->

        <div class='field'>

            <span class='field-label'>
                ना-हरकत प्रमाणपत्र दिनांक :
            </span>

            {nocDateHtml}

        </div>


        <!-- SITE ADDRESS -->

        <div class='field'>

            <span class='field-label'>
                जागेचा पत्ता:
            </span>

            {layoutAddressHtml}

        </div>


        <!-- LAYOUT NUMBER -->

        <div class='field'>

            <span class='field-label'>
                प्राथमिक मंजूर लेआऊट क्रमांक :
            </span>

            {layoutNoHtml}

        </div>


        <!-- LAYOUT DATE -->

        <div class='field'>

            <span class='field-label'>
                प्राथमिक मंजूर लेआऊट दिनांक :
            </span>

            {layoutDateHtml}

        </div>


        <div class='intro'>

            वरील नमूद ठिकाणी पिण्याचे पाण्याचे
            पाईपलाईन अस्तित्वात आहे.

        </div>


        <!-- NOTE -->

        <div class='note'>

            टीप :- सदरचा अभिप्राय फक्त बांधकाम
            परवानगीसाठीच वापर करावा.

        </div>


        <!-- SIGNATURE -->

        <div class='signature-area'>

            <div class='signature-box'>
                <img class='signature-image' src='{signatureUrl}' alt='Officer Signature' />
            </div>

            <div class='engineer'>
                सार्वजनिक आरोग्य अभियंता
            </div>

            <div class='engineer'>
                सोलापूर महानगरपालिका सोलापूर
            </div>

        </div>


        <!-- QR -->

        <div class='qr-area'>

            <div class='qr-box'>

                <img src='data:image/png;base64,{qrBase64}' alt='QR Code' style='max-width:100%; height:auto; display:block;' />

            </div>

        </div>

    </div>

</div>

</body>

</html>
";

        return Content(
            html,
            "text/html; charset=utf-8"
        );
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


private static string GenerateQrBase64(string text)
{
    var writer = new BarcodeWriterPixelData
    {
        Format = BarcodeFormat.QR_CODE,
        Options = new EncodingOptions
        {
            Width = 170,
            Height = 170,
            Margin = 0
        }
    };

    var pixelData = writer.Write(text);

    using var bitmap = new Bitmap(
        pixelData.Width,
        pixelData.Height,
        PixelFormat.Format32bppArgb);

    var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
    var bitmapData = bitmap.LockBits(
        rect,
        ImageLockMode.WriteOnly,
        PixelFormat.Format32bppArgb);

    try
    {
        Marshal.Copy(
            pixelData.Pixels,
            0,
            bitmapData.Scan0,
            pixelData.Pixels.Length);
    }
    finally
    {
        bitmap.UnlockBits(bitmapData);
    }

    using var stream = new MemoryStream();
    bitmap.Save(stream, ImageFormat.Png);

    return Convert.ToBase64String(stream.ToArray());
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