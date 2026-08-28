using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using PHE.API.Data;

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