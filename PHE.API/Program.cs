using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi;
using PHE.API.Application.Common.Interfaces;
using PHE.API.Application.Features.Applications.Commands;
using PHE.API.Application.Features.Applications.Queries;
using PHE.API.Application.Features.DeScrutiny.Commands;
using PHE.API.Application.Features.DeScrutiny.History;
using PHE.API.Application.Features.DeScrutiny.Queries;
using PHE.API.Application.Features.Documents.Queries;
using PHE.API.Application.Features.JEApplications.Queries;
using PHE.API.Application.Features.JEScrutiny.Commands;
using PHE.API.Application.Features.JEScrutiny.History;
using PHE.API.Application.Features.JEScrutiny.Queries;
using PHE.API.Application.Features.PHEScrutiny.Commands;
using PHE.API.Application.Features.PHEScrutiny.History;
using PHE.API.Application.Features.PHEScrutiny.Queries;
using PHE.API.Data;
using PHE.API.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// DATABASE
// =========================================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));


// =========================================================
// CONTROLLERS
// =========================================================

builder.Services.AddControllers();

builder.Services.AddScoped<IApplicantReadService, ApplicantReadService>();
builder.Services.AddScoped<IApplicantWriteService, ApplicantWriteService>();
builder.Services.AddScoped<IGetApplicantsHandler, GetApplicantsHandler>();
builder.Services.AddScoped<ICreateApplicantHandler, CreateApplicantHandler>();
builder.Services.AddScoped<IUpdateApplicantLayoutHandler, UpdateApplicantLayoutHandler>();
builder.Services.AddScoped<ISaveApplicantSiteVisitHandler, SaveApplicantSiteVisitHandler>();
builder.Services.AddScoped<IUploadApplicantDocumentsHandler, UploadApplicantDocumentsHandler>();
builder.Services.AddScoped<ISendApplicantOtpHandler, SendApplicantOtpHandler>();
builder.Services.AddScoped<IVerifyApplicantOtpHandler, VerifyApplicantOtpHandler>();
builder.Services.AddScoped<IGetApplicantByApplicationNoHandler, GetApplicantByApplicationNoHandler>();
builder.Services.AddScoped<IDeScrutinyReadService, DeScrutinyReadService>();
builder.Services.AddScoped<IDeScrutinyWriteService, DeScrutinyWriteService>();
builder.Services.AddScoped<IGetDeScrutinyApplicationsHandler, GetDeScrutinyApplicationsHandler>();
builder.Services.AddScoped<IGetDeScrutinyApplicationByApplicationNoHandler, GetDeScrutinyApplicationByApplicationNoHandler>();
builder.Services.AddScoped<ISaveDeScrutinyActionHandler, SaveDeScrutinyActionHandler>();
builder.Services.AddScoped<IDeScrutinyHistoryReadService, DeScrutinyHistoryReadService>();
builder.Services.AddScoped<IGetDeScrutinyHistoryHandler, GetDeScrutinyHistoryHandler>();
builder.Services.AddScoped<IJEApplicationReadService, JEApplicationReadService>();
builder.Services.AddScoped<IGetJEApplicationsHandler, GetJEApplicationsHandler>();
builder.Services.AddScoped<IGetJEApplicationByApplicationNoHandler, GetJEApplicationByApplicationNoHandler>();
builder.Services.AddScoped<IPHEScrutinyReadService, PHEScrutinyReadService>();
builder.Services.AddScoped<IPHEScrutinyWriteService, PHEScrutinyWriteService>();
builder.Services.AddScoped<IGetPHEScrutinyApplicationsHandler, GetPHEScrutinyApplicationsHandler>();
builder.Services.AddScoped<IGetPHEScrutinyApplicationByApplicationNoHandler, GetPHEScrutinyApplicationByApplicationNoHandler>();
builder.Services.AddScoped<ISavePHEScrutinyActionHandler, SavePHEScrutinyActionHandler>();
builder.Services.AddScoped<IPHEScrutinyHistoryReadService, PHEScrutinyHistoryReadService>();
builder.Services.AddScoped<IGetPHEScrutinyHistoryHandler, GetPHEScrutinyHistoryHandler>();
builder.Services.AddScoped<IGetJEScrutinyApplicationsHandler, GetJEScrutinyApplicationsHandler>();
builder.Services.AddScoped<IJEScrutinyReadService, JEScrutinyReadService>();
builder.Services.AddScoped<IJEScrutinyWriteService, JEScrutinyWriteService>();
builder.Services.AddScoped<IGetJEScrutinyApplicationByApplicationNoHandler, GetJEScrutinyApplicationByApplicationNoHandler>();
builder.Services.AddScoped<ISaveJEScrutinyActionHandler, SaveJEScrutinyActionHandler>();
builder.Services.AddScoped<IGetJEScrutinyHistoryHandler, GetJEScrutinyHistoryHandler>();
builder.Services.AddScoped<IGetJEScrutinyNocHandler, GetJEScrutinyNocHandler>();
builder.Services.AddScoped<INocService, NocService>();
builder.Services.AddScoped<IDocumentReadService, DocumentReadService>();
builder.Services.AddScoped<IGetJEScrutinyDocumentHandler, GetJEScrutinyDocumentHandler>();
builder.Services.AddScoped<IGetApplicationDocumentHandler, GetApplicationDocumentHandler>();
builder.Services.AddScoped<IGetApplicationDocumentDownloadHandler, GetApplicationDocumentDownloadHandler>();


// =========================================================
// CORS - NEXT.JS
// =========================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("NextJsPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://127.0.0.1:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// =========================================================
// SWAGGER
// =========================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PHE.API",
        Version = "v1"
    });
});


// =========================================================
// BUILD APP
// =========================================================

var app = builder.Build();


// =========================================================
// DATABASE DI CHECK
// =========================================================

try
{
    using (var checkScope = app.Services.CreateScope())
    {
        var svc =
            checkScope.ServiceProvider
                .GetService<ApplicationDbContext>();

        if (svc == null)
        {
            Console.WriteLine(
                "DI CHECK: ApplicationDbContext NOT registered."
            );
        }
        else
        {
            Console.WriteLine(
                "DI CHECK: ApplicationDbContext registered."
            );
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(
        "DI CHECK: Exception while checking ApplicationDbContext: "
        + ex.Message
    );
}


// =========================================================
// SWAGGER - DEVELOPMENT
// =========================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "PHE.API v1"
        );
    });
}


// =========================================================
// STATIC FILES
// =========================================================
//
// IMPORTANT:
// Existing uploaded files are currently inside:
//
// D:\PHE_Project\PHE.API\Uploads
//
// They are NOT inside wwwroot.
//
// Therefore Documents should be served through
// JEScrutinyController's /document endpoint.
//
// UseStaticFiles() is still kept for wwwroot files.
//

app.UseStaticFiles();

var frontendPublicRoot = Path.Combine(
    Directory.GetCurrentDirectory(),
    "..",
    "phe-water-ui",
    "public");

if (Directory.Exists(frontendPublicRoot))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(frontendPublicRoot),
        RequestPath = ""
    });
}


// =========================================================
// CORS
// =========================================================
//
// IMPORTANT:
// CORS must execute before MapControllers().
//

app.UseCors("NextJsPolicy");


// =========================================================
// AUTHORIZATION
// =========================================================

app.UseAuthorization();


// =========================================================
// CONTROLLERS
// =========================================================

app.MapControllers();


// =========================================================
// START APPLICATION
// =========================================================

app.Run();