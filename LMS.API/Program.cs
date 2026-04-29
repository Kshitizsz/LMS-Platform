using Hangfire;
using LMS.API.Middleware;
using LMS.Application;
using LMS.Domain.Interfaces;
using LMS.Infrastructure;
using LMS.Infrastructure.Hubs;
using LMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────────────────────
// 1. INFRASTRUCTURE — EF Core, UnitOfWork, Hangfire, EmailService, StripeService
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddInfrastructure(builder.Configuration);

// ─────────────────────────────────────────────────────────────────────────────
// 2. APPLICATION — MediatR, FluentValidation, ValidationBehavior pipeline
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddApplication();

// ─────────────────────────────────────────────────────────────────────────────
// 3. JWT TOKEN SERVICE
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// ─────────────────────────────────────────────────────────────────────────────
// 4. JWT AUTHENTICATION
// ─────────────────────────────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:SecretKey"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
                                   Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero,
        NameClaimType = ClaimTypes.NameIdentifier, // ← add this
        RoleClaimType = ClaimTypes.Role            // ← add this
    };

    // Allow SignalR to read JWT from query string
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            var accessToken = ctx.Request.Query["access_token"];
            var path = ctx.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/hubs"))
            {
                ctx.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// ─────────────────────────────────────────────────────────────────────────────
// 5. CONTROLLERS
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ─────────────────────────────────────────────────────────────────────────────
// 6. SWAGGER / OPENAPI
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LMS API",
        Version = "v1",
        Description = "Learning Management System — ASP.NET Core Web API",
        Contact = new OpenApiContact
        {
            Name = "Kshitiz",
            Email = "your@email.com"
        }
    });

    // XML comments (/// summaries on controllers show in Swagger UI)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        opt.IncludeXmlComments(xmlPath);

    // JWT Bearer auth input in Swagger UI
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste your JWT token here. Example: eyJhbGci..."
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ─────────────────────────────────────────────────────────────────────────────
// 7. CORS — allow Angular dev server
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddCors(opt => opt.AddPolicy("AllowAngular", policy =>
    policy.WithOrigins("http://localhost:4200", "https://lms-platform-one-sigma.vercel.app/courses", "https://localhost:4200")
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials()));


// ─────────────────────────────────────────────────────────────────────────────
// 8. SIGNALR
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddSignalR();

// ─────────────────────────────────────────────────────────────────────────────
// BUILD
// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

//var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
//app.Run($"http://0.0.0.0:{port}");
// ─────────────────────────────────────────────────────────────────────────────
// MIDDLEWARE PIPELINE (order matters)
// ─────────────────────────────────────────────────────────────────────────────
//if (app.Environment.IsDevelopment())
//{
//    app.Run("http://localhost:5217");
//}
//else
//{
//    var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
//    app.Run($"http://0.0.0.0:{port}");
//}


// 1. Global exception handler — must be first
app.UseMiddleware<ExceptionMiddleware>();

// 2. Swagger — dev only
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LMS API v1");
//        c.RoutePrefix = "swagger"; // http://localhost:{port}/swagger
//    });
//}
app.UseSwagger();
app.UseSwaggerUI();

// 3. HTTPS redirect
//app.UseHttpsRedirection();

// 4. CORS — before auth
app.UseCors("AllowAngular");

// 5. Auth
app.UseAuthentication();
app.UseAuthorization();

// 6. Controllers
app.MapControllers();

// 7. Hangfire dashboard (admin only in production — open for now)
// Hangfire dashboard — open access in dev
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireDevAuthFilter() }
});
// 8. SignalR hubs (Phase 4 — placeholder ready)
app.MapHub<NotificationHub>("/hubs/notifications");
// Also add this so SignalR knows which claim is the user identifier
app.MapGet("/", () => "API is running ");
app.Run();