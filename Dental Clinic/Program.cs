using Dental_Clinic.StartupExtensions;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using Services.Interfaces;
using Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Server=tcp:itgh.database.windows.net,1433;Initial Catalog=DentalSolutionDb;Persist Security Info=False;User ID=indridiGG;Password=icelandPower33;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
var jwt_secret = "abdivbeiuvbiaubviubviwnvjwnviubwnifbwuybvuwbfubvuebvuybwbfbfgwevgyv4354366erbvuwybvyuwbvueybve";

builder.Services.AddDataAccess(connectionString);
//builder.Services.AddDbContext<ApplicationContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDependencyInjection();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureCors();
builder.Services.AddMvc();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddHostedService<ReminderHostedService>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwt_secret)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var app = builder.Build();
app.UseCors();
app.MapControllers();
app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();


app.Run();
