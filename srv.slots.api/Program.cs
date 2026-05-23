using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using srv.slots.api.Middlewares;
using srv.slots.application;
using srv.slots.infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ───────────────────────────────────────────────────────────────
//  CONFIGURATION — environment variables override appsettings.json
//  In production (Render), set these as env vars. Locally, the
//  values in appsettings.Local.json are used as a fallback.
// ───────────────────────────────────────────────────────────────

// Connection string: env var first, then appsettings
var connectionString =
    Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No database connection string configured.");

builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;

// JWT settings: env vars first, then appsettings
var jwtSecret =
    Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("JWT secret not configured.");

if (string.IsNullOrWhiteSpace(jwtSecret) || jwtSecret.Length < 16)
    throw new InvalidOperationException("JWT secret is empty or too short. Set the JWT_SECRET environment variable.");

var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
    ?? builder.Configuration["Jwt:Issuer"] ?? "ItsMyTurn";

var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
    ?? builder.Configuration["Jwt:Audience"] ?? "ItsMyTurnUsers";

var corsOriginsRaw = Environment.GetEnvironmentVariable("CORS_ORIGINS");

// ───────────────────────────────────────────────────────────────
//  SERVICES
// ───────────────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ItsMyTurn API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token (without 'Bearer ' prefix)."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AppCors", policy =>
    {
        if (string.IsNullOrWhiteSpace(corsOriginsRaw))
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        }
        else
        {
            var origins = corsOriginsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
        }
    });
});

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Project layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// ───────────────────────────────────────────────────────────────
//  PIPELINE
// ───────────────────────────────────────────────────────────────
app.UseMiddleware<GlobalExceptionMiddleware>();

// Swagger enabled in ALL environments so you can test the deployed API
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ItsMyTurn API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AppCors");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health-check endpoints
app.MapGet("/", () => Results.Ok(new { status = "ItsMyTurn API is running", time = DateTime.UtcNow }));
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();
