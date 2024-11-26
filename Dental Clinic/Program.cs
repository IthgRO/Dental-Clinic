using Dental_Clinic.StartupExtensions;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Server=tcp:itgh.database.windows.net,1433;Initial Catalog=DentalSolutionDb;Persist Security Info=False;User ID=indridiGG;Password=icelandPower33;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
builder.Services.AddDataAccess(connectionString);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");



app.Run();
