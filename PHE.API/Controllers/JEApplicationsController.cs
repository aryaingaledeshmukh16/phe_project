using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PHE.API.Application.Features.JEApplications.Queries;
using PHE.API.Data;
using PHE.API.Models;

namespace PHE.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JEApplicationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IGetJEApplicationsHandler _getJEApplicationsHandler;
        private readonly IGetJEApplicationByApplicationNoHandler _getJEApplicationByApplicationNoHandler;

        public JEApplicationsController(
            ApplicationDbContext context,
            IGetJEApplicationsHandler getJEApplicationsHandler,
            IGetJEApplicationByApplicationNoHandler getJEApplicationByApplicationNoHandler)
        {
            _context = context;
            _getJEApplicationsHandler = getJEApplicationsHandler;
            _getJEApplicationByApplicationNoHandler = getJEApplicationByApplicationNoHandler;
        }

        // GET: api/JEApplications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Applicant>>> GetApplications()
        {
            var applications = await _getJEApplicationsHandler.HandleAsync();

            return Ok(applications);
        }

        // GET: api/JEApplications/20260001
        [HttpGet("{applicationNo}")]
        public async Task<ActionResult<Applicant>> GetApplication(
            string applicationNo)
        {
            var application = await _getJEApplicationByApplicationNoHandler.HandleAsync(applicationNo);

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

