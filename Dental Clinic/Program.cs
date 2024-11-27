using Dental_Clinic.StartupExtensions;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Server=tcp:itgh.database.windows.net,1433;Initial Catalog=DentalSolutionDb;Persist Security Info=False;User ID=indridiGG;Password=icelandPower33;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
var jwt_secret = "abdivbeiuvbiaubviubvierbvuwybvyuwbvueybve";

builder.Services.AddDataAccess(connectionString);
builder.Services.AddDependencyInjection();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureCors();
builder.Services.AddMvc();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddFluentValidationAutoValidation();

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

app.MapControllers();
app.UseHttpsRedirection();


app.Run();
