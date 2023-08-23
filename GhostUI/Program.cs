using GhostUI.Hubs;
using GhostUI.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using System.IO;
using Serilog;
using System;
using GhostUI.Middleware;
using GhostUI.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
//using Serilog.Core;
//using System.Linq;
//using System.Reflection.Metadata;
//using NSwag.Generation.Processors.Security;
//using Microsoft.EntityFrameworkCore.Design;
//using GhostUI.Abstraction;
//using GhostUI.Middleware;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using System;
//using System.Threading.Tasks;


var spaSrcPath = "ClientApp";
var corsPolicyName = "AllowAll";
Log.Logger = new LoggerConfiguration().MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .Enrich.FromLogContext()    
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = Directory.GetCurrentDirectory(),
    
    //EnvironmentName =  Environments.Development,
    //WebRootPath = "wwwroot"
});

//hosting environment variable in iwebhostenvironment
//var isproduction = builder.Environment.IsProduction();

builder.Services.AddTransient<ProblemDetailsFactory, CustomProblemDetailsFactory>();

builder.Host.UseSerilog((ctx,srv, cfg) => { 
    
    cfg
    .ReadFrom.Configuration(ctx.Configuration)
    .ReadFrom.Services(srv); 

    
});
//only execute once
builder.Services.AddHostedService<TracerService>();

// Custom healthcheck example
builder.Services.AddHealthChecks()
    .AddGCInfoCheck("GCInfo");

// Write healthcheck custom results to healthchecks-ui (use InMemory for the DB - AspNetCore.HealthChecks.UI.InMemory.Storage nuget package)
builder.Services.AddHealthChecksUI()
    .AddInMemoryStorage();

builder.Services.AddCorsConfig(corsPolicyName);
builder.Services.AddControllers(); 

//builder.Services.AddCustomLocalization();
builder.Services.AddSignalR();



// Add Brotli/Gzip response compression (prod only)
builder.Services.AddResponseCompressionConfig(builder.Configuration);

// Config change in asp.net core 3.0+ - 'Async' suffix in action names get stripped by default - so, to access them by full name with 'Async' part - opt out of this feature.
builder.Services.AddMvc(opt => opt.SuppressAsyncSuffixInActionNames = false);

// In production, the React files will be served from this directory
builder.Services.AddSpaStaticFiles(opt => opt.RootPath = $"{spaSrcPath}/dist");

// (NSwage library) Register the Swagger services (using OpenApi 3.0)
// this inject language preference to support different language.
builder.Services.AddOpenApiDocument(settings =>
{
    settings.Version = "v1";
    settings.Title = "GhostUI API";
    settings.Description = "Detailed Description of API";

    //* JWT security setup for nswag *//
    //settings.AddSecurity("JWT", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
    //{
    //    Type = NSwag.OpenApiSecuritySchemeType.ApiKey, //this requires "Bearer " prefix in the auth header
    //    Name="Authorization",
    //    In= NSwag.OpenApiSecurityApiKeyLocation.Header,
    //    Description = "Type into the textbox: Bearer {your JWT token}"


    //});
    //settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));


    
    //*If you want language specific work you can try this following code*//
    //settings.OperationProcessors.Add(new LanguageHeaderProcessor());
});


//*If you want language specific work you can try this following code*//

//builder.Services.AddHttpContextAccessor();

//builder.Services.AddScoped<ILanguageService, LanguageAccessorService>();


//*Jwt setup*//
var jwtissuer = builder.Configuration["JWTAuth:Issuer"];
var jwtaudience = builder.Configuration["JWTAuth:Audience"];
//builder.Services.AddAuthentication(opt =>
//{
//    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(options =>
//{
//    options.SaveToken = true;
//    options.Events = new JwtBearerEvents
//    {
//        OnAuthenticationFailed = jwtctx =>
//        {
//            if (jwtctx.Exception.GetType() == typeof(SecurityTokenExpiredException))
//            {
//                jwtctx.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
//            }
//            return Task.CompletedTask;
//        }
//    };
//    options.TokenValidationParameters = new TokenValidationParameters()
//    {
//        ClockSkew = TimeSpan.Zero,
        
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = jwtissuer,
//        ValidAudience = jwtaudience,
//        IssuerSigningKey = new SymmetricSecurityKey(
//            Encoding.UTF8.GetBytes("!AnywhereSecret!")
//        ),
//    };
//});

var app = builder.Build();


// If development, enable Hot Module Replacement
// If production, enable Brotli/Gzip response compression & strict transport security headers
if (app.Environment.IsDevelopment())
{
 
    
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseResponseCompression();
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseStaticFiles();//path to static files i.e. wwwroot if not specified
    app.UseSpaStaticFiles();
}

//* this help Linux deployment *//
//app.UseForwardedHeaders(new ForwardedHeadersOptions
//{
//    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All
//});

app.UseApiExceptionHandling();
app.UseCors(corsPolicyName);



// Show/write HealthReport data from healthchecks (AspNetCore.HealthChecks.UI.Client nuget package)
app.UseHealthChecksUI();
app.UseHealthChecks("/healthchecks-json", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Register the Swagger generator and the Swagger UI middlewares
// NSwage.MsBuild + adding automation config in GhostUI.csproj makes this part of the build step (updates to API will be handled automatically)
app.UseOpenApi(settings=>
{
    //settings.PostProcess = (doc, request) =>
    //{
    //    doc.Info.Version = "v1";
    //    doc.Info.Title = "GhostUI Api for react";
    //    doc.Info.Description = "Detail of Api";
    //    doc.Info.Contact = new NSwag.OpenApiContact
    //    {
    //        Name = "TinyCodeCraft",
    //        Email = "raymond.cw.yau@outlook.com",
    //        Url = "twitter.com/rcwyau",

    //    };
    //    doc.Info.License = new NSwag.OpenApiLicense
    //    {
    //        Name = "GhostShell",
    //        Url = "https://ghostshell.com"
    //    };


    //};

});

//add enrich parameter for each logging request
app.UseSerilogRequestLogging( option=>
{
    option.EnrichDiagnosticContext = (diagnostic, http) =>
    {
        diagnostic.Set("LocalTime", DateTime.Now.ToString("yyyyMMdd+HHmmss"));

    };
});
app.UseSwaggerUi3();
app.UseHttpsRedirection();
app.UseRouting();
//*Jwt enabled for auth*//
//app.UseAuthentication();
//app.UseAuthorization();



// Map controllers / SignalR hubs
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<UsersHub>("/hubs/users");
});

// Killing .NET debug session does not kill spawned Node.js process (have to manually kill)
app.UseSpa(spa =>
{
    spa.Options.SourcePath = spaSrcPath;

    if (app.Environment.IsDevelopment())
        spa.UseReactDevelopmentServer(npmScript: "start");
});

app.Run();