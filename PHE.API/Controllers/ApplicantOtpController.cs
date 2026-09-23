using Microsoft.AspNetCore.Mvc;
using PHE.API.Application.Features.Applications.Commands;

namespace PHE.API.Controllers
{
    [ApiController]
    [Route("api/Applicant")]
    public class ApplicantOtpController : ControllerBase
    {
        private readonly ISendApplicantOtpHandler _sendApplicantOtpHandler;
        private readonly IVerifyApplicantOtpHandler _verifyApplicantOtpHandler;

        public ApplicantOtpController(
            ISendApplicantOtpHandler sendApplicantOtpHandler,
            IVerifyApplicantOtpHandler verifyApplicantOtpHandler)
        {
            _sendApplicantOtpHandler = sendApplicantOtpHandler;
            _verifyApplicantOtpHandler = verifyApplicantOtpHandler;
        }

        [HttpPost("SendOtp/{applicationNo}")]
        public async Task<IActionResult> SendOtp(string applicationNo)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return BadRequest(new
                {
                    message = "Application Number is required."
                });
            }

            try
            {
                var applicant = await _sendApplicantOtpHandler.HandleAsync(applicationNo);

                return Ok(new
                {
                    message = "OTP sent successfully.",
                    applicationNo,
                    otp = applicant.OTP
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
                    message = ex.Message,
                    applicationNo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "OTP generation failed.",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        [HttpPost("VerifyOtp/{applicationNo}")]
        public async Task<IActionResult> VerifyOtp(string applicationNo, [FromBody] VerifyOtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return BadRequest(new
                {
                    message = "Application Number is required."
                });
            }

            if (request == null || string.IsNullOrWhiteSpace(request.Otp))
            {
                return BadRequest(new
                {
                    message = "OTP is required."
                });
            }

            try
            {
                var applicant = await _verifyApplicantOtpHandler.HandleAsync(applicationNo, request.Otp);

                return Ok(new
                {
                    message = "OTP verified successfully.",
                    applicationNo,
                    verified = applicant.IsOTPVerified
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
                    message = ex.Message,
                    applicationNo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "OTP verification failed.",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }
    }

    public class VerifyOtpRequest
    {
        public string Otp { get; set; } = string.Empty;
    }
}
