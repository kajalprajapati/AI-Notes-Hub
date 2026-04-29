using System;
using System.Text;
using AINotesHub.API.Data;
using AINotesHub.API.Services;
using AINotesHub.WPF.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

//Debug.WriteLine("Admin hash: " + BCrypt.Net.BCrypt.HashPassword("admin123"));
//Debug.WriteLine("Manager hash: " + BCrypt.Net.BCrypt.HashPassword("manager123"));
//Debug.WriteLine("User hash: " + BCrypt.Net.BCrypt.HashPassword("User123"));
//Environment.Exit(0); // stop app immediately after printing

//var builder = WebApplication.CreateBuilder(args);

// ✅ Read configuration from appsettings.json
//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration)
//    .Enrich.FromLogContext()
//    .WriteTo.Console()
//    .CreateLogger();


try
{
    //✅ Read configuration from appsettings.json
    Log.Information("🚀 {AppName} started at {StartTime}",
"AINotesHub API", DateTime.UtcNow);

    var builder = WebApplication.CreateBuilder(args);
    var environment = builder.Environment.EnvironmentName;

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()   // ✔ recommended
    .WriteTo.Console()         // ✔ show logs in console
        .CreateLogger();

    // Log.Information("Starting API...");

    // 1. Read the settings into a variable
    var jwtKey = builder.Configuration["Jwt:Key"];
    var jwtIssuer = builder.Configuration["Jwt:Issuer"];
    var jwtAudience = builder.Configuration["Jwt:Audience"];
    // Add services to the container.
    // Read JWT settings from appsettings.json
    // JWT setup
    builder.Services.AddSingleton<JwtTokenService>();
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"];


    builder.Services.AddScoped<DapperService>();
    //builder.Services.AddScoped<NoteService>();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddCookie(options =>
    {
        options.Cookie.Name = "AINotesAuth";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true; // IMPORTANT: Renews the cookie on every request
        options.LoginPath = "/api/account/login";
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer, // Uses "AINotesHub" from JSON

            ValidateAudience = true,
            ValidAudience = jwtAudience, // Uses "AINotesHubUsers" from JSON

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

    // Configure DbContext to use SQL Server
    //builder.Services.AddDbContext<NotesDbContext>(options =>
    //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddDbContext<NotesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging()
           .EnableDetailedErrors());

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();//For Swagger
    builder.Services.AddAuthorization();//For Authorization
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    var app = builder.Build();

    // Add authentication & authorization middleware
    app.UseAuthentication();
    app.UseAuthorization();

    // Create DB automatically if not exists
    //This will automatically create the database and apply migrations when you run your project.
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<NotesDbContext>();
        db.Database.Migrate(); // <-- Important for auto-create + apply migrations
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage(); // ADD THIS LINE
        app.UseSwagger();              // Enable Swagger
                                       //app.UseSwaggerUI();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            c.RoutePrefix = "swagger"; // Default
        });

    }
    //app.UseSwagger();
    app.UseHttpsRedirection();
    app.MapControllers();
    app.UseHttpsRedirection();
    //app.Run();

    // ✅ Ensure Serilog logs app start & stop

    //Log.Information("🚀 Starting up the AINotesHub API...");
    app.Run();   // 🔹 This runs inside the try block
}
catch (Exception ex)
{
    Log.Fatal(ex.InnerException,
    "❌ Application start-up failed in {Environment}!");
    
    //Log.Fatal(ex, "❌ Application start-up failed!");
}
finally
{
    Log.CloseAndFlush();// ✅ Flush logs before app fully stops
}
