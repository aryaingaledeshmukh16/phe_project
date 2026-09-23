using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using PHE.API.Application.Common.Interfaces;
using PHE.API.Application.Features.JEScrutiny.Queries;
using PHE.API.Data;
using ZXing;
using ZXing.Common;

namespace PHE.API.Infrastructure.Services
{
    public class NocService : INocService
    {
        private readonly ApplicationDbContext _context;

        public NocService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<JEScrutinyNocResult> GetWaterNocAsync(string applicationNo, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return new JEScrutinyNocResult
                {
                    StatusCode = 400,
                    Message = "Application number is required.",
                    ApplicationNo = applicationNo
                };
            }

            try
            {
                var application = await _context.Applicants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.ApplicationNo == applicationNo,
                        cancellationToken);

                if (application == null)
                {
                    return new JEScrutinyNocResult
                    {
                        StatusCode = 404,
                        Message = "Application not found.",
                        ApplicationNo = applicationNo
                    };
                }

                var currentStatus =
                    (application.Application_status ?? application.Status ?? "")
                    .Trim();

                if (!currentStatus.Equals(
                    "Application Approved by PHE",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return new JEScrutinyNocResult
                    {
                        StatusCode = 403,
                        Message = "NOC is available only after Application Approved by PHE.",
                        ApplicationNo = applicationNo,
                        CurrentStatus = currentStatus
                    };
                }

                var applicantName = application.FullName ?? "";
                var applicantAddress = application.Address ?? "";
                var applicationNoValue = application.ApplicationNo ?? "";
                var nocNumber = applicationNoValue;
                var nocDate = application.entry_date?.ToString("dd-MM-yyyy") ?? "";
                var applicationDate = application.CreatedDate.ToString("dd-MM-yyyy");
                var layoutAddress = application.LayoutAddress ?? "";
                var layoutNo = application.ApprovedLayoutNumber ?? "";
                var layoutDate = application.ApprovedLayoutDate?.ToString("dd-MM-yyyy") ?? "";

                var qrText = $"App. No : {applicationNoValue} / Name : {applicantName}";

                var nameHtml = System.Net.WebUtility.HtmlEncode(applicantName);
                var addressHtml = System.Net.WebUtility.HtmlEncode(applicantAddress);
                var applicationNoHtml = System.Net.WebUtility.HtmlEncode(applicationNoValue);
                var nocNumberHtml = System.Net.WebUtility.HtmlEncode(nocNumber);
                var nocDateHtml = System.Net.WebUtility.HtmlEncode(nocDate);
                var applicationDateHtml = System.Net.WebUtility.HtmlEncode(applicationDate);
                var layoutAddressHtml = System.Net.WebUtility.HtmlEncode(layoutAddress);
                var layoutNoHtml = System.Net.WebUtility.HtmlEncode(layoutNo);
                var layoutDateHtml = System.Net.WebUtility.HtmlEncode(layoutDate);
                var qrTextHtml = System.Net.WebUtility.HtmlEncode(qrText);

                var frontendBaseUrl = Environment.GetEnvironmentVariable("FRONTEND_BASE_URL") ?? "http://localhost:3000";
                var logoUrl = $"{frontendBaseUrl.TrimEnd('/')}/smc-logo.png.jpg";
                var signatureUrl = $"{frontendBaseUrl.TrimEnd('/')}/choubesirsin.jpeg";
                var qrBase64 = GenerateQrBase64(qrText);

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
                प्राथमिक मंजूर लेआउट क्रमांक :
            </span>

            {layoutNoHtml}

        </div>


        <!-- LAYOUT DATE -->

        <div class='field'>

            <span class='field-label'>
                प्राथमिक मंजूर लेआउट दिनांक :
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

                return new JEScrutinyNocResult
                {
                    StatusCode = 200,
                    ApplicationNo = applicationNo,
                    HtmlContent = html
                };
            }
            catch (Exception ex)
            {
                return new JEScrutinyNocResult
                {
                    StatusCode = 500,
                    Message = "NOC generation failed.",
                    ApplicationNo = applicationNo,
                    Error = ex.Message
                };
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
    }
}
