using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PHE.API.Data;
using PHE.API.Models;

namespace PHE.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JEApplicationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public JEApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/JEApplications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Applicant>>> GetApplications()
        {
            var applications = await _context.Applicants
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return Ok(applications);
        }

        // GET: api/JEApplications/20260001
        [HttpGet("{applicationNo}")]
        public async Task<ActionResult<Applicant>> GetApplication(
            string applicationNo)
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
                    message = "Application not found",
                    applicationNo = applicationNo
                });
            }

            return Ok(application);
        }
    }
}

