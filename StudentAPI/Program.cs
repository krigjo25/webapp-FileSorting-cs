using DotNetEnv;
using Webapp.sorting.cs;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
// Load the environment variables
Env.Load();

// Configure the database
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();

    
string _db = Environment.GetEnvironmentVariable("MSQL_db");
string _user = Environment.GetEnvironmentVariable("MSQL_user");
string _server = Environment.GetEnvironmentVariable("MSQL_server");
string _password = Environment.GetEnvironmentVariable("MSQL_passcode");

var conn = $"Server={_server};Database={_db};User Id={_user};Password={_password};TrustServerCertificate=True;";


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<MSSQL>(opt => opt.UseSqlServer(conn));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "StudentAPI", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student API")); // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.  Replace "Your API Name" with a descriptive name.
}

app.MapGet("/", () => "{ Use correct api to fetch data }");

//app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();

// Get all students
app.MapGet("/students/", async (MSSQL db) => 
    await db.Student.ToListAsync()
);


app.Run();