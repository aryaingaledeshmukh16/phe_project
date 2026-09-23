using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PHE.API.Application.Common.Interfaces;
using PHE.API.Application.Common.Models;
using PHE.API.Data;
using PHE.API.Models;

namespace PHE.API.Infrastructure.Services
{
    public class DocumentReadService : IDocumentReadService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DocumentReadService(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<DocumentReadResult> GetDocumentAsync(string path, bool download = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return new DocumentReadResult
                {
                    Message = "Document path is required."
                };
            }

            path = path.Replace("/", Path.DirectorySeparatorChar.ToString())
                       .Replace("\\", Path.DirectorySeparatorChar.ToString());

            if (!path.StartsWith("Uploads" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                return new DocumentReadResult
                {
                    Message = "Invalid document path."
                };
            }

            var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            var fullPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), path));
            var uploadsFullPath = Path.GetFullPath(uploadsRoot);

            if (!fullPath.StartsWith(uploadsFullPath, StringComparison.OrdinalIgnoreCase))
            {
                return new DocumentReadResult
                {
                    Message = "Invalid document location."
                };
            }

            if (!System.IO.File.Exists(fullPath))
            {
                return new DocumentReadResult
                {
                    Message = "Document file not found on server.",
                    Error = new
                    {
                        dbPath = path,
                        physicalPath = fullPath
                    }
                };
            }

            var extension = Path.GetExtension(fullPath).ToLowerInvariant();
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
            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            if (download)
            {
                return new DocumentReadResult
                {
                    Result = new FileStreamResult(stream, contentType)
                    {
                        FileDownloadName = fileName,
                        EnableRangeProcessing = true
                    }
                };
            }

            return new DocumentReadResult
            {
                Result = new FileStreamResult(stream, contentType)
                {
                    EnableRangeProcessing = true
                }
            };
        }

        public async Task<DocumentReadResult> GetApplicationDocumentAsync(string applicationNo, string documentType, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return new DocumentReadResult { Message = "Application number is required." };
            }

            if (string.IsNullOrWhiteSpace(documentType))
            {
                return new DocumentReadResult { Message = "Document type is required." };
            }

            try
            {
                var application = await _context.Applicants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ApplicationNo == applicationNo, cancellationToken);

                if (application == null)
                {
                    return new DocumentReadResult { Message = "Application not found." };
                }

                string? dbPath = documentType.Trim().ToLowerInvariant() switch
                {
                    "satbara" => application.SatBaraPath,
                    "layoutmap" => application.ApprovedLayoutMapPath,
                    "geotag" => application.GeoTagPhotoPath,
                    "kml" => application.KMLFilePath,
                    "taxnoc" => application.TaxNocPath,
                    _ => null
                };

                if (string.IsNullOrWhiteSpace(dbPath))
                {
                    return new DocumentReadResult { Message = "Document not uploaded." };
                }

                var cleanPath = dbPath.Replace("\\", "/").Trim();
                cleanPath = cleanPath.TrimStart('/');

                if (cleanPath.StartsWith("wwwroot/", StringComparison.OrdinalIgnoreCase))
                {
                    cleanPath = cleanPath.Substring("wwwroot/".Length);
                }

                var webRoot = _environment.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRoot))
                {
                    webRoot = Path.Combine(_environment.ContentRootPath, "wwwroot");
                }

                var physicalPath = Path.Combine(webRoot, cleanPath.Replace("/", Path.DirectorySeparatorChar.ToString()));

                if (!System.IO.File.Exists(physicalPath))
                {
                    return new DocumentReadResult
                    {
                        Message = "Document file not found on server.",
                        Error = new
                        {
                            dbPath,
                            physicalPath
                        }
                    };
                }

                var extension = Path.GetExtension(physicalPath).ToLowerInvariant();
                var contentType = extension switch
                {
                    ".pdf" => "application/pdf",
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".kml" => "application/vnd.google-earth.kml+xml",
                    ".kmz" => "application/vnd.google-earth.kmz",
                    ".doc" => "application/msword",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    _ => "application/octet-stream"
                };

                var fileName = Path.GetFileName(physicalPath);
                var stream = new FileStream(physicalPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                var result = new FileStreamResult(stream, contentType)
                {
                    EnableRangeProcessing = true
                };

                return new DocumentReadResult { Result = result };
            }
            catch (Exception ex)
            {
                return new DocumentReadResult
                {
                    Message = "Document opening failed.",
                    Error = ex.Message
                };
            }
        }

        public async Task<DocumentReadResult> GetApplicationDocumentDownloadAsync(string applicationNo, string documentType, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return new DocumentReadResult { Message = "Application number is required." };
            }

            if (string.IsNullOrWhiteSpace(documentType))
            {
                return new DocumentReadResult { Message = "Document type is required." };
            }

            try
            {
                var application = await _context.Applicants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ApplicationNo == applicationNo, cancellationToken);

                if (application == null)
                {
                    return new DocumentReadResult { Message = "Application not found." };
                }

                string? dbPath = documentType.Trim().ToLowerInvariant() switch
                {
                    "satbara" => application.SatBaraPath,
                    "layoutmap" => application.ApprovedLayoutMapPath,
                    "geotag" => application.GeoTagPhotoPath,
                    "kml" => application.KMLFilePath,
                    "taxnoc" => application.TaxNocPath,
                    _ => null
                };

                if (string.IsNullOrWhiteSpace(dbPath))
                {
                    return new DocumentReadResult { Message = "Document not uploaded." };
                }

                var cleanPath = dbPath.Replace("\\", "/").Trim().TrimStart('/');
                if (cleanPath.StartsWith("wwwroot/", StringComparison.OrdinalIgnoreCase))
                {
                    cleanPath = cleanPath.Substring("wwwroot/".Length);
                }

                var webRoot = _environment.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRoot))
                {
                    webRoot = Path.Combine(_environment.ContentRootPath, "wwwroot");
                }

                var physicalPath = Path.Combine(webRoot, cleanPath.Replace("/", Path.DirectorySeparatorChar.ToString()));

                if (!System.IO.File.Exists(physicalPath))
                {
                    return new DocumentReadResult
                    {
                        Message = "Document file not found on server.",
                        Error = new { dbPath }
                    };
                }

                var extension = Path.GetExtension(physicalPath).ToLowerInvariant();
                var contentType = extension switch
                {
                    ".pdf" => "application/pdf",
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".kml" => "application/vnd.google-earth.kml+xml",
                    ".kmz" => "application/vnd.google-earth.kmz",
                    _ => "application/octet-stream"
                };

                var fileName = Path.GetFileName(physicalPath);
                var stream = new FileStream(physicalPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                return new DocumentReadResult
                {
                    Result = new FileStreamResult(stream, contentType)
                    {
                        FileDownloadName = fileName,
                        EnableRangeProcessing = true
                    }
                };
            }
            catch (Exception ex)
            {
                return new DocumentReadResult
                {
                    Message = "Document download failed.",
                    Error = ex.Message
                };
            }
        }
    }
}
